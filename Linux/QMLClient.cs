using Toggl.Core.Models.Interfaces;

namespace Linux
{
    public class QMLClient
    {
        private IThreadSafeClient _client;
        public QMLClient(IThreadSafeClient client)
        {
            this._client = client;
        }

        public string Name
        {
            get
            {
                return _client.Name;
            }
        }
    }
}
