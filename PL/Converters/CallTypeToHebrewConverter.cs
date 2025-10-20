using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class CallTypeToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CallType callType)
            {
                return callType switch
                {
                    BO.CallType.deliveringfood => "משלוח אוכל",
                    BO.CallType.makingfood => "הכנת אוכל",
                    BO.CallType.None => "אינו בפעילות",
                    BO.CallType.All => "הכל",
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
                "משלוח אוכל" => BO.CallType.deliveringfood,
                "הכנת אוכל" => BO.CallType.makingfood,
                "אינו בפעילות" => BO.CallType.None,
                "הכל" => BO.CallType.All,
                _ => Binding.DoNothing
            };
        }
    }
}
