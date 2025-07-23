using System.Configuration;
using System.Data;
using System.Windows;
using MySleepHelperApp.Views;

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
    }
}
