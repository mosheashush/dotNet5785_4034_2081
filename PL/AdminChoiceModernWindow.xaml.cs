using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL
{
    /// <summary>
    /// Interaction logic for AdminChoiceModernWindow.xaml
    /// </summary>
    public partial class AdminChoiceModernWindow : Window
    {
        public bool? IsAdminChosen { get; private set; }  // true = מנהל, false = מתנדב, null = ביטול

        public AdminChoiceModernWindow()
        {
            InitializeComponent();
        }

        private void AdminButton_Click(object sender, RoutedEventArgs e)
        {
            IsAdminChosen = true;
            DialogResult = true;  // סוגר את החלון
        }

        private void VolunteerButton_Click(object sender, RoutedEventArgs e)
        {
            IsAdminChosen = false;
            DialogResult = true;  // סוגר את החלון
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            IsAdminChosen = null;
            DialogResult = false; // סוגר את החלון
        }
    }

}
