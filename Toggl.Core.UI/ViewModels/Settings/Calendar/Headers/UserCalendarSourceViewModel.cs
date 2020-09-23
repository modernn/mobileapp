using Toggl.Core.Extensions;
using Toggl.Core.UI.ViewModels.Settings.Calendar;

namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public sealed class UserCalendarSourceViewModel : CalendarSettingsSectionViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            private string name { get; }

            public Key(string name)
            {
                this.name = name;
            }

            public bool Equals(ICalendarSettingsKey other)
                => other is Key otherKey && name == otherKey.name;

            public long Identifier()
                => name.GetHashCode();
        }

        public string Name { get; }

        public UserCalendarSourceViewModel(string name)
        {
            Identity = new Key(name);
            Name = name;
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!(other is UserCalendarSourceViewModel viewModel)) return false;

            return Name == viewModel.Name;
        }
    }
}
