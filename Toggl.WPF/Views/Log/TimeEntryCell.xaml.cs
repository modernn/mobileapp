using System.Windows.Controls;
using Toggl.Core.UI.ViewModels.MainLog;

namespace Toggl.WPF.Views.Log
{
    public partial class TimeEntryCell : UserControl
    {
        public TimeEntryCell(TimeEntryLogItemViewModel timeEntryLogItemViewModel)
        {
            InitializeComponent();
            this.TimeEntryLabel.DescriptionLabel.Text = timeEntryLogItemViewModel.Description;
            this.DurationLabel.Text = timeEntryLogItemViewModel.Duration;
        }
    }
}

