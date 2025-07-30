using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MySleepHelperApp.Services;

namespace MySleepHelperApp.Views
{
    public partial class KeyboardLockView : UserControl
    {
        private KeyboardHook? _keyboardHook;
        private static TransparentOverlay? _currentBlocker;

        public bool IsKeyboardHookActive => _isBlockRunning;
        public static TransparentOverlay? CurrentBlocker => _currentBlocker;

        public KeyboardLockView()
        {
            InitializeComponent();
            ResetUI();
        }

        public void ReleaseKeyboardHook()
        {
            _keyboardHook?.Dispose();
            _keyboardHook = null;
            _isBlockRunning = false;
        }

        private void KeyboardLockButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentBlocker != null)
            {
                _currentBlocker.Activate();
                return;
            }

            try
            {
                _keyboardHook = new KeyboardHook();
                _isBlockRunning = true;

                _currentBlocker = new TransparentOverlay();
                _currentBlocker.Closed += (s, args) =>
                {
                    ReleaseKeyboardHook();
                    _currentBlocker = null;
                    UpdateUI(false);
                };

                _currentBlocker.Show();
                UpdateUI(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
                ReleaseKeyboardHook();
                UpdateUI(false);
            }
        }

        private void KeyboardUnLockButton_Click(object sender, RoutedEventArgs e)
        {
            _currentBlocker?.Close();
        }

        private void UpdateUI(bool isLocked)
        {
            _isBlockRunning = isLocked; // Обновляем флаг

            // 1. Обновляем кнопку
            UpdateButtonState();

            // 2. Обновляем плашку
            UpdateStatusPanel(isLocked);
        }

        private void ResetUI()
        {
            UpdateUI(false);
        }

        private bool _isBlockRunning = false; // Флаг состояния
        private void UpdateButtonState()
        {
            if (_isBlockRunning)
            {
                // Если блокировка работает, показываем "разблокировать" и вешаем обработчик разблокировки
                ButtonText.Text = "Разблокировать";
                KeyboardLockButton.Click -= KeyboardLockButton_Click; // Удаляем старый обработчик
                KeyboardLockButton.Click += KeyboardUnLockButton_Click;     // Добавляем новый
            }
            else
            {
                // Если блокировка не работает, показываем "Заблокировать" и вешаем обработчик блокировки
                ButtonText.Text = "Заблокировать";
                KeyboardLockButton.Click -= KeyboardUnLockButton_Click;     // Удаляем старый обработчик
                KeyboardLockButton.Click += KeyboardLockButton_Click; // Добавляем новый
            }
        }

        private void UpdateStatusPanel(bool isLocked)
        {
            // Меняем иконку
            StatusIcon.Source = (ImageSource)FindResource(isLocked ? "CrossIcon" : "CheckmarkIcon");

            // Меняем текст
            StatusText.Text = isLocked
                ? "Сейчас клавиатура заблокирована"
                : "Сейчас клавиатура активна";
        }

    }
}