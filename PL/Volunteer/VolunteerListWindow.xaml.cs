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

namespace PL.Volunteer
{
    /// <summary>
    /// מסך תצוגת רשימת מתנדבים
    /// </summary>
    public partial class VolunteerListWindow : Window, INotifyPropertyChanged
    {
        // תלות עם BL
        static readonly BIApi.IBl s_bl = BIApi.Factory.Get();

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private BO.CallType? _typeVolunteer;
        public BO.CallType? TypeVolunteer
        {
            get => _typeVolunteer;
            set
            {
                if (_typeVolunteer != value)
                {
                    _typeVolunteer = value;
                    OnPropertyChanged(nameof(TypeVolunteer));
                    // אם אתה רוצה שהרשימה תתעדכן אוטומטית:
                    FilterVolunteersByStatus();
                }
            }
        }

        public VolunteerListWindow()
        {
            InitializeComponent();

            // Attach event handlers in the constructor
            Loaded += Window_Loaded;
            Closed += Window_Closed;
        }

        #region Dependency Properties

        /// <summary>
        /// רשימת המתנדבים המוצגת
        /// </summary>
        public ObservableCollection<VolunteerInList> VolunteersList
        {
            get { return (ObservableCollection<VolunteerInList>)GetValue(VolunteersListProperty); }
            set { SetValue(VolunteersListProperty, value); }
        }

        public static readonly DependencyProperty VolunteersListProperty =
            DependencyProperty.Register(nameof(VolunteersList),
                typeof(ObservableCollection<VolunteerInList>),
                typeof(VolunteerListWindow));

        /// <summary>
        /// המתנדב הנבחר ברשימה
        /// </summary>
        public VolunteerInList SelectedVolunteer
        {
            get { return (VolunteerInList)GetValue(SelectedVolunteerProperty); }
            set { SetValue(SelectedVolunteerProperty, value); }
        }
        public static readonly DependencyProperty SelectedVolunteerProperty =
            DependencyProperty.Register(nameof(SelectedVolunteer),
                typeof(VolunteerInList),
                typeof(VolunteerListWindow));

        #endregion

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // רישום מתודת ההשקפה על רשימת המתנדבים
                s_bl.Volunteer.AddObserver(RefreshVolunteersList);

                // טען את הרשימה הראשונית
                FilterVolunteersByStatus();
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
        private void RefreshVolunteersList()
        {
            // חובה להריץ ב-UI Thread
            Dispatcher.Invoke(() =>
            {
                FilterVolunteersByStatus();
            });
        }

        /// <summary>
        /// סינון הרשימה לפי סטטוס נבחר משלוח אוכל/הכנת אוכל)
        /// </summary>
        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterVolunteersByStatus();
        }

        /// <summary>
        /// סינון מתנדבים לפי הסטטוס הנבחר
        /// </summary>
        private void FilterVolunteersByStatus()
        {
            try
            {
                IEnumerable<VolunteerInList> filtered;

                if (TypeVolunteer == null || TypeVolunteer == CallType.All)
                {
                    filtered = s_bl.Volunteer.listOfVolunteer(null, null, null);
                }
                else
                {
                    filtered = s_bl.Volunteer.listOfVolunteer(null, BO.VolunteerInListFields.Type, TypeVolunteer);
                }

                VolunteersList = new ObservableCollection<VolunteerInList>(filtered);
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
            if (SelectedVolunteer != null)
            {
                try
                {
                    // פתיחת מסך עריכה עם המתנדב הנבחר
                    new VolunteerWindow(SelectedVolunteer.IdVolunteer).ShowDialog();
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
                new VolunteerWindow().ShowDialog();
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
            try
            {
                // פתיחת מסך הוספה (ללא ID)
                new VolunteerWindow().ShowDialog();
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
                s_bl.Volunteer.RemoveObserver(RefreshVolunteersList);
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