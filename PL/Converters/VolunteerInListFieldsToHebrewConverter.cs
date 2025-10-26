using System;
using System.Globalization;
using System.Windows.Data;

namespace PL.Converters
{
    public class VolunteerInListFieldsToHebrewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is BO.VolunteerInListFields field)
            {
                return field switch
                {
                    BO.VolunteerInListFields.IdVolunteer => "תעודת זהות",
                    BO.VolunteerInListFields.FullName => "שם מלא",
                    BO.VolunteerInListFields.Active => "בפעילות",
                    BO.VolunteerInListFields.IdCall => "מזהה קריאה",
                    BO.VolunteerInListFields.Type => "סוג קריאה",
                    BO.VolunteerInListFields.SumCallsCompleted => "קריאות שהושלמו",
                    BO.VolunteerInListFields.SumCallsExpired => "קריאות שפגו",
                    BO.VolunteerInListFields.SumCallsConcluded => "קריאות שבוטלו",
                    _ => "לא ידוע"
                };
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return str.Trim() switch
                {
                    "תעודת זהות" => BO.VolunteerInListFields.IdVolunteer,
                    "שם מלא" => BO.VolunteerInListFields.FullName,
                    "בפעילות" => BO.VolunteerInListFields.Active,
                    "מזהה קריאה" => BO.VolunteerInListFields.IdCall,
                    "סוג קריאה" => BO.VolunteerInListFields.Type,
                    "קריאות שהושלמו" => BO.VolunteerInListFields.SumCallsCompleted,
                    "קריאות שפגו" => BO.VolunteerInListFields.SumCallsExpired,
                    "קריאות שבוטלו" => BO.VolunteerInListFields.SumCallsConcluded,
                    _ => BO.VolunteerInListFields.FullName // ערך ברירת מחדל סביר
                };
            }

            return Binding.DoNothing;
        }
    }
}
