using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toggl.Networking.ApiClients.Interfaces;
using Toggl.Networking.Models.Calendar;
using Toggl.Networking.Network;
using Toggl.Networking.Serialization;
using Toggl.Shared.Models.Calendar;

namespace Toggl.Networking.ApiClients
{
    internal class ExternalCalendarsApi : BaseApi, IExternalCalendarsApi
    {
        private readonly CalendarEndpoints endpoints;
        private readonly IJsonSerializer serializer;

        public ExternalCalendarsApi(Endpoints endpoints, IApiClient apiClient, IJsonSerializer serializer, Credentials credidentials)
            : base(apiClient, serializer, credidentials, endpoints.LoggedIn)
        {
            this.endpoints = endpoints.IntegrationsEndpoints.Calendars;
            this.serializer = serializer;
        }

        public Task<List<ICalendarIntegration>> GetIntegrations()
            => SendRequest<CalendarIntegration, ICalendarIntegration>(endpoints.GetIntegrations, AuthHeader);

        public Task<IExternalCalendarsPage> GetCalendars(
            long integrationId,
            string nextPageToken = null,
            long? limit = null)
            => SendRequest<ExternalCalendarsPage>(endpoints.GetAllCalendars(integrationId, nextPageToken, limit), AuthHeader)
                .ContinueWith(task => task.Result as IExternalCalendarsPage);

        public async Task<IExternalCalendar> SetCalendarSelected(long integrationId, long calendarId, bool selected)
        {
            var json = new Dictionary<string, bool> { { "selected", selected } };
            var body = serializer.Serialize(json, SerializationReason.Post);
            var updatedCalendar = await SendRequest<ExternalCalendar>(endpoints.SetCalendarSelected(integrationId, calendarId), AuthHeader, body);
            return updatedCalendar;
        }

        public Task<IExternalCalendarEventsPage> GetCalendarEvents(
            long integrationId,
            long calendarId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            string nextPageToken = null,
            long? limit = null)
            => SendRequest<ExternalCalendarEventsPage>(endpoints.GetAllCalendarEvents(integrationId, calendarId, startDate, endDate, nextPageToken, limit), AuthHeader)
                .ContinueWith(task => task.Result as IExternalCalendarEventsPage);
    }
}
