using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
              WindowSoftInputMode = SoftInput.AdjustResize,
              ScreenOrientation = ScreenOrientation.Portrait,
              ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class CalendarSettingsActivity : ReactiveActivity<CalendarSettingsViewModel>
    {
        protected CalendarSettingsActivity(ActivityTransitionSet transitions) : base(
            Resource.Layout.CalendarSettingsActivity,
            Resource.Style.AppTheme,
            transitions)
        {
        }

        public CalendarSettingsActivity() : base(
            Resource.Layout.CalendarSettingsActivity,
            Resource.Style.AppTheme,
            Transitions.SlideInFromRight)
        { }

        public CalendarSettingsActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            ViewModel
                .Calendars
                .Subscribe(userCalendarsAdapter.UpdateCalendars)
                .DisposedBy(DisposeBag);

            userCalendarsAdapter
                .ItemTapObservable
                .Where(viewModel => viewModel is SelectableNativeCalendarViewModel)
                .Cast<SelectableNativeCalendarViewModel>()
                .Subscribe(ViewModel.SelectNativeCalendar.Inputs)
                .DisposedBy(DisposeBag);

            userCalendarsAdapter
                .ItemTapObservable
                .Where(viewModel => viewModel is SelectableExternalCalendarViewModel)
                .Cast<SelectableExternalCalendarViewModel>()
                .Subscribe(ViewModel.SelectExternalCalendar.Inputs)
                .DisposedBy(DisposeBag);

            userCalendarsAdapter
                .ItemTapObservable
                .Where(viewModel => viewModel is NativeCalendarsToggleIntegrationViewModel)
                .Cast<NativeCalendarsToggleIntegrationViewModel>()
                .SelectUnit()
                .Subscribe(ViewModel.ToggleNativeCalendarIntegration.Inputs)
                .DisposedBy(DisposeBag);
        }
    }
}
