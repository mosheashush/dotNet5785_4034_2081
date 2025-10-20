using BIApi;
using PL.Volunteer;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();
public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += Window_Closed;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Default values for display
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskTimeSpan();

            // Register to watch changes
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);    
        }

        #region Dependency Properties – Clock and Configuration

        // CurrentTime (DateTime) 
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

        // RiskRange (TimeSpan)
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

        private void AdvanceMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Minute);
        }
        private void AdvanceHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Hour);
        }
        private void AdvanceDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Day);
        }
        private void AdvanceMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Month);
        }
        private void AdvanceYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.AdvanceClock(BO.TimeUnit.Year);
        }

        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void configObserver()
        {
            RiskRange = s_bl.Admin.GetRiskTimeSpan();
        }


        private void RefreshFromBlClock_Click(object sender, RoutedEventArgs e)
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        private void ResetClockToNow_Click(object sender, RoutedEventArgs e)
        {
           // s_bl.Admin. (DateTime.Now);
        }

        #endregion

        #region Config – Save/Load

        private void SaveConfigToBl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetRiskTimeSpan(RiskRange);
                MessageBox.Show("ההגדרות נשמרו", "הודעה", MessageBoxButton.OK, MessageBoxImage.Information);
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
                Mouse.OverrideCursor = Cursors.Wait;

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                s_bl.Admin.ResetDatabase();
                MessageBox.Show("(האתחול בוצע (דמו.", "מסד נתונים", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"האתחול נכשל:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void InitDb_Click(object sender, RoutedEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Wait;

            foreach (Window window in Application.Current.Windows)
            {
                if (window != this)
                    window.Close();
            }

            if (MessageBox.Show("אתחול מסד הנתונים עם נתונים התחלתיים?",
                                "אישור פעולה", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes)
                return;

            try
            {
                 s_bl.Admin.ResetDatabase();
                 s_bl.Admin.InitializeDatabase();

                MessageBox.Show("(האתחול בוצע (דמו.", "מסד נתונים", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"האתחול נכשל:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
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
