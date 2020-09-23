namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public sealed class NativeCalendarsToggleIntegrationViewModel : CalendarSettingsItemViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            public bool Equals(ICalendarSettingsKey other)
                => other is Key;

            public long Identifier()
                => -1101;
        }

        public bool NativeCalendarIntegrationEnabled { get; set; }

        public NativeCalendarsToggleIntegrationViewModel(bool nativeCalendarIntegrationEnabled)
        {
            Identity = new Key();
            NativeCalendarIntegrationEnabled = nativeCalendarIntegrationEnabled;
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
            => other is NativeCalendarsToggleIntegrationViewModel;
    }
}
