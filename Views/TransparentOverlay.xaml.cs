using System;
using System.Windows;
using System.Windows.Media;
using MySleepHelperApp.Services;

namespace MySleepHelperApp
{
    public partial class TransparentOverlay : Window
    {
        // Изменяем на nullable поле
        private readonly KeyboardHook? _keyboardHook;

        public TransparentOverlay()
        {
            InitializeComponent();
            SetupOverlay();

            try
            {
                _keyboardHook = new KeyboardHook();
                _keyboardHook.Disposed += OnKeyboardHookDisposed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка блокировки клавиатуры: {ex.Message}");
                _keyboardHook = null; // Явное указание null
                Close();
            }
        }

        private void SetupOverlay()
        {
            WindowStyle = WindowStyle.None;
            AllowsTransparency = true;
            Background = Brushes.Transparent;
            Topmost = true;
            ShowInTaskbar = false;
            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;
        }

        private void OnKeyboardHookDisposed()
        {
            Dispatcher.Invoke(Close);
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_keyboardHook != null)
            {
                _keyboardHook.Disposed -= OnKeyboardHookDisposed; // Отписываемся от события
                _keyboardHook.Dispose(); // Вызываем Dispose() напрямую
            }
            base.OnClosed(e);
        }
    }
}