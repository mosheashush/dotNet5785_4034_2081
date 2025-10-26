using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using BO;
using DO;

namespace PL
{
    /// <summary>
    /// מסך כניסה למערכת - מאפשר כניסה למנהל ומתנדב
    /// </summary>
    public partial class LoginWindow : Window
    {
        #region שדה גישה ל-BL

        /// <summary>
        /// אובייקט גישה לשכבת הלוגיקה העסקית
        /// </summary>
        static readonly BIApi.IBl s_dal = BIApi.Factory.Get();

        #endregion

        #region תכונות תלות (Dependency Properties)

        /// <summary>
        /// תכונת תלות לשם משתמש/תעודת זהות
        /// </summary>
        public int Username
        {
            get { return (int)GetValue(UsernameProperty); }
            set { SetValue(UsernameProperty, value); }
        }
        public static readonly DependencyProperty UsernameProperty =
            DependencyProperty.Register("Username", typeof(int), typeof(LoginWindow),
                new PropertyMetadata(0));

        /// <summary>
        /// תכונת תלות לזכור אותי
        /// </summary>
        public bool RememberMe
        {
            get { return (bool)GetValue(RememberMeProperty); }
            set { SetValue(RememberMeProperty, value); }
        }
        public static readonly DependencyProperty RememberMeProperty =
            DependencyProperty.Register("RememberMe", typeof(bool), typeof(LoginWindow),
                new PropertyMetadata(false));

        /// <summary>
        /// תכונת תלות להודעת סטטוס
        /// </summary>
        public string StatusMessage
        {
            get { return (string)GetValue(StatusMessageProperty); }
            set { SetValue(StatusMessageProperty, value); }
        }
        public static readonly DependencyProperty StatusMessageProperty =
            DependencyProperty.Register("StatusMessage", typeof(string), typeof(LoginWindow),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// תכונת תלות לצבע הודעת סטטוס
        /// </summary>
        public Brush StatusMessageColor
        {
            get { return (Brush)GetValue(StatusMessageColorProperty); }
            set { SetValue(StatusMessageColorProperty, value); }
        }
        public static readonly DependencyProperty StatusMessageColorProperty =
            DependencyProperty.Register("StatusMessageColor", typeof(Brush), typeof(LoginWindow),
                new PropertyMetadata(Brushes.Green));

        #endregion

        #region בנאי

        public LoginWindow()
        {
            InitializeComponent();

            // אתחול בסיסי
            InitializeLoginForm();
        }

        #endregion

        #region אתחול טופס

        /// <summary>
        /// אתחול הטופס עם הגדרות בסיסיות
        /// </summary>
        private void InitializeLoginForm()
        {
            // ניקוי הודעות סטטוס
            StatusMessage = string.Empty;

            // אתחול מסד הנתונים
            try
            {
                s_dal.Admin.InitializeDatabase();
                // בדיקת משתמשים קיימים
                CheckExistingUsers();
            }
            catch (Exception ex)
            {
                if (!(ex is DalXMLFileLoadCreateException))
                    ShowStatusMessage($"שגיאה באתחול המערכת: {ex.Message}", true);
            }
        }

        #endregion

        #region אירועי כפתורים

        /// <summary>
        /// אירוע לחיצה על כפתור הכניסה הראשי
        /// </summary>
        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ניקוי הודעות קודמות
                StatusMessage = string.Empty;

                // קבלת נתוני הכניסה - Username מגיע מה-Binding
                int idNumber = Username;
                string password = passwordBox.Password; // PasswordBox - חריג מהכלל!

                // בדיקות תקינות
                if (idNumber==0)
                {
                    ShowStatusMessage("נא להזין תעודת זהות", true);
                    return;
                }

                if (string.IsNullOrEmpty(password))
                {
                    ShowStatusMessage("נא להזין סיסמא", true);
                    return;
                }

                // ניסיון כניסה למערכת
                BO.User userRole;
                try
                {
                    // קבלת רשימת המתנדבים
                    var volunteers = s_dal.Volunteer.listOfVolunteer(null, null, null);

                    // חיפוש המשתמש לפי תעודת זהות
                    var volunteer = volunteers.FirstOrDefault(v => v.IdVolunteer == idNumber);

                    if (volunteer == null)
                    {
                        ShowStatusMessage($"תעודת זהות {idNumber} לא נמצאה במערכת", true);
                        return;
                    }

                    ShowStatusMessage($"מתחבר כ: {volunteer.FullName}...", false);

                    // ניסיון כניסה עם שם מלא וסיסמה
                    userRole = s_dal.Volunteer.Entrance(volunteer.FullName, password);
                }
                catch (BO.BlDoesNotExistException)
                {
                    ShowStatusMessage("שם משתמש או סיסמה שגויים", true);
                    return;
                }
                catch (Exception ex)
                {
                    ShowStatusMessage($"שגיאה בחיפוש משתמש: {ex.Message}", true);
                    return;
                }

                // הצגת הודעת הצלחה
                string roleText = userRole == BO.User.admin ? "מנהל" : "מתנדב";
                ShowStatusMessage($"ברוכים הבאים! {roleText}", false);

                // השהיה קצרה להצגת ההודעה
                await System.Threading.Tasks.Task.Delay(1000);

                // מעבר למסך הראשי
                OpenMainWindow(userRole, idNumber);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"שגיאה כללית: {ex.Message}", true);
            }
        } 
        #endregion

        #region ניהול הודעות

        /// <summary>
        /// הצגת הודעת סטטוס
        /// </summary>
        /// <param name="message">הודעה להצגה</param>
        /// <param name="isError">האם זו הודעת שגיאה</param>
        private void ShowStatusMessage(string message, bool isError)
        {
            StatusMessage = message;
            StatusMessageColor = isError ? Brushes.Red : Brushes.Green;
        }

        #endregion

        #region מעבר למסכים אחרים

        /// <summary>
        /// פתיחת המסך הראשי בהתאם לסוג המשתמש
        /// </summary>
        /// <param name="userRole">סוג המשתמש</param>
        /// <param name="userId">מזהה המשתמש</param>
        private void OpenMainWindow(BO.User userRole, int userId)
        {
            try
            {
                // פתיחת MainWindow
                var mainWindow = new MainWindow(Username);

                // הגדרת כותרת החלון בהתאם לסוג המשתמש
                if (userRole == BO.User.admin)
                {
                    mainWindow.Title = "מסך ניהול ראשי - מנהל";
                }
                else
                {
                    mainWindow.Title = "מסך ניהול ראשי - מתנדב";
                }

                // הצגת MainWindow
                mainWindow.Show();

                // סגירת חלון הכניסה
                this.Close();
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"שגיאה בפתיחת המסך הראשי: {ex.Message}", true);
            }
        }

        #endregion

        #region בדיקת משתמשים קיימים

        /// <summary>
        /// בדיקה איזה משתמשים קיימים במסד הנתונים
        /// </summary>
        private void CheckExistingUsers()
        {
            try
            {
                // ננסה לקבל רשימת מתנדבים
                var volunteers = s_dal.Volunteer.listOfVolunteer(null, null, null);
                ShowStatusMessage($"נמצאו {volunteers.Count()} מתנדבים במסד הנתונים", false);
            }
            catch (Exception ex)
            {
                ShowStatusMessage($"שגיאה בבדיקת משתמשים: {ex.Message}", true);
            }
        }

        #endregion
    }
}