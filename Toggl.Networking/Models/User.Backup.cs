﻿using Newtonsoft.Json;
using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Networking.Models
{
    internal sealed partial class User : IUser
    {
        [JsonIgnore]
        public bool ContainsBackup { get; set; }

        [JsonIgnore]
        public long? DefaultWorkspaceIdBackup { get; set; }

        [JsonIgnore]
        public BeginningOfWeek BeginningOfWeekBackup { get; set; }
    }
}