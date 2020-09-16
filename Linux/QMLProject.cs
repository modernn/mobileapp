using Qml.Net;
using Toggl.Core.Models.Interfaces;

namespace Linux
{
    public class QMLProject
    {
        private IThreadSafeProject _project;

        public QMLProject(IThreadSafeProject project)
        {
            this._project = project;
        }

        [NotifySignal]
        public string Name
        {
            get
            {
                return _project.Name;
            }
        }
        public string Color
        {
            get
            {
                return _project.Color;
            }
        }

        [NotifySignal]
        public QMLClient Client
        {
            get
            {
                if (_project.Client != null)
                    return new QMLClient(_project.Client);
                return null;
            }
        }
    }
}
