using Toggl.Core.Models.Interfaces;

namespace Linux
{
    public class QMLTask
    {
        private IThreadSafeTask _task;

        public QMLTask(IThreadSafeTask task)
        {
            this._task = task;
        }
    }
}
