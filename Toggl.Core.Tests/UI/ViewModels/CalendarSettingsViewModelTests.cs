using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.Exceptions;
using Toggl.Core.Models.Calendar;
using Toggl.Core.Tests.Generators;
using Toggl.Core.Tests.TestExtensions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Core.UI.Views;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    using ImmutableCalendarSectionModel = IImmutableList<SectionModel<CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel>>;

    public sealed class CalendarSettingsViewModelTests
    {
        public abstract class CalendarSettingsViewModelTest : BaseViewModelTests<CalendarSettingsViewModel>
        {
            protected override CalendarSettingsViewModel CreateViewModel()
                => new CalendarSettingsViewModel(UserPreferences, InteractorFactory, DataSource, OnboardingStorage, AnalyticsService, NavigationService, RxActionFactory, PermissionsChecker, SchedulerProvider);
        }

        public sealed class TheConstructor : CalendarSettingsViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useUserPreferences,
                bool useInteractorFactory,
                bool useDataSource,
                bool useOnboardingStorage,
                bool useAnalyticsService,
                bool useNavigationService,
                bool useRxActionFactory,
                bool usePermissionsChecker,
                bool useSchedulerProvider)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarSettingsViewModel(
                        useUserPreferences ? UserPreferences : null,
                        useInteractorFactory ? InteractorFactory : null,
                        useDataSource ? DataSource : null,
                        useOnboardingStorage ? OnboardingStorage : null,
                        useAnalyticsService ? AnalyticsService : null,
                        useNavigationService ? NavigationService : null,
                        useRxActionFactory ? RxActionFactory : null,
                        usePermissionsChecker ? PermissionsChecker : null,
                        useSchedulerProvider ? SchedulerProvider : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheInitializeMethod : CalendarSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task FillsTheCalendarList()
            {
                var externalCalendarsObservable = Enumerable
                    .Range(0, 3)
                    .Select(id => new ExternalCalendar(
                        id,
                        0,
                        $"Calendar #{id}",
                        $"Calendar #{id}",
                        false,
                        false))
                    .Apply(Observable.Return);

                var nativeCalendarsObservable = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false))
                    .Apply(Observable.Return);
                DataSource.ExternalCalendars.GetAll().Returns(externalCalendarsObservable);
                InteractorFactory.GetUserCalendars().Execute().Returns(nativeCalendarsObservable);
                UserPreferences.CalendarIntegrationEnabled().Returns(true);
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                TestScheduler.Start();

                var calendars = await viewModel.Calendars.FirstAsync();
                calendars.Should().HaveCount(6);
                calendars
                    .Where(section => section.Header is UserCalendarSourceViewModel)
                    .ForEach(group => group.Items.Should().HaveCount(3));
            }

            [Fact, LogIfTooSlow]
            public void SetsProperExternalCalendarsAsSelected()
            {
                var enabledCalendarIds = new List<long> { 1, 2, 3 };
                var unenabledCalendarIds = new List<long> { 4, 5, 6 };
                var allCalendarIds = enabledCalendarIds.Concat(unenabledCalendarIds).ToList();

                var externalCalendars = allCalendarIds
                    .Select(id => new ExternalCalendar(
                        id,
                        0,
                        $"Calendar #{id}",
                        $"Calendar #{id}",
                        enabledCalendarIds.Contains(id),
                        false));

                DataSource.ExternalCalendars.GetAll().Returns(Observable.Return(externalCalendars));
                var viewModel = CreateViewModel();

                viewModel.Initialize().Wait();

                var sections = viewModel.Calendars.FirstAsync().Wait();
                var calendars = sections.Where(section => section.Header is UserCalendarSourceViewModel);
                foreach (var calendarGroup in calendars)
                {
                    foreach (var calendar in calendarGroup.Items)
                    {
                        var externalCalendar = (SelectableExternalCalendarViewModel)calendar;
                        if (enabledCalendarIds.Contains(externalCalendar.Id))
                            externalCalendar.Selected.Should().BeTrue();
                    }
                }
            }

            [Property]
            public void SetsProperNativeCalendarsAsSelected(
                NonEmptySet<NonEmptyString> strings0,
                NonEmptySet<NonEmptyString> strings1)
            {
                var enabledCalendarIds = strings0.Get.Select(str => str.Get).ToList();
                var unenabledCalendarIds = strings1.Get.Select(str => str.Get).ToList();
                var allCalendarIds = enabledCalendarIds.Concat(unenabledCalendarIds).ToList();
                UserPreferences.EnabledCalendarIds().Returns(enabledCalendarIds);

                var userCalendars = allCalendarIds
                    .Select(id => new UserCalendar(
                        id,
                        "Does not matter",
                        "Does not matter, pt.2"
                    ));
                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));
                var viewModel = CreateViewModel();

                viewModel.Initialize().Wait();

                var sections = viewModel.Calendars.FirstAsync().Wait();
                var calendars = sections.Where(section => section.Header is UserCalendarSourceViewModel);
                foreach (var calendarGroup in calendars)
                {
                    foreach (var calendar in calendarGroup.Items)
                    {
                        var nativeCalendar = (SelectableNativeCalendarViewModel)calendar;
                        if (enabledCalendarIds.Contains(nativeCalendar.Id))
                            nativeCalendar.Selected.Should().BeTrue();
                    }
                }
            }

            [Fact, LogIfTooSlow]
            public async Task SetsTheEnabledCalendarsToNullWhenCalendarPermissionsWereNotGranted()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(false));
                UserPreferences.EnabledCalendarIds().Returns(new List<string>());
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                TestScheduler.Start();

                UserPreferences.Received().SetEnabledCalendars(Arg.Is<string[]>(strings => strings == null || strings.Length == 0));
            }

            [Fact, LogIfTooSlow]
            public async Task DoesNotSetTheEnabledCalendarsToNullWhenCalendarPermissionsWereGranted()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                UserPreferences.EnabledCalendarIds().Returns(new List<string>());
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                TestScheduler.Start();

                UserPreferences.DidNotReceive().SetEnabledCalendars(Arg.Is<string[]>(strings => strings == null || strings.Length == 0));
            }

            [Fact, LogIfTooSlow]
            public async Task DisablesTheCalendarIntegrationIfCalendarPermissionIsNotGranted()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(false));
                UserPreferences.EnabledCalendarIds().Returns(new List<string>());
                var viewModel = CreateViewModel();

                await viewModel.Initialize();
                TestScheduler.Start();

                UserPreferences.Received().SetCalendarIntegrationEnabled(false);
            }

            [Fact, LogIfTooSlow]
            public async Task HandlesNotAuthorizedException()
            {
                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Throw<IEnumerable<UserCalendar>>(new NotAuthorizedException("")));

                await ViewModel.Initialize();
                var calendars = await ViewModel.Calendars.FirstAsync();

                calendars.Should().HaveCount(2);
            }
        }

        public sealed class TheSaveAction : CalendarSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task SavesTheSelectedCalendars()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                UserPreferences.CalendarIntegrationEnabled().Returns(true);
                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));
                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));
                await ViewModel.Initialize();
                var selectedIds = new[] { "0", "2", "4", "7" };
                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableNativeCalendarViewModel(calendar, false))
                    .ToArray();

                ViewModel.SelectNativeCalendar.ExecuteSequentially(calendars)
                    .AppendAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                UserPreferences.Received().SetEnabledCalendars(selectedIds);
            }

            [Fact, LogIfTooSlow]
            public async Task PushesTheSelectedNativeCalendars()
            {
                await ViewModel.Initialize();
                ViewModel.Save.Execute();
                await InteractorFactory.PushSelectedExternalCalendars().Received().Execute();
            }
        }

        public sealed class TheViewAppearedMethod : CalendarSettingsViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task AsksForCalendarPermissionIfIsFirstTimeOpeningThisView()
            {
                var viewModel = CreateViewModel();
                viewModel.AttachView(Substitute.For<IView>());
                await viewModel.Initialize();

                viewModel.ViewAppeared();

                viewModel.View.Received().RequestCalendarAuthorization();
            }


            [Fact, LogIfTooSlow]
            public async Task DoesNotAskForPermissionIfItWasAskedAlready()
            {
                OnboardingStorage.CalendarPermissionWasAskedBefore().Returns(true);
                var viewModel = CreateViewModel();
                viewModel.AttachView(Substitute.For<IView>());
                await viewModel.Initialize();

                viewModel.ViewAppeared();

                viewModel.View.DidNotReceive().RequestCalendarAuthorization();
            }
        }

        public sealed class TheSelectCalendarAction : CalendarSettingsViewModelTest
        {
            public TheSelectCalendarAction()
            {
                var externalCalendars = Enumerable
                    .Range(0, 3)
                    .Select(id => new ExternalCalendar(
                        id,
                        0,
                        $"Calendar #{id}",
                        $"Calendar #{id}",
                        false,
                        false));

                var nativeCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));
                var externalCalendarsObservable = Observable.Return(externalCalendars);
                var nativeCalendarsObservable = Observable.Return(nativeCalendars);
                DataSource.ExternalCalendars.GetAll().Returns(externalCalendarsObservable);
                InteractorFactory.GetUserCalendars().Execute().Returns(nativeCalendarsObservable);
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                UserPreferences.CalendarIntegrationEnabled().Returns(true);
            }

            [Fact, LogIfTooSlow]
            public async Task MarksTheCalendarAsSelectedIfItIsNotSelected()
            {
                await ViewModel.Initialize();
                var viewModelCalendars = await ViewModel.Calendars.FirstAsync();
                var externalCalendarToBeSelected = viewModelCalendars[1].Items.First() as SelectableExternalCalendarViewModel;
                var nativeCalendarToBeSelected = viewModelCalendars[3].Items.First() as SelectableNativeCalendarViewModel;

                ViewModel.SelectExternalCalendar.Execute(externalCalendarToBeSelected);
                ViewModel.SelectNativeCalendar.Execute(nativeCalendarToBeSelected);
                TestScheduler.Start();

                externalCalendarToBeSelected.Selected.Should().BeTrue();
                nativeCalendarToBeSelected.Selected.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public async Task MarksTheCalendarAsNotSelectedIfItIsSelected()
            {
                await ViewModel.Initialize();
                var viewModelCalendars = await ViewModel.Calendars.FirstAsync();
                var externalCalendarToBeSelected = viewModelCalendars[1].Items.First() as SelectableExternalCalendarViewModel;
                var nativeCalendarToBeSelected = viewModelCalendars[3].Items.First() as SelectableNativeCalendarViewModel;

                ViewModel.SelectExternalCalendar.Execute(externalCalendarToBeSelected); //Select the calendar
                ViewModel.SelectNativeCalendar.Execute(nativeCalendarToBeSelected); //Select the calendar
                TestScheduler.Start();
                ViewModel.SelectExternalCalendar.Execute(externalCalendarToBeSelected); //Deselect the calendar
                ViewModel.SelectNativeCalendar.Execute(nativeCalendarToBeSelected); //Deselect the calendar
                TestScheduler.Start();

                externalCalendarToBeSelected.Selected.Should().BeFalse();
                nativeCalendarToBeSelected.Selected.Should().BeFalse();
            }
        }

        public sealed class TheToggleCalendarIntegrationAction
        {
            public sealed class WhenDisablingTheCalendarIntegration : CalendarSettingsViewModelTest
            {
                public WhenDisablingTheCalendarIntegration()
                {
                    UserPreferences.CalendarIntegrationEnabled().Returns(true);
                    PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                }

                [Fact, LogIfTooSlow]
                public async Task SetsCalendarIntegrationToDisabled()
                {
                    await ViewModel.Initialize();

                    ViewModel.ToggleNativeCalendarIntegration.Execute();
                    TestScheduler.Start();

                    UserPreferences.Received().SetCalendarIntegrationEnabled(false);
                }

                [Fact, LogIfTooSlow]
                public async Task SetsEnabledCalendarsToNull()
                {
                    await ViewModel.Initialize();

                    ViewModel.ToggleNativeCalendarIntegration.Execute();
                    TestScheduler.Start();

                    UserPreferences.Received().SetEnabledCalendars(null);
                }

                [Fact, LogIfTooSlow]
                public async Task RemovesAllNativeCalendarsFromTheCalendarsProperty()
                {
                    var externalCalendars = Enumerable
                        .Range(0, 9)
                        .Select(id => new ExternalCalendar(
                            id,
                            0,
                            $"Calendar #{id}",
                            $"Calendar #{id}",
                            false,
                            false));

                    var nativeCalendars = Enumerable
                        .Range(0, 9)
                        .Select(id => new UserCalendar(
                            id.ToString(),
                            $"Calendar #{id}",
                            $"Source #{id % 3}",
                            false));
                    var externalCalendarsObservable = Observable.Return(externalCalendars);
                    var nativeCalendarsObservable = Observable.Return(nativeCalendars);

                    DataSource.ExternalCalendars.GetAll().Returns(externalCalendarsObservable);
                    InteractorFactory.GetUserCalendars().Execute().Returns(nativeCalendarsObservable);
                    var calendarsObserver = TestScheduler.CreateObserver<ImmutableCalendarSectionModel>();
                    ViewModel.Calendars.Subscribe(calendarsObserver);
                    await ViewModel.Initialize();

                    UserPreferences.CalendarIntegrationEnabled().Returns(false);
                    ViewModel.ToggleNativeCalendarIntegration.Execute();
                    TestScheduler.Start();

                    //The first calendar list is empty, so the second one actually holds initial values
                    calendarsObserver.Values().ElementAt(1).Should().HaveCount(6);
                    calendarsObserver.Values().Last().Should().HaveCount(3);
                }
            }

            public sealed class WhenEnablingTheCalendarIntegration : CalendarSettingsViewModelTest
            {
                public WhenEnablingTheCalendarIntegration()
                {
                    UserPreferences.CalendarIntegrationEnabled().Returns(false);
                    PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                }

                [Fact, LogIfTooSlow]
                public async Task SetsCalendarIntegrationEnabled()
                {
                    await ViewModel.Initialize();

                    ViewModel.ToggleNativeCalendarIntegration.Execute();
                    TestScheduler.Start();

                    UserPreferences.Received().SetCalendarIntegrationEnabled(true);
                }

                [Fact, LogIfTooSlow]
                public async Task ReloadsTheCalendars()
                {
                    UserPreferences
                        .When(preferences => preferences.SetCalendarIntegrationEnabled(true))
                        .Do(_ => UserPreferences.CalendarIntegrationEnabled().Returns(true));
                    await ViewModel.Initialize();

                    ViewModel.ToggleNativeCalendarIntegration.Execute();
                    TestScheduler.Start();

                    InteractorFactory.GetUserCalendars().Received().Execute();
                }

                public sealed class WhenCalendarPermissionIsNotGranted : CalendarSettingsViewModelTest
                {
                    public WhenCalendarPermissionIsNotGranted()
                    {
                        UserPreferences.CalendarIntegrationEnabled().Returns(false);
                        PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(false));
                    }

                    [Fact, LogIfTooSlow]
                    public async Task AsksForCalendarPermission()
                    {
                        await ViewModel.Initialize();
                        var view = Substitute.For<IView>();
                        ViewModel.AttachView(view);

                        ViewModel.ToggleNativeCalendarIntegration.Execute();
                        TestScheduler.Start();

                        view.Received().RequestCalendarAuthorization(true);
                    }

                    [Fact, LogIfTooSlow]
                    public async Task EnablesCalendarIntegrationIfPermissionRequestIsApproved()
                    {
                        await ViewModel.Initialize();
                        var view = Substitute.For<IView>();
                        view.RequestCalendarAuthorization(true).Returns(Observable.Return(true));
                        ViewModel.AttachView(view);

                        ViewModel.ToggleNativeCalendarIntegration.Execute();
                        TestScheduler.Start();

                        UserPreferences.Received().SetCalendarIntegrationEnabled(true);
                    }

                    [Fact, LogIfTooSlow]
                    public async Task ReloadsTheCalendarsIfPermissionRequestIsApproved()
                    {
                        await ViewModel.Initialize();
                        var view = Substitute.For<IView>();
                        view.RequestCalendarAuthorization(true).Returns(Observable.Return(true));
                        view.When(view => view.RequestCalendarAuthorization(true))
                            .Do(_ => UserPreferences.CalendarIntegrationEnabled().Returns(true));
                        ViewModel.AttachView(view);

                        ViewModel.ToggleNativeCalendarIntegration.Execute();
                        TestScheduler.Start();

                        InteractorFactory.GetUserCalendars().Received().Execute();
                    }

                    [Fact, LogIfTooSlow]
                    public async Task DoesNotEnableCalendarIntegrationIfPermissionRequestIsDenied()
                    {
                        await ViewModel.Initialize();
                        var view = Substitute.For<IView>();
                        view.RequestCalendarAuthorization(true).Returns(Observable.Return(false));
                        ViewModel.AttachView(view);

                        ViewModel.ToggleNativeCalendarIntegration.Execute();
                        TestScheduler.Start();

                        UserPreferences.DidNotReceive().SetCalendarIntegrationEnabled(true);
                    }

                    [Fact, LogIfTooSlow]
                    public async Task DoesNotReloadCalendarsIfPermissionRequestIsDenied()
                    {
                        await ViewModel.Initialize();
                        var view = Substitute.For<IView>();
                        view.RequestCalendarAuthorization(true).Returns(Observable.Return(false));
                        ViewModel.AttachView(view);

                        ViewModel.ToggleNativeCalendarIntegration.Execute();
                        TestScheduler.Start();

                        InteractorFactory.GetUserCalendars().DidNotReceive().Execute();
                    }
                }
            }
        }

        public sealed class TheAnalytics : CalendarSettingsViewModelTest
        {
            public TheAnalytics()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                UserPreferences.CalendarIntegrationEnabled().Returns(true);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksNumberOfLinkedCalendarsChanged()
            {
                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));

                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));

                await ViewModel.Initialize();
                var selectedIds = new[] { "0", "2", "4", "7" };

                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableNativeCalendarViewModel(calendar, false));

                ViewModel.SelectNativeCalendar.ExecuteSequentially(calendars)
                    .AppendAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                AnalyticsService.NumberOfLinkedCalendarsChanged.Received().Track(4);
                AnalyticsService.NumberOfLinkedCalendarsNewUser.DidNotReceiveWithAnyArgs().Track(4);
            }

            [Fact, LogIfTooSlow]
            public async Task TracksNumberOfLinkedCalendarsNewUser()
            {
                var initialSelectedIds = new List<string> { };
                UserPreferences.EnabledCalendarIds().Returns(initialSelectedIds);
                OnboardingStorage.IsFirstTimeConnectingCalendars().Returns(true);

                var userCalendars = Enumerable
                    .Range(0, 9)
                    .Select(id => new UserCalendar(
                        id.ToString(),
                        $"Calendar #{id}",
                        $"Source #{id % 3}",
                        false));

                InteractorFactory
                    .GetUserCalendars()
                    .Execute()
                    .Returns(Observable.Return(userCalendars));

                await ViewModel.Initialize();
                var selectedIds = new[] { "2", "4", "7" };

                var calendars = userCalendars
                    .Where(calendar => selectedIds.Contains(calendar.Id))
                    .Select(calendar => new SelectableNativeCalendarViewModel(calendar, false));

                ViewModel.SelectNativeCalendar.ExecuteSequentially(calendars)
                    .AppendAction(ViewModel.Save)
                    .Subscribe();

                TestScheduler.Start();

                ViewModel.ViewDisappeared();

                AnalyticsService.NumberOfLinkedCalendarsNewUser.Received().Track(3);
                AnalyticsService.NumberOfLinkedCalendarsChanged.DidNotReceiveWithAnyArgs().Track(3);
            }
        }
    }
}
