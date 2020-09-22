using System;
using System.Web;

namespace Toggl.Networking.Network
{
    internal struct CalendarEndpoints
    {
        private readonly Uri baseUrl;

        public CalendarEndpoints(Uri baseUrl)
        {
            this.baseUrl = baseUrl;
        }

        public Endpoint GetIntegrations => Endpoint.Get(baseUrl, "calendar");

        public Endpoint GetAllCalendars(long integrationId, string nextPageToken, long? limit)
        {
            var path = $"calendar/{integrationId}/calendars";

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                queryParams["page_token"] = nextPageToken;
            }
            queryParams["limit"] = limit.ToString();
            path += $"?{queryParams}";

            return Endpoint.Get(baseUrl, path);
        }

        public Endpoint SetCalendarSelected(long integrationId, long calendarId)
        {
            var path = $"calendar/{integrationId}/calendars/{calendarId}";
            return Endpoint.Patch(baseUrl, path);
        }

        public Endpoint GetAllCalendarEvents(
            long integrationId,
            long calendarId,
            DateTimeOffset startDate,
            DateTimeOffset endDate,
            string nextPageToken,
            long? limit)
        {
            var path = $"calendar/{integrationId}/calendars/{calendarId}/events";

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["start_date"] = $"{startDate:yyyy-MM-dd}";
            queryParams["end_date"] = $"{endDate:yyyy-MM-dd}";
            if (!string.IsNullOrEmpty(nextPageToken))
            {
                queryParams["page_token"] = nextPageToken;
            };
            queryParams["limit"] = limit.ToString();
            path += $"?{queryParams}";

            return Endpoint.Get(baseUrl, path);
        }
    }
}
