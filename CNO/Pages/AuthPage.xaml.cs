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
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {

            var login = TbLogin.Text.Trim();
            var password = PasswordBox.Password;

            // Проверка на пустые поля
            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите логин и пароль", "Ошибка",
                              MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Подключение к БД и поиск пользователя
            using (var db = new db_cnoEntities1()) // Замените на ваш контекст БД
            {
                var Login = LoginBox.Text;
                var Password = PasswordBox.Password;
                var auth = App.DB.Users.FirstOrDefault(x => x.Login == Login && x.Password == Password);
                if (auth == null)
                {
                    MessageBox.Show("Данные введены неверно");
                }
                switch (auth.RoleId)
                {
                    case 1: // Администратор
                        NavigationService.Navigate(new DoctorsPage());
                        break;
                    case 2: // Врач
                        NavigationService.Navigate(new MainPage());
                        break;
                    case 3: // Менеджер
                        NavigationService.Navigate(new MainPageManager());
                        break;
                    default:
                        NavigationService.Navigate(new MainPage());
                        break;
                }
                
            }
        }
        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

    }
}

      


