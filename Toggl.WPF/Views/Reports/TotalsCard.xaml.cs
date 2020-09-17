using System.Windows.Controls;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.Shared;
using Toggl.Shared.Extensions;

namespace Toggl.WPF.Views.Reports
{
    public partial class TotalsCard : UserControl
    {
        private static readonly string middleDash = "—";

        public TotalsCard(ReportSummaryElement summaryElement)
        {
            InitializeComponent();

            Total.Content = convertReportTimespanToDurationString(summaryElement);
            Billable.Content = convertBillablePercentageToSpannable(summaryElement);
        }

        private static string convertReportTimespanToDurationString(ReportSummaryElement summaryElement)
        {
            if (!summaryElement.TotalTime.HasValue || summaryElement.IsLoading)
                return middleDash;

            var timeString = summaryElement.TotalTime.Value.ToFormattedString(summaryElement.DurationFormat);
            return timeString;
        }

        private static string convertBillablePercentageToSpannable(ReportSummaryElement summaryElement)
        {
            if (!summaryElement.BillablePercentage.HasValue || summaryElement.IsLoading)
                return middleDash;

            return $"{summaryElement.BillablePercentage.Value:0.00}%";
        }
    }
}
