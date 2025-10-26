using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Converters
{
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם הרשימה null או ריקה - הצג את ההודעה
            if (value is null)
                return Visibility.Visible;

            if (value is IEnumerable collection)
            {
                // בודק אם יש אלמנטים ברשימה
                foreach (var item in collection)
                {
                    return Visibility.Collapsed; // יש אלמנטים - הסתר את ההודעה
                }
                return Visibility.Visible; // אין אלמנטים - הצג את ההודעה
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}