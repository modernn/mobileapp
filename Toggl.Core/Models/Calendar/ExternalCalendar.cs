using Toggl.Shared.Models.Calendar;
using Toggl.Storage;
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

        public bool NeedsSync { get; }

        public ExternalCalendar(long id, long integrationId, string externalId, string name, bool selected, bool needsSync)
        {
            Id = id;
            IntegrationId = integrationId;
            ExternalId = externalId;
            Name = name;
            Selected = selected;
            NeedsSync = needsSync;
        }

        public ExternalCalendar(IDatabaseExternalCalendar entity)
            : this(entity.Id, entity.IntegrationId, entity.ExternalId, entity.Name, entity.Selected, entity.NeedsSync)
        {
        }

        public static ExternalCalendar From(IDatabaseExternalCalendar entity)
            => new ExternalCalendar(entity);

        public static ExternalCalendar From(IExternalCalendar entity)
            => new ExternalCalendar(entity.Id, entity.IntegrationId, entity.ExternalId, entity.Name, entity.Selected, false);

        public static ExternalCalendar SetSelected(IThreadSafeExternalCalendar calendar, bool selected)
            => new ExternalCalendar(calendar.Id, calendar.IntegrationId, calendar.ExternalId, calendar.Name, selected, true);
    }
}
