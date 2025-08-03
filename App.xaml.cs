using System.Configuration;
using System.Data;
using System.Windows;
using MySleepHelperApp.Views;
using System.Drawing;
using MySleepHelperApp.Services;

namespace MySleepHelperApp
{
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            // Дублируем освобождение ресурсов на случай, если MainWindow не успел обработать
            KeyboardLockView.CurrentBlocker?.Close();
            base.OnExit(e);
        }

        // Инициализируем сразу при объявлении
        public static FontService FontService { get; private set; } = new FontService();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Теперь просто используем уже созданный сервис
            System.Diagnostics.Debug.WriteLine($"Загружено шрифтов: {FontService.FontCount}");
        }
    }
}
