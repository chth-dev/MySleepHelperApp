using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Diagnostics;

namespace MySleepHelperApp.Services
{
    public class FontService
    {
        private PrivateFontCollection _privateFonts;
        private string _fontsDirectory;

        // Конструктор
        public FontService()
        {
            // Инициализируем коллекцию приватных шрифтов
            _privateFonts = new PrivateFontCollection();

            // Получаем путь к папке шрифтов
            _fontsDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts");

            // Загружаем шрифты
            LoadFonts();
        }

        // Метод для загрузки шрифтов
        private void LoadFonts()
        {
            try
            {
                if (Directory.Exists(_fontsDirectory))
                {
                    // Получаем список файлов шрифтов в папке
                    string[] fontFiles = Directory.GetFiles(_fontsDirectory, "*.ttf");

                    // Загружаем каждый шрифт
                    foreach (string fontFile in fontFiles)
                    {
                        _privateFonts.AddFontFile(fontFile);
                        Debug.WriteLine($"Загружен шрифт: {Path.GetFileName(fontFile)}");
                    }
                }
                else
                {
                    Debug.WriteLine("Папка со шрифтами не найдена.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка при загрузке шрифтов: {ex.Message}");
            }
        }

        // Метод для получения шрифта по индексу
        public Font GetFont(int index = 0, float size = 12)
        {
            if (_privateFonts != null && _privateFonts.Families.Length > index)
            {
                return new Font(_privateFonts.Families[index], size);
            }
            Debug.WriteLine("Шрифт не найден, возвращаем стандартный шрифт");
            return SystemFonts.DefaultFont; // Если шрифт не найден, возвращаем стандартный шрифт
        }

        // Метод для получения всех доступных шрифтов
        public FontFamily[] GetFontFamilies()
        {
            return _privateFonts?.Families ?? Array.Empty<FontFamily>();
        }

        // Количество загруженных шрифтов
        public int FontCount => _privateFonts?.Families.Length ?? 0;
    }
}
