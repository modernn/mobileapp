using Toggl.Storage.Models.Calendar;

namespace Toggl.Core.Models.Calendar
{
    public sealed class ExternalCalendar : IThreadSafeExternalCalendar
    {
        public long Id { get; }

        public long IntegrationId { get; }

        public string ExternalId { get; }

        public string Name { get; }

        public bool Selected { get; }

        public ExternalCalendar(long id, long integrationId, string externalId, string name, bool selected)
        {
            Id = id;
            IntegrationId = integrationId;
            ExternalId = externalId;
            Name = name;
            Selected = selected;
        }

        public ExternalCalendar(IDatabaseExternalCalendar entity) : this(entity.Id, entity.IntegrationId, entity.ExternalId, entity.Name, entity.Selected)
        {
        }

        public static ExternalCalendar From(IDatabaseExternalCalendar entity)
            => new ExternalCalendar(entity);
    }
}
