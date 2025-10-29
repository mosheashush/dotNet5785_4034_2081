using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using BlApi;
using BO;
using DalApi;
using DO;

namespace PL.Call
{
    /// <summary>
    /// Call Add/Edit Screen
    /// </summary>
    public partial class CallWindow : Window
    {
        // תלות עם BL
        static readonly BIApi.IBl s_bl = BIApi.Factory.Get();


        private readonly int? _IdCall; // null = add mode, has value = update mode

        public event PropertyChangedEventHandler? PropertyChanged;

        public void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private BO.CallState? _typeCallState;
        public BO.CallState? TypeCallState
        {
            get => _typeCallState;
            set
            {
                if (_typeCallState != value)
                {
                    _typeCallState = value;
                    OnPropertyChanged(nameof(TypeCallState));
                }
            }
        }

        #region Dependency Properties

        /// <summary>
        /// The current volunteer being displayed/edited
        /// </summary>
        public BO.Call CurrentCall
        {
            get { return (BO.Call)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register(nameof(CurrentCall),
                typeof(BO.Call),
                typeof(CallWindow));

        /// <summary>
        /// Button text (Add/Update)
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(nameof(ButtonText),
                typeof(string),
                typeof(CallWindow));

        /// <summary>
        /// Constructor for add mode (no parameters)
        /// </summary>
        public CallWindow()
        {
            InitializeComponent();

            try
            {
                _IdCall = null;

                // initialize a new call
                CurrentCall = new BO.Call
                {
                    IdCall = 0,
                    Type = BO.CallType.None,           
                    description = null,
                    FullAddress = "",
                    Latitude = 0,
                    Longitude = 0,
                    CallStartTime = s_bl.Admin.GetClock(),
                    MaxTimeForCall = null,
                    CallState = CallState.open,      
                    callAssignInLists = null,
                };

                ButtonText = "הוסף";
                Title = "הוסף קריאה חדשה";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בעת איתחול החלון:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Constructor for edit mode (with ID)
        /// </summary>
        /// <param name="volunteerId">Volunteer ID for editing</param>
        public CallWindow(int IdCall) : this()
        {
            _IdCall = IdCall;

            try
            {
                // Load existing volunteer
                CurrentCall = s_bl.Call.Read(IdCall)
                    ?? throw new Exception("המתנדב לא נמצא במערכת.");

                ButtonText = "ערוך";
                Title = $"ערוך קריאה: {CurrentCall.IdCall}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת הקריאה:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// Window loaded event
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Additional logic can be added here when the window loads
        }

        /// <summary>
        /// Add/Update button click
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Basic validation
                if (!ValidateCallInput())
                    return;

                // Perform action
                if (_IdCall == null) // Add mode
                {
                    s_bl.Call.Create(CurrentCall);
                    MessageBox.Show($"הקריאה נוסף בהצלחה!\nID: {CurrentCall.IdCall}",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else // Update mode
                {
                    s_bl.Call.Update(CurrentCall);
                    MessageBox.Show("הקריאה התעדכנה בהצלחה!",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                Close();
            }
            catch (DalXMLFileLoadCreateException ex)
            {
                // שגיאה ספציפית בקבצי XML
                MessageBox.Show($"שגיאה בשמירת הקריאה בקובץ XML:\n{ex.Message}\n\nפרטים נוספים:\n{ex.InnerException?.Message}",
                    "שגיאה קריטית",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // אל תסגור את החלון! תן למשתמש לנסות שוב
            }
            catch (Exception ex)
            {
                // שגיאה כללית
                MessageBox.Show($"שגיאה בשמירת הקריאה:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                // אל תסגור את החלון! תן למשתמש לנסות שוב
            }
        }

        /// <summary>
        /// Basic input validation
        /// </summary>
        private bool ValidateCallInput()
        {
            // בדיקת מזהה קריאה
            if (_IdCall == null)
            {
                if (CurrentCall.IdCall < 0)
                {
                    MessageBox.Show("מזהה קריאה חייב להיות מספר חיובי!",
                        "שגיאת קלט",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }
            }

            // סוג קריאה
            if (CurrentCall.Type == BO.CallType.None || CurrentCall.Type == BO.CallType.All)
            {
                MessageBox.Show("!יש לבחור סוג קריאה תקין\nאינו בפעילות' ו-'הכל' אינם ערכים חוקיים לקריאה'",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            if (!Enum.IsDefined(typeof(BO.CallType), CurrentCall.Type))
            {
                MessageBox.Show("סוג הקריאה אינו תקין!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // כתובת
            if (string.IsNullOrWhiteSpace(CurrentCall.FullAddress))
            {
                MessageBox.Show("יש להזין כתובת מלאה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // קואורדינטות
            if (CurrentCall.Latitude < -90 || CurrentCall.Latitude > 90)
            {
                MessageBox.Show("קו רוחב (Latitude) חייב להיות בין ‎-90 ל-90‎!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (CurrentCall.Longitude < -180 || CurrentCall.Longitude > 180)
            {
                MessageBox.Show("קו אורך (Longitude) חייב להיות בין ‎-180 ל-180‎!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // זמנים
            if (CurrentCall.CallStartTime == default)
            {
                MessageBox.Show("חייב להיות זמן התחלה תקף לקריאה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            if (CurrentCall.MaxTimeForCall.HasValue &&
                CurrentCall.MaxTimeForCall <= CurrentCall.CallStartTime)
            {
                MessageBox.Show("הזמן המקסימלי לקריאה חייב להיות מאוחר מזמן ההתחלה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // מצב קריאה
            if (!Enum.IsDefined(typeof(CallState), CurrentCall.CallState))
            {
                MessageBox.Show("מצב הקריאה אינו תקין!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            return true;
        }


        /// <summary>
        /// Cancel button click
        /// </summary>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Confirmation before closing
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
        /// Window closed
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            // Additional logic can be added here when the window closes
        }
    }
    #endregion Dependency Properties
}
