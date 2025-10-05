using PL.Volunteer;
using System;
using System.Globalization;
using System.Windows;

namespace PL
{
    public partial class MainWindow : Window
    {
        // === BL Access (בטל הערה כשקיים אצלך) ===
        // static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MainWindow()
        {
            InitializeComponent();

            // ברירות מחדל לתצוגה
            CurrentTime = DateTime.Now;
            RiskRange = TimeSpan.FromMinutes(30);

            // אם יש לך אירועים מה-BL (Observer/Events) אפשר להירשם כאן:
            // s_bl.ClockChanged  += (s, e) => CurrentTime = s_bl.GetClock();
            // s_bl.ConfigChanged += (s, e) => RiskRange  = s_bl.GetRiskRange();
        }

        #region Dependency Properties – שעון ותצורה

        // CurrentTime (DateTime) + נגזרת לתצוגה (string)
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(nameof(CurrentTime), typeof(DateTime), typeof(MainWindow),
                new PropertyMetadata(DateTime.Now, (d, e) =>
                {
                    var win = (MainWindow)d;
                    win.CurrentTimeString = ((DateTime)e.NewValue).ToString("dd/MM/yyyy HH:mm:ss");
                }));

        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }

        public static readonly DependencyProperty CurrentTimeStringProperty =
            DependencyProperty.Register(nameof(CurrentTimeString), typeof(string), typeof(MainWindow),
                new PropertyMetadata(""));

        public string CurrentTimeString
        {
            get => (string)GetValue(CurrentTimeStringProperty);
            set => SetValue(CurrentTimeStringProperty, value);
        }

        // RiskRange (TimeSpan) + נגזרת לתצוגה/קלט (string)
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register(nameof(RiskRange), typeof(TimeSpan), typeof(MainWindow),
                new PropertyMetadata(TimeSpan.Zero, (d, e) =>
                {
                    var win = (MainWindow)d;
                    win.RiskRangeString = ((TimeSpan)e.NewValue).ToString(); // פורמט סטנדרטי: hh:mm:ss
                }));

        public TimeSpan RiskRange
        {
            get => (TimeSpan)GetValue(RiskRangeProperty);
            set => SetValue(RiskRangeProperty, value);
        }

        public static readonly DependencyProperty RiskRangeStringProperty =
            DependencyProperty.Register(nameof(RiskRangeString), typeof(string), typeof(MainWindow),
                new PropertyMetadata("", (d, e) =>
                {
                    var win = (MainWindow)d;
                    if (TimeSpan.TryParse((string)e.NewValue, CultureInfo.InvariantCulture, out var ts))
                        win.RiskRange = ts; // Two-way binding על ה-TextBox
                }));

        public string RiskRangeString
        {
            get => (string)GetValue(RiskRangeStringProperty);
            set => SetValue(RiskRangeStringProperty, value);
        }

        #endregion

        #region Clock – קידום/רענון/איפוס

        private void AdvanceBy(TimeSpan delta)
        {
            // מימוש מלא דרך ה-BL:
            // s_bl.AdvanceClock(delta);
            // CurrentTime = s_bl.GetClock();

            // מימוש זמני מקומי (יעבוד גם בלי BL):
            CurrentTime = CurrentTime.Add(delta);
        }

        private void AdvanceMinute_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromMinutes(1));
        private void AdvanceHour_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromHours(1));
        private void AdvanceDay_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromDays(1));
        private void AdvanceMonth_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromDays(30));  // פשטות
        private void AdvanceYear_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromDays(365)); // פשטות

        private void RefreshFromBlClock_Click(object sender, RoutedEventArgs e)
        {
            // CurrentTime = s_bl.GetClock();
            CurrentTime = DateTime.Now; // זמני
        }

        private void ResetClockToNow_Click(object sender, RoutedEventArgs e)
        {
            // s_bl.SetClock(DateTime.Now);
            CurrentTime = DateTime.Now;
        }

        #endregion

        #region Config – שמירה/טעינה

        private void SaveConfigToBl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // s_bl.SetRiskRange(RiskRange);
                MessageBox.Show("ההגדרות נשמרו (דמו).", "תצורה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שמירת תצורה נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfigFromBl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // RiskRange = s_bl.GetRiskRange();
                MessageBox.Show("ההגדרות נטענו (דמו).", "תצורה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"טעינת תצורה נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region DB – איפוס/אתחול

        private void ResetDb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("לאפס את בסיס הנתונים? הפעולה תמחק נתונים ותשיב הגדרות לברירת־מחדל.",
                                "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)
                return;

            try
            {
                // s_bl.ResetDatabase();
                MessageBox.Show("בוצע איפוס (דמו).", "בסיס נתונים", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"האיפוס נכשל:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitDb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("לאתחל את בסיס הנתונים בנתוני התחלה?",
                                "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                // דוגמה לזרימה טיפוסית:
                // s_bl.ResetDatabase();
                // s_bl.InitializeDatabase();

                MessageBox.Show("בוצע אתחול (דמו).", "בסיס נתונים", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"האתחול נכשל:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Navigation – פתיחת חלונות ניהול

        private void OpenVolunteers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // צור מופע של חלון ניהול מתנדבים
                var window = new VolunteerListWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"פתיחת מסך מתנדבים נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCalls_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // החלף לשם החלון בפועל, לדוגמה:
                // new Calls.CallsWindow().Show();

                MessageBox.Show("כאן ייפתח חלון ניהול קריאות (התאם Namespace לפי הפרויקט).",
                                "ניווט", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"פתיחת מסך קריאות נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
