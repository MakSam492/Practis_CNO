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
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateToPatients(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new PatientPage());
        }

        private void NavigateToDoctors(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DoctorsPage());
        }

        private void NavigateToAppointments(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateToDiagnosis(object sender, RoutedEventArgs e)
        {

        }

        private void EditProfile(object sender, RoutedEventArgs e)
        {

        }

        private void Search(object sender, RoutedEventArgs e)
        {

        }

        private void NavigateToNewAppointment(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRecordPage());
        }
       

        private void AddNews_Click(object sender, RoutedEventArgs e)
        {

        }

        private void WorkSchedule_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Appointments_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Memos_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EditProfile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StackPanel_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
