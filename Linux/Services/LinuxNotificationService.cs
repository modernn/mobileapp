using System;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Services;

namespace Linux
{
    internal class LinuxNotificationService : INotificationService
    {
        public IObservable<Unit> Schedule(IImmutableList<global::Toggl.Shared.Notification> notifications)
        {
            return Observable.Return(Unit.Default);
        }

        public IObservable<Unit> UnscheduleAllNotifications()
        {
            return Observable.Return(Unit.Default);
        }
    }
}
