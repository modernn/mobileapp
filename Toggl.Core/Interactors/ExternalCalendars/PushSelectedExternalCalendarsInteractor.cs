using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Toggl.Core.DataSources;
using Toggl.Core.Models.Calendar;
using Toggl.Networking;
using Toggl.Networking.Exceptions;
using Toggl.Shared;

namespace Toggl.Core.Interactors
{
    public class PushSelectedExternalCalendarsInteractor : IInteractor<Task<Unit>>
    {
        private readonly ITogglDataSource dataSource;
        private readonly ITogglApi api;

        public PushSelectedExternalCalendarsInteractor(ITogglDataSource dataSource, ITogglApi api)
        {
            Ensure.Argument.IsNotNull(dataSource, nameof(dataSource));
            Ensure.Argument.IsNotNull(api, nameof(api));

            this.dataSource = dataSource;
            this.api = api;
        }

        public async Task<Unit> Execute()
        {
            var calendarsToSync = await dataSource.ExternalCalendars.GetAll(calendar => calendar.NeedsSync);

            foreach (var calendar in calendarsToSync)
            {
                try
                {
                    var updatedCalendar = await api.ExternalCalendars.SetCalendarSelected(calendar.IntegrationId, calendar.Id, calendar.Selected);
                    var threadSafeCalendar = ExternalCalendar.From(updatedCalendar);
                    await dataSource.ExternalCalendars.Update(threadSafeCalendar);
                }
                catch (ApiException)
                {
                    // Ignore
                    // This exception can happen when the integration was deleted
                    // but since we don't a proper way to resolve conflicts when
                    // syncing calendar data (i.e: no `At` property), we can ignore
                    // this exception and then fetch the calendars again
                }
            }

            return Unit.Default;
        }
    }
}
