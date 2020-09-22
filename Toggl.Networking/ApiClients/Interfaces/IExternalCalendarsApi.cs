using System;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Shared.Models.Calendar;

namespace Toggl.Networking.ApiClients.Interfaces
{
    public interface IExternalCalendarsApi
    {
        Task<List<ICalendarIntegration>> GetIntegrations();

        Task<IExternalCalendarsPage> GetCalendars(
            long integrationId,
            string nextPageToken = null,
            long? limit = null);

        Task<IExternalCalendar> SetCalendarSelected(
            long integrationId,
            long calendarId,
            bool selected);

        Task<IExternalCalendarEventsPage> GetCalendarEvents(
            long integrationId,
            long calendarId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            string nextPageToken = null,
            long? limit = null);
    }
}
