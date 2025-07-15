using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;


namespace MySleepHelperApp.Services
{
    public class TimerService
    {
        private int _remainingSeconds;

        private readonly DispatcherTimer _timer;

        public event Action<string> TimerUpdated = delegate { };
        public event Action TimerFinished = delegate { };

        //..............................Конструктор
        public TimerService()
        {
            // 1. Создаём экземпляр
            _timer = new DispatcherTimer
            {
                // 2. Настраиваем
                Interval = TimeSpan.FromSeconds(1) // Тик каждую секунду
            };

            // 3. Подписываемся на событие
            _timer.Tick += Timer_Tick; // Теперь безопасно
        }

        //..............................Обработчик тиков
        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_remainingSeconds > 0)
            {
                _remainingSeconds--;
                UpdateTimerText();
            }
            else
            {
                Stop();
                TimerFinished?.Invoke(); // Уведомляем подписчиков
            }
        }

        //..............................Дополнительные методы 
        public void Start(int totalSeconds)
        {
            _remainingSeconds = totalSeconds;
            UpdateTimerText();
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
        }

        private void UpdateTimerText()
        {
            // Форматируем время в ЧЧ:ММ:СС
            string formattedTime = TimeSpan.FromSeconds(_remainingSeconds)
                .ToString(@"hh\:mm\:ss");

            // Вызываем событие для обновления UI
            TimerUpdated?.Invoke(formattedTime);
        }

    }
}
