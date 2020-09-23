using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders
{
    public sealed class NativeCalendarsToggleIntegrationViewHolder : BaseRecyclerViewHolder<CalendarSettingsItemViewModel>
    {
        private Switch toggle;
        private TextView label;

        public NativeCalendarsToggleIntegrationViewHolder(View itemView) : base(itemView)
        {
        }

        public NativeCalendarsToggleIntegrationViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            toggle = ItemView.FindViewById<Switch>(Resource.Id.Switch);
            label = ItemView.FindViewById<TextView>(Resource.Id.Label);
        }

        protected override void UpdateView()
        {
            var viewModel = (NativeCalendarsToggleIntegrationViewModel) Item;
            toggle.Checked = viewModel.NativeCalendarIntegrationEnabled;
            label.Text = Resources.AllowAccess;
        }

        protected override void OnItemViewClick(object sender, EventArgs args)
        {
            base.OnItemViewClick(sender, args);
            toggle.Checked = !toggle.Checked;
        }
    }
}
