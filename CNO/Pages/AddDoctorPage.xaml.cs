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
    /// Логика взаимодействия для AddDoctorPage.xaml
    /// </summary>
    public partial class AddDoctorPage : Page
    {
        public AddDoctorPage()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameBox.Text) ||
               string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("Фамилия и имя обязательны для заполнения!",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            // Создание нового пациента
            var doctor = new Doctors
            {
                LastName = LastNameBox.Text.Trim(),
                FirstName = FirstNameBox.Text.Trim(),
                Specialization = SpecializationBox.Text.Trim(),
                ContactNumber = PhoneBox.Text.Trim(),
            };

            try
            {
                App.DB.Doctors.Add(doctor);
                App.DB.SaveChanges();

                MessageBox.Show("Доктор успешно добавлен в базу данных!",
                               "Успех",
                               MessageBoxButton.OK,
                               MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}",
                              "Ошибка базы данных",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DoctorsPage());

        }
    }
}
