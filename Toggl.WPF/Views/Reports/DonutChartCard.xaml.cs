using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Toggl.Core.UI.ViewModels.Reports;
using Toggl.WPF.Extensions;
using Toggl.WPF.Views.Reports.Donut;

namespace Toggl.WPF.Views.Reports
{
    /// <summary>
    /// Interaction logic for DonutChartCard.xaml
    /// </summary>
    public partial class DonutChartCard : UserControl
    {
        public DonutChartCard(ReportDonutChartDonutElement donutChartElement)
        {
            InitializeComponent();

            var percentageSegments = DonutChartGrouping.Group(donutChartElement.Segments);
            var sliceCollection = new SlicesCollection(percentageSegments);
            var radius = 380 / 2;
            var center = new Point(radius, radius);

            foreach (var slice in sliceCollection)
            {               
                var sliceBrush = slice.PercentageSegment
                    .Segment.Color
                    .ToAdjustedColor()
                    .ToNativeColor()
                    .ToBrush();

                var arc = new Arc
                {
                    Center = center,
                    Stroke = sliceBrush,
                    StartAngle = slice.StartAngle,
                    EndAngle = slice.StartAngle + slice.SweepAngle,
                    Radius = radius
                };

                DonutChart.Children.Add(arc);
            }
        }
    }
}
