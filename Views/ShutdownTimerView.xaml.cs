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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySleepHelperApp.Services;


namespace MySleepHelperApp.Views
{
    public partial class ShutdownTimerView : UserControl
    {
        private readonly TimerService _timerService;

        // Конструктор для ShutdownTimerView
        public ShutdownTimerView()
        {
            InitializeComponent();
            ResetUI();

            // Инициализация сервиса таймера
            _timerService = new TimerService();

            // Подписка на события таймера
            _timerService.TimerUpdated += time => CountdownText.Text = time;
            _timerService.TimerFinished += () =>
            {
                MessageBox.Show("Спокойной ночи.");
                ResetUI();
                _isTimerRunning = false;
                UpdateButtonState();
            };

            // Начальная настройка кнопки
            UpdateButtonState();
        }

        //........................................... 1.Работа с интерфейсом

        // Метод сброса интерфейса
        private void ResetUI()
        {
            TimerPanel.Visibility = Visibility.Collapsed;
            InputPanel.Visibility = Visibility.Visible;
            CountdownText.Text = "00:00:00";

            // Сброс полей ввода
            HoursTextBox.Text = "00";
            MinutesTextBox.Text = "00";
            SecondsTextBox.Text = "00";

            TitleText.Text = "Настройка таймера выключения";
        }

        //........................................... 2.Обработчики текстовых полей

        // Обрабатывает получение фокуса текстовым полем: скрывает плейсхолдер "00".
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


        // Обрабатывает потерю фокуса: валидирует значение, форматирует вывод и управляет плейсхолдером.
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


        // Проверяет вводимые символы: разрешает только цифры и проверяет допустимость значений.
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Блокируем нецифровые символы
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
                return;
            }

            // Проверяем, что событие вызвано TextBox
            if (sender is not TextBox textBox)
            {
                e.Handled = true;
                return;
            }

            // Получаем текущий текст (с защитой от null)
            string currentText = textBox.Text ?? string.Empty;

            // Удаляем выделенный текст (если есть выделение)
            int selectionStart = textBox.SelectionStart;
            int selectionLength = textBox.SelectionLength;
            if (selectionLength > 0)
            {
                currentText = currentText.Remove(selectionStart, selectionLength);
            }

            // Вставляем новый символ в позицию курсора
            string newText = currentText.Insert(selectionStart, e.Text);

            // Проверяем максимальное значение (23 для часов, 59 для минут/секунд)
            int maxValue = textBox.Name == "HoursTextBox" ? 23 : 59;
            if (int.TryParse(newText, out int value) && value > maxValue)
            {
                e.Handled = true;
                return;
            }

            // Применяем изменения вручную (для обработки выделения)
            textBox.Text = newText;
            textBox.CaretIndex = selectionStart + 1; // Перемещаем курсор после ввода
            e.Handled = true; // Блокируем стандартную обработку
        }


        // Блокирует нежелательные клавиши: пробел, Enter и Escape.
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space ||
                e.Key == Key.Enter ||
                e.Key == Key.Escape)
            {
                e.Handled = true;
            }
        }


        // Проверяет и корректирует значение текстового поля, возвращая валидное число.
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


        //........................................... 3.Обработчики кнопок

        // Обрабатывает нажатие кнопки "Запланировать": запускает таймер выключения.
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
                var result = CustomMessageBox.ShowDialog(
                    "Пожалуйста, введите время больше нуля, иначе таймер выключения не сможет работать \n:(",
                    "Предупреждение!"
                    );
                return;
            }

            SystemCommandService.ShutdownComputer(totalSeconds);
            _timerService.Start(totalSeconds); // Запускаем через сервис

            _isTimerRunning = true;  // Ставим флаг "таймер работает"
            UpdateButtonState();     // Обновляем кнопку
        }


        // Обрабатывает нажатие кнопки "Отменить": останавливает таймер и сбрасывает состояние.
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _timerService.Stop();
            SystemCommandService.CancelShutdown();
            ResetUI();

            _isTimerRunning = false; // Сбрасываем флаг
            UpdateButtonState();     // Возвращаем кнопку в исходное состояние
        }

        private bool _isTimerRunning = false; // Флаг состояния
        private void UpdateButtonState()
        {
            if (_isTimerRunning)
            {
                // Если таймер работает, показываем "Отменить" и вешаем обработчик отмены
                ButtonText.Text = "Отменить выключение компьютера";
                ScheduleButton.Click -= ScheduleShutdown_Click; // Удаляем старый обработчик
                ScheduleButton.Click += CancelButton_Click;     // Добавляем новый
                TimerPanel.Visibility = Visibility.Visible;
                InputPanel.Visibility = Visibility.Collapsed;
                TitleText.Text = "Компьютер будет выключен через:";

            }
            else
            {
                // Если таймер не работает, показываем "Запланировать" и вешаем обработчик запуска
                ButtonText.Text = "Запланировать выключение компьютера";
                ScheduleButton.Click -= CancelButton_Click;     // Удаляем старый обработчик
                ScheduleButton.Click += ScheduleShutdown_Click; // Добавляем новый
                TimerPanel.Visibility = Visibility.Collapsed;
                InputPanel.Visibility = Visibility.Visible;
                TitleText.Text = "Настройка таймера выключения";
            }
        }

        public bool IsTimerActive => _timerService?.IsRunning ?? false;

    }
}
