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
using System.Windows.Shapes;

namespace MySleepHelperApp.Views
{
    public partial class BrightnessOverlayWindow : Window
    {
        public BrightnessOverlayWindow()
        {
            InitializeComponent();
            // Делаем окно "невидимым" для мыши
            this.IsHitTestVisible = false;
            this.Focusable = false;
            this.IsEnabled = false;
        }

        public void SetBrightness(double opacity)
        {
            // Ограничиваем значение от 0 до 1
            Opacity = System.Math.Max(0, System.Math.Min(1, 1 - opacity / 100.0));
        }
    }
}
