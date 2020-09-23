using System;

namespace Toggl.Core.UI.ViewModels.Settings
{
    public interface ICalendarSettingsKey : IEquatable<ICalendarSettingsKey>
    {
        long Identifier();
    }
}
