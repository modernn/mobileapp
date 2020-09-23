using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.ViewHolders
{
    public sealed class SelectableNativeCalendarViewHolder : BaseRecyclerViewHolder<CalendarSettingsItemViewModel>
    {
        private Switch toggle;
        private TextView calendarName;

        public SelectableNativeCalendarViewHolder(View itemView) : base(itemView)
        {
        }

        public SelectableNativeCalendarViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            toggle = ItemView.FindViewById<Switch>(Resource.Id.Switch);
            calendarName = ItemView.FindViewById<TextView>(Resource.Id.CalendarName);
        }

        protected override void UpdateView()
        {
            var viewModel = (SelectableNativeCalendarViewModel) Item;
            toggle.Checked = viewModel.Selected;
            calendarName.Text = viewModel.Name;
        }

        protected override void OnItemViewClick(object sender, EventArgs args)
        {
            base.OnItemViewClick(sender, args);
            toggle.Checked = !toggle.Checked;
        }
    }
}
