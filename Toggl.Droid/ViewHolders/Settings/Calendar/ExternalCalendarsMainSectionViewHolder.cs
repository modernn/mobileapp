using System;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Toggl.Core.UI.ViewModels.Settings.Calendar;
using Toggl.Shared;

namespace Toggl.Droid.ViewHolders
{
    public sealed class ExternalCalendarsMainSectionViewHolder : BaseRecyclerViewHolder<CalendarSettingsItemViewModel>
    {
        private TextView title;
        private TextView subtitle;

        public ExternalCalendarsMainSectionViewHolder(View itemView) : base(itemView)
        {
        }

        public ExternalCalendarsMainSectionViewHolder(IntPtr handle, JniHandleOwnership ownership) : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            title = ItemView.FindViewById<TextView>(Resource.Id.Title);
            subtitle = ItemView.FindViewById<TextView>(Resource.Id.Subtitle);
        }

        protected override void UpdateView()
        {
            title.Text = Resources.ExternalCalendarsTitle;
            subtitle.Text = Resources.ExternalCalendarsSubtitle;
        }
    }
}
