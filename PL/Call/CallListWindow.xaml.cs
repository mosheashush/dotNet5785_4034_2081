using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;

namespace PL.Call
{
    /// <summary>
    /// מסך תצוגת רשימת קריאות
    /// </summary>
    public partial class CallListWindow : Window, INotifyPropertyChanged
    {
        // תלות עם BL
        static readonly BIApi.IBl s_bl = BIApi.Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private BO.CallState? _typeCall;
        public BO.CallState? TypeCall
        {
            get => _typeCall;
            set
            {
                if (_typeCall != value)
                {
                    _typeCall = value;
                    OnPropertyChanged(nameof(TypeCall));
                    // אם אתה רוצה שהרשימה תתעדכן אוטומטית:
                    FilterCallsByStatus();
                }
            }
        }

        public CallListWindow()
        {
            InitializeComponent();

            // Attach event handlers in the constructor
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        #region Dependency Properties

        /// <summary>
        /// רשימת הקריאות המוצגת
        /// </summary>
        public ObservableCollection<CallInList> CallsList
        {
            get { return (ObservableCollection<CallInList>)GetValue(CallsListProperty); }
            set { SetValue(CallsListProperty, value); }
        }

        public static readonly DependencyProperty CallsListProperty =
            DependencyProperty.Register(nameof(CallsList),
                typeof(ObservableCollection<CallInList>),
                typeof(CallListWindow));

        /// <summary>
        /// הקריאה הנבחר ברשימה
        /// </summary>
        public CallInList SelectedCall
        {
            get { return (CallInList)GetValue(SelectedCallProperty); }
            set { SetValue(SelectedCallProperty, value); }
        }
        public static readonly DependencyProperty SelectedCallProperty =
            DependencyProperty.Register(nameof(SelectedCall),
                typeof(CallInList),
                typeof(CallListWindow));

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // רישום מתודת ההשקפה על רשימת הקריאות
                s_bl.Call.AddObserver(RefreshCallsList);

                // טען את הרשימה הראשונית
                FilterCallsByStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בעת טעינת המסך:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// עדכון אוטומטי של הרשימה (Observer)
        /// </summary>
        private void RefreshCallsList()
        {
            // חובה להריץ ב-UI Thread
            Dispatcher.Invoke(() =>
            {
                FilterCallsByStatus();
            });
        }

        /// <summary>
        /// סינון הרשימה לפי סטטוס נבחר משלוח אוכל/הכנת אוכל)
        /// </summary>
        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterCallsByStatus();
        }

        /// <summary>
        /// סינון קריאות לפי הסטטוס הנבחר
        /// </summary>
        private void FilterCallsByStatus()
        {
            try
            {
                IEnumerable<CallInList> filtered;

                if (TypeCall == null || TypeCall == CallState.all)
                {
                    filtered = s_bl.Call.GetCallsList(null, null, null);
                }
                else
                {
                    filtered = s_bl.Call.GetCallsList(BO.CallInListFields.CallState, TypeCall, null);
                }

                CallsList = new ObservableCollection<CallInList>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת המתנדבים:\n{ex.Message}", "שגיאה",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// לחיצה כפולה על מתנדב - פתיחת מסך עריכה
        /// </summary>
        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                try
                {
                    // פתיחת מסך עריכה עם המתנדב הנבחר
                    new CallWindow(SelectedCall.IdCall).ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה בפתיחת מסך המתנדב:\n{ex.Message}",
                        "שגיאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// לחיצה על כפתור הוספה - פתיחת מסך הוספת מתנדב חדש
        /// </summary>
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // פתיחת מסך הוספה (ללא ID)
                new CallWindow().ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בפתיחת מסך הוספה:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// לחיצה על כפתור הוספה - פתיחת מסך הוספת מתנדב חדש
        /// </summary>
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall != null)
            {
                try
                {
                    // פתיחת מסך עריכה עם המתנדב הנבחר
                    new CallWindow(SelectedCall.IdCall).ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה בפתיחת מסך המתנדב:\n{ex.Message}",
                        "שגיאה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }

        /// <summary>
        /// לחיצה על כפתור מחיקה בשורה
        /// </summary>
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // קבלת המתנדב מה-DataContext של הכפתור
                var button = sender as Button;
                var volunteer = button?.DataContext as VolunteerInList;

                if (volunteer == null)
                    return;

                // אישור מחיקה
                var result = MessageBox.Show(
                    $"האם אתה בטוח שברצונך למחוק את המתנדב:\n{volunteer.FullName}?\n\n" +
                    "שים לב: ניתן למחוק מתנדב רק אם הוא לא טיפל מעולם בקריאות.",
                    "אישור מחיקה",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    s_bl.Volunteer.Delete(volunteer.IdVolunteer);
                    MessageBox.Show("המתנדב נמחק בהצלחה!",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה במחיקת המתנדב:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// סגירת המסך - ביטול רישום ל-Observer
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                // הסרת המשקיף
                s_bl.Volunteer.RemoveObserver(RefreshCallsList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בעת סגירת המסך:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}