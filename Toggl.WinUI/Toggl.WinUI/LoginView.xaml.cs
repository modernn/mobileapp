using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Toggl.WinUI.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Toggl.WinUI
{
    public sealed partial class LoginView : UserControl
    {
        internal LoginViewModel ViewModel
        {
            get => DataContext as LoginViewModel;
            set => DataContext = value;
        }
        public LoginView()
        {
            this.InitializeComponent();
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            ViewModel.Password = Shared.Password.From(passwordTextBox.Password);
        }
    }
}
