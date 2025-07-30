using System.Windows;
using System.Windows.Controls;
using MySleepHelperApp.Services;


namespace MySleepHelperApp.Views
{
    public partial class BrightnessView : UserControl
    {
        // Ссылка на мульти-экран оверлей
        private MultiScreenBrightnessOverlay? _overlay = null;
        // Состояние: включён ли оверлей
        private bool _isOverlayActive = false;

        // Публичное свойство для проверки состояния оверлея
        public bool IsOverlayActive => _isOverlayActive; // Только для чтения

        public BrightnessView()
        {
            InitializeComponent();
            UpdateBrightnessText();
        }

        private void BrightnessButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_isOverlayActive)
            {
                // Создаём и показываем оверлей на всех экранах
                _overlay = new MultiScreenBrightnessOverlay();
                _overlay.Show();
                ButtonText.Text = "Выключить";
                UpdateOverlayBrightness();
                _isOverlayActive = true;
            }
            else
            {
                // Скрываем оверлей
                _overlay?.Hide();
                _overlay = null;
                ButtonText.Text = "Включить";
                _isOverlayActive = false;
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
            if (_overlay != null)
            {
                _overlay.SetBrightness(BrightnessSlider.Value);
            }
        }

        // Публичный метод для выключения оверлея извне
        public void TurnOffOverlay()
        {
            if (_isOverlayActive && _overlay != null)
            {
                _overlay.Hide();
                _overlay = null;
                _isOverlayActive = false; // <-- Сбрасываем флаг
            }
        }
    }
}