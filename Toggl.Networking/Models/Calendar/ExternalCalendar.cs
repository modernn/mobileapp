using Newtonsoft.Json;
using Toggl.Shared;
using Toggl.Shared.Models.Calendar;

namespace Toggl.Networking.Models.Calendar
{
    [Preserve(AllMembers = true)]
    internal sealed class ExternalCalendar : IExternalCalendar
    {
        [JsonProperty("calendar_id")]
        public long Id { get; set; }

        [JsonProperty("calendar_integration_id")]
        public long IntegrationId { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public ExternalCalendar() { }

        public ExternalCalendar(IExternalCalendar entity)
        {
            Id = entity.Id;
            IntegrationId = entity.IntegrationId;
            ExternalId = entity.ExternalId;
            Name = entity.Name;
            Selected = entity.Selected;
        }
    }
}
