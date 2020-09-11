using System;
using Newtonsoft.Json;
using Toggl.Shared;
using Toggl.Shared.Models.Calendar;

namespace Toggl.Networking.Models.Calendar
{
    [Preserve(AllMembers = true)]
    internal sealed class ExternalCalendarEvent : IExternalCalendarEvent
    {
        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("ical_uid")]
        public string ICalId { get; set; }

        public string Title { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public DateTimeOffset Updated { get; set; }

        public string BackgroundColor { get; set; }

        public string ForegroundColor { get; set; }

        public ExternalCalendarEvent() { }

        public ExternalCalendarEvent(IExternalCalendarEvent entity)
        {
            EventId = entity.EventId;
            ICalId = entity.ICalId;
            Title = entity.Title;
            StartTime = entity.StartTime;
            EndTime = entity.EndTime;
            Updated = entity.Updated;
            BackgroundColor = entity.BackgroundColor;
            ForegroundColor = entity.ForegroundColor;
        }
    }
}
