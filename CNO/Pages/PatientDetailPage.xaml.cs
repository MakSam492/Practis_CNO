// C# Code-behind (PatientDetailPage.xaml.cs)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Data.Entity; // или Microsoft.EntityFrameworkCore для EF Core

// namespace CNO.Models; // Убедитесь, что есть using для ваших сущностей

namespace CNO.Pages
{
    public class MedicalHistoryViewModel
    {
        public int RecordID { get; set; }
        public DateTime RecordDate { get; set; }
        public string DoctorFullName { get; set; }
        public string DiagnosisOrNotes { get; set; }
    }

    public partial class PatientDetailPage : Page
    {
        private int _patientId;
        private Patients _loadedPatientData; // Тип вашей сущности Пациент

        public PatientDetailPage(int patientId)
        {
            InitializeComponent();
            _patientId = patientId;
            LoadPatientData(); // Сначала загружаем данные пациента
            // LoadMedicalHistory() будет вызван из LoadPatientData
            SetEditMode(false);
        }

        private void LoadPatientData()
        {
            try
            {
                _loadedPatientData = App.DB.Patients.FirstOrDefault(p => p.PatientID == _patientId);

                if (_loadedPatientData != null)
                {
                    PatientIdTextBox.Text = _loadedPatientData.PatientID.ToString();
                    LastNameTextBox.Text = _loadedPatientData.LastName;
                    FirstNameTextBox.Text = _loadedPatientData.FirstName;
                    
                    DateOfBirthTextBox.Text = _loadedPatientData.DateOfBirth?.ToString("dd.MM.yyyy") ?? "";
                    GenderTextBox.Text = _loadedPatientData.Gender;
                    PhoneNumberTextBox.Text = _loadedPatientData.ContactNumber;
                    AddressTextBox.Text = _loadedPatientData.Address;

                    LoadMedicalHistory(); // Вызываем загрузку истории после загрузки данных пациента
                }
                else
                {
                    MessageBox.Show("Пациент с таким ID не найден.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    if (NavigationService.CanGoBack) NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных пациента: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                if (NavigationService.CanGoBack) NavigationService.GoBack();
            }
        }

        // C# Code-behind (PatientDetailPage.xaml.cs) - метод LoadMedicalHistory

        private void LoadMedicalHistory()
        {
            try
            {
                // Используем ваш реальный контекст App.DB
                var query = from mr in App.DB.MedicalRecords // Начинаем с таблицы медицинских записей
                                                             // Явное объединение с таблицей докторов
                            join d in App.DB.Doctors on mr.DoctorID equals d.DoctorID into doctorGroup
                            from doctorData in doctorGroup.DefaultIfEmpty() // LEFT JOIN, чтобы не терять записи, если доктор не найден/удален
                            where mr.PatientID == _patientId // Фильтруем по ID текущего пациента
                            orderby mr.RecordDate descending // Сортируем по дате записи (сначала новые)
                            select new MedicalHistoryViewModel
                            {
                                RecordID = mr.RecordID,
                                RecordDate = (DateTime)mr.RecordDate, // Убедитесь, что тип r.RecordDate это DateTime
                                                            // Если DateTime?, то r.RecordDate.Value или r.RecordDate ?? defaultValue
                                DoctorFullName = (doctorData != null)
                                                 ? (doctorData.LastName + " " + doctorData.FirstName).Trim()
                                                 : "Врач не указан",
                                DiagnosisOrNotes = mr.Notes
                            };

                var history = query.ToList();
                MedicalHistoryListView.ItemsSource = history;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке истории медицинских записей: {ex.Message}\n{ex.StackTrace}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SetEditMode(bool isEditing)
        {
            LastNameTextBox.IsReadOnly = !isEditing;
            FirstNameTextBox.IsReadOnly = !isEditing;
            MiddleNameTextBox.IsReadOnly = !isEditing;
            PhoneNumberTextBox.IsReadOnly = !isEditing;
            AddressTextBox.IsReadOnly = !isEditing;

            EditButton.Visibility = isEditing ? Visibility.Collapsed : Visibility.Visible;
            SaveButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
            CancelButton.Visibility = isEditing ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            SetEditMode(true);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LastNameTextBox.Text) || string.IsNullOrWhiteSpace(FirstNameTextBox.Text))
            {
                MessageBox.Show("Фамилия и Имя не могут быть пустыми.", "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var patientToUpdate = App.DB.Patients.FirstOrDefault(p => p.PatientID == _patientId);
                if (patientToUpdate == null)
                {
                    MessageBox.Show("Не удалось найти пациента в базе данных для сохранения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                patientToUpdate.LastName = LastNameTextBox.Text.Trim();
                patientToUpdate.FirstName = FirstNameTextBox.Text.Trim();
                
                patientToUpdate.ContactNumber = PhoneNumberTextBox.Text.Trim();
                patientToUpdate.Address = AddressTextBox.Text.Trim();

                App.DB.Entry(patientToUpdate).State = System.Data.Entity.EntityState.Modified;
                App.DB.SaveChanges();

                // Обновляем _loadedPatientData после успешного сохранения
                if (_loadedPatientData != null) // Добавил проверку, на всякий случай
                {
                    _loadedPatientData.LastName = patientToUpdate.LastName;
                    _loadedPatientData.FirstName = patientToUpdate.FirstName;
                   
                    _loadedPatientData.ContactNumber = patientToUpdate.ContactNumber;
                    _loadedPatientData.Address = patientToUpdate.Address;
                }


                MessageBox.Show("Данные пациента успешно сохранены.", "Сохранение", MessageBoxButton.OK, MessageBoxImage.Information);
                SetEditMode(false);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении данных: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelEditButton_Click(object sender, RoutedEventArgs e)
        {
            // Перед выходом из режима редактирования, восстанавливаем значения из _loadedPatientData
            // чтобы UI вернулся к состоянию на момент последней загрузки/сохранения.
            if (_loadedPatientData != null)
            {
                LastNameTextBox.Text = _loadedPatientData.LastName;
                FirstNameTextBox.Text = _loadedPatientData.FirstName;
                
                PhoneNumberTextBox.Text = _loadedPatientData.ContactNumber;
                AddressTextBox.Text = _loadedPatientData.Address;
                // DateOfBirthTextBox.Text = _loadedPatientData.DateOfBirth?.ToString("dd.MM.yyyy") ?? "";
                // GenderTextBox.Text = _loadedPatientData.Gender;
            }
            SetEditMode(false);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}