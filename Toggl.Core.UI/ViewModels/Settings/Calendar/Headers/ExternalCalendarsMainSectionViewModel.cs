namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public sealed class ExternalCalendarsMainSectionViewModel : CalendarSettingsSectionViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            public bool Equals(ICalendarSettingsKey other)
                => other is Key;

            public long Identifier()
                => -1000;
        }

        public bool ShouldShowCard { get; set; }

        public ExternalCalendarsMainSectionViewModel(bool shouldShowCard)
        {
            Identity = new Key();
            ShouldShowCard = shouldShowCard;
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
            => other is ExternalCalendarsMainSectionViewModel;
    }
}
