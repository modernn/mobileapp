using System.Windows.Media;
using ModernWpf;
using TogglColor = Toggl.Shared.Color;

namespace Toggl.WPF.Extensions
{
    public static class ColorExtensions
    {
        public static Color ToNativeColor(this TogglColor color)
            => Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue);

        public static TogglColor ToAdjustedColor(this string color)
            => Shared.Color.ParseAndAdjustToLabel(
                color,
                isInDarkMode: ThemeManager.Current.ActualApplicationTheme == ApplicationTheme.Dark);

        public static SolidColorBrush ToBrush(this Color color) => new SolidColorBrush(color);
    }
}
