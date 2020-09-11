using System;
namespace Toggl.Shared.Models.Calendar
{
    public interface IExternalCalendar : IIdentifiable
    {
        long IntegrationId { get; }
        string ExternalId { get; }
        string Name { get; }
        bool Selected { get; }
    }
}
