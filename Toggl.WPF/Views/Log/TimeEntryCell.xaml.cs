using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Toggl.Core.Analytics;
using Toggl.Core.Models.Interfaces;
using Toggl.Core.UI.Helper;
using Toggl.Core.UI.ViewModels.MainLog;
using Toggl.Shared.Extensions;
using Toggl.WPF.Extensions;

namespace Toggl.WPF.Views.Log
{
    public partial class TimeEntryCell
    {
        public TimeEntryCell(
            TimeEntryLogItemViewModel timeEntryLogItemViewModel,
            RxAction<ContinueTimeEntryInfo, IThreadSafeTimeEntry> continueTimeEntry,
            InputAction<GroupId> toggleGroupExpansion)
        {
            InitializeComponent();
            ViewModel = timeEntryLogItemViewModel;
            DataContext = ViewModel;

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

            this.ContinueButton.Command = continueTimeEntry.ToCommand();
            this.ContinueButton.CommandParameter =
                new ContinueTimeEntryInfo(
                    timeEntryLogItemViewModel,
                    timeEntryLogItemViewModel.IsTimeEntryGroupHeader
                        ? ContinueTimeEntryMode.TimeEntriesGroupContinueButton
                        : ContinueTimeEntryMode.SingleTimeEntryContinueButton);

            this.GroupToggleButton.IsChecked = timeEntryLogItemViewModel.VisualizationIntent ==
                                               LogItemVisualizationIntent.ExpandedGroupHeader;
            this.GroupToggleButton.Command = toggleGroupExpansion.ToCommand();
            this.GroupToggleButton.CommandParameter = timeEntryLogItemViewModel.GroupId;
        }
    }
}

