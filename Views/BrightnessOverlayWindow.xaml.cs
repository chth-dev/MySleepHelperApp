using System.Windows;
using System.Windows.Interop;

namespace MySleepHelperApp.Views
{
    public partial class BrightnessOverlayWindow : Window
    {
        public BrightnessOverlayWindow()
        {
            InitializeComponent();

            // Устанавливаем размеры на весь виртуальный экран
            this.Left = SystemParameters.VirtualScreenLeft;
            this.Top = SystemParameters.VirtualScreenTop;
            this.Width = SystemParameters.VirtualScreenWidth;
            this.Height = SystemParameters.VirtualScreenHeight;

            // Дополнительные настройки для "невидимости"
            this.Focusable = false;
            this.IsHitTestVisible = false;
            this.ShowActivated = false;

            // Настраиваем поведение окна через Windows API
            Loaded += BrightnessOverlayWindow_Loaded;
        }

        private void BrightnessOverlayWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).EnsureHandle();

            // Устанавливаем дополнительные стили окна
            const int GWL_EXSTYLE = -20;
            const int WS_EX_TRANSPARENT = 0x00000020;
            const int WS_EX_TOOLWINDOW = 0x00000080;

            var extendedStyle = Win32.GetWindowLong(hwnd, GWL_EXSTYLE);
            Win32.SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        public void SetBrightness(double brightnessPercent)
        {
            // Убеждаемся, что значение в допустимом диапазоне
            brightnessPercent = System.Math.Max(0, System.Math.Min(100, brightnessPercent));

            // Минимальная яркость 5% означает максимальную непрозрачность 95%
            // Максимальная яркость 100% означает минимальную непрозрачность 0%
            double minBrightness = 5.0;
            double maxBrightness = 100.0;

            // Нормализуем значение яркости в диапазон 0-1
            // При этом 5% яркости = 0, 100% яркости = 1
            double normalizedBrightness = (brightnessPercent - minBrightness) / (maxBrightness - minBrightness);
            // Убеждаемся, что значение не выходит за границы [0, 1] из-за округления или мин. значения
            normalizedBrightness = System.Math.Max(0.0, System.Math.Min(1.0, normalizedBrightness));

            // Opacity должен быть обратным: чем больше яркость, тем меньше Opacity затемнения
            // normalizedBrightness: 0 -> Opacity: 0.95 (максимальное затемнение)
            // normalizedBrightness: 1 -> Opacity: 0.0 (минимальное затемнение)
            Opacity = (1.0 - normalizedBrightness) * 0.95;
        }
    }

    // Вспомогательный класс для работы с Win32 API
    public static class Win32
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int GetWindowLong(System.IntPtr hwnd, int index);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SetWindowLong(System.IntPtr hwnd, int index, int newStyle);
    }
}