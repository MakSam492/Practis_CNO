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
    /// Логика взаимодействия для PatientPage.xaml
    /// </summary>
    public partial class PatientPage : Page
    {
        private List<object> _allPatientsOriginalData;
        public PatientPage()
        {
            InitializeComponent();
            LoadPatients();
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GoToMainPage_Click(object sender, RoutedEventArgs e)
        {

        }
        private void LoadPatients()
        {
            try
            {
                // Загружаем все записи медицинской истории
                var Patient = App.DB.Patients
             .Select(h => new
             {
                 ID = h.PatientID,
                 ID_Пациента = h.PatientID,
                 NumPatient = h.ContactNumber,
                 GenPatient = h.Gender,
                 AddressPatient = h.Address,
                 DateBirh = h.DateOfBirth,// ID пациента из медицинской истории
                 ФИО_Пациента = App.DB.Patients
                     .Where(p => p.PatientID == h.PatientID)
                     .Select(p => p.LastName + " " + p.FirstName)
                     .FirstOrDefault() // Получаем полное ФИО
                 

             })

             .ToList();

                lvPatient.ItemsSource = Patient;
                _allPatientsOriginalData = Patient.Cast<object>().ToList();
                ApplyPatientFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки медицинских записей: {ex.Message}\n\n{ex.InnerException?.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddPatient());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MainPageManager());
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ApplyPatientFilter();
        }
        private void LvPatient_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listView = sender as ListView;
            if (listView != null && listView.SelectedItem != null)
            {
                // Получаем выбранный элемент. Тип элемента зависит от того, 
                // что вы присваиваете lvPatient.ItemsSource.
                // В вашем предыдущем примере это был анонимный тип.
                dynamic selectedPatientData = listView.SelectedItem;

                try
                {
                    // Пытаемся получить ID_Пациента. Имя свойства должно совпадать
                    // с тем, что вы определили при загрузке данных в lvPatient
                    int patientId = selectedPatientData.ID_Пациента;

                    // Переход на страницу PatientDetailPage
                    if (NavigationService != null)
                    {
                        NavigationService.Navigate(new PatientDetailPage(patientId));
                    }
                }
                catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
                {
                    MessageBox.Show("Не удалось получить ID выбранного пациента.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void ApplyPatientFilter()
        {
            if (_allPatientsOriginalData == null)
            {
                // Данные еще не загружены или произошла ошибка при загрузке
                lvPatient.ItemsSource = null;
                return;
            }

            // Начинаем с полного списка пациентов
            IEnumerable<object> filteredPatients = _allPatientsOriginalData;

            var searchText = SearchTextBox.Text; // Получаем текст из вашего TextBox

            if (string.IsNullOrWhiteSpace(searchText) == false)
            {
                string lowerSearchText = searchText.ToLower(); // Для регистронезависимого поиска

                filteredPatients = _allPatientsOriginalData.Where(item =>
                {
                    // Так как item - это анонимный тип, используем dynamic для доступа к свойствам
                    dynamic patientData = item;
                    try
                    {
                        // Проверяем совпадение в нужных полях
                        // Имена свойств (ID_Пациента, ФИО_Пациента и т.д.) должны точно совпадать
                        // с теми, что определены в вашем .Select(h => new { ... })

                        string idStr = patientData.ID_Пациента?.ToString().ToLower() ?? "";
                        string fio = patientData.ФИО_Пациента?.ToString().ToLower() ?? "";
                        string numTel = patientData.NumPatient?.ToString().ToLower() ?? "";
                        string address = patientData.AddressPatient?.ToString().ToLower() ?? "";
                        string gender = patientData.GenPatient?.ToString().ToLower() ?? "";

                        string dateBirthStr = "";
                        if (patientData.DateBirh != null)
                        {
                            if (patientData.DateBirh is DateTime dateValue)
                            {
                                dateBirthStr = dateValue.ToString("dd.MM.yyyy").ToLower(); // Форматируем дату для поиска
                            }
                            else
                            {
                                dateBirthStr = patientData.DateBirh.ToString().ToLower();
                            }
                        }

                        return idStr.Contains(lowerSearchText) ||
                               fio.Contains(lowerSearchText) ||
                               numTel.Contains(lowerSearchText) ||
                               address.Contains(lowerSearchText) ||
                               dateBirthStr.Contains(lowerSearchText) ||
                               gender.Contains(lowerSearchText);
                    }
                    catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException ex)
                    {
                        // Ошибка доступа к свойству через dynamic (например, опечатка в имени свойства)
                        System.Diagnostics.Debug.WriteLine($"Ошибка связывания в фильтре пациентов: {ex.Message}");
                        return false;
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Общая ошибка в фильтре пациентов: {ex.Message}");
                        return false;
                    }
                }).ToList(); // Применяем фильтр и материализуем результат
            }

            // Если других фильтров нет (как selectedTransport в вашем примере), то это все.
            // Если бы были другие ComboBox'ы для фильтрации, их условия добавлялись бы сюда:
            // if (CBКакойТоФильтр.SelectedItem != null)
            // {
            //     var selectedFilterValue = (CBКакойТоФильтр.SelectedItem as YourFilterType);
            //     filteredPatients = filteredPatients.Where(item => {
            //          dynamic patientData = item;
            //          return patientData.КакоеТоСвойство == selectedFilterValue.Id; // Пример
            //     }).ToList();
            // }

            lvPatient.ItemsSource = filteredPatients; // Обновляем источник данных для ListView
        }
    }
}
