using Toggl.Core.UI.ViewModels.Settings.Calendar;

namespace Toggl.Core.UI.ViewModels.Settings
{
    public sealed class NativeCalendarsMainSectionViewModel : CalendarSettingsSectionViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            public bool Equals(ICalendarSettingsKey other)
                => other is Key;

            public long Identifier()
                => -1100;
        }

        public NativeCalendarsMainSectionViewModel()
        {
            Identity = new Key();
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
            => other is NativeCalendarsMainSectionViewModel;
    }
}
