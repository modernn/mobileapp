using Qml.Net;
using Toggl.Core.Models.Interfaces;

namespace Linux
{
    public class QMLUser
    {
        private IThreadSafeUser _user;
        public QMLUser(IThreadSafeUser user)
        {
            this._user = user;
        }
        [NotifySignal]
        public string Email
        {
            get
            {
                return _user.Email.ToString();
            }
        }
    }
}
