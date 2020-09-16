using Qml.Net;
using System;
using Toggl.Core.Models.Interfaces;

namespace Linux
{
    public class QMLTimeEntry
    {
        private IThreadSafeTimeEntry _timeEntry;

        public QMLTimeEntry(IThreadSafeTimeEntry timeEntry)
        {
            this._timeEntry = timeEntry;
        }

        [NotifySignal]
        public string Description {
            get
            {
                return _timeEntry.Description;
            }
        }
        [NotifySignal]
        public long Duration
        {
            get
            {
                return _timeEntry.Duration ?? -1;
            }
        }
        public DateTimeOffset Start
        {
            get
            {
                return _timeEntry.Start;
            }
        }
        [NotifySignal]
        public QMLProject Project
        {
            get
            {
                if (_timeEntry.Project != null)
                    return new QMLProject(_timeEntry.Project);
                return null;
            }
        }
        [NotifySignal]
        public QMLTask Task
        {
            get
            {
                if (_timeEntry.Task != null)
                    return new QMLTask(_timeEntry.Task);
                return null;
            }
        }
        [NotifySignal]
        public QMLUser User
        {
            get
            {
                if (_timeEntry.User != null)
                    return new QMLUser(_timeEntry.User);
                return null;
            }
        }
    }
}
