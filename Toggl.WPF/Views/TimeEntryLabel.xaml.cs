using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Toggl.WPF.Views
{
    public partial class TimeEntryLabel
    {
        public TimeEntryLabel()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty IconsPanelBackgroundProperty = DependencyProperty.Register(
            "IconsPanelBackground", typeof(Brush), typeof(TimeEntryLabel), new PropertyMetadata(default(Brush)));

        public Brush IconsPanelBackground
        {
            get { return (Brush) GetValue(IconsPanelBackgroundProperty); }
            set { SetValue(IconsPanelBackgroundProperty, value); }
        }
    }
}

