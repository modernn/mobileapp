using System.Windows;
using System.Windows.Controls;

namespace Toggl.WPF.Views
{
    public partial class ValidationErrorControl : UserControl
    {
        public ValidationErrorControl()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ErrorTextProperty =
            DependencyProperty.Register(
                "ErrorText", typeof(string),
                typeof(ValidationErrorControl)
            );
        public string ErrorText
        {
            get => (string)GetValue(ErrorTextProperty);
            set => SetValue(ErrorTextProperty, value);
        }

    }
}

