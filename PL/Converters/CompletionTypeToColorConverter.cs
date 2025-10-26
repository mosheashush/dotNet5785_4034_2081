using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PL.Converters
{
    public class CompletionTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                // בטיפול - כתום
                return new SolidColorBrush(Color.FromRgb(255, 152, 0)); // #FF9800
            }

            if (value is BO.CompletionType completionType)
            {
                return completionType switch
                {
                    BO.CompletionType.completed => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // ירוק #4CAF50
                    BO.CompletionType.canceledVolunteer => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // אדום #F44336
                    BO.CompletionType.canceledAdmin => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // אדום #F44336
                    BO.CompletionType.expired => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // אדום #F44336
                    _ => new SolidColorBrush(Color.FromRgb(158, 158, 158)) // אפור
                };
            }

            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}