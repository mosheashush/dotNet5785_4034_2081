using System;
using System.Windows;
using BlApi;
using BO;

namespace PL.Volunteer
{
    /// <summary>
    /// Volunteer Add/Edit Screen
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        // תלות עם BL
        static readonly BIApi.IBl s_dal = BIApi.Factory.Get();

        private readonly int? _volunteerId; // null = add mode, has value = update mode

        #region Dependency Properties

        /// <summary>
        /// The current volunteer being displayed/edited
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
                typeof(VolunteerWindow));

        /// <summary>
        /// Constructor for add mode (no parameters)
        /// </summary>
        public VolunteerWindow()
        {
            InitializeComponent();

            try
            {
                _volunteerId = null;

                // initialize a new volunteer
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
                    CurrentPosition = User.volunteer, // default
                    Active = true,
                    MaxDistanceForCall = null,
                    TypeOfDistance = Distance.air, // default
                    SumCallsCompleted = 0,
                    SumCallsExpired = 0,
                    SumCallsConcluded = 0,
                    CallInProgress = null
                };

                ButtonText = "הוסף";
                Title = "הוסף מתנדב חדש";
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
        public VolunteerWindow(int volunteerId) : this()
        {
            _volunteerId = volunteerId;

            try
            {
                // Load existing volunteer
                CurrentVolunteer = s_dal.Volunteer.Read(volunteerId)
                    ?? throw new Exception("המתנדב לא נמצא במערכת.");

                // Load password to PasswordBox (if exists)
                if (!string.IsNullOrEmpty(CurrentVolunteer.Password))
                {
                    passwordBox.Password = CurrentVolunteer.Password;
                }

                ButtonText = "ערוך";
                Title = $"ערוך מתנדב: {CurrentVolunteer.FullName}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת המתנדב:\n{ex.Message}",
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
                // Update password from PasswordBox before saving
                CurrentVolunteer.Password = passwordBox.Password;

                // Basic validation
                if (!ValidateInput())
                    return;

                // Perform action
                if (_volunteerId == null) // Add mode
                {
                    s_dal.Volunteer.Create(CurrentVolunteer);
                    MessageBox.Show($"המתנדב נוסף בהצלחה!\nID: {CurrentVolunteer.id}",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
                else // Update mode
                {
                    // In update, also send the requester's ID
                    // In our case, assume it's admin (0) - adjust according to your logic
                    s_dal.Volunteer.Update(0, CurrentVolunteer);
                    MessageBox.Show("המתנדב התעדכן בהצלחה!",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }

                // Automatically close the window
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בשמירת המתנדב\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Basic input validation
        /// </summary>
        private bool ValidateInput()
        {
            // Check ID (only in add mode)
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

            // Check full name
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
            {
                MessageBox.Show("שם מלא הוא שדה חובה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // Check phone number
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.CallNumber))
            {
                MessageBox.Show("מספר טלפון הוא שדה חובה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // Check email
            if (string.IsNullOrWhiteSpace(CurrentVolunteer.EmailAddress))
            {
                MessageBox.Show("כתובת אימייל היא שדה חובה!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // Basic email format check
            if (!CurrentVolunteer.EmailAddress.Contains("@") ||
                !CurrentVolunteer.EmailAddress.Contains("."))
            {
                MessageBox.Show("כתובת האימייל אינה בפורמט תקין!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // Check password (only in add mode)
            if (_volunteerId == null && string.IsNullOrWhiteSpace(passwordBox.Password))
            {
                MessageBox.Show("סיסמה היא שדה חובה עבור מתנדב חדש!",
                    "שגיאת קלט",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            // Check max distance (if entered)
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