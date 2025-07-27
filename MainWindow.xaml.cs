using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MySleepHelperApp.Views;
using MySleepHelperApp.Services;

namespace MySleepHelperApp
{
    public partial class MainWindow : Window
    {
        private HomeView _homeView;
        private ShutdownTimerView _shutdownTimerView;
        private BrightnessView _brightnessView;
        private KeyboardLockView _keyboardView;
        private HelpView _helpView;

        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed; // Подписываемся на событие закрытия

            // Инициализация вкладок
            _homeView = new HomeView();
            _shutdownTimerView = new ShutdownTimerView();
            _brightnessView = new BrightnessView();
            _keyboardView = new KeyboardLockView();
            _helpView = new HelpView();

            // Подписка на события переключения вкладок
            HomeTab.Checked += HomeTab_Checked;
            ShutdownTab.Checked += ShutdownTab_Checked;
            BrightnessTab.Checked += BrightnessTab_Checked;
            KeyboardTab.Checked += KeyboardTab_Checked;
            HelpTab.Checked += HelpTab_Checked;

            // Установка вкладки по умолчанию
            SwitchToTab(_homeView, "Домашняя страница");
            HomeTab.IsChecked = true;
        }

        private void SwitchToTab(UserControl tabContent, string headerText)
        {
            TabContent.Content = tabContent;
        }

        private void HomeTab_Checked(object sender, RoutedEventArgs e)
        {
            if (HomeTab.IsChecked == true)
                SwitchToTab(new HomeView(), "Добро пожаловать в Sleep Helper");
        }

        private void ShutdownTab_Checked(object sender, RoutedEventArgs e)
        {
            if (ShutdownTab.IsChecked == true)
                SwitchToTab(_shutdownTimerView, "Настройка таймера выключения");
        }

        private void BrightnessTab_Checked(object sender, RoutedEventArgs e)
        {
            if (BrightnessTab.IsChecked == true)
                SwitchToTab(_brightnessView, "Управление яркостью экрана");
        }

        private void KeyboardTab_Checked(object sender, RoutedEventArgs e)
        {
            if (KeyboardTab.IsChecked == true)
                SwitchToTab(_keyboardView, "Настройки клавиатуры");
        }

        private void HelpTab_Checked(object sender, RoutedEventArgs e)
        {
            if (HelpTab.IsChecked == true)
                SwitchToTab(_helpView, "Справка и помощь");
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            // 1. Отменяем запланированное выключение компьютера
            SystemCommandService.CancelShutdown();

            // 2. Закрываем блокировщик клавиатуры
            KeyboardLockView.CurrentBlocker?.Close();

            // 3. Освобождаем хук клавиатуры
            _keyboardView?.ReleaseKeyboardHook();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreCriticalFunctionsActive())
            {
                // Используем кастомный диалог
                var result = CustomMessageBox.ShowYesNoDialog(
                    "При закрытии приложения функции таймера выключения, блокировки клавиатуры и регулировки яркости будут остановлены. Всё равно закрыть?",
                    "Предупреждение!",
                    this);

                if (result != true)
                {
                    // Пользователь нажал "Нет" или закрыл окно
                    return;
                }
            }

            // Если критические функции не активны или пользователь нажал "Да"
            this.Close(); // Это вызовет MainWindow_Closed
        }

        private bool _isDragging = false;
        private Point _startPoint;

        private void TopPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isDragging = true;
            _startPoint = e.GetPosition(this);
            ((UIElement)sender).CaptureMouse();
        }

        private void TopPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            Point currentPoint = e.GetPosition(this);
            this.Left += currentPoint.X - _startPoint.X;
            this.Top += currentPoint.Y - _startPoint.Y;
        }

        private void TopPanel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isDragging = false;
            ((UIElement)sender).ReleaseMouseCapture();
        }

        private bool AreCriticalFunctionsActive()
        {
            // Проверяем активен ли таймер выключения
            bool isShutdownTimerActive = _shutdownTimerView?.IsTimerActive ?? false;

            // Проверяем активна ли блокировка клавиатуры
            bool isKeyboardLockActive = _keyboardView?.IsKeyboardHookActive ?? false;

            return isShutdownTimerActive || isKeyboardLockActive;
        }
    }
}