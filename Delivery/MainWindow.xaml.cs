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
using System.Windows.Threading;
using Delivery.Classes;
using EasyCaptcha.Wpf;

namespace Delivery
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string password;					//Для переключения видимости пароля
        bool first = true;					//Первый раз вводится пароль-да (true)


        String captchaText;
        DispatcherTimer timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            const int lengthCaptcha = 4;            //Длина каптчи
            captcha.CreateCaptcha(Captcha.LetterOption.Alphanumeric, lengthCaptcha);
            captchaText = captcha.CaptchaText;	//Сформированная строка каптчи
            try
            {
                Helper.DB = new Model.CourierDBEntities(); //связь с бд
                MessageBox.Show("Связь с бд");
            }
            catch
            {
                MessageBox.Show("Проблема связи с базой");
                return;
            }
        }

        private void btnAuth_Click(object sender, RoutedEventArgs e)
        {

            string login = tbLogin.Text;
            string password;
            //Если не первый раз вводится неверный пароль, то отобразить каптчу
            if (!first)
            {
                captcha.CreateCaptcha(EasyCaptcha.Wpf.Captcha.LetterOption.Alphanumeric, 5);
                string answer = captcha.CaptchaText;		//Строка каптчи
                captcha.Visibility = Visibility.Visible;		//Отобразить каптчу
                textboxcaptcha.Visibility = Visibility.Visible;
            }

            //Получить пароль
            if (isVisiblePassword.IsChecked == true)
            {
                password = tbPassword.Text;
            }
            else
            {
                password = pbPassword.Password;
            }
            if (login == "" || password == "") 			//Проверка заполненности
            {
                MessageBox.Show("Не все данные введены");
                return;
            }
            //Получить данные о пользователе с введенным логином и паролем
            Helper.user = Helper.DB.User.Where(x => x.UserLogin == login &&
                                                          x.UserPassword == password).ToList().FirstOrDefault();
            if (Helper.user == null)			//Пустой пользователь
            {
                MessageBox.Show("Ваших данные нет в БД");
                first = false;				//Не первый раз входите, неудачная попытка
                return;
            }
            MessageBox.Show("Вы зашли с ролью " + Helper.user.Role.RoleName + " " + Helper.user.UserFullName);	//Название роли
            GoCatalog();
        }

        private void GoCatalog()
        {
            View.ViewCatalog viewCatalog = new View.ViewCatalog();
            this.Hide();
            viewCatalog.ShowDialog();
            this.Show();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            btnAuth.IsEnabled = true;
            timer.Stop();
        }

        private void btnGuest_Click(object sender, RoutedEventArgs e)
        {
            Helper.user = null;					//Пустой пользователь
            MessageBox.Show("Вы зашли как гость");
            View.ViewCatalog catalog = new View.ViewCatalog();	//Переход в окно каталога
            this.Hide();
            catalog.ShowDialog();
            this.Show();

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void isVisiblePassword_Unchecked(object sender, RoutedEventArgs e)
        {
            pbPassword.Password = tbPassword.Text;
            tbPassword.Visibility = Visibility.Hidden;
            pbPassword.Visibility = Visibility.Visible;
        }

        private void isVisiblePassword_Checked(object sender, RoutedEventArgs e)
        {
            tbPassword.Text = pbPassword.Password;
            tbPassword.Visibility = Visibility.Visible;
            pbPassword.Visibility = Visibility.Hidden;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            pbPassword.Visibility = Visibility.Visible;
            tbPassword.Visibility = Visibility.Hidden;
            isVisiblePassword.IsChecked = false;

            captcha.Visibility = Visibility.Hidden;		//Объект-каптча невидим
            textboxcaptcha.Visibility = Visibility.Hidden;

        }
    }
}
