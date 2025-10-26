using BO;
using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class CallInListFieldsToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is CallInListFields field)
            {
                return field switch
                {
                    CallInListFields.IdAssignment => "מספר משימה",
                    CallInListFields.IdCall => "מזהה קריאה",
                    CallInListFields.Type => "סוג הקריאה",
                    CallInListFields.CallStartTime => "זמן התחלת הקריאה",
                    CallInListFields.TimeRemaining => "זמן נותר",
                    CallInListFields.SumTimeProcess => "סך זמן טיפול",
                    CallInListFields.NameFinalVolunteer => "שם המתנדב אחרון",
                    CallInListFields.CallState => "מצב הקריאה",
                    CallInListFields.SumOfAssignments => "כמות שיבוצים",
                    _ => "לא ידוע"
                };
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value switch
            {
                "מספר משימה" => CallInListFields.IdAssignment,
                "מספר קריאה" => CallInListFields.IdCall,
                "סוג הקריאה" => CallInListFields.Type,
                "זמן התחלת הקריאה" => CallInListFields.CallStartTime,
                "זמן נותר" => CallInListFields.TimeRemaining,
                "שם המתנדב הסופי" => CallInListFields.NameFinalVolunteer,
                "סך זמן טיפול" => CallInListFields.SumTimeProcess,
                "מצב הקריאה" => CallInListFields.CallState,
                "סך כל המשימות" => CallInListFields.SumOfAssignments,
                _ => Binding.DoNothing
            };
        }
    }
}
