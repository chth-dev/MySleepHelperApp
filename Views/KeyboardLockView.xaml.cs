using System;
using System.Windows;
using System.Windows.Controls;
using MySleepHelperApp.Services;

namespace MySleepHelperApp.Views
{
    public partial class KeyboardLockView : UserControl
    {
        private KeyboardHook? _keyboardHook;
        private static TransparentOverlay? _currentBlocker;
        private bool _isLockActive;

        public bool IsKeyboardHookActive => _isLockActive;
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
            _isLockActive = false;
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
                _isLockActive = true;

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

        private void UpdateUI(bool locked)
        {
            KeyboardUnLockButton.Visibility = locked ? Visibility.Visible : Visibility.Collapsed;
            KeyboardLockButton.Visibility = locked ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ResetUI()
        {
            UpdateUI(false);
        }
    }
}