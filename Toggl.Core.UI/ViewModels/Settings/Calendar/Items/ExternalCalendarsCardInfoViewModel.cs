namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public sealed class ExternalCalendarsCardInfoViewModel : CalendarSettingsItemViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            public bool Equals(ICalendarSettingsKey other)
                => other is Key;

            public long Identifier()
                => -1001;
        }

        public ExternalCalendarsCardInfoViewModel()
        {
            Identity = new Key();
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
            => other is ExternalCalendarsCardInfoViewModel;
    }
}
