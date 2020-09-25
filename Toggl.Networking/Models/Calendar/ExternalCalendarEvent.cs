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

        [JsonProperty("external_id")]
        public string ExternalId { get; set; }

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
            ExternalId = entity.ExternalId;
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
