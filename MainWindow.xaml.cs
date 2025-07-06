using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MySleepHelperApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private double aspectRatio = 4.0 / 3.0; // Соотношение сторон 4:3

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Получаем текущие размеры окна
            double currentWidth = this.Width;
            double currentHeight = this.Height;

            // Вычисляем новую ширину/высоту, чтобы сохранить пропорции
            if (currentWidth / currentHeight > aspectRatio)
            {
                // Ограничение: ширина не должна выходить за рамки пропорций
                this.Width = currentHeight * aspectRatio;
            }
            else
            {
                // Ограничение: высота не должна выходить за рамки пропорций
                this.Height = currentWidth / aspectRatio;
            }
        }
    }
}