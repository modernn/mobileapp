using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Toggl.WPF.Views.Reports
{
    public sealed class Arc : Shape
    {
        public Point Center
        {
            get => (Point)GetValue(CenterProperty);
            set => SetValue(CenterProperty, value);
        }

        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register(nameof(Center), typeof(Point), typeof(Arc),
                new FrameworkPropertyMetadata(new Point(), FrameworkPropertyMetadataOptions.AffectsRender));

        public double StartAngle
        {
            get => (double)GetValue(StartAngleProperty);
            set => SetValue(StartAngleProperty, value);
        }

        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(Arc),
                new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double EndAngle
        {
            get => (double)GetValue(EndAngleProperty);
            set => SetValue(EndAngleProperty, value);
        }

        public static readonly DependencyProperty EndAngleProperty =
            DependencyProperty.Register(nameof(EndAngle), typeof(double), typeof(Arc),
                new FrameworkPropertyMetadata(Math.PI / 2.0, FrameworkPropertyMetadataOptions.AffectsRender));

        public double Radius
        {
            get => (double)GetValue(RadiusProperty);
            set => SetValue(RadiusProperty, value);
        }

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.Register(nameof(Radius), typeof(double), typeof(Arc),
                new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));

        static Arc() => DefaultStyleKeyProperty.OverrideMetadata(typeof(Arc), new FrameworkPropertyMetadata(typeof(Arc)));

        protected override Geometry DefiningGeometry
        {
            get
            {
                var figure = new PathFigure { StartPoint = Center };

                var line = new LineSegment
                {
                    Point = new Point(
                        Radius + Math.Sin(StartAngle * Math.PI / 180) * Radius,
                        Radius + Math.Cos(StartAngle * Math.PI / 180) * Radius
                    )
                };
                figure.Segments.Add(line);

                var arc = new ArcSegment
                {
                    IsLargeArc = (EndAngle - StartAngle) >= 180.0,
                    Point = new Point(
                        Radius + Math.Sin(EndAngle * Math.PI / 180) * Radius,
                        Radius + Math.Cos(EndAngle * Math.PI / 180) * Radius
                    ),
                    Size = new Size(Radius, Radius),
                    SweepDirection = SweepDirection.Counterclockwise
                };
                figure.Segments.Add(arc);

                return new PathGeometry { Figures = { figure } };
            }
        }
    }
}
