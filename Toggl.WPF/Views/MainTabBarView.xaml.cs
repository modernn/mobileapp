using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared.Extensions;
using Toggl.WPF.Presentation;

namespace Toggl.WPF.Views
{
    public partial class MainTabBarView
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        private readonly Dictionary<Type, Func<ViewModel, UserControl>> tabViews =
            new Dictionary<Type, Func<ViewModel, UserControl>>
            {
                {typeof(MainViewModel), vm => new MainView((MainViewModel)vm)}
            };

        private readonly Dictionary<Type, string> tabHeaders =
            new Dictionary<Type, string>
            {
                {typeof(MainViewModel), Toggl.Shared.Resources.Timer }
            };

        public MainTabBarView(MainTabBarViewModel mainTabBarViewModel)
        {
            ViewModel = mainTabBarViewModel;
            InitializeComponent();

            TimerView.Bind(ViewModel.TimerViewModel.Value as TimerViewModel);

            createTabFor(ViewModel.MainViewModel)
                .GetAwaiter()
                .GetResult();

            async Task<UserControl> createTabFor(Lazy<ViewModel> lazyViewModel)
            {
                var viewModel = lazyViewModel.Value;
                var viewModelType = viewModel.GetType();
                if (!tabViews.ContainsKey(viewModelType)) return null;
                await viewModel.Initialize();
                var tabView = tabViews[viewModelType](viewModel);
                var tabItem = new TabItem
                {
                    Header = tabHeaders[viewModelType],
                    Content = tabView
                };
                TabControl.Items.Add(tabItem);
                return tabView;
            }
        }

        private void MainTabBarView_OnLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}

