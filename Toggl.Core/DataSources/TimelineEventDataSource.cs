using Toggl.Core.Models;
using Toggl.Core.Models.Interfaces;
using Toggl.Storage;
using Toggl.Storage.Models;

namespace Toggl.Core.DataSources
{
    public class TimelineEventDataSource : DataSource<IThreadSafeTimelineEvent, IDatabaseTimelineEvent>
    {
        public TimelineEventDataSource(IRepository<IDatabaseTimelineEvent> repository)
            : base(repository)
        {
        }

        protected override IThreadSafeTimelineEvent Convert(IDatabaseTimelineEvent entity)
            => TimelineEvent.From(entity);

        protected override ConflictResolutionMode ResolveConflicts(
            IDatabaseTimelineEvent first,
            IDatabaseTimelineEvent second)
            => ConflictResolutionMode.Ignore;
    }
}
