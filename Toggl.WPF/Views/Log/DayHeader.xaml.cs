using System.Collections.Immutable;
using System.Linq;
using System.Windows.Controls;
using Toggl.Core.UI.ViewModels.MainLog;

namespace Toggl.WPF.Views.Log
{
    public partial class DayHeader
    {
        public DayHeader(DaySummaryViewModel daySummaryViewModel, IImmutableList<MainLogItemViewModel> items)
        {
            InitializeComponent();
            this.DateHeaderTextBlock.Text = daySummaryViewModel.Title;
            this.DateDurationTextBlock.Text = daySummaryViewModel.TotalTrackedTime;
            TimeEntriesPanel.Children.Clear();
            foreach (var item in items.OfType<TimeEntryLogItemViewModel>())
            {
                TimeEntriesPanel.Children.Add(new TimeEntryCell(item));
            }
        }
    }
}

