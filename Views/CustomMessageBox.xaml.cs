using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace MySleepHelperApp.Views
{
    public partial class CustomMessageBox : UserControl
    {
        public ObservableCollection<MessageBoxButton> Buttons { get; }
        = new ObservableCollection<MessageBoxButton>();

        public CustomMessageBox()
        {
            InitializeComponent();
            DataContext = this; // Важно для привязки данных!
        }
        public class MessageBoxButton
        {
            public required string Text { get; set; }          // Текст кнопки
            public required ICommand Command { get; set; }     // Команда при нажатии
            public object? Result { get; set; }        // Результат (если нужно)
        }

        public void Show(string message, string? title = null, MessageBoxButton[]? buttons = null)
        {
            Buttons.Clear();

            if (buttons != null)
            {
                foreach (var btn in buttons)
                {
                    Buttons.Add(btn);
                }
            }

            MessageText.Text = message;

            if (!string.IsNullOrEmpty(title))
                TitleText.Text = title;
        }

        public static bool? ShowDialog(string message, string title, Window? owner = null)
        {
            var dialog = new CustomMessageBox();
            var window = new Window
            {
                Content = dialog,
                Width = 400,                           // ← Точный размер ширины
                Height = 300,                          // ← Точный размер высоты
                WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,      // Запрещаем изменение размера
                WindowStyle = WindowStyle.None,        // Убираем стандартную рамку
                Owner = owner,
                Title = title,
            };

            dialog.Buttons.Add(new MessageBoxButton
            {
                Text = "OK",
                Command = new RelayCommand(_ => window.DialogResult = true)
            });

            dialog.Show(message, title);

            return window.ShowDialog();
        }

        public static bool? ShowYesNoDialog(string message, string title, Window? owner = null)
        {
            var dialog = new CustomMessageBox();
            var window = new Window
            {
                Content = dialog,
                Width = 400,                           // ← Точный размер ширины
                Height = 300,                          // ← Точный размер высоты
                WindowStartupLocation = owner != null ? WindowStartupLocation.CenterOwner : WindowStartupLocation.CenterScreen,
                ResizeMode = ResizeMode.NoResize,      // Запрещаем изменение размера
                WindowStyle = WindowStyle.None,        // Убираем стандартную рамку
                Owner = owner,
                Title = title
            };

            // Команда для кнопки "Да"
            var yesCommand = new RelayCommand(_ =>
            {
                window.DialogResult = true;
            });

            // Команда для кнопки "Нет"
            var noCommand = new RelayCommand(_ =>
            {
                window.DialogResult = false;
            });

            // Подготавливаем массив кнопок
            var buttons = new[]
            {
            new MessageBoxButton { Text = "Да", Command = yesCommand },
            new MessageBoxButton { Text = "Нет", Command = noCommand }
            };

            // Передаём кнопки в метод Show
            dialog.Show(message, title, buttons);

            return window.ShowDialog();
        }

        public static bool? ShowCustomDialog(
            string message,
            string title,
            MessageBoxButton[] buttons,
            Window? owner = null)
        {
            var dialog = new CustomMessageBox();
            var window = new Window
            {
                Content = dialog,
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.None,
                Owner = owner,
                Title = title
            };

            dialog.Show(message, title, buttons);
            return window.ShowDialog();
        }

        public class RelayCommand : ICommand
        {
            private readonly Action<object?> _execute;
            private readonly Func<object?, bool>? _canExecute;

            public event EventHandler? CanExecuteChanged;  // Допускает null

            public RelayCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
            {
                _execute = execute ?? throw new ArgumentNullException(nameof(execute));
                _canExecute = canExecute;
            }

            public RelayCommand(Action execute)
             : this(execute != null ? new Action<object?>(_ => execute()) : null!, null)
            {
                if (execute == null) throw new ArgumentNullException(nameof(execute));
            }

            public bool CanExecute(object? parameter)
            {
                return _canExecute?.Invoke(parameter) ?? true;
            }

            public void Execute(object? parameter)
            {
                _execute(parameter);
            }

            public void RaiseCanExecuteChanged()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

    }
}
