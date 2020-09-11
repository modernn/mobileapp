using Newtonsoft.Json;
using Toggl.Shared;
using Toggl.Shared.Models.Calendar;

namespace Toggl.Networking.Models.Calendar
{
    [Preserve(AllMembers = true)]
    public sealed class CalendarIntegration : ICalendarIntegration
    {
        [JsonProperty("calendar_integration_id")]
        public long Id { get; set; }

        public string Email { get; set; }

        public string Provider { get; set; }
    }
}
