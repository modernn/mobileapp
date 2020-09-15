using System.Windows;
using Toggl.Core.UI;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.Parameters;
using Toggl.Core.UI.ViewModels;
using Toggl.WPF.Startup;

namespace Toggl.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WpfDependencyContainer.EnsureInitialized();
            var dependencyContainer = WpfDependencyContainer.Instance;
            var app = new AppStart(dependencyContainer);
            app.LoadLocalizationConfiguration();
            var accessLevel = app.GetAccessLevel();
            app.SetupBackgroundSync();
            app.SetFirstOpened();


            if (accessLevel == AccessLevel.LoggedIn)
            {
                app.ForceFullSync();
            }

            var navigationService = dependencyContainer.NavigationService;

            switch (accessLevel)
            {
                case AccessLevel.AccessRestricted:
                    break;
                case AccessLevel.NotLoggedIn:
                    navigationService.Navigate<LoginViewModel, CredentialsParameter>(CredentialsParameter.Empty, null);
                    break;
                case AccessLevel.TokenRevoked:
                    break;
                case AccessLevel.LoggedIn:
                    dependencyContainer.UserAccessManager.LoginWithSavedCredentials();
                    navigationService.Navigate<MainTabBarViewModel, MainTabBarParameters>(MainTabBarParameters.Default, null);
                    break;
            }
        }
    }
}
