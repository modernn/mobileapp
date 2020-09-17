using System;
using Realms;
using Toggl.Storage.Models;

namespace Toggl.Storage.Realm.Models
{
    public class RealmTimelineEvent : RealmObject, IDatabaseTimelineEvent, IModifiableId, IUpdatesFrom<IDatabaseTimelineEvent>
    {
        public RealmTimelineEvent()
        {

        }

        public RealmTimelineEvent(IDatabaseTimelineEvent entity, Realms.Realm realm)
        {
            SetPropertiesFrom(entity, realm);
        }

        public long Id { get; set; }
        public long? OriginalId { get; set; }
        public void ChangeId(long id)
        {
            Id = id;
        }

        public string Title { get; set; }
        public string FileName { get; set; }
        public DateTimeOffset Start { get; set; }
        public long Duration { get; set; }
        public void SetPropertiesFrom(IDatabaseTimelineEvent entity, Realms.Realm realm)
        {
            Id = entity.Id;
            Title = entity.Title;
            FileName = entity.FileName;
            Start = entity.Start;
            Duration = entity.Duration;
        }
    }
}
