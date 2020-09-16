using System.Windows.Controls;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.WPF.Extensions;

namespace Toggl.WPF.Views.Log
{
    public partial class TimeEntryCell : UserControl
    {
        public TimeEntryCell(TimeEntryLogItemViewModel timeEntryLogItemViewModel)
        {
            InitializeComponent();
            this.TimeEntryLabel.DescriptionLabel.Text = timeEntryLogItemViewModel.Description;
            if (timeEntryLogItemViewModel.HasProject)
            {
                this.TimeEntryLabel.ProjectLabel.Text = timeEntryLogItemViewModel.ProjectName;
                this.TimeEntryLabel.ProjectLabel.Foreground = timeEntryLogItemViewModel.ProjectColor.ToAdjustedColor().ToNativeColor().ToBrush();
            }
            else
            {
                this.TimeEntryLabel.ProjectLabel.Text = "";
            }

            this.DurationLabel.Text = timeEntryLogItemViewModel.Duration;
        }
    }
}

