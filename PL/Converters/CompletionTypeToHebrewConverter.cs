using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class CompletionTypeToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.CompletionType completionType)
            {
                return completionType switch
                {
                    BO.CompletionType.completed => "הושלם",
                    BO.CompletionType.canceledVolunteer => "ביטול מתנדב",
                    BO.CompletionType.canceledAdmin => "ביטול מנהל",
                    BO.CompletionType.expired => "פג תוקף",
                    _ => " לא ידוע "
                };
            }
            else if(value is null)
            {
                return "בטיפול";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // במקרה של ComboBox נדרש גם כיוון הפוך כדי לשמור את הערך הנכון
            return value switch
            {
                "הושלם" => BO.CompletionType.completed,
                "ביטול מתנדב" => BO.CompletionType.canceledVolunteer,
                "ביטול מנהל" => BO.CompletionType.canceledAdmin,
                "פג תוקף" => BO.CompletionType.expired,
                _ => Binding.DoNothing
            };
        }
    }
}