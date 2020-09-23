using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders
{
    public sealed class ExternalCalendarsCardInfoViewHolder : BaseRecyclerViewHolder<CalendarSettingsItemViewModel>
    {
        private TextView title;
        private TextView body;

        public ExternalCalendarsCardInfoViewHolder(View itemView) : base(itemView)
        {
        }

        public ExternalCalendarsCardInfoViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            title = ItemView.FindViewById<TextView>(Resource.Id.Title);
            body = ItemView.FindViewById<TextView>(Resource.Id.Body);
        }

        protected override void UpdateView()
        {
            title.Text = Resources.ExternalCalendarsCardInfoTitle;
            body.Text = Resources.ExternalCalendarsCardInfoBody;
        }
    }
}
