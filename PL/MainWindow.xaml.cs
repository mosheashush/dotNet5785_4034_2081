using PL.Volunteer;
using System;
using System.Globalization;
using System.Windows;

namespace PL
{
    public partial class MainWindow : Window
    {
        // === BL Access (Uncomment when you have it) ===
        // static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public MainWindow()
        {
            InitializeComponent();

            // Default values for display
            CurrentTime = DateTime.Now;
            RiskRange = TimeSpan.FromMinutes(30);

            // Ability to update the UI when the clock or config changes in the BL
            // s_bl.ClockChanged  += (s, e) => CurrentTime = s_bl.GetClock();
            // s_bl.ConfigChanged += (s, e) => RiskRange  = s_bl.GetRiskRange();
        }

        #region Dependency Properties – Clock and Configuration

        // CurrentTime (DateTime) + derived for display (string)
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

        // RiskRange (TimeSpan) + derived for display/input (string)
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register(nameof(RiskRange), typeof(TimeSpan), typeof(MainWindow),
                new PropertyMetadata(TimeSpan.Zero, (d, e) =>
                {
                    var win = (MainWindow)d;
                    win.RiskRangeString = ((TimeSpan)e.NewValue).ToString(); // Standard format: hh:mm:ss
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
                        win.RiskRange = ts; // Two-way binding on the TextBox
                }));

        public string RiskRangeString
        {
            get => (string)GetValue(RiskRangeStringProperty);
            set => SetValue(RiskRangeStringProperty, value);
        }

        #endregion

        #region Clock – Advance/Refresh/Reset

        private void AdvanceBy(TimeSpan delta)
        {
            // Full implementation via BL:
            // s_bl.AdvanceClock(delta);
            // CurrentTime = s_bl.GetClock();

            // Temporary local implementation (works even without BL):
            CurrentTime = CurrentTime.Add(delta);
        }

        private void AdvanceMinute_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromMinutes(1));
        private void AdvanceHour_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromHours(1));
        private void AdvanceDay_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromDays(1));
        private void AdvanceMonth_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromDays(30));  // Simplicity
        private void AdvanceYear_Click(object sender, RoutedEventArgs e) => AdvanceBy(TimeSpan.FromDays(365)); // Simplicity

        private void RefreshFromBlClock_Click(object sender, RoutedEventArgs e)
        {
            // CurrentTime = s_bl.GetClock();
            CurrentTime = DateTime.Now; // Temporary
        }

        private void ResetClockToNow_Click(object sender, RoutedEventArgs e)
        {
            // s_bl.SetClock(DateTime.Now);
            CurrentTime = DateTime.Now;
        }

        #endregion

        #region Config – Save/Load

        private void SaveConfigToBl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // s_bl.SetRiskRange(RiskRange);
                MessageBox.Show("(ההגדרות נשמרו (דמו.", "תצורה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שמירת ההגדרות נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConfigFromBl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // RiskRange = s_bl.GetRiskRange();
                MessageBox.Show("שומר ההגדרות (דמו).", "תצורה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"טעינת ההגדרות נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region DB – Reset/Initialize

        private void ResetDb_Click(object sender, RoutedEventArgs e)
        {
             if (MessageBox.Show("איפוס מסד הנתונים? פעולה זו תמחק נתונים ותשחזר הגדרות ברירת מחדל.",
                                  "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes)

                return;

            try
            {
                // s_bl.ResetDatabase();
                MessageBox.Show("(האתחול בוצע (דמו.", "מסד נתונים", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"האתחול נכשל:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitDb_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("אתחול מסד הנתונים עם נתונים התחלתיים?",
                                "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                // Example of typical flow:
                // s_bl.ResetDatabase();
                // s_bl.InitializeDatabase();

                MessageBox.Show("(האתחול בוצע (דמו.", "מסד נתונים", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"האתחול נכשל:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region Navigation – Open Management Windows

        private void OpenVolunteers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create an instance of the volunteer management window
                var window = new VolunteerListWindow();
                window.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"פתיחת מסך המתנדבים נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OpenCalls_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Replace with the actual window name, for example:
                // new Calls.CallsWindow().Show();
                MessageBox.Show("כאן ייפתח מסך ניהול הקריאות (התאם את ה-Namespace לפי הפרויקט).",
                                "ניווט", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"פתיחת מסך הקריאות נכשלה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
