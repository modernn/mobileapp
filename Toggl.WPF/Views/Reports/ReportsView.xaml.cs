using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Toggl.Core.UI.ViewModels.Reports;

namespace Toggl.WPF.Views.Reports
{
    public partial class ReportsView
    {
        private readonly CompositeDisposable disposeBag = new CompositeDisposable();

        public ReportsView(ReportsViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
            ViewModel.Elements
                .Subscribe(UpdateTimeEntriesList)
                .DisposeWith(disposeBag);
        }

        private void UpdateTimeEntriesList(IImmutableList<IReportElement> reportsElements)
        {
            ReportsPanel.Children.Clear();
            foreach (var item in reportsElements)
            {
                UIElement child = item switch
                {
                    ReportWorkspaceNameElement _ => null,
                    ReportSummaryElement summaryElement => new TotalsCard(summaryElement),
                    ReportBarChartElement barChartElement => new BarChart(barChartElement),
                    ReportDonutChartDonutElement donutChartElement => new DonutChartCard(donutChartElement),
                    ReportDonutChartLegendItemElement _ => null,
                    ReportNoDataElement _ => null,
                    ReportErrorElement _ => null,
                    ReportAdvancedReportsViaWebElement _ => new AdvancedReportsViaWeb(),
                    _ => null
                };

                if (child == null)
                    continue;

                ReportsPanel.Children.Add(child);
            }
        }
    }
}
