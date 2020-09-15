using System.Drawing;
using TogglColor = Toggl.Shared.Color;

namespace Toggl.WPF.Extensions
{
    public static class ColorExtensions
    {
        public static Color ToNativeColor(this TogglColor color)
            => Color.FromArgb(color.Red, color.Green, color.Blue);
    }
}
