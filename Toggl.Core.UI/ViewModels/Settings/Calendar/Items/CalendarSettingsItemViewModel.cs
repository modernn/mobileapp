using Toggl.Core.UI.Collections;
using Toggl.Core.UI.Interfaces;

namespace Toggl.Core.UI.ViewModels.Settings.Calendar
{
    public abstract class CalendarSettingsItemViewModel : IDiffable<ICalendarSettingsKey>, IDiffableByIdentifier<CalendarSettingsItemViewModel>
    {
        public ICalendarSettingsKey Identity { get; protected set; }

        public long Identifier => Identity.Identifier();

        public abstract bool Equals(CalendarSettingsItemViewModel other);
    }
}
