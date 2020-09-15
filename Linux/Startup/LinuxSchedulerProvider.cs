using System.Reactive.Concurrency;
using Toggl.Shared;

namespace Linux
{
    internal class LinuxSchedulerProvider : ISchedulerProvider
    {
        public IScheduler MainScheduler { get; } = Scheduler.Default;

        public IScheduler DefaultScheduler { get; } = Scheduler.Default;

        public IScheduler BackgroundScheduler { get; } = Scheduler.Default;
    }
}
