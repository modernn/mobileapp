using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.DataSources;
using Toggl.Core.Exceptions;
using Toggl.Core.Interactors;
using Toggl.Core.Services;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Services;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Toggl.Storage.Settings;

namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    using CalendarSectionModel = SectionModel<CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel>;
    using ImmutableCalendarSectionModel = IImmutableList<SectionModel<CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel>>;

    [Preserve(AllMembers = true)]
    public class CalendarSettingsViewModel : ViewModel
    {
        private readonly IUserPreferences userPreferences;
        private readonly IAnalyticsService analyticsService;
        private readonly IOnboardingStorage onboardingStorage;
        private readonly IInteractorFactory interactorFactory;
        private readonly ITogglDataSource dataSource;
        private readonly ISchedulerProvider schedulerProvider;
        private readonly IPermissionsChecker permissionsChecker;

        private readonly ISubject<bool> calendarIntegrationEnabledSubject = new Subject<bool>();
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        private readonly ISubject<ImmutableCalendarSectionModel> calendarsSubject =
            new BehaviorSubject<ImmutableCalendarSectionModel>(ImmutableList.Create<CalendarSectionModel>());

        private HashSet<string> initialSelectedCalendarIds { get; } = new HashSet<string>();
        private HashSet<string> selectedCalendarIds { get; } = new HashSet<string>();

        public IObservable<ImmutableCalendarSectionModel> Calendars { get; }

        public IObservable<bool> CalendarIntegrationEnabled { get;}

        public ViewAction ToggleNativeCalendarIntegration { get; }
        public ViewAction Save { get; }

        public ViewAction Reload { get; }
        public InputAction<SelectableNativeCalendarViewModel> SelectNativeCalendar { get; }

        public InputAction<SelectableExternalCalendarViewModel> SelectExternalCalendar { get; }

        public CalendarSettingsViewModel(
            IUserPreferences userPreferences,
            IInteractorFactory interactorFactory,
            ITogglDataSource dataSource,
            IOnboardingStorage onboardingStorage,
            IAnalyticsService analyticsService,
            INavigationService navigationService,
            IRxActionFactory rxActionFactory,
            IPermissionsChecker permissionsChecker,
            ISchedulerProvider schedulerProvider)
            : base(navigationService)
        {
            Ensure.Argument.IsNotNull(userPreferences, nameof(userPreferences));
            Ensure.Argument.IsNotNull(rxActionFactory, nameof(rxActionFactory));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(interactorFactory, nameof(interactorFactory));
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(onboardingStorage, nameof(onboardingStorage));
            Ensure.Argument.IsNotNull(schedulerProvider, nameof(schedulerProvider));
            Ensure.Argument.IsNotNull(permissionsChecker, nameof(permissionsChecker));

            this.userPreferences = userPreferences;
            this.analyticsService = analyticsService;
            this.onboardingStorage = onboardingStorage;
            this.interactorFactory = interactorFactory;
            this.dataSource = dataSource;
            this.schedulerProvider = schedulerProvider;
            this.permissionsChecker = permissionsChecker;

            Save = rxActionFactory.FromAction(save);
            Reload = rxActionFactory.FromAsync(reloadCalendars);
            SelectNativeCalendar = rxActionFactory.FromAction<SelectableNativeCalendarViewModel>(selectNativeCalendar);
            SelectExternalCalendar = rxActionFactory.FromAction<SelectableExternalCalendarViewModel>(selectExternalCalendar);
            ToggleNativeCalendarIntegration = rxActionFactory.FromAsync(toggleCalendarIntegration);

            Calendars = calendarsSubject.AsObservable().DistinctUntilChanged();
            CalendarIntegrationEnabled = userPreferences.CalendarIntegrationEnabledObservable
                .StartWith(userPreferences.CalendarIntegrationEnabled())
                .Merge(calendarIntegrationEnabledSubject.AsObservable());
        }

        public override async Task Initialize()
        {
            await base.Initialize();

            var calendarPermissionGranted = await permissionsChecker.CalendarPermissionGranted;
            if (calendarPermissionGranted)
            {
                var enabledCalendars = userPreferences.EnabledCalendarIds();
                if (enabledCalendars != null)
                {
                    initialSelectedCalendarIds.AddRange(enabledCalendars);
                    selectedCalendarIds.AddRange(enabledCalendars);
                }
            }
            else
            {
                userPreferences.SetCalendarIntegrationEnabled(false);
                userPreferences.SetEnabledCalendars(null);
            }
            reloadCalendars();
        }

        public override void ViewAppeared()
        {
            base.ViewAppeared();

            if (!onboardingStorage.CalendarPermissionWasAskedBefore())
            {
                View.RequestCalendarAuthorization(false)
                    .ObserveOn(schedulerProvider.MainScheduler)
                    .Subscribe(onCalendarPermission)
                    .DisposedBy(disposeBag);
                onboardingStorage.SetCalendarPermissionWasAskedBefore();
            }
        }

        private void onCalendarPermission(bool granted)
        {
            if (granted)
                toggleCalendarIntegration();
        }

        public override void ViewDestroyed()
        {
            base.ViewDestroyed();

            disposeBag.Dispose();
        }

        private async Task reloadCalendars()
        {
            var sections = new List<CalendarSectionModel>();

            var externalCalendars = await dataSource.ExternalCalendars.GetAll();
            var externalCalendarsGroups = externalCalendars
                .GroupBy(cal => cal.IntegrationId)
                .Select(section => section.OrderBy(cal => cal.Name));

            // Add the external calendars main section
            var shouldShowCard = externalCalendars.None();
            var externalCalendarsMainHeader = new ExternalCalendarsMainSectionViewModel(shouldShowCard);
            var externalCalendarsMainItems = new List<CalendarSettingsItemViewModel>();

            if (shouldShowCard)
            {
                var externalCalendarsCardItem = new ExternalCalendarsCardInfoViewModel();
                externalCalendarsMainItems.Add(externalCalendarsCardItem);
            }
            var externalCalendarsMainSection = new CalendarSectionModel(externalCalendarsMainHeader, externalCalendarsMainItems.ToImmutableList());
            sections.Add(externalCalendarsMainSection);

            // Add the external calendars sections
            foreach (var externalCalendarsGroup in externalCalendarsGroups)
            {
                var externalCalendarsHeader = new UserCalendarSourceViewModel($"Google Calendar");
                var externalCalendarsItems = externalCalendarsGroup.Select(cal => new SelectableExternalCalendarViewModel(cal, cal.Selected));
                var externalCalendarsSection = new CalendarSectionModel(externalCalendarsHeader, externalCalendarsItems);
                sections.Add(externalCalendarsSection);
            }

            var nativeCalendarsIntegrationEnabled = userPreferences.CalendarIntegrationEnabled();

            // Add the native calendars main section
            var nativeCalendarsMainHeader = new NativeCalendarsMainSectionViewModel();
            var nativeCalendarsMainItems = new List<CalendarSettingsItemViewModel>
            {
                new NativeCalendarsToggleIntegrationViewModel(nativeCalendarsIntegrationEnabled),
            };
            var nativeCalendarsMainSection = new CalendarSectionModel(
                nativeCalendarsMainHeader,
                nativeCalendarsMainItems.ToImmutableList());

            sections.Add(nativeCalendarsMainSection);

            // Add the native calendars sections
            if (nativeCalendarsIntegrationEnabled)
            {
                var nativeCalendars = await interactorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Catch((NotAuthorizedException _) => Observable.Return(new List<UserCalendar>()))
                    .Select(group);
                sections.AddRange(nativeCalendars);
            }

            calendarsSubject.OnNext(sections.ToImmutableList());
        }

        private ImmutableCalendarSectionModel group(IEnumerable<UserCalendar> calendars)
            => calendars
                .Select(toSelectable)
                .GroupBy(calendar => calendar.SourceName)
                .Select(group =>
                    new CalendarSectionModel(
                        new UserCalendarSourceViewModel(group.First().SourceName),
                        group.OrderBy(calendar => calendar.Name)
                    )
                )
                .ToImmutableList();

        private SelectableNativeCalendarViewModel toSelectable(UserCalendar calendar)
            => new SelectableNativeCalendarViewModel(calendar, selectedCalendarIds.Contains(calendar.Id));

        private async void save()
        {
            if (!userPreferences.CalendarIntegrationEnabled())
                selectedCalendarIds.Clear();

            userPreferences.SetEnabledCalendars(selectedCalendarIds.ToArray());

            if (onboardingStorage.IsFirstTimeConnectingCalendars() && initialSelectedCalendarIds.Count == 0)
            {
                analyticsService.NumberOfLinkedCalendarsNewUser.Track(selectedCalendarIds.Count);
            }
            else if (!selectedCalendarIds.SetEquals(initialSelectedCalendarIds))
            {
                analyticsService.NumberOfLinkedCalendarsChanged.Track(selectedCalendarIds.Count);
            }

            onboardingStorage.SetIsFirstTimeConnectingCalendars();

            await interactorFactory.PushSelectedExternalCalendars().Execute();
        }

        private void selectNativeCalendar(SelectableNativeCalendarViewModel calendar)
        {
            if (selectedCalendarIds.Contains(calendar.Id))
                selectedCalendarIds.Remove(calendar.Id);
            else
                selectedCalendarIds.Add(calendar.Id);
            calendar.Selected = !calendar.Selected;
        }

        private void selectExternalCalendar(SelectableExternalCalendarViewModel calendar)
        {
            interactorFactory.SetExternalCalendarSelected(calendar.Id, !calendar.Selected).Execute();
            calendar.Selected = !calendar.Selected;
        }

        private async Task toggleCalendarIntegration()
        {
            //Disabling the calendar integration
            if (userPreferences.CalendarIntegrationEnabled())
            {
                userPreferences.SetCalendarIntegrationEnabled(false);
                userPreferences.SetEnabledCalendars(null);
                selectedCalendarIds.Clear();
                await reloadCalendars();
                return;
            }

            //Enabling the calendar integration
            var calendarPermissionGranted = await permissionsChecker.CalendarPermissionGranted;
            if (!calendarPermissionGranted)
            {
                var permissionGranted = await View.RequestCalendarAuthorization(true);
                if (!permissionGranted)
                {
                    calendarIntegrationEnabledSubject.OnNext(false);
                    return;
                }
            }
            userPreferences.SetCalendarIntegrationEnabled(true);
            await reloadCalendars();
        }

        public override void Close()
        {
            Save.Execute();
            base.Close();
        }
    }
}
