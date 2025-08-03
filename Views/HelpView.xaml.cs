using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;


namespace MySleepHelperApp.Views
{
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();
        }

        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            // Сначала спрашиваем пользователя
            var result = CustomMessageBox.CenteredShowYesNoDialog(
                "Вы будете перенаправлены на страницу GitHub для создания обращения. Продолжить?",
                "Переход к GitHub",
                this);

            // Если пользователь нажал "Да"
            if (result == true)
            {
                try
                {
                    // Открываем страницу создания нового issue
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/chth-dev/MySleepHelperApp/issues/new/choose",
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // Если не удалось открыть браузер - показываем ошибку
                    CustomMessageBox.CenteredShowDialog(
                        "Не удалось открыть браузер :(",
                        "Что-то пошло не так",
                        this);
                }
            }
            // Если пользователь нажал "Нет" - просто закрываем диалог, ничего дополнительно делать не нужно
        }

        private void GitButton_Click(object sender, RoutedEventArgs e)
        {
            // Сначала спрашиваем пользователя
            var result = CustomMessageBox.CenteredShowYesNoDialog(
                "Вы будете перенаправлены на страницу GitHub проекта. Продолжить?",
                "Переход к GitHub",
                this);

            // Если пользователь нажал "Да"
            if (result == true)
            {
                try
                {
                    // Открываем страницу вашего GitHub репозитория
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = "https://github.com/chth-dev/MySleepHelperApp",
                        UseShellExecute = true
                    });
                }
                catch
                {
                    // Если не удалось открыть браузер - показываем ошибку
                        CustomMessageBox.CenteredShowDialog(
                        "Не удалось открыть браузер :(",
                        "Что-то пошло не так",
                        this);
                }
            }
            // Если пользователь нажал "Нет" - просто закрываем диалог
        }
    }
}