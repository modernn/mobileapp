using System;
using System.Collections.Immutable;
using System.Linq;
using Android.Views;
using Toggl.Core.Extensions;
using Toggl.Core.UI.Collections;
using Toggl.Core.UI.ViewModels.Settings;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Droid.ViewHolders;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Adapters
{
    using CalendarSettingsSection = AnimatableSectionModel<CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel, ICalendarSettingsKey>;

    public sealed class UserCalendarsRecyclerAdapter : BaseRecyclerAdapter<CalendarSettingsItemViewModel>
    {
        public const int externalCalendarsMainSectionViewType = 0;
        public const int externalCalendarsCardInfoViewType = 1;
        public const int externalCalendarsItemViewType = 2;
        public const int nativeCalendarsMainSectionViewType = 3;
        public const int nativeCalendarsToggleIntegrationViewType = 4;
        public const int nativeCalendarsItemViewType = 5;
        public const int userCalendarsHeaderViewType = 6;

        private readonly InputAction<SelectableExternalCalendarViewModel> selectExternalCalendar;
        private readonly ViewAction toggleNativeCalendarsIntegration;
        private readonly InputAction<SelectableNativeCalendarViewModel> selectNativeCalendar;

        public UserCalendarsRecyclerAdapter(
            InputAction<SelectableExternalCalendarViewModel> selectExternalCalendar,
            ViewAction toggleNativeCalendarsIntegration,
            InputAction<SelectableNativeCalendarViewModel> selectNativeCalendar)
        {
            this.selectExternalCalendar = selectExternalCalendar;
            this.toggleNativeCalendarsIntegration = toggleNativeCalendarsIntegration;
            this.selectNativeCalendar = selectNativeCalendar;
        }

        public void UpdateCalendars(IImmutableList<SectionModel<CalendarSettingsSectionViewModel, CalendarSettingsItemViewModel>> sections)
        {
            var flattenItems = sections.Aggregate(
                ImmutableList<CalendarSettingsItemViewModel>.Empty,
                (acc, nextSection) =>
                {
                    if (nextSection.Header is ExternalCalendarsMainSectionViewModel externalCalendarsMainSection)
                    {
                        return externalCalendarsMainSection.ShouldShowCard
                                ? acc.AddRange(nextSection.Items)
                                : acc.AddRange(new []{ nextSection.Header });
                    }
                    return acc.AddRange(nextSection.Items.Prepend(nextSection.Header));
                });
            SetItems(flattenItems);
        }

        public override int GetItemViewType(int position)
        {

            var viewModel = GetItem(position);
            switch (viewModel)
            {
                case ExternalCalendarsMainSectionViewModel _:
                    return externalCalendarsMainSectionViewType;
                case ExternalCalendarsCardInfoViewModel _:
                    return externalCalendarsCardInfoViewType;
                case SelectableExternalCalendarViewModel _ :
                    return externalCalendarsItemViewType;
                case NativeCalendarsMainSectionViewModel _:
                    return nativeCalendarsMainSectionViewType;
                case NativeCalendarsToggleIntegrationViewModel _:
                    return nativeCalendarsToggleIntegrationViewType;
                case SelectableNativeCalendarViewModel _:
                    return nativeCalendarsItemViewType;
                case UserCalendarSourceViewModel _:
                    return userCalendarsHeaderViewType;
                default:
                    throw new Exception($"Invalid item type {viewModel.GetSafeTypeName()}");
            }
        }

        protected override BaseRecyclerViewHolder<CalendarSettingsItemViewModel> CreateViewHolder(ViewGroup parent, LayoutInflater inflater, int viewType)
        {
            View view;
            switch (viewType)
            {
                case externalCalendarsMainSectionViewType:
                    view = inflater.Inflate(Resource.Layout.ExternalCalendarsMainSectionHeader, parent, false);
                    return new ExternalCalendarsMainSectionViewHolder(view);
                case externalCalendarsCardInfoViewType:
                    view = inflater.Inflate(Resource.Layout.ExternalCalendarsCardInfoView, parent, false);
                    return new ExternalCalendarsCardInfoViewHolder(view);
                case externalCalendarsItemViewType:
                    view = inflater.Inflate(Resource.Layout.UserCalendarItem, parent, false);
                    return new SelectableExternalCalendarViewHolder(view);
                case nativeCalendarsMainSectionViewType:
                    view = inflater.Inflate(Resource.Layout.NativeCalendarsMainSectionHeader, parent, false);
                    return new NativeCalendarsMainSectionViewHolder(view);
                case nativeCalendarsToggleIntegrationViewType:
                    view = inflater.Inflate(Resource.Layout.NativeCalendarsToggleIntegrationView, parent, false);
                    return new NativeCalendarsToggleIntegrationViewHolder(view);
                case nativeCalendarsItemViewType:
                    view = inflater.Inflate(Resource.Layout.UserCalendarItem, parent, false);
                    return new SelectableNativeCalendarViewHolder(view);
                case userCalendarsHeaderViewType:
                    view = inflater.Inflate(Resource.Layout.UserCalendarHeader, parent, false);
                    return new UserCalendarHeaderViewHolder(view);
                default:
                    throw new Exception($"Invalid view type: {viewType}");
            }
        }
    }
}
