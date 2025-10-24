using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class CallStateToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallState callType)
            {
                return callType switch
                {
                    BO.CallState.open => "פתוח",
                    BO.CallState.openOnRisk => "פתוח בסיכון",
                    BO.CallState.processed => "בטיפול",
                    BO.CallState.processedOnRisk => "בטיפול בסיכון",
                    BO.CallState.completed => "הושלם",
                    BO.CallState.expired => "פג תוקף",
                    BO.CallState.all => "הכל",
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
                "פתוח" => BO.CallState.open,
                "פתוח בסיכון" => BO.CallState.openOnRisk,
                "בטיפול" => BO.CallState.processed,
                "בטיפול בסיכון" => BO.CallState.processedOnRisk,
                "הושלם" => BO.CallState.completed,
                "פג תוקף" => BO.CallState.expired,
                "הכל" => BO.CallState.all,
                _ => Binding.DoNothing
            };
        }
    }
}
