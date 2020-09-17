using System;
using Toggl.Shared.Models;

namespace Toggl.Storage.Models
{
    public interface IDatabaseTimelineEvent : ITimelineEvent, IDatabaseModel
    {
    }
}
