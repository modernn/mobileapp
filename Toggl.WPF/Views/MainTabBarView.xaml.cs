using System.Reactive.Disposables;
using System.Windows;
using Toggl.Core.UI.ViewModels;

namespace Toggl.WPF.Views
{
    public partial class MainTabBarView
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();
        public MainTabBarView(MainTabBarViewModel mainTabBarViewModel)
        {
            ViewModel = mainTabBarViewModel;
            InitializeComponent();
        }

        private void MainTabBarView_OnLoaded(object sender, RoutedEventArgs e)
        {

        }
    }
}

