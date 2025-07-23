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
using System.Runtime.InteropServices;
using System.Windows.Interop;
using MySleepHelperApp.Views;


namespace MySleepHelperApp.Views
{
    public partial class KeyboardLockView : UserControl
    {
        // Храним ссылку на окно-блокиратор
        private TransparentOverlay? _keyboardBlocker;

        public KeyboardLockView()
        {
            InitializeComponent();

            // Инициализируем кнопки в нужном состоянии
            ResetUI();
        }

        private void ResetUI()
        {
            KeyboardUnLockButton.Visibility = Visibility.Collapsed;
            KeyboardLockButton.Visibility = Visibility.Visible;
        }

        private void KeyboardLockButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 0. Закрываем предыдущий блокиратор, если он есть
                _keyboardBlocker?.Close();

                // 1. Создаем и показываем блокиратор
                _keyboardBlocker = new TransparentOverlay();
                _keyboardBlocker.Closed += (s, args) => ResetUI(); // Автоматический сброс при закрытии
                _keyboardBlocker.Show();
 
                // 2. Меняем состояние кнопок
                KeyboardUnLockButton.Visibility = Visibility.Visible;
                KeyboardLockButton.Visibility = Visibility.Collapsed;

            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Не удалось активировать блокировку: {ex.Message}");
                ResetUI();
            }
        }

        private void KeyboardUnLockButton_Click(object sender, RoutedEventArgs e)
        {
            // 1. Закрываем блокиратор, если он существует
            _keyboardBlocker?.Close();
            _keyboardBlocker = null;

            // 2. Восстанавливаем UI
            ResetUI();
        }

        // Дополнительно: закрываем блокиратор при уничтожении контрола
        protected override void OnUnloaded(RoutedEventArgs e)
        {
            _keyboardBlocker?.Close();
            base.OnUnloaded(e);
        }
    }
}