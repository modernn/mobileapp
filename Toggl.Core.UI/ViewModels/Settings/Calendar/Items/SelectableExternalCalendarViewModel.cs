using Toggl.Core.Models.Calendar;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public sealed class SelectableExternalCalendarViewModel : CalendarSettingsItemViewModel
    {
        private sealed class Key : ICalendarSettingsKey
        {
            private long id;

            public Key(long id)
            {
                this.id = id;
            }

            public bool Equals(ICalendarSettingsKey other)
                => other is Key otherKey && id == otherKey.id;

            public long Identifier()
                => id;
        }

        public long Id { get; }

        public string Name { get; }

        public bool Selected { get; set; }

        public SelectableExternalCalendarViewModel(IThreadSafeExternalCalendar calendar, bool selected)
        {
            Ensure.Argument.IsNotNull(calendar, nameof(calendar));
            Identity = new Key(calendar.Id);
            Id = calendar.Id;
            Name = calendar.Name;
            Selected = selected;
        }

        public override bool Equals(CalendarSettingsItemViewModel other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            if (!(other is SelectableExternalCalendarViewModel calendarViewModel)) return false;

            return Id == calendarViewModel.Id
                   && Name == calendarViewModel.Name
                   && Selected == calendarViewModel.Selected;
        }
    }
}
