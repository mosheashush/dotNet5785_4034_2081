using BIApi;
using BlApi;
using BO;
using System;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for MainWindowVolunteer.xaml
    /// מסך ראשי של מתנדב - Dashboard מרכזי
    /// </summary>
    public partial class MainWindowVolunteer : Window
    {
        #region Fields

        static readonly BIApi.IBl s_dal = BIApi.Factory.Get();
        private int volunteerId;
        private BO.Volunteer currentVolunteer;
        private DispatcherTimer refreshTimer;

        #endregion

        #region Constructor

        /// <summary>
        /// קונסטרקטור
        /// </summary>
        /// <param name="volunteerId">מזהה המתנדב</param>
        public MainWindowVolunteer(int volunteerId)
        {
            InitializeComponent();

            this.volunteerId = volunteerId;

            // טעינת נתונים ראשונית
            LoadVolunteerData();

            // הפעלת Timer לרענון אוטומטי כל 30 שניות
            InitializeRefreshTimer();
        }

        #endregion

        #region Methods

        /// <summary>
        /// טעינת נתוני המתנדב
        /// </summary>
        private void LoadVolunteerData()
        {
            try
            {
                // טעינה מהשכבה הלוגית
                currentVolunteer = s_dal.Volunteer.Read(volunteerId);

                // עדכון התצוגה
                UpdateUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה בטעינת נתוני המתנדב:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // סגירת המסך במקרה של שגיאה קריטית
                this.Close();
            }
        }

        /// <summary>
        /// עדכון כל אלמנטי ה-UI
        /// </summary>
        private void UpdateUI()
        {
            if (currentVolunteer == null)
                return;

            // עדכון פרטים אישיים
            VolunteerNameText.Text = currentVolunteer.FullName ?? "מתנדב";
            IdText.Text = currentVolunteer.id.ToString();
            PhoneText.Text = currentVolunteer.CallNumber ?? "לא צוין";
            EmailText.Text = currentVolunteer.EmailAddress ?? "לא צוין";

            // עדכון סטטוס קריאה
            UpdateCallStatus();

            // עדכון סטטיסטיקות
            CompletedCountText.Text = currentVolunteer.SumCallsCompleted.ToString();
            CancelledCountText.Text = currentVolunteer.SumCallsConcluded.ToString();

            // עדכון זמן
            LastUpdateText.Text = DateTime.Now.ToString("HH:mm:ss");

            // עדכון כותרת החלון
            this.Title = $"מערכת ניהול מתנדבים - {currentVolunteer.FullName}";
        }

        /// <summary>
        /// עדכון סטטוס הקריאה הנוכחית
        /// </summary>
        private void UpdateCallStatus()
        {
            if (currentVolunteer.CallInProgress != null)
            {
                // יש קריאה בטיפול
                HasCallPanel.Visibility = Visibility.Visible;
                NoCallPanel.Visibility = Visibility.Collapsed;

                // עדכון פרטי הקריאה
                CallIdText.Text = currentVolunteer.CallInProgress.IdCall.ToString();
                CallTypeText.Text = currentVolunteer.CallInProgress.Type.ToString();
                CallAddressText.Text = currentVolunteer.CallInProgress.FullAddress ?? "לא צוין";

                // הפעלת כפתור ניהול קריאה
                ManageCallButton.IsEnabled = true;
                ManageCallButton.Opacity = 1.0;

                // כפתור בחירת קריאה - מושבת
                SelectCallButton.IsEnabled = false;
                SelectCallButton.Opacity = 0.5;
            }
            else
            {
                // אין קריאה בטיפול
                HasCallPanel.Visibility = Visibility.Collapsed;
                NoCallPanel.Visibility = Visibility.Visible;

                // השבתת כפתור ניהול קריאה
                ManageCallButton.IsEnabled = false;
                ManageCallButton.Opacity = 0.5;

                // כפתור בחירת קריאה - מופעל
                SelectCallButton.IsEnabled = true;
                SelectCallButton.Opacity = 1.0;
            }
        }

        /// <summary>
        /// אתחול Timer לרענון אוטומטי
        /// </summary>
        private void InitializeRefreshTimer()
        {
            refreshTimer = new DispatcherTimer();
            refreshTimer.Interval = TimeSpan.FromSeconds(30);
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
        }

        /// <summary>
        /// רענון אוטומטי כל 30 שניות
        /// </summary>
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                LoadVolunteerData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in auto-refresh: {ex.Message}");
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// כפתור הפרטים שלי
        /// </summary>
        private void MyDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var detailsWindow = new VolunteerWindow(volunteerId);
                detailsWindow.ShowDialog();

                // רענון לאחר סגירת החלון
                LoadVolunteerData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה בפתיחת מסך הפרטים:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// כפתור בחירת קריאה
        /// </summary>
        private void SelectCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentVolunteer.CallInProgress != null)
                {
                    MessageBox.Show(
                        "יש לך כבר קריאה בטיפול.\nסיים או בטל אותה לפני בחירת קריאה חדשה.",
                        "לא ניתן לבחור קריאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                var selectCallWindow = new SelectCallWindow(volunteerId);

                if (selectCallWindow.ShowDialog() == true)
                {
                    // קריאה נבחרה - רענון
                    LoadVolunteerData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה בפתיחת מסך בחירת קריאה:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// כפתור ניהול קריאה נוכחית
        /// </summary>
        private void ManageCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (currentVolunteer.CallInProgress == null)
                {
                    MessageBox.Show(
                        "אין קריאה בטיפול כרגע.",
                        "אין קריאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                var manageWindow = new ManageCurrentCallWindow(volunteerId);
                manageWindow.ShowDialog();

                // רענון לאחר סגירת החלון
                LoadVolunteerData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה בפתיחת מסך ניהול קריאה:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// כפתור היסטוריית קריאות
        /// </summary>
        private void HistoryButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var historyWindow = new VolunteerCallHistoryWindow(volunteerId);
                historyWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה בפתיחת היסטוריית קריאות:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// כפתור יציאה
        /// </summary>
        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "האם אתה בטוח שברצונך לצאת מהמערכת?",
                "יציאה מהמערכת",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // עצירת Timer
                refreshTimer?.Stop();

                // חזרה למסך כניסה
                var loginWindow = new LoginWindow();
                loginWindow.Show();

                this.Close();
            }
        }

        /// <summary>
        /// סגירת החלון
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // עצירת Timer
            refreshTimer?.Stop();

            // אישור סגירה
            var result = MessageBox.Show(
                "האם אתה בטוח שברצונך לסגור את המערכת?",
                "סגירת המערכת",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
                // הפעלה מחדש של Timer אם המשתמש ביטל
                refreshTimer?.Start();
            }
        }

        #endregion
    }
}