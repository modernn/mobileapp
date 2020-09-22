using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Calendar;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public sealed class SetExternalCalendarSelectedInteractor : IInteractor<Unit>
    {
        private readonly ITogglDataSource dataSource;
        private readonly long calendarId;
        private readonly bool selected;

        public SetExternalCalendarSelectedInteractor(ITogglDataSource dataSource, long calendarId, bool selected)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            this.dataSource = dataSource;
            this.calendarId = calendarId;
            this.selected = selected;
        }

        public Unit Execute()
        {
            executeAsync();
            return Unit.Default;
        }

        private async void executeAsync()
        {
            var calendar = await dataSource.ExternalCalendars.GetById(calendarId);
            if (calendar == null)
                return;

            calendar = ExternalCalendar.SetSelected(calendar, selected);
            dataSource.ExternalCalendars.Update(calendar);
        }
    }
}
