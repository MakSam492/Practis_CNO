// CNO.Pages.DoctorsPage.xaml.cs
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
// using System.Threading.Tasks; // Не строго нужен, если LoadDoctors не async
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CNO.ViewModel; // Ваша ViewModel для врача

// Если вы используете свой контекст db_cnoEntities1, раскомментируйте
// using CNO.Model; // Предполагая, что ваш db_cnoEntities1 здесь

namespace CNO.Pages
{
    public partial class DoctorsPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<DoctorViewModel> AllDoctors { get; private set; }

        public DoctorsPage()
        {
            InitializeComponent();
            InitializeCollectionAndDataContext();
            // Loaded += DoctorsPage_Loaded; // Можно убрать, если LoadDoctors() синхронный
            LoadDoctors(); // Загружаем данные сразу
        }

        private void InitializeCollectionAndDataContext()
        {
            AllDoctors = new ObservableCollection<DoctorViewModel>();
            DoctorsListView.ItemsSource = AllDoctors; // Убедитесь, что x:Name="DoctorsListView" в XAML
            this.DataContext = this;
        }

        // Убрал async Task, так как ваш оригинальный метод был синхронным.
        // Если планируете асинхронность (рекомендуется для EF), верните async Task и ToListAsync()
        private void LoadDoctors()
        {
            AllDoctors.Clear(); // Очищаем перед загрузкой
            Debug.WriteLine("LoadDoctors: Загрузка списка врачей...");
            try
            {
                var doctorsFromDb = App.DB.Doctors
                    .Select(h => new // Создаем сразу ViewModel
                    {
                        DoctorEntity = h, // Сохраняем всю сущность для доступа к FirstName/LastName
                        Spez = h.Specialization,
                        Num = h.ContactNumber,
                    })
                    .ToList(); // Сначала материализуем данные из БД

                // Теперь, когда данные в памяти, формируем DoctorName
                foreach (var docData in doctorsFromDb)
                {
                    AllDoctors.Add(new DoctorViewModel
                    {
                        ID = docData.DoctorEntity.DoctorID,
                        Spez = docData.Spez,
                        Num = docData.Num,
                        DoctorName = $"{docData.DoctorEntity.LastName} {docData.DoctorEntity.FirstName}".Trim() // Формируем ФИО
                    });
                }

                Debug.WriteLine($"LoadDoctors: Загружено {AllDoctors.Count} врачей.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"LoadDoctors: ОШИБКА - {ex.Message}\n{ex.InnerException?.Message}");
                MessageBox.Show($"Ошибка загрузки списка врачей: {ex.Message}\n\n{ex.InnerException?.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            // OnPropertyChanged(nameof(AllDoctors)); // Для ObservableCollection это обычно не нужно для содержимого
        }

        // Ваши существующие обработчики
        private void GoToMainPage_Click(object sender, RoutedEventArgs e)
        {
            // Ваша логика
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e) // Убедитесь, что сигнатура совпадает с XAML
        {
            // var db = new db_cnoEntities1(); // Если нужен этот контекст
            // Ваша логика фильтрации
            // После фильтрации, возможно, потребуется перезагрузить или отфильтровать AllDoctors
            MessageBox.Show("Фильтрация еще не реализована.");
        }

        private void ListBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            // Ваша логика
        }

        private void AddDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            // Предполагается, что AddDoctorPage существует
            // NavigationService.Navigate(new AddDoctorPage());
            MessageBox.Show("Переход на страницу добавления врача (заглушка).");
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // Предполагается, что MainPageManager существует
            // NavigationService.Navigate(new MainPageManager());
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            else
            {
                MessageBox.Show("Нет страницы для возврата (заглушка).");
            }
        }

        // Обработчик для клика по карточке врача (если нужно)
        private void DoctorCard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is DoctorViewModel selectedDoctor)
            {
                Debug.WriteLine($"Выбран врач: {selectedDoctor.DoctorName}, ID: {selectedDoctor.ID}");
                // Здесь можно реализовать навигацию на страницу деталей врача или редактирования
                MessageBox.Show($"Клик по врачу ID: {selectedDoctor.ID}. ФИО: {selectedDoctor.DoctorName}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}