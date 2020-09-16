using Microsoft.UI.Xaml.Data;
using System;
using Toggl.Shared;

namespace Toggl.WinUI.Converters
{
    public class StringToEmailConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            var email = value.ToString();
            return Email.From(email);
        }
    }
}
