using System;

namespace Toggl.Shared.Models
{
    public interface ITimelineEvent : IIdentifiable
    {
        string Title { get; }

        string FileName { get; }

        DateTimeOffset Start { get; }

        long Duration { get; }
    }
}
