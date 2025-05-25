// CNO.Pages.RecordsPage.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CNO.ViewModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Data.Entity;

namespace CNO.Pages
{
    public partial class RecordsPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<RecordViewModel> AllRecords { get; private set; }
        public int TotalRecordsCount => AllRecords?.Count ?? 0;

        public RecordsPage()
        {
            InitializeComponent();
            InitializeCollectionAndDataContext();
            Loaded += RecordsPage_Loaded;
        }

        private void InitializeCollectionAndDataContext()
        {
            Debug.WriteLine("RecordsPage: Инициализация коллекции и DataContext...");
            AllRecords = new ObservableCollection<RecordViewModel>();
            AllRecords.CollectionChanged += (s, e) => OnPropertyChanged(nameof(TotalRecordsCount));

            RecordsListView.ItemsSource = AllRecords;

            this.DataContext = this;
            Debug.WriteLine("RecordsPage: Коллекция и DataContext инициализированы.");
        }

        private async void RecordsPage_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("RecordsPage: Событие Page Loaded.");
            await LoadRecordsAsync();
        }

        private async Task LoadRecordsAsync()
        {
            Debug.WriteLine("LoadRecordsAsync: Начало загрузки записей...");
            AllRecords.Clear();
            try
            {
                var recordsFromDb = await App.DB.MedicalRecords
                                        .Where(r => r.RecordID != null)
                                        .OrderByDescending(r => r.RecordDate)
                                        .ToListAsync();

                Debug.WriteLine($"LoadRecordsAsync: Загружено {recordsFromDb.Count} записей из БД.");

                foreach (var dbRecord in recordsFromDb)
                {
                    if (dbRecord.PatientID == null || dbRecord.DoctorID == null)
                    {
                        Debug.WriteLine($"LoadRecordsAsync: Пропуск записи ID {dbRecord.RecordID} из-за отсутствия PatientID или DoctorID.");
                        continue;
                    }

                    var viewModel = new RecordViewModel
                    {
                        RecordID = dbRecord.RecordID,
                        OriginalPatientID = dbRecord.PatientID.Value,
                        OriginalDoctorID = dbRecord.DoctorID.Value,
                        RecordDate = dbRecord.RecordDate,
                        Notes = dbRecord.Notes,
                        
                    };

                    var patient = await App.DB.Patients.FirstOrDefaultAsync(p => p.PatientID == dbRecord.PatientID.Value);
                    viewModel.PatientName = patient?.FirstName ?? $"Пациент ID: {dbRecord.PatientID.Value}";

                    var doctor = await App.DB.Doctors.FirstOrDefaultAsync(d => d.DoctorID == dbRecord.DoctorID.Value);
                    viewModel.DoctorName = doctor?.FirstName ?? $"Врач ID: {dbRecord.DoctorID.Value}";

                    AllRecords.Add(viewModel);
                }
                Debug.WriteLine("LoadRecordsAsync: Завершение обработки записей.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadRecordsAsync: ОШИБКА - {ex.Message}\n{ex.StackTrace}");
                MessageBox.Show($"Ошибка загрузки записей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            OnPropertyChanged(nameof(TotalRecordsCount));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }

        private async void AddRecordButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddRecordPage());
            // await LoadRecordsAsync(); 
        }

        private void RecordItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is RecordViewModel selectedRecord)
            {
                Debug.WriteLine($"Двойной клик по записи: {selectedRecord.PatientName}, ID: {selectedRecord.RecordID}");
                MessageBox.Show($"Редактирование записи ID: {selectedRecord.RecordID} (функционал не реализован)");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}