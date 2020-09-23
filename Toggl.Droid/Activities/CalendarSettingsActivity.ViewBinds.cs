using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Activities
{
    public partial class CalendarSettingsActivity
    {
        private RecyclerView calendarsRecyclerView;
        private TextView selectCalendarsMessage;
        private UserCalendarsRecyclerAdapter userCalendarsAdapter;

        protected override void InitializeViews()
        {
            selectCalendarsMessage = FindViewById<TextView>(Resource.Id.SelectCalendarsMessage);
            calendarsRecyclerView = FindViewById<RecyclerView>(Resource.Id.CalendarsRecyclerView);

            userCalendarsAdapter = new UserCalendarsRecyclerAdapter(
                ViewModel.SelectExternalCalendar,
                ViewModel.ToggleNativeCalendarIntegration,
                ViewModel.SelectNativeCalendar);

            calendarsRecyclerView.SetAdapter(userCalendarsAdapter);
            calendarsRecyclerView.SetLayoutManager(new LinearLayoutManager(this));

            selectCalendarsMessage.Text = Shared.Resources.SelectCalendarsMessage;

            SetupToolbar(Shared.Resources.CalendarSettingsTitle);

            calendarsRecyclerView.FitBottomMarginInset();
        }
    }
}
