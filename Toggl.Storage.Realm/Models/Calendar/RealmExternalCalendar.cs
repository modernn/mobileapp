using Realms;
using Toggl.Shared.Models.Calendar;
using Toggl.Storage.Models.Calendar;

namespace Toggl.Storage.Realm.Models.Calendar
{
    public class RealmExternalCalendar : RealmObject, IDatabaseExternalCalendar, IUpdatesFrom<IDatabaseExternalCalendar>, IModifiableId
    {
        public RealmExternalCalendar() { }

        public RealmExternalCalendar(IDatabaseExternalCalendar entity, Realms.Realm realm)
        {
            Id = entity.Id;
            SetPropertiesFrom(entity, realm);
        }

        public RealmExternalCalendar(long id, IExternalCalendar entity, Realms.Realm realm)
        {
            Id = id;
            SetPropertiesFrom(entity, realm);
        }

        public long Id { get; set; }

        public long? OriginalId { get; set; }

        public long IntegrationId { get; set; }

        public string ExternalId { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }

        public bool NeedsSync { get; set; }


        public void SetPropertiesFrom(IDatabaseExternalCalendar entity, Realms.Realm realm)
        {
            IntegrationId = entity.IntegrationId;
            ExternalId = entity.ExternalId;
            Name = entity.Name;
            Selected = entity.Selected;
            NeedsSync = entity.NeedsSync;
        }

        public void SetPropertiesFrom(IExternalCalendar entity, Realms.Realm realm)
        {
            IntegrationId = entity.IntegrationId;
            ExternalId = entity.ExternalId;
            Name = entity.Name;
            Selected = entity.Selected;
            NeedsSync = false;
        }

        public void ChangeId(long id)
        {
            Id = id;
        }
    }
}
