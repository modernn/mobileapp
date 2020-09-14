using System.Windows.Controls;
using Toggl.Core.UI.ViewModels;

namespace Toggl.WPF.Presentation
{
    public partial class MainView
    {
        public MainView(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}

