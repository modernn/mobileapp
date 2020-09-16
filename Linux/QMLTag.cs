using Qml.Net;
using Toggl.Core.Models.Interfaces;

namespace Linux
{
    public class QMLTag
    {
        private IThreadSafeTag _tag;
        public QMLTag(IThreadSafeTag tag)
        {
            this._tag = tag;
        }
        [NotifySignal]
        public string Name
        {
            get
            {
                return _tag.Name;
            }
        }
    }
}
