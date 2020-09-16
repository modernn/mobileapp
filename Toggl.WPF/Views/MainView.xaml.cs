using Toggl.Core.UI.ViewModels;

namespace Toggl.WPF.Views
{
    public partial class MainView
    {
        public MainView(MainViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            TimeEntriesView.Bind(ViewModel.TimeEntriesViewModel, ViewModel.ContinueTimeEntry);
        }
    }
}

