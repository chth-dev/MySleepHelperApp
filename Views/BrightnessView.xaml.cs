using System.Windows;
using System.Windows.Controls;


namespace MySleepHelperApp.Views
{
    public partial class BrightnessView : UserControl
    {
            // Ссылка на оверлей
            private BrightnessOverlayWindow _overlayWindow;
            // Состояние: включён ли оверлей
            private bool _isOverlayActive = false;

            public BrightnessView()
            {
                InitializeComponent();
                // Установим начальное значение текста
                UpdateBrightnessText();
            }

            private void BrightnessButton_Click(object sender, RoutedEventArgs e)
            {
                if (!_isOverlayActive)
                {
                    // Создаём и показываем оверлей
                    _overlayWindow = new BrightnessOverlayWindow();
                    _overlayWindow.Show();
                    _isOverlayActive = true;
                    ButtonText.Text = "Выключить";

                    // Устанавливаем яркость по текущему значению ползунка
                    UpdateOverlayBrightness();
                }
                else
                {
                    // Скрываем оверлей
                    _overlayWindow?.Close();
                    _overlayWindow = null;
                    _isOverlayActive = false;
                    ButtonText.Text = "Включить";
                }
            }

        private void BrightnessSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateBrightnessText();
            UpdateOverlayBrightness();
        }

        // Обновляем текст с текущим значением яркости
        private void UpdateBrightnessText()
        {
            if (BrightnessValueText != null)
            {
                BrightnessValueText.Text = $"{BrightnessSlider.Value:0}%";
            }
        }

        // Обновляем яркость оверлея
        private void UpdateOverlayBrightness()
        {
            if (_overlayWindow != null)
            {
                _overlayWindow.SetBrightness(BrightnessSlider.Value);
            }
        }

    }
}
