using BIApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class OpenCallWindow : Window
    {
        #region Fields

        static readonly BIApi.IBl s_bl = BIApi.Factory.Get();
        private int volunteerId;

        #endregion

        #region Event

        /// <summary>
        /// Event שמתרחש כשנבחרה קריאה בהצלחה
        /// </summary>
        public event EventHandler CallSelected;

        #endregion

        #region Dependency Properties

        public IEnumerable<BO.OpenCallInList> OpenCalls
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallsProperty); }
            set { SetValue(OpenCallsProperty, value); }
        }
        public static readonly DependencyProperty OpenCallsProperty =
            DependencyProperty.Register(nameof(OpenCalls),
                typeof(IEnumerable<BO.OpenCallInList>),
                typeof(OpenCallWindow),
                new PropertyMetadata(null));

        public BO.OpenCallInList SelectedCall
        {
            get { return (BO.OpenCallInList)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register(nameof(SelectedCall),
                typeof(BO.OpenCallInList),
                typeof(OpenCallWindow),
                new PropertyMetadata(null));

        public bool HasCalls
        {
            get { return (bool)GetValue(HasCallsProperty); }
            set { SetValue(HasCallsProperty, value); }
        }
        public static readonly DependencyProperty HasCallsProperty =
            DependencyProperty.Register(nameof(HasCalls),
                typeof(bool),
                typeof(OpenCallWindow),
                new PropertyMetadata(false));

        public int CallsCount
        {
            get { return (int)GetValue(CallsCountProperty); }
            set { SetValue(CallsCountProperty, value); }
        }
        public static readonly DependencyProperty CallsCountProperty =
            DependencyProperty.Register(nameof(CallsCount),
                typeof(int),
                typeof(OpenCallWindow),
                new PropertyMetadata(0));

        #endregion

        #region Constructor

        public OpenCallWindow(int volunteerId)
        {
            InitializeComponent();
            DataContext = this;

            this.volunteerId = volunteerId;

            LoadOpenCalls();
        }

        #endregion

        #region Methods

        private void LoadOpenCalls()
        {
            try
            {
                OpenCalls = s_bl.Call.GetOpenCallsForVolunteer(volunteerId,null,null)
                    .OrderBy(c => c.DistanceFromVolunteer)
                    .ToList();

                CallsCount = OpenCalls?.Count() ?? 0;
                HasCalls = CallsCount > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"שגיאה בטעינת רשימת הקריאות:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                OpenCalls = new List<BO.OpenCallInList>();
                HasCalls = false;
                CallsCount = 0;
            }
        }

        #endregion

        #region Event Handlers

        private void SelectCallButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall == null)
            {
                MessageBox.Show(
                    "אנא בחר קריאה מהרשימה",
                    "לא נבחרה קריאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show(
                $"האם אתה בטוח שברצונך לטפל בקריאה מספר {SelectedCall.IdCall}?\n\n" +
                $"סוג: {SelectedCall.Type}\n" +
                $"כתובת: {SelectedCall.FullAddress}\n" +
                $"מרחק: {SelectedCall.DistanceFromVolunteer:F2} ק\"מ",
                "אישור בחירת קריאה",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.ChooseCallForTreatment(volunteerId, SelectedCall.IdCall);

                    MessageBox.Show(
                        $"הקריאה {SelectedCall.IdCall} הוקצתה לך בהצלחה!",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    // הפעל את ה-Event במקום DialogResult
                    CallSelected?.Invoke(this, EventArgs.Empty);

                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"שגיאה בהקצאת הקריאה:\n{ex.Message}",
                        "שגיאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOpenCalls();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CallItem_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is BO.OpenCallInList call)
            {
                SelectedCall = call;
            }
        }

        #endregion
    }
}