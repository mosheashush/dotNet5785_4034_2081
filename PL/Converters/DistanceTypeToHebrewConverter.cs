using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class DistanceTypeToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.Distance distanceType)
            {
                return distanceType switch
                {
                    BO.Distance.walk => "הליכה",
                    BO.Distance.air => "אווירי",
                    BO.Distance.drive => "נסיעה",
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
                "הליכה" => BO.Distance.walk,
                "אווירי" => BO.Distance.air,
                "נסיעה" => BO.Distance.drive,
                _ => Binding.DoNothing
            };
        }
    }
}
