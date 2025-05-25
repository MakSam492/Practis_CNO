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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CNO.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainPageManager.xaml
    /// </summary>
    public partial class MainPageManager : Page
    {
        public MainPageManager()
        {
            InitializeComponent();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ManageDoctors_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DoctorsPage());
        }

        private void ManagePatients_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientPage());
        }

        private void ManageAppointments_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RecordsPage());
        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
