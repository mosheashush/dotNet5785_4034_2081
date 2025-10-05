using System;
using System.Windows;
using BlApi;
using BO;

namespace PL.Volunteer
{
    /// <summary>
    /// מסך הוספה/עריכה של מתנדב
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        // תלות עם BL
        static readonly BIApi.IBl s_dal = BIApi.Factory.Get();

        private readonly int? _volunteerId; // null = מצב הוספה, יש ערך = מצב עדכון

        #region Dependency Properties

        /// <summary>
        /// המתנדב הנוכחי שמוצג/נערך
        /// </summary>
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer),
                typeof(BO.Volunteer),
                typeof(VolunteerWindow));

        /// <summary>
        /// טקסט הכפתור (Add/Update)
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText),
                typeof(string),
                typeof(VolunteerWindow));

        #endregion

        /// <summary>
        /// בנאי למצב הוספה (ללא פרמטרים)
        /// </summary>
        public VolunteerWindow()
        {
            InitializeComponent();

            try
            {
                _volunteerId = null;

                // אתחול מתנדב חדש
                CurrentVolunteer = new BO.Volunteer
                {
                    id = 0,
                    FullName = "",
                    CallNumber = "",
                    EmailAddress = "",
                    Password = null,
                    FullCurrentAddress = null,
                    Latitude = null,
                    Longitude = null,
                    CurrentPosition = User.volunteer, // ברירת מחדל
                    Active = true,
                    MaxDistanceForCall = null,
                    TypeOfDistance = Distance.air, // ברירת מחדל
                    SumCallsCompleted = 0,
                    SumCallsExpired = 0,
                    SumCallsConcluded = 0,
                    CallInProgress = null
                };

                ButtonText = "Add";
                Title = "הוספת מתנדב חדש";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה באתחול המסך:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// בנאי למצב עריכה (עם ID)
        /// </summary>
        /// <param name="volunteerId">מזהה המתנדב (ת.ז) לעריכה</param>
        public VolunteerWindow(int volunteerId) : this()
        {
            _volunteerId = volunteerId;

            try
            {
                // טעינת המתנדב הקיים
                CurrentVolunteer = s_dal.Volunteer.Read(volunteerId)
                    ?? throw new Exception("המתנדב לא נמצא במערכת.");

                // טעינת הסיסמה ל-PasswordBox (אם קיימת)
                if (!string.IsNullOrEmpty(CurrentVolunteer.Password))
                {
                    passwordBox.Password = CurrentVolunteer.Password;
                }

                ButtonText = "Update";
                Title = $"עריכת מתנדב: {CurrentVolunteer.FullName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת המתנדב:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// אירוע טעינת החלון
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ניתן להוסיף כאן לוגיקה נוספת בעת טעינת החלון
        }

        /// <summary>
        /// לחיצה על כפתור הוסף/עדכן
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // עדכון הסיסמה מה-PasswordBox לפני השמירה
                CurrentVolunteer.Password = passwordBox.Password;

                // ולידציה בסיסית
                if (!ValidateInput())
                    return;

                // ביצוע הפעולה
                if (_volunteerId == null) // מצב הוספה
                {
                    s_dal.Volunteer.Create(CurrentVolunteer);
                    MessageBox.Show($"המתנדב נוסף בהצלחה!\nת.ז: {CurrentVolunteer.id}",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else // מצב עדכון
                {
                    // בעדכון, צריך לשלוח גם את ת.ז של המבקש
                    // במקרה שלנו נניח שזה מנהל (0) - תצטרך להתאים לפי הלוגיקה שלך
                    s_dal.Volunteer.Update(0, CurrentVolunteer);
                    MessageBox.Show("המתנדב עודכן בהצלחה!",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                // סגירת המסך אוטומטית
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בשמירת המתנדב:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// ולידציה בסיסית של הקלט
        /// </summary>
        private bool ValidateInput()
        {
            // בדיקת ת.ז (רק במצב הוספה)
            if (_volunteerId == null)
            {
                if (CurrentVolunteer.id <= 0 || CurrentVolunteer.id.ToString().Length != 9)
                {
                    MessageBox.Show("ת.ז חייבת להיות בת 9 ספרות!",
                        "שגיאת קלט",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            // בדיקת שם מלא
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
            {
                MessageBox.Show("שם מלא הוא שדה חובה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // בדיקת טלפון
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.CallNumber))
            {
                MessageBox.Show("מספר טלפון הוא שדה חובה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // בדיקת אימייל
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.EmailAddress))
            {
                MessageBox.Show("כתובת אימייל היא שדה חובה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // בדיקה בסיסית של פורמט אימייל
            if (!CurrentVolunteer.EmailAddress.Contains("@") ||
                !CurrentVolunteer.EmailAddress.Contains("."))
            {
                MessageBox.Show("כתובת האימייל אינה בפורמט תקין!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // בדיקת סיסמה (רק במצב הוספה)
            if (_volunteerId == null && string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBox.Show("סיסמה היא שדה חובה עבור מתנדב חדש!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // בדיקת מרחק מקסימלי (אם הוזן)
            if (CurrentVolunteer.MaxDistanceForCall.HasValue &&
                CurrentVolunteer.MaxDistanceForCall.Value <= 0)
            {
                MessageBox.Show("מרחק מקסימלי חייב להיות גדול מ-0!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// לחיצה על כפתור ביטול
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // שאלת אישור לפני סגירה
            var result = MessageBox.Show(
                "האם אתה בטוח שברצונך לבטל? כל השינויים יאבדו.",
                "אישור ביטול",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                DialogResult = false;
                Close();
            }
        }

        /// <summary>
        /// סגירת החלון
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // ניתן להוסיף כאן לוגיקה נוספת בעת סגירת החלון
        }
    }
}