using System.Collections.Generic;
using System.Windows;
using MySleepHelperApp.Views;

namespace MySleepHelperApp.Services
{
    public class MultiScreenBrightnessOverlay
    {
        private List<BrightnessOverlayWindow> _overlays = new List<BrightnessOverlayWindow>();

        public void Show()
        {
            Hide(); // На случай, если уже показано

            // Создаем один оверлей на весь виртуальный экран
            var overlay = new BrightnessOverlayWindow();

            // Устанавливаем размеры на весь виртуальный экран
            overlay.Left = SystemParameters.VirtualScreenLeft;
            overlay.Top = SystemParameters.VirtualScreenTop;
            overlay.Width = SystemParameters.VirtualScreenWidth;
            overlay.Height = SystemParameters.VirtualScreenHeight;

            overlay.Show();
            _overlays.Add(overlay);
        }

        public void Hide()
        {
            foreach (var overlay in _overlays)
            {
                overlay.Close();
            }
            _overlays.Clear();
        }

        public void SetBrightness(double brightnessPercent)
        {
            foreach (var overlay in _overlays)
            {
                overlay.SetBrightness(brightnessPercent);
            }
        }
    }
}