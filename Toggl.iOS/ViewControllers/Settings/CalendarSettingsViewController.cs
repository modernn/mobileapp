using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.iOS.Extensions;
using Toggl.iOS.Extensions.Reactive;
using Toggl.iOS.ViewSources;
using Toggl.Shared.Extensions;
using Toggl.Shared;
using UIKit;

namespace Toggl.iOS.ViewControllers.Settings
{
    public sealed partial class CalendarSettingsViewController : ReactiveViewController<CalendarSettingsViewModel>
    {
        private const int tableViewHeaderHeight = 67;

        public CalendarSettingsViewController(CalendarSettingsViewModel viewModel)
            : base(viewModel, nameof(CalendarSettingsViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            NavigationItem.Title = Resources.CalendarSettingsTitle;

            var header = CalendarSettingsTableViewHeader.Create();
            UserCalendarsTableView.TableHeaderView = header;
            UserCalendarsTableView.AllowsSelection = false;

            var source = new SelectUserCalendarsTableViewSource(
                UserCalendarsTableView,
                ViewModel.SelectExternalCalendar,
                ViewModel.ToggleNativeCalendarIntegration,
                ViewModel.SelectNativeCalendar);

            UserCalendarsTableView.Source = source;

            ViewModel.Calendars
                .Subscribe(UserCalendarsTableView.Rx().ReloadSections(source))
                .DisposedBy(DisposeBag);

            IosDependencyContainer.Instance.BackgroundService
                .AppResumedFromBackground
                .SelectUnit()
                .Subscribe(ViewModel.Reload.Inputs)
                .DisposedBy(DisposeBag);

            if (ViewModel is IndependentCalendarSettingsViewModel)
            {
                NavigationItem.RightBarButtonItem = ReactiveNavigationController.CreateSystemItem(
                    Resources.Done, UIBarButtonItemStyle.Done, Close);
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            ViewModel.Save.Execute();
            base.ViewDidDisappear(animated);
        }

        public override Task<bool> DismissFromNavigationController()
            => Task.FromResult(true);
    }
}
