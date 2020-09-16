using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.Analytics;
using Toggl.Core.Login;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.Extensions;
using Toggl.Shared;
using Toggl.Networking;

namespace Toggl.WinUI.ViewModels
{
    internal class LoginViewModel : ViewModelWithInput<CredentialsParameter>
    {
        private readonly IUserAccessManager _userAccessManager;

        public LoginViewModel(INavigationService navigationService,
            IUserAccessManager userAccessManager,
            IAnalyticsService analyticsService) : base(navigationService)
        {
            _userAccessManager = userAccessManager;
            LoginCommand = ReactiveCommand.Create(() => userAccessManager
                    .Login(Email, Password)
                    .Track(analyticsService.Login, AuthenticationMethod.EmailAndPassword)
                    .Subscribe(OnAuthenticated, OnError, OnCompleted));
        }

        private async void OnAuthenticated(ITogglApi api)
        {
            
        }

        private async void OnError(Exception exception)
        {

        }

        private async void OnCompleted()
        {

        }

        public Email Email { get; set; }

        public Password Password { get; set; }

        public ReactiveCommand<Unit, IDisposable> LoginCommand { get; }
    }
}
