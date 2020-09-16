using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Extensions;
using Toggl.Core.Interactors;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.Services;
using Toggl.Core.Sync;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Shared;
using System.ComponentModel;
using static Toggl.Core.Analytics.ContinueTimeEntryMode;
using static Toggl.Core.Analytics.ContinueTimeEntryOrigin;
using Toggl.Storage.Settings;
using Toggl.Shared.Extensions;
using Toggl.Storage;
using static Toggl.Core.Helper.Constants;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class TimerViewModel : ViewModel
    {
        private static readonly TimeSpan minTimeBetweenProgressChanges = TimeSpan.FromMilliseconds(20);

        private bool noWorkspaceViewPresented;
        private bool hasStopButtonEverBeenUsed;
        private bool noDefaultWorkspaceViewPresented;

        private readonly ISyncManager syncManager;
        private readonly ITogglDataSource dataSource;
        private readonly IUserPreferences userPreferences;
        private readonly IRxActionFactory rxActionFactory;
        private readonly IAnalyticsService analyticsService;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IInteractorFactory interactorFactory;
        private readonly IAccessibilityService accessibilityService;
        private readonly IAccessRestrictionStorage accessRestrictionStorage;

        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        
        public IObservable<bool> IsInManualMode { get; private set; }
        public IObservable<string> ElapsedTime { get; private set; }
        public IObservable<bool> IsTimeEntryRunning { get; private set; }
        public IObservable<int> NumberOfSyncFailures { get; private set; }
        public IObservable<Unit> ShouldReloadTimeEntryLog { get; private set; }
        public IObservable<SyncProgress> SyncProgressState { get; private set; }
        public IObservable<IThreadSafeTimeEntry> CurrentRunningTimeEntry { get; private set; }

        public IOnboardingStorage OnboardingStorage { get; }

        public ViewAction Refresh { get; private set; }
        public ViewAction OpenSettings { get; private set; }
        public ViewAction StartTimeEntry { get; private set; }
        public InputAction<EditTimeEntryInfo> SelectTimeEntry { get; private set; }
        public InputAction<TimeEntryStopOrigin> StopTimeEntry { get; private set; }

        public ITimeService TimeService { get; }

        public TimerViewModel(
            ITogglDataSource dataSource,
            ISyncManager syncManager,
            ITimeService timeService,
            IUserPreferences userPreferences,
            IAnalyticsService analyticsService,
            IOnboardingStorage onboardingStorage,
            IInteractorFactory interactorFactory,
            INavigationService navigationService,
            IAccessibilityService accessibilityService,
            IAccessRestrictionStorage accessRestrictionStorage,
            ISchedulerProvider schedulerProvider,
            IRxActionFactory rxActionFactory) : base(navigationService)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(syncManager, nameof(syncManager));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(accessibilityService, nameof(accessibilityService));
            Ensure.Argument.IsNotNull(accessRestrictionStorage, nameof(accessRestrictionStorage));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
 
            this.dataSource = dataSource;
            this.syncManager = syncManager;
            this.userPreferences = userPreferences;
            this.rxActionFactory = rxActionFactory;
            this.analyticsService = analyticsService;
            this.interactorFactory = interactorFactory;
            this.schedulerProvider = schedulerProvider;
            this.accessibilityService = accessibilityService;
            this.accessRestrictionStorage = accessRestrictionStorage;

            TimeService = timeService;
            OnboardingStorage = onboardingStorage;
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            interactorFactory.GetCurrentUser().Execute()
                .Select(u => u.Id)
                .Subscribe(analyticsService.SetUserId);

            SyncProgressState = syncManager.ProgressObservable
                .Throttle(minTimeBetweenProgressChanges)
                .AsDriver(schedulerProvider);

            var isWelcome = OnboardingStorage.IsNewUser;

            IsInManualMode = userPreferences
                .IsManualModeEnabledObservable
                .AsDriver(schedulerProvider);

            CurrentRunningTimeEntry = dataSource.TimeEntries
                .CurrentlyRunningTimeEntry
                .AsDriver(schedulerProvider);

            IsTimeEntryRunning = dataSource.TimeEntries
                .CurrentlyRunningTimeEntry
                .Select(te => te != null)
                .DistinctUntilChanged()
                .AsDriver(schedulerProvider);

            var durationObservable = dataSource
                .Preferences
                .Current
                .Select(preferences => preferences.DurationFormat);

            ElapsedTime = TimeService
                .CurrentDateTimeObservable
                .CombineLatest(CurrentRunningTimeEntry, (now, te) => (now - te?.Start) ?? TimeSpan.Zero)
                .CombineLatest(durationObservable, (duration, format) => duration.ToFormattedString(format))
                .AsDriver(schedulerProvider);

            NumberOfSyncFailures = interactorFactory
                .GetItemsThatFailedToSync()
                .Execute()
                .Select(i => i.Count())
                .AsDriver(schedulerProvider);

            ShouldReloadTimeEntryLog = Observable.Merge(
                TimeService.MidnightObservable.SelectUnit(),
                TimeService.SignificantTimeChangeObservable.SelectUnit())
                .AsDriver(schedulerProvider);

            Refresh = rxActionFactory.FromAsync(refresh);
            OpenSettings = rxActionFactory.FromAsync(openSettings);
            SelectTimeEntry = rxActionFactory.FromAsync<EditTimeEntryInfo>(timeEntrySelected);
            StartTimeEntry = rxActionFactory.FromAsync(startTimeEntry, IsTimeEntryRunning.Invert());
            StopTimeEntry = rxActionFactory.FromObservable<TimeEntryStopOrigin>(stopTimeEntry, IsTimeEntryRunning);
          
            OnboardingStorage.StopButtonWasTappedBefore
                             .Subscribe(hasBeen => hasStopButtonEverBeenUsed = hasBeen)
                             .DisposedBy(disposeBag);
            SyncProgressState
                .Subscribe(postAccessibilityAnnouncementAboutSync)
                .DisposedBy(disposeBag);

            syncManager.Errors
                .AsDriver(schedulerProvider)
                .SelectMany(handleSyncError)
                .Subscribe()
                .DisposedBy(disposeBag);
        }

        private IObservable<bool> createRunningTimeEntryTooltipPredicate()
        {
            var timeEntryTappedObservable = SelectTimeEntry
                .Inputs
                .SelectValue(false)
                .Track(analyticsService.TooltipDismissed, OnboardingConditionKey.RunningTimeEntryTooltip, TooltipDismissReason.ConditionMet);

            return CurrentRunningTimeEntry
                .DelaySubscription(TimeSpan.FromSeconds(2))
                .DistinctUntilChanged()
                .Select(te => te != null)
                .Merge(timeEntryTappedObservable)
                .Select(shouldShow => !OnboardingStorage.OnboardingConditionWasMetBefore(OnboardingConditionKey.StartTimeEntryTooltip) && shouldShow)
                .AsDriver(schedulerProvider);
        }

        private async Task handleNoWorkspaceState()
        {
            if (accessRestrictionStorage.HasNoWorkspace() && !noWorkspaceViewPresented)
            {
                noWorkspaceViewPresented = true;
                await Navigate<NoWorkspaceViewModel, Unit>();
                noWorkspaceViewPresented = false;
            }
        }

        private async Task handleNoDefaultWorkspaceState()
        {
            if (!accessRestrictionStorage.HasNoWorkspace() && accessRestrictionStorage.HasNoDefaultWorkspace() && !noDefaultWorkspaceViewPresented)
            {
                noDefaultWorkspaceViewPresented = true;
                await Navigate<SelectDefaultWorkspaceViewModel, Unit>();
                noDefaultWorkspaceViewPresented = false;
            }
        }

        private Task openSettings()
        {
            return navigate<SettingsViewModel>();
        }

        private async Task startTimeEntry()
        {
            OnboardingStorage.StartButtonWasTapped();

            var duration = userPreferences.IsManualModeEnabled
                ? (TimeSpan?)TimeSpan.FromMinutes(DefaultTimeEntryDurationForManualModeInMinutes)
                : null;

            var defaultWorkspace = await interactorFactory.GetDefaultWorkspace().Execute();
            var prototype = "".AsTimeEntryPrototype(
                TimeService.CurrentDateTime,
                defaultWorkspace.Id,
                duration
            );

            await interactorFactory.CreateTimeEntry(prototype, TimeEntryStartOrigin.Manual).Execute();
        }


        private async Task timeEntrySelected(EditTimeEntryInfo editTimeEntryInfo)
        {
            OnboardingStorage.TimeEntryWasTapped();

            analyticsService.EditViewOpened.Track(editTimeEntryInfo.Origin);
            await navigate<EditTimeEntryViewModel, long[]>(editTimeEntryInfo.Ids);
        }

        private async Task refresh()
        {
            await syncManager.ForceFullSync();
        }

        private IObservable<Unit> stopTimeEntry(TimeEntryStopOrigin origin)
        {
            OnboardingStorage.StopButtonWasTapped();

            return interactorFactory
                .StopTimeEntry(TimeService.CurrentDateTime, origin)
                .Execute()
                .ToObservable()
                .Do(syncManager.InitiatePushSync)
                .SelectUnit();
        }

        private Task navigate<TModel, TParameters>(TParameters value)
            where TModel : ViewModelWithInput<TParameters>
        {
            if (hasStopButtonEverBeenUsed)
                OnboardingStorage.SetNavigatedAwayFromMainViewAfterStopButton();

            return Navigate<TModel, TParameters>(value);
        }

        private Task navigate<TModel>()
            where TModel : ViewModel
        {
            if (hasStopButtonEverBeenUsed)
                OnboardingStorage.SetNavigatedAwayFromMainViewAfterStopButton();

            return Navigate<TModel>();
        }

        private void postAccessibilityAnnouncementAboutSync(SyncProgress syncProgress)
        {
            string message = "";
            switch (syncProgress)
            {
                case SyncProgress.Failed:
                    message = Resources.SyncFailed;
                    break;
                case SyncProgress.OfflineModeDetected:
                    message = Resources.SyncFailedOffline;
                    break;
                case SyncProgress.Synced:
                    message = Resources.SuccessfullySyncedData;
                    break;

                //These 2 are not announced
                case SyncProgress.Syncing:
                    return;
                case SyncProgress.Unknown:
                    return;
            }

            accessibilityService.PostAnnouncement(message);
        }

        private async Task<Unit> handleSyncError(Exception exception)
        {
            await handleNoWorkspaceState();
            handleNoDefaultWorkspaceState();

            return Unit.Default;
        }
    }
}
