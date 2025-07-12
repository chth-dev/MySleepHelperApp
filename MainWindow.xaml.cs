using System.Diagnostics;
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
using System.Windows.Threading;
using MySleepHelperApp.Services;


namespace MySleepHelperApp
{
    public partial class MainWindow : Window
    {
        private TimerService _timerService;

        //........................................... 1.Конструктор (MainWindow())
        #region Конструктор (MainWindow())
        public MainWindow()
        {
            InitializeComponent();

            // 1. Инициализация сервиса таймера
            _timerService = new TimerService();

            // 2. Подписка на события таймера:
            _timerService.TimerUpdated += time => CountdownText.Text = time; // Когда таймер обновляется - меняем текст на экране
            _timerService.TimerFinished += () => // Когда таймер завершается
            {
                MessageBox.Show("Спокойной ночи.");

                // 3. Возвращаем интерфейс в исходное состояние:
                TimerPanel.Visibility = Visibility.Collapsed;
                InputPanel.Visibility = Visibility.Visible;
                ScheduleButton.Visibility = Visibility.Visible;
                CancelButton.Visibility = Visibility.Collapsed;
            };
        }
        #endregion

        //........................................... 2.UI-обработчики: Обработчики текстовых полей
        #region Обработчики текстовых полей

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Parent is Grid grid)
            {
                foreach (var child in grid.Children)
                {
                    if (child is TextBlock placeholder &&
                        placeholder.Name == textBox.Name + "Placeholder")
                    {
                        placeholder.Visibility = Visibility.Collapsed;
                        break;
                    }
                }
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Parent is Grid grid)
            {
                // Корректируем значение и форматируем текст
                GetSafeValue(textBox, textBox.Name == "HoursTextBox");

                // Обновляем плейсхолдер
                foreach (var child in grid.Children)
                {
                    if (child is TextBlock placeholder &&
                        placeholder.Name == textBox.Name + "Placeholder")
                    {
                        if (string.IsNullOrEmpty(textBox.Text))
                        {
                            textBox.Text = "0";
                            placeholder.Visibility = Visibility.Collapsed;
                        }
                        break;
                    }
                }
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // 1. Проверка вводимого символа
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            // 2. Безопасное приведение типа
            if (sender is not TextBox textBox)
            {
                e.Handled = true;
                return;
            }

            // 3. Защита от null и пустого значения
            string currentText = textBox.Text ?? string.Empty;

            try
            {
                // 4. Формируем новое значение с проверкой позиции курсора
                string newText = currentText.Insert(
                    Math.Clamp(textBox.CaretIndex, 0, currentText.Length),
                    e.Text);

                // 5. Проверка диапазона значений
                if (string.IsNullOrEmpty(newText)) return;

                int maxValue = textBox.Name == "HoursTextBox" ? 23 : 59;
                if (int.TryParse(newText, out int value) && value > maxValue)
                {
                    e.Handled = true;
                }
            }
            catch
            {
                e.Handled = true; // На всякий случай
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Блокируем нажатие пробела, Enter, Escape
            if (e.Key == Key.Space ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }
        private static int GetSafeValue(TextBox textBox, bool isHours = false)
        {
            // 1. Обрабатываем пустое поле
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = "00"; // Автоматически ставим "00" вместо пустоты
                return 0;
            }

            // 2. Пробуем преобразовать в число
            if (!int.TryParse(textBox.Text, out int value))
            {
                textBox.Text = "00"; // Если ввели не цифры, сбрасываем
                return 0;
            }

            // 3. Определяем ограничения
            int maxValue = isHours ? 23 : 59;

            // 4. Корректируем значение
            value = Math.Clamp(value, 0, maxValue);

            // 5. Обновляем TextBox (добавляем ведущий ноль для 1-значных чисел)
            textBox.Text = value.ToString("00");

            return value;
        }
        #endregion

        //........................................... 2.UI-обработчики: Кнопки действий
        #region Кнопки действий
        private void ScheduleShutdown_Click(object sender, RoutedEventArgs e)
        {
            // Используем наш метод GetSafeValue для получения значений с ограничениями
            int hours = GetSafeValue(HoursTextBox, true);    // true - это часы (0-23)
            int minutes = GetSafeValue(MinutesTextBox);     // минуты (0-59)
            int seconds = GetSafeValue(SecondsTextBox);     // секунды (0-59)

            // Показываем скорректированные значения
            HoursTextBox.Text = hours.ToString("00");
            MinutesTextBox.Text = minutes.ToString("00");
            SecondsTextBox.Text = seconds.ToString("00");

            // Считаем общее количество секунд
            int totalSeconds = hours * 3600 + minutes * 60 + seconds;

            // Проверяем, чтобы время было больше нуля
            if (totalSeconds <= 0)
            {
                MessageBox.Show("Введите время больше нуля.");
                return;
            }

            SystemCommandService.ShutdownComputer(totalSeconds);
            _timerService.Start(totalSeconds); // Запускаем через сервис

            // Переключение видимости элементов
            TimerPanel.Visibility = Visibility.Visible;
            InputPanel.Visibility = Visibility.Collapsed;
            ScheduleButton.Visibility = Visibility.Collapsed;
            CancelButton.Visibility = Visibility.Visible;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _timerService.Stop();
            SystemCommandService.CancelShutdown();

            // Возврат к исходному интерфейсу
            TimerPanel.Visibility = Visibility.Collapsed;
            InputPanel.Visibility = Visibility.Visible;
            ScheduleButton.Visibility = Visibility.Visible;
            CancelButton.Visibility = Visibility.Collapsed;

            // Сброс таймера
            CountdownText.Text = "00:00:00";
        }
        #endregion 
    }
}