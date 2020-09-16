using Microsoft.UI.Xaml.Data;
using System;
using Toggl.Shared;
namespace Toggl.WinUI.Converters
{
    public class StringToPasswordConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var password = value.ToString();
            return Password.From(password);
        }
    }
}
