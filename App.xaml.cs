using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using MySleepHelperApp.Services;
using MySleepHelperApp.Views;

namespace MySleepHelperApp
{
    public partial class App : Application
    {
        // --- Поля для хранения ссылок ---
        private MainWindow? _mainWindow; // Ссылка на ЕДИНСТВЕННЫЙ экземпляр MainWindow
        private HwndSource? _hwndSource;


        // Публичное свойство для доступа к FontService из любого места приложения
        public static FontService? FontService { get; private set; }

        // --- Импорты Win32 API ---
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // --- Константы ---
        private const int WM_HOTKEY = 0x0312;
        private const uint MOD_ALT = 0x0001;
        private const uint MOD_CONTROL = 0x0002;

        // Уникальные ID для горячих клавиш
        private const int HOTKEY_ID_INCREASE = 9001;
        private const int HOTKEY_ID_DECREASE = 9002;
        private const int HOTKEY_ID_INCREASE_NUMPAD = 9003;
        private const int HOTKEY_ID_DECREASE_NUMPAD = 9004;

        // Коды виртуальных клавиш (VK codes)
        private const uint VK_OEM_PLUS = 0xBB;   // Клавиша "+" (обычная)
        private const uint VK_OEM_MINUS = 0xBD;  // Клавиша "-" (обычная)
        private const uint VK_ADD = 0x6B;        // Клавиша "+" (нумпад)
        private const uint VK_SUBTRACT = 0x6D;   // Клавиша "-" (нумпад)

        protected override void OnStartup(StartupEventArgs e)
        {
            // Инициализация FontService
            FontService = new FontService();

            // Проверка загрузки шрифтов
            Debug.WriteLine($"Шрифты загружены. Regular: {FontService.RegularFont.Source}, Bold: {FontService.BoldFont.Source}");

            // 1. Создаём ЕДИНСТВЕННЫЙ экземпляр главного окна
            _mainWindow = new MainWindow();

            // 2. Регистрация горячих клавиш
            var helper = new WindowInteropHelper(_mainWindow);
            IntPtr hwnd = helper.EnsureHandle();

            _hwndSource = HwndSource.FromHwnd(hwnd);
            _hwndSource.AddHook(HwndHook);

            // Регистрируем Ctrl + Alt + "+" и Ctrl + Alt + "-" (обычные клавиши)
            bool result1 = RegisterHotKey(hwnd, HOTKEY_ID_INCREASE, MOD_CONTROL | MOD_ALT, VK_OEM_PLUS);
            bool result2 = RegisterHotKey(hwnd, HOTKEY_ID_DECREASE, MOD_CONTROL | MOD_ALT, VK_OEM_MINUS);

            // Регистрируем Ctrl + Alt + "+" и Ctrl + Alt + "-" (клавиши нумпада)
            bool result3 = RegisterHotKey(hwnd, HOTKEY_ID_INCREASE_NUMPAD, MOD_CONTROL | MOD_ALT, VK_ADD);
            bool result4 = RegisterHotKey(hwnd, HOTKEY_ID_DECREASE_NUMPAD, MOD_CONTROL | MOD_ALT, VK_SUBTRACT);

            if (!result1 || !result2)
            {
                System.Diagnostics.Debug.WriteLine("Не удалось зарегистрировать одну или несколько горячих клавиш.");
            }

            _mainWindow.Show();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                int id = wParam.ToInt32();

                switch (id)
                {
                    case HOTKEY_ID_INCREASE:
                    case HOTKEY_ID_INCREASE_NUMPAD: // Обрабатываем оба варианта увеличения
                        AdjustBrightness(5.0);
                        handled = true;
                        break;
                    case HOTKEY_ID_DECREASE:
                    case HOTKEY_ID_DECREASE_NUMPAD: // Обрабатываем оба варианта уменьшения
                        AdjustBrightness(-5.0);
                        handled = true;
                        break;
                }
            }
            return IntPtr.Zero;
        }

        private void AdjustBrightness(double delta)
        {
            // Получаем ссылку на BrightnessView 
            var brightnessViewInstance = _mainWindow?._brightnessView;

            // Проверяем, что BrightnessView существует и оверлей активен
            if (brightnessViewInstance != null && brightnessViewInstance.IsOverlayActive)
            {
                try
                {
                    // Получаем текущее значение из публичного слайдера
                    double currentValue = brightnessViewInstance.BrightnessSlider.Value;
                    // Вычисляем и ограничиваем новое значение
                    double newValue = Math.Max(0, Math.Min(100, currentValue + delta));
                    // Устанавливаем новое значение - это автоматически вызовет BrightnessSlider_ValueChanged
                    brightnessViewInstance.BrightnessSlider.Value = newValue;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Ошибка при регулировке яркости: {ex.Message}");
                }
            }
            else
            {
                // Это сообщение будет появляться, если оверлей не активен
                System.Diagnostics.Debug.WriteLine("Яркость можно регулировать только при активном оверлее.");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // --- Освобождение ресурсов горячих клавиш ---
            if (_mainWindow != null && _hwndSource != null) 
            {
                var helper = new WindowInteropHelper(_mainWindow);
                IntPtr hwnd = helper.Handle;
                if (hwnd != IntPtr.Zero)
                {
                    UnregisterHotKey(hwnd, HOTKEY_ID_INCREASE);
                    UnregisterHotKey(hwnd, HOTKEY_ID_DECREASE);
                    UnregisterHotKey(hwnd, HOTKEY_ID_INCREASE_NUMPAD); 
                    UnregisterHotKey(hwnd, HOTKEY_ID_DECREASE_NUMPAD); 
                }

                _hwndSource.RemoveHook(HwndHook);
                _hwndSource.Dispose();
            }

            // --- Логика освобождения ресурсов ---
            KeyboardLockView.CurrentBlocker?.Close();

            base.OnExit(e);
        }
    }
}