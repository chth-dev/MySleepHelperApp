using System;
using System.Diagnostics;
using System.Windows.Media; // Для FontFamily
using System.Collections.Generic;
using System.Reflection; // Для получения информации о сборке

namespace MySleepHelperApp.Services
{
    public class FontService
    {
        public FontFamily RegularFont { get; }
        public FontFamily BoldFont { get; }

        public FontService()
        {
            try
            {
                // Упрощенная загрузка шрифтов
                RegularFont = LoadFont("EpilepsySans.ttf");
                BoldFont = LoadFont("EpilepsySansBold.ttf");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки шрифтов: {ex}");
                // Fallback шрифты
                RegularFont = new FontFamily("Arial");
                BoldFont = new FontFamily("Arial");
            }
        }

        private FontFamily LoadFont(string fileName)
        {
            try
            {
                // Способ 1: Простая загрузка (чаще всего работает)
                var font = new FontFamily(new Uri("pack://application:,,,/"), $"./Fonts/#Epilepsy Sans");
                Debug.WriteLine($"Успешно загружен шрифт: {fileName}");
                return font;
            }
            catch
            {
                // Способ 2: Альтернативная загрузка
                try
                {
                    var font = new FontFamily(new Uri($"pack://application:,,,/Fonts/{fileName}"), "./#Epilepsy Sans");
                    Debug.WriteLine($"Успешно загружен шрифт (альтернативный способ): {fileName}");
                    return font;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Ошибка загрузки шрифта {fileName}: {ex.Message}");
                    throw;
                }
            }
        }

        // Для обратной совместимости
        public FontFamily GetFontFamily(int index = 0) => index == 0 ? RegularFont : BoldFont;
        public int FontCount => 2; // Теперь у нас всегда 2 шрифта
    }
}