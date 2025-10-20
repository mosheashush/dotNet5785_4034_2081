using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace PL.Converters
{
    /// <summary>
    /// ממיר סטטוס פעיל לצבע רקע
    /// </summary>
    public class ActiveToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isActive)
            {
                return isActive
                    ? new SolidColorBrush(Color.FromRgb(92, 184, 92))   // ירוק - פעיל
                    : new SolidColorBrush(Color.FromRgb(217, 83, 79));  // אדום - לא פעיל
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
