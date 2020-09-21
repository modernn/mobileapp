using System;
using Newtonsoft.Json;
using Toggl.Shared;
using Toggl.Shared.Models.Calendar;

namespace Toggl.Networking.Models.Calendar
{
    [Preserve(AllMembers = true)]
    internal sealed class ExternalCalendarEvent : IExternalCalendarEvent
    {
        internal sealed class ExternalCalendarEventColor
        {
            public string Foreground { get; set; }
            public string Background { get; set; }
        }

        [JsonProperty("event_id")]
        public string EventId { get; set; }

        [JsonProperty("ical_uid")]
        public string ICalId { get; set; }

        public string Title { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset EndTime { get; set; }

        public DateTimeOffset Updated { get; set; }

        public ExternalCalendarEventColor Color { get; set; }

        public string BackgroundColor => Color.Background;

        public string ForegroundColor => Color.Foreground;

        public ExternalCalendarEvent() { }

        public ExternalCalendarEvent(IExternalCalendarEvent entity)
        {
            EventId = entity.EventId;
            ICalId = entity.ICalId;
            Title = entity.Title;
            StartTime = entity.StartTime;
            EndTime = entity.EndTime;
            Updated = entity.Updated;
            Color = new ExternalCalendarEventColor
            {
                Background = entity.BackgroundColor,
                Foreground = entity.ForegroundColor,
            };
        }
    }
}
