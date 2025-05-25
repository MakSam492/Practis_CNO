using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using CNO.ViewModel; // ЗАМЕНИТЕ на ваше пространство имен для сущностей // Для EF Core, если используете

namespace CNO.Pages // ЗАМЕНИТЕ на ваше пространство имен
{
    public partial class AddRecordPage : Page
    {
        public AddRecordPage()
        {
            InitializeComponent();
            Loaded += AddRecordPage_Loaded;
        }

        private void AddRecordPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDoctorComboBoxData(); // Загружаем только врачей
            RecordDatePicker.SelectedDate = DateTime.Today; // Дата приема по умолчанию
            // PatientBirthDatePicker можно оставить пустым или установить какую-то дату по умолчанию
        }

        private void LoadDoctorComboBoxData()
        {
            try
            {
                // ЗАМЕНИТЕ App.DB.Doctors, FullName, DoctorID на ваши актуальные значения
                var doctors = App.DB.Doctors.OrderBy(d => d.FirstName).ToList();
                DoctorComboBox.ItemsSource = doctors;
                DoctorComboBox.DisplayMemberPath = "FirstName";
                DoctorComboBox.SelectedValuePath = "DoctorID";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки списка врачей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Валидация полей пациента
            if (string.IsNullOrWhiteSpace(PatientLastNameTextBox.Text))
            {
                MessageBox.Show("Фамилия пациента обязательна для заполнения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                PatientLastNameTextBox.Focus(); return;
            }
            if (string.IsNullOrWhiteSpace(PatientFirstNameTextBox.Text))
            {
                MessageBox.Show("Имя пациента обязательно для заполнения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                PatientFirstNameTextBox.Focus(); return;
            }
            string phoneNumber = PatientPhoneNumberTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                MessageBox.Show("Номер телефона пациента обязателен для заполнения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                PatientPhoneNumberTextBox.Focus(); return;
            }

            // 2. Валидация полей записи
            if (DoctorComboBox.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите врача.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                DoctorComboBox.Focus(); return;
            }
            if (RecordDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите дату записи.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                RecordDatePicker.Focus(); return;
            }

            // 3. Поиск или создание пациента
            Patients patientForRecord; // Переменная для хранения пациента для записи

            // ЗАМЕНИТЕ App.DB.Patients и ContactNumber/PhoneNumber на ваши актуальные значения
            patientForRecord = App.DB.Patients.FirstOrDefault(p => p.ContactNumber == phoneNumber);

            if (patientForRecord == null) // Пациент не найден, создаем нового
            {
                // Дополнительная валидация для нового пациента
                if (!PatientBirthDatePicker.SelectedDate.HasValue)
                {
                    MessageBox.Show("Для нового пациента необходимо указать дату рождения.", "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Warning);
                    PatientBirthDatePicker.Focus(); return;
                }

                patientForRecord = new Patients // ЗАМЕНИТЕ Patient и имена свойств
                {
                    LastName = PatientLastNameTextBox.Text.Trim(),
                    FirstName = PatientFirstNameTextBox.Text.Trim(),
                    
                    ContactNumber = phoneNumber, // Используем очищенный номер
                    DateOfBirth = PatientBirthDatePicker.SelectedDate.Value
                    // Другие поля пациента, если есть
                };

                try
                {
                    App.DB.Patients.Add(patientForRecord);
                    App.DB.SaveChanges(); // Сохраняем нового пациента СНАЧАЛА
                    MessageBox.Show("Новый пациент успешно создан.", "Создание пациента", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при создании нового пациента: {ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
                    return; // Прерываем, если пациента не удалось создать
                }
            }
            else
            {
                MessageBox.Show($"Найден существующий пациент: {patientForRecord.FirstName}. Запись будет создана для него.", "Поиск пациента", MessageBoxButton.OK, MessageBoxImage.Information);
                // Опционально: можно заполнить поля данными найденного пациента, если они были пустыми,
                // или спросить пользователя, действительно ли это тот пациент.
                // Для простоты сейчас просто используем найденного.
            }

            // 4. Создание медицинской записи
            Doctors selectedDoctor = DoctorComboBox.SelectedItem as Doctors; // ЗАМЕНИТЕ Doctor
            if (selectedDoctor == null)
            {
                // Этого не должно случиться после валидации, но для безопасности
                MessageBox.Show("Ошибка получения выбранного врача.", "Внутренняя ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Обработка времени (если есть поле RecordTimeTextBox)
            DateTime recordDateTime = RecordDatePicker.SelectedDate.Value;
            if (!string.IsNullOrWhiteSpace(RecordTimeTextBox.Text) && TimeSpan.TryParse(RecordTimeTextBox.Text.Trim(), out TimeSpan timeOfDay))
            {
                recordDateTime = recordDateTime.Date + timeOfDay;
            }


            var newRecord = new MedicalRecords // ЗАМЕНИТЕ MedicalRecord и имена свойств
            {
                PatientID = patientForRecord.PatientID, // PatientID теперь всегда есть (либо найден, либо только что создан)
                DoctorID = selectedDoctor.DoctorID,
                RecordDate = recordDateTime, // Используем дату (и возможно время)
                Notes = NotesTextBox.Text.Trim()
            };

            try
            {
                App.DB.MedicalRecords.Add(newRecord); // ЗАМЕНИТЕ MedicalRecords
                App.DB.SaveChanges();

                MessageBox.Show("Медицинская запись успешно добавлена!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // ЗАМЕНИТЕ RecordsPage на имя вашей страницы со списком записей
                NavigationService.Navigate(new RecordsPage());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении медицинской записи: {ex.Message}", "Ошибка базы данных", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // ЗАМЕНИТЕ RecordsPage на имя вашей страницы со списком записей
            NavigationService.Navigate(new RecordsPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                // ЗАМЕНИТЕ RecordsPage на имя вашей страницы со списком записей
                NavigationService.Navigate(new RecordsPage());
            }
        }
    }
}