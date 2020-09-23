using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public sealed class SelectableNativeCalendarViewModel : CalendarSettingsItemViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            private string id;

            public Key(string id)
            {
                this.id = id;
            }

            public bool Equals(ICalendarSettingsKey other)
                => other is Key otherKey && id == otherKey.id;

            public long Identifier()
                => id.GetHashCode() * 100000;
        }

        public string Id { get; }

        public string Name { get; }

        public string SourceName { get; }

        public bool Selected { get; set; }

        public SelectableNativeCalendarViewModel(UserCalendar calendar, bool selected)
        {
            Ensure.Argument.IsNotNull(calendar, nameof(calendar));
            Identity = new Key(calendar.Name);
            Id = calendar.Id;
            Name = calendar.Name;
            SourceName = calendar.SourceName;
            Selected = selected;
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!(other is SelectableNativeCalendarViewModel calendarViewModel)) return false;

            return Id == calendarViewModel.Id
                   && Name == calendarViewModel.Name
                   && SourceName == calendarViewModel.SourceName
                   && Selected == calendarViewModel.Selected;
        }
    }
}
