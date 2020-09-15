using System;
using Toggl.Storage.Settings;

namespace Linux
{
    internal class LinuxLastTimeUsageStorage : ILastTimeUsageStorage
    {
        public DateTimeOffset? LastSyncAttempt { get; set; }
        public DateTimeOffset? LastSuccessfulSync { get; set; }
        public DateTimeOffset? LastLogin { get; set; }
        public DateTimeOffset? LastTimePlaceholdersWereCreated { get; set; }
        public DateTimeOffset? LastTimeExternalCalendarsSynced { get; set; }

        public void SetFullSyncAttempt(DateTimeOffset now)
        {
            LastSyncAttempt = now;
        }

        public void SetLastTimeExternalCalendarsSynced(DateTimeOffset now)
        {
            LastTimeExternalCalendarsSynced = now;
        }

        public void SetLogin(DateTimeOffset now)
        {
            LastLogin = now;
        }

        public void SetPlaceholdersWereCreated(DateTimeOffset now)
        {
            LastTimePlaceholdersWereCreated = now;
        }

        public void SetSuccessfulFullSync(DateTimeOffset now)
        {
            LastSuccessfulSync = now;
        }
    }
}
