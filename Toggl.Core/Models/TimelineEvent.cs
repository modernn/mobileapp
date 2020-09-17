using System;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage.Models;

namespace Toggl.Core.Models
{
    public class TimelineEvent : IThreadSafeTimelineEvent
    {
        public static TimelineEvent From(IDatabaseTimelineEvent databaseTimelineEvent)
        {
            return new TimelineEvent(
                databaseTimelineEvent.Id,
                databaseTimelineEvent.Title,
                databaseTimelineEvent.FileName,
                databaseTimelineEvent.Start,
                databaseTimelineEvent.Duration);
        }

        private TimelineEvent(long id, string title, string fileName, DateTimeOffset start, long duration)
        {
            Id = id;
            Title = title;
            FileName = fileName;
            Start = start;
            Duration = duration;
        }

        public long Id { get; }
        public string Title { get; }
        public string FileName { get; }
        public DateTimeOffset Start { get; }
        public long Duration { get; }
    }
}
