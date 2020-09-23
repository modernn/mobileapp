using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using Toggl.Core.UI.ViewModels.Settings.Calendar;

namespace Toggl.Droid.ViewHolders
{
    public class UserCalendarHeaderViewHolder : BaseRecyclerViewHolder<CalendarSettingsItemViewModel>
    {
        private TextView sourceName;

        public UserCalendarHeaderViewHolder(View itemView)
            : base(itemView)
        {
        }

        public UserCalendarHeaderViewHolder(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override void InitializeViews()
        {
            sourceName = ItemView.FindViewById<TextView>(Resource.Id.CalendarSource);
        }

        protected override void UpdateView()
        {
            var viewModel = (UserCalendarSourceViewModel) Item;
            sourceName.Text = viewModel.Name;
        }
    }
}
