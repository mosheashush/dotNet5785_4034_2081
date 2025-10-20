using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class UserToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.User callType)
            {
                return callType switch
                {
                    BO.User.volunteer => "מתנדב",
                    BO.User.admin => "מנהל",
                    _ => " לא ידוע "
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // במקרה של ComboBox נדרש גם כיוון הפוך כדי לשמור את הערך הנכון
            return value switch
            {
                "מתנדב" => BO.User.volunteer,
                "מנהל" => BO.User.admin,
                _ => Binding.DoNothing
            };
        }
    }
}
