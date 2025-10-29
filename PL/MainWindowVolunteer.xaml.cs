using BIApi;
using BlApi;
using BO;
using PL.Call;
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

        static readonly BIApi.IBl s_bl = BIApi.Factory.Get();
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
                CurrentVolunteer = s_bl.Volunteer.Read(volunteerId);

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

                var selectCallWindow = new OpenCallWindow(volunteerId);

                // הירשם ל-Event
                selectCallWindow.CallSelected += (s, args) =>
                {
                    // רענן את הנתונים כשנבחרה קריאה
                    LoadVolunteerData();
                };

                // פשוט Show (לא ShowDialog)
                selectCallWindow.Show();
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

                var manageWindow = new CallWindow(CurrentVolunteer.CallInProgress.IdCall);

                // הירשם ל-Event
                manageWindow.PropertyChanged += (s, args) =>
                {
                    // רענן את הנתונים כשנבחרה קריאה
                    LoadVolunteerData();
                };

                // פשוט Show (לא ShowDialog)
                manageWindow.Show();
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
        /// כפתור ביטול/מחיקת קריאה נוכחית
        /// </summary>
        private void DeleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקה שיש קריאה בטיפול
                if (CurrentVolunteer?.CallInProgress == null)
                {
                    MessageBox.Show(
                        "אין קריאה בטיפול כרגע.",
                        "אין קריאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // אישור מהמשתמש
                var result = MessageBox.Show(
                    $"האם אתה בטוח שברצונך לבטל את הקריאה מספר {CurrentVolunteer.CallInProgress.IdCall}?\n\n" +
                    $"סוג: {CurrentVolunteer.CallInProgress.Type}\n" +
                    $"כתובת: {CurrentVolunteer.CallInProgress.FullAddress}\n\n" +
                    "הקריאה תחזור להיות זמינה למתנדבים אחרים.",
                    "אישור ביטול קריאה",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // ביטול הקריאה דרך ה-BL
                        s_bl.Call.CancelTreatment(volunteerId, CurrentVolunteer.CallInProgress.IdAssignment);

                        MessageBox.Show(
                            "הקריאה בוטלה בהצלחה!",
                            "הצלחה",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // רענון הנתונים
                        LoadVolunteerData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"שגיאה בביטול הקריאה:\n{ex.Message}",
                            "שגיאה",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה כללית:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// כפתור סיום קריאה - סימון הקריאה כהושלמה
        /// </summary>
        private void CompleteCallButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // בדיקה שיש קריאה בטיפול
                if (CurrentVolunteer?.CallInProgress == null)
                {
                    MessageBox.Show(
                        "אין קריאה בטיפול כרגע.",
                        "אין קריאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    return;
                }

                // אישור מהמשתמש
                var result = MessageBox.Show(
                    $"האם אתה בטוח שברצונך לסיים את הקריאה מספר {CurrentVolunteer.CallInProgress.IdCall}?\n\n" +
                    $"סוג: {CurrentVolunteer.CallInProgress.Type}\n" +
                    $"כתובת: {CurrentVolunteer.CallInProgress.FullAddress}\n\n" +
                    "הקריאה תסומן כהושלמה בהצלחה.",
                    "אישור סיום קריאה",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        // סימון הקריאה כהושלמה דרך ה-BL
                        s_bl.Call.FinishTreatment(volunteerId, CurrentVolunteer.CallInProgress.IdAssignment);

                        MessageBox.Show(
                            $"כל הכבוד! הקריאה {CurrentVolunteer.CallInProgress.IdCall} הושלמה בהצלחה!",
                            "הצלחה",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);

                        // רענון הנתונים
                        LoadVolunteerData();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(
                            $"שגיאה בסיום הקריאה:\n{ex.Message}",
                            "שגיאה",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה כללית:\n{ex.Message}",
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
        #endregion
    }
}