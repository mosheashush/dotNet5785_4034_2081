using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PL.Converters
{
    /// <summary>
    /// ממיר סטטוס פעיל לטקסט
    /// </summary>
    public class ActiveToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive ? "✓ פעיל" : "✗ לא פעיל";
            }
            return "לא ידוע";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}