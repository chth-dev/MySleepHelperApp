using System.Windows;
using System.Windows.Controls;


namespace MySleepHelperApp.Views
{
    public partial class HomeView : UserControl
    {
        public HomeView()
        {
            InitializeComponent();

            // Подписываемся на событие загрузки элемента управления
            this.Loaded += HomeView_Loaded;
        }

        // Обработчик события, который срабатывает при полной загрузке элемента
        private void HomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // Получаем имя пользователя компьютера
            string userName = Environment.UserName;

            // Преобразуем имя: первая буква - заглавная, остальные - строчные
            string formattedName = FormatUserName(userName);

            // Обновляем текст приветствия
            WelcomeText.Text = $"Приветствую, {formattedName}!";
        }

        // Метод для форматирования имени пользователя
        private static string FormatUserName(string name)
        {
            // Если имя пустое, возвращаем "друг"
            if (string.IsNullOrEmpty(name))
                return "друг";

            // Делаем первую букву заглавной, остальные - строчными
            return char.ToUpper(name[0]) + name.Substring(1).ToLower();
        }

    }
}
