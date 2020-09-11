using System;
namespace Toggl.Shared.Models.Calendar
{
    public interface ICalendarIntegration
    {
        long Id { get; }

        public string Email { get; set; }

        public string Provider { get; set; }
    }
}
