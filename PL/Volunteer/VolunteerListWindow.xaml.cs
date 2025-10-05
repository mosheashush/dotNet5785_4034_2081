using System;
using System.Collections.ObjectModel;
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
    public partial class VolunteerListWindow : Window
    {
        // תלות עם BL
        static readonly BIApi.IBl s_dal = BIApi.Factory.Get();

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

        /// <summary>
        /// פילטר לפי סטטוס פעיל/לא פעיל
        /// </summary>
        public bool? IsActiveFilter
        {
            get { return (bool?)GetValue(IsActiveFilterProperty); }
            set { SetValue(IsActiveFilterProperty, value); }
        }
        public static readonly DependencyProperty IsActiveFilterProperty =
            DependencyProperty.Register(nameof(IsActiveFilter),
                typeof(bool?),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// בנאי המסך
        /// </summary>
        public VolunteerListWindow()
        {
            InitializeComponent();

            try
            {
                // רישום ל-Observer לעדכון אוטומטי
                s_dal.Volunteer.AddObserver(RefreshVolunteersList);

                // טעינה ראשונית של הרשימה
                LoadVolunteers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה באתחול המסך:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Close();
            }
        }

        /// <summary>
        /// טעינת כל המתנדבים לרשימה
        /// </summary>
        private void LoadVolunteers()
        {
            try
            {
                // קבלת הרשימה מה-BL (ללא סינון ומיון ברירת מחדל)
                var volunteers = s_dal.Volunteer.listOfVolunteer(null, null);
                VolunteersList = new ObservableCollection<VolunteerInList>(volunteers);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת המתנדבים:\n{ex.Message}",
                    "שגיאה",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                if (IsActiveFilter == null)
                    LoadVolunteers();
                else
                    FilterVolunteersByStatus();
            });
        }

        /// <summary>
        /// סינון הרשימה לפי סטטוס נבחר (פעיל/לא פעיל)
        /// </summary>
        private void Status_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.SelectedIndex >= 0)
            {
                switch (comboBox.SelectedIndex)
                {
                    case 0: // הכל
                        IsActiveFilter = null;
                        break;
                    case 1: // פעילים בלבד
                        IsActiveFilter = true;
                        break;
                    case 2: // לא פעילים בלבד
                        IsActiveFilter = false;
                        break;
                }

                FilterVolunteersByStatus();
            }
        }

        /// <summary>
        /// סינון מתנדבים לפי הסטטוס הנבחר
        /// </summary>
        private void FilterVolunteersByStatus()
        {
            try
            {
                var filtered = s_dal.Volunteer.listOfVolunteer(IsActiveFilter, null);
                VolunteersList = new ObservableCollection<VolunteerInList>(filtered);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בסינון המתנדבים:\n{ex.Message}",
                    "שגיאה",
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
                    s_dal.Volunteer.Delete(volunteer.IdVolunteer);
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
        protected override void OnClosed(EventArgs e)
        {
            s_dal.Volunteer.RemoveObserver(RefreshVolunteersList);
            base.OnClosed(e);
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}