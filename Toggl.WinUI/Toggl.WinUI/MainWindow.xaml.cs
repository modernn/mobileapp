using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Xaml;
using Toggl.Core.UI.Navigation;
using Toggl.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Toggl.WinUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow(IViewModelLoader viewModelLoader)
        {
            this.InitializeComponent();
            Root.Children.Add(new LoginView() { DataContext = viewModelLoader.Load<LoginViewModel>() });
        }
    }
}
