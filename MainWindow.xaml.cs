using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;
using MySleepHelperApp.Services;
using MySleepHelperApp.Views;

namespace MySleepHelperApp
{
    public partial class MainWindow : Window
    {
        private TaskbarIcon notifyIcon;
        private HomeView _homeView;
        private ShutdownTimerView _shutdownTimerView;
        internal BrightnessView _brightnessView;
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

            // Инициализируем TaskbarIcon, найдя его по имени из XAML
            notifyIcon = (TaskbarIcon)FindName("MyNotifyIcon");
            if (notifyIcon == null)
            {
                System.Diagnostics.Debug.WriteLine("Ошибка: TaskbarIcon не найден в XAML по имени 'MyNotifyIcon'.");
            }
        }

        private void SwitchToTab(UserControl tabContent, string headerText)
        {
            TabContent.Content = tabContent;
        }

        private void HomeTab_Checked(object sender, RoutedEventArgs e)
        {
            if (HomeTab.IsChecked == true)
                SwitchToTab(new HomeView(), "Добро пожаловать в My Sleep Helper");
        }

        private void ShutdownTab_Checked(object sender, RoutedEventArgs e)
        {
            if (ShutdownTab.IsChecked == true)
                SwitchToTab(_shutdownTimerView, "Настройка таймера выключения");
        }

        private void BrightnessTab_Checked(object sender, RoutedEventArgs e)
        {
            if (BrightnessTab.IsChecked == true)
                SwitchToTab(_brightnessView, "Управление затемнением экрана");
        }

        private void KeyboardTab_Checked(object sender, RoutedEventArgs e)
        {
            if (KeyboardTab.IsChecked == true)
                SwitchToTab(_keyboardView, "Блокировка клавиатуры");
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

        // Вся логика очистки и завершения
        private bool PerformCloseApplicationLogic()
        {
            SystemCommandService.CancelShutdown();
            KeyboardLockView.CurrentBlocker?.Close();
            _keyboardView?.ReleaseKeyboardHook();
            _brightnessView?.TurnOffOverlay();
            return true; // Успешное выполнение
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            Debug.WriteLine("[MainWindow] Событие Closed сработало.");
            PerformCloseApplicationLogic();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            if (AreCriticalFunctionsActive())
            {
                var result = CustomMessageBox.CenteredShowYesNoDialog(
                    "При закрытии приложения функции таймера выключения, блокировки клавиатуры и регулировки яркости будут остановлены. Всё равно закрыть?",
                    "Предупреждение!",
                    TabContent);

                if (result != true)
                {
                    return;
                }
            }
            this.Close();
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

            // Проверяем активен ли оверлей регулировки яркости 
            bool isBrightnessOverlayActive = _brightnessView?.IsOverlayActive ?? false;

            return isShutdownTimerActive || isKeyboardLockActive || isBrightnessOverlayActive;
        }

        // Метод для показа окна
        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = WindowState.Normal; // Восстановить из свернутого/скрытого состояния
            this.Activate(); // Активировать окно, чтобы оно появилось на переднем плане
        }

        // Метод для скрытия окна в трей
        private void HideToTray()
        {
            this.Hide(); // Скрываем окно WPF
        }

        private void TrayCloseButton_Click(object sender, RoutedEventArgs e)
        {
            HideToTray(); // Скрываем окно в трей        
            
            // Проверяем активен ли таймер выключения через ваш UserControl
            bool isShutdownTimerActive = _shutdownTimerView?.IsTimerActive ?? false;

            // Показываем уведомление только если таймер активен
            if (isShutdownTimerActive)
            {
                notifyIcon?.ShowBalloonTip(
                    "Приложение свернуто",
                    "Таймер продолжает работать в трее.",
                    Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info
                );
            }
        }

        // Обработчик для пункта "Открыть" в контекстном меню иконки в трее
        private void MenuItem_Open_Click(object sender, RoutedEventArgs e)
        {
            ShowMainWindow(); // Показываем главное окно
        }

        // Обработчик для пункта "Закрыть" в контекстном меню иконки в трее
        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[MainWindow] Закрытие через контекстное меню трея.");
            if (AreCriticalFunctionsActive())
            {
                // Показываем окно перед диалогом, если оно скрыто
                this.Show();
                this.WindowState = WindowState.Normal;
                this.Activate();

                var result = CustomMessageBox.CenteredShowYesNoDialog(
                    "При закрытии приложения функции таймера выключения, блокировки клавиатуры и регулировки яркости будут остановлены. Всё равно закрыть?",
                    "Предупреждение!",
                    TabContent);

                if (result != true)
                {
                    Debug.WriteLine("[MainWindow] Пользователь отменил закрытие через трей.");
                    return;
                }
            }

            // Выполняем общую логику закрытия
            PerformCloseApplicationLogic();

            // Завершаем работу всего приложения
            Debug.WriteLine("[MainWindow] Завершение работы приложения через Application.Current.Shutdown().");
            Application.Current.Shutdown();
        }

        // Логика двойного клика по иконке в трее
        private void MyNotifyIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("[MainWindow] Двойной клик по иконке в трее.");
            ShowMainWindow(); // Показываем главное окно
        }
    }
}