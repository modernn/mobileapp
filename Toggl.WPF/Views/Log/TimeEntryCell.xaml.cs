using System.Windows.Controls;
using System.Windows.Input;
using Toggl.Core.Analytics;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.WPF.Extensions;

namespace Toggl.WPF.Views.Log
{
    public partial class TimeEntryCell : UserControl
    {
        public TimeEntryCell(TimeEntryLogItemViewModel timeEntryLogItemViewModel, ICommand continueTimeEntryCommand)
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

            this.ContinueButton.Command = continueTimeEntryCommand;
            this.ContinueButton.CommandParameter =
                new ContinueTimeEntryInfo(
                    timeEntryLogItemViewModel,
                    timeEntryLogItemViewModel.IsTimeEntryGroupHeader
                        ? ContinueTimeEntryMode.TimeEntriesGroupContinueButton
                        : ContinueTimeEntryMode.SingleTimeEntryContinueButton);
        }
    }
}

