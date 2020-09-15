using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using Qml.Net;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Shared;

namespace Linux
{
    public class Toggl : IPresenter
    {
        private string _status;
        private string _error;
        [NotifySignal]
        public string Status {
            get {
                return _status;
            }
            set {
                _status = value;
                _ = this.ActivateSignal("statusChanged");
            }
        }
        [NotifySignal]
        public string Error {
            get {
                return _error;
            }
            set {
                _error = value;
                _ = this.ActivateSignal("errorChanged");
            }
        }
        public Toggl()
        {
            QmlNetConfig.ShouldEnsureUIThread = false; // TODO this could break stuff

            Status = "Just constructed";
        }
        public void Bootup()
        {
            Status = "Booting up";
            var app = new AppStart(LinuxDependencyContainer.Instance);
            app.LoadLocalizationConfiguration();
            var accessLevel = app.GetAccessLevel();
            app.SetupBackgroundSync();
            app.SetFirstOpened();

            if (accessLevel == AccessLevel.LoggedIn)
                app.ForceFullSync();

            var navigationService = LinuxDependencyContainer.Instance.NavigationService;

            switch (accessLevel)
            {
                case AccessLevel.AccessRestricted:
                    break;
                case AccessLevel.NotLoggedIn:
                    navigationService.Navigate<LoginViewModel, CredentialsParameter, Unit>(CredentialsParameter.Empty, null);
                    break;
                case AccessLevel.TokenRevoked:
                    break;
                case AccessLevel.LoggedIn:
                    navigationService.Navigate<MainTabBarViewModel, MainTabBarParameters, Unit>(MainTabBarParameters.Default, null);
                    break;
            }
        }

        private LoginViewModel loginView;
        public void Login(string username, string password)
        {
            if (loginView != null)
            {
                Error = null;
                loginView.PasswordErrorMessage.Subscribe(errorMessage => { if (errorMessage.Length > 0) Error = errorMessage; });
                loginView.EmailErrorMessage.Subscribe(errorMessage => { if (errorMessage.Length > 0) Error = errorMessage; });
                loginView.LoginErrorMessage.Subscribe(errorMessage => { if (errorMessage.Length > 0) Error = errorMessage; });
                Console.Error.Write(Error);
                loginView.Email.Accept(Email.From(username));
                loginView.Password.Accept(Password.From(password));
                loginView.Login.Execute();
            }
        }

        public bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
        {
            return true;
        }

        public async Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            switch(viewModel)
            {
                case LoginViewModel loginViewModel:
                    Status = "LOGIN VIEW";
                    loginView = loginViewModel;
                    break;
                case MainTabBarViewModel mainTabBarViewModel:
                    Status = "MAIN TAB BAR VIEW";
                    loginView = null;
                    break;
            }
        }

        public bool ChangePresentation(IPresentationChange presentationChange)
        {
            return true;
        }
    }
}
