using System;
namespace Toggl.Shared.Models.Calendar
{
    public interface IExternalCalendarEvent
    {
        string EventId { get; }
        string ICalId { get; }
        string Title { get; }

        DateTimeOffset StartTime { get; }
        DateTimeOffset EndTime { get; }
        DateTimeOffset Updated { get; }

        string BackgroundColor { get; }
        string ForegroundColor { get; }
    }
}
