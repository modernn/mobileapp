using Foundation;
using System;
using System.Collections.Immutable;
using System.Reactive;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.iOS.Cells;
using Toggl.iOS.Cells.Calendar;
using Toggl.iOS.Cells.Calendar.Settings;
using Toggl.iOS.Views.Interfaces;
using Toggl.Shared;
using Toggl.Shared.Extensions;
using UIKit;

namespace Toggl.iOS.ViewSources
{
    using CalendarSectionModel = SectionModel<CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel>;

    public sealed class SelectUserCalendarsTableViewSource : BaseTableViewSource<CalendarSectionModel, CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel>
    {
        private const int cardHeight = 118;
        private const int rowHeight = 48;
        private const int calendarsSourceHeaderHeight = 48;
        private const int externalCalendarsHeaderHeight = 56;
        private const int nativeCalendarsHeaderHeight = 81;
        private readonly InputAction<SelectableExternalCalendarViewModel> selectExternalCalendar;
        private readonly ViewAction toggleNativeCalendarsIntegration;
        private readonly InputAction<SelectableNativeCalendarViewModel> selectNativeCalendar;

        public SelectUserCalendarsTableViewSource(
            UITableView tableView,
            InputAction<SelectableExternalCalendarViewModel> selectExternalCalendar,
            ViewAction toggleNativeCalendarsIntegration,
            InputAction<SelectableNativeCalendarViewModel> selectNativeCalendar)
            : base(ImmutableList<CalendarSectionModel>.Empty)
        {
            Ensure.Argument.IsNotNull(selectNativeCalendar, nameof(selectNativeCalendar));

            this.selectExternalCalendar = selectExternalCalendar;
            this.toggleNativeCalendarsIntegration = toggleNativeCalendarsIntegration;
            this.selectNativeCalendar = selectNativeCalendar;

            tableView.RegisterNibForCellReuse(ExternalCalendarsCardInfoViewCell.Nib, ExternalCalendarsCardInfoViewCell.Identifier);
            tableView.RegisterNibForCellReuse(NativeCalendarsToggleIntegrationViewCell.Nib, NativeCalendarsToggleIntegrationViewCell.Identifier);
            tableView.RegisterNibForCellReuse(SelectableExternalCalendarViewCell.Nib, SelectableExternalCalendarViewCell.Identifier);
            tableView.RegisterNibForCellReuse(SelectableNativeCalendarViewCell.Nib, SelectableNativeCalendarViewCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(ExternalCalendarsMainHeaderViewCell.Nib, ExternalCalendarsMainHeaderViewCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(NativeCalendarsMainHeaderViewCell.Nib, NativeCalendarsMainHeaderViewCell.Identifier);
            tableView.RegisterNibForHeaderFooterViewReuse(UserCalendarListHeaderViewCell.Nib, UserCalendarListHeaderViewCell.Identifier);
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            var viewModel = ModelAt(indexPath);

            switch (viewModel)
            {
                case ExternalCalendarsCardInfoViewModel _:
                    return cardHeight;
                default:
                    return rowHeight;
            }
        }

        public override nfloat GetHeightForHeader(UITableView tableView, nint section)
        {
            var viewModel = HeaderOf(section);

            switch (viewModel)
            {
                case UserCalendarSourceViewModel _:
                    return calendarsSourceHeaderHeight;
                case ExternalCalendarsMainSectionViewModel externalCalendarsMainSection:
                    return externalCalendarsMainSection.ShouldShowCard ? 0 : externalCalendarsHeaderHeight;
                case NativeCalendarsMainSectionViewModel _:
                    return nativeCalendarsHeaderHeight;
                default:
                    return 0;
            }
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var viewModel = HeaderOf(section);

            switch (viewModel)
            {
                case UserCalendarSourceViewModel calendarsSectionViewModel:
                    var calendarsHeader = (BaseTableHeaderFooterView<UserCalendarSourceViewModel>)tableView.DequeueReusableHeaderFooterView(
                        UserCalendarListHeaderViewCell.Identifier);
                    calendarsHeader.Item = calendarsSectionViewModel;
                    return calendarsHeader;

                case ExternalCalendarsMainSectionViewModel externalCalendarsMainSection:
                    if (externalCalendarsMainSection.ShouldShowCard)
                        return null;

                    var externalCalendarsHeader = (ExternalCalendarsMainHeaderViewCell)tableView.DequeueReusableHeaderFooterView(
                        ExternalCalendarsMainHeaderViewCell.Identifier);
                    return externalCalendarsHeader;

                case NativeCalendarsMainSectionViewModel _:
                    var nativeCalendarsHeader = (NativeCalendarsMainHeaderViewCell)tableView.DequeueReusableHeaderFooterView(
                        NativeCalendarsMainHeaderViewCell.Identifier);
                    return nativeCalendarsHeader;
            }

            return null;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var viewModel = ModelAt(indexPath);

            switch (viewModel)
            {
                case SelectableExternalCalendarViewModel externalCalendarViewModel:
                    var externalCalendarCell = (SelectableExternalCalendarViewCell)tableView.DequeueReusableCell(
                        SelectableExternalCalendarViewCell.Identifier, indexPath);
                    externalCalendarCell.Item = externalCalendarViewModel;
                    externalCalendarCell.SelectCalendar = selectExternalCalendar;
                    return externalCalendarCell;

                case SelectableNativeCalendarViewModel nativeCalendarViewModel:
                    var nativeCalendarCell = (SelectableNativeCalendarViewCell)tableView.DequeueReusableCell(
                        SelectableNativeCalendarViewCell.Identifier, indexPath);
                    nativeCalendarCell.Item = nativeCalendarViewModel;
                    nativeCalendarCell.SelectCalendar = selectNativeCalendar;
                    return nativeCalendarCell;

                case ExternalCalendarsCardInfoViewModel externalCalendarsCardViewModel:
                    var externalCalendarsCardCell = (ExternalCalendarsCardInfoViewCell)tableView.DequeueReusableCell(
                        ExternalCalendarsCardInfoViewCell.Identifier, indexPath);
                    externalCalendarsCardCell.Item = externalCalendarsCardViewModel;
                    return externalCalendarsCardCell;

                case NativeCalendarsToggleIntegrationViewModel nativeCalendarsPermissionViewModel:
                    var nativeCalendarsPermissionCell = (NativeCalendarsToggleIntegrationViewCell)tableView.DequeueReusableCell(
                        NativeCalendarsToggleIntegrationViewCell.Identifier, indexPath);
                    nativeCalendarsPermissionCell.Item = nativeCalendarsPermissionViewModel;
                    nativeCalendarsPermissionCell.ToggleNativeCalendarIntegration = toggleNativeCalendarsIntegration;
                    return nativeCalendarsPermissionCell;
            }

            return null;
        }
    }
}
