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

namespace MySleepHelperApp
{
    public partial class TransparentOverlay : Window
    {
        // Конструктор
        public TransparentOverlay()
        {
            InitializeComponent();
            SetupOverlay();
        }

        // Настройка прозрачного окна
        private void SetupOverlay()
        {
            // 1. Делаем окно невидимым для Windows (без рамок, кнопок и т.д.)
            WindowStyle = WindowStyle.None;

            // 2. Разрешаем прозрачность (чтобы видеть контент под окном)
            AllowsTransparency = true;

            // 3. Задаем почти прозрачный фон (1% непрозрачности)
            Background = System.Windows.Media.Brushes.Transparent;

            // 4. Окно всегда поверх других программ
            Topmost = true;

            // 5. Не показываем в панели задач
            ShowInTaskbar = false;

            // 6. Растягиваем на весь экран (автоматически работает для всех мониторов)
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }

        // Перехват нажатия клавиш
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Разблокировка по Ctrl+K
            if (e.Key == Key.K && Keyboard.Modifiers == ModifierKeys.Control)
            {
                Close(); // Закрываем это окно
            }

            // Блокируем все остальные клавиши
            e.Handled = true;
        }
    }
}
