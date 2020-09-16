using System;
using System.Threading.Tasks;
using System.Windows.Threading;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.WPF.Presentation;
using Toggl.WPF.Views;

namespace Toggl.WPF
{
    public sealed class MainWindowPresenter : IPresenter
    {
        private readonly MainWindow window;

        public MainWindowPresenter(MainWindow window)
        {
            this.window = window;
        }

        public bool CanPresent<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel)
            => true;

        public bool ChangePresentation(IPresentationChange presentationChange)
        {
            throw new NotImplementedException();
        }

        public Task Present<TInput, TOutput>(ViewModel<TInput, TOutput> viewModel, IView sourceView)
        {
            window.Dispatcher.Invoke(() =>
            {
                switch (viewModel)
                {
                    case LoginViewModel loginViewModel:
                        var loginView = new LoginView(loginViewModel);
                        window.SetMainView(loginView);
                        break;
                    case MainTabBarViewModel mainTabBarViewModel:
                        var mainTabBarView = new MainTabBarView(mainTabBarViewModel);
                        window.SetMainView(mainTabBarView);
                        break;
                }
            });
            return Task.CompletedTask;
        }
    }
}
