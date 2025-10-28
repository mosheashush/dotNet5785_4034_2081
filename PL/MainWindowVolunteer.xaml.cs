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
        private DispatcherTimer refreshTimer;

        #endregion

        #region Dependency Properties

        /// <summary>
        /// המתנדב הנוכחי
        /// </summary>
        public BO.Volunteer CurrentVolunteer
        {
            get { return (BO.Volunteer)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }
        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register(nameof(CurrentVolunteer),
                typeof(BO.Volunteer),
                typeof(MainWindowVolunteer),
                new PropertyMetadata(null, OnCurrentVolunteerChanged));

        /// <summary>
        /// זמן עדכון אחרון
        /// </summary>
        public string LastUpdateTime
        {
            get { return (string)GetValue(LastUpdateTimeProperty); }
            set { SetValue(LastUpdateTimeProperty, value); }
        }
        public static readonly DependencyProperty LastUpdateTimeProperty =
            DependencyProperty.Register(nameof(LastUpdateTime),
                typeof(string),
                typeof(MainWindowVolunteer),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// האם יש קריאה בטיפול
        /// </summary>
        public bool HasCallInProgress
        {
            get { return (bool)GetValue(HasCallInProgressProperty); }
            set { SetValue(HasCallInProgressProperty, value); }
        }
        public static readonly DependencyProperty HasCallInProgressProperty =
            DependencyProperty.Register(nameof(HasCallInProgress),
                typeof(bool),
                typeof(MainWindowVolunteer),
                new PropertyMetadata(false));

        /// <summary>
        /// Callback כאשר CurrentVolunteer משתנה
        /// </summary>
        private static void OnCurrentVolunteerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MainWindowVolunteer;
            if (window != null && e.NewValue != null)
            {
                var volunteer = e.NewValue as BO.Volunteer;
                window.HasCallInProgress = volunteer?.CallInProgress != null;
                window.Title = $"מערכת ניהול מתנדבים - {volunteer?.FullName ?? "מתנדב"}";
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// קונסטרקטור
        /// </summary>
        /// <param name="volunteerId">מזהה המתנדב</param>
        public MainWindowVolunteer(int volunteerId)
        {
            InitializeComponent();
            DataContext = this;

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
                CurrentVolunteer = s_dal.Volunteer.Read(volunteerId);

                // עדכון זמן
                LastUpdateTime = DateTime.Now.ToString("HH:mm:ss");
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
                if (CurrentVolunteer?.CallInProgress != null)
                {
                    MessageBox.Show(
                        "יש לך כבר קריאה בטיפול.\nסיים או בטל אותה לפני בחירת קריאה חדשה.",
                        "לא ניתן לבחור קריאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return;
                }

                //var selectCallWindow = new SelectCallWindow(volunteerId);
                //if (selectCallWindow.ShowDialog() == true)
                //{
                //    LoadVolunteerData();
                //}
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
                if (CurrentVolunteer?.CallInProgress == null)
                {
                    MessageBox.Show(
                        "אין קריאה בטיפול כרגע.",
                        "אין קריאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                //var manageWindow = new ManageCurrentCallWindow(volunteerId);
                //manageWindow.ShowDialog();
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
                //var historyWindow = new VolunteerCallHistoryWindow(volunteerId);
                //historyWindow.ShowDialog();
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