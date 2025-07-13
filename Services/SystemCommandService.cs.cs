using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MySleepHelperApp.Services
{
    public static class SystemCommandService
    {

        // Выключает компьютер через указанное время.
        public static void ShutdownComputer(int seconds)
        {
            Process process = new(); // Создаём процесс (как если бы cmd.exe запустили вручную)

            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden, // Скрываем окно командной строки
                FileName = "cmd.exe",                    // Запускаем командную строку
                Arguments = $"/C shutdown -s -f -t {seconds}", // Команда выключения
                Verb = "runas" // Запуск от имени администратора (важно!)
            }; // Настройки запуска процесса

            process.StartInfo = startInfo; // Привязываем настройки к процессу

            process.Start(); // Запускаем процесс
        }

        // Отменяет запланированное выключение. 
        public static void CancelShutdown()
        {
            Process process = new();
            ProcessStartInfo startInfo = new()
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                FileName = "cmd.exe",
                Arguments = "/C shutdown -a", // Отмена выключения
                Verb = "runas"
            };

            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
