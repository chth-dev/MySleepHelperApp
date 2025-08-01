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
        public static bool ShutdownComputer(int seconds)
        {

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo
                    {
                        FileName = "shutdown.exe",
                        Arguments = $"/s /f /t {seconds}",
                        Verb = "runas", // Запуск от имени администратора
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    };

                    process.Start();
                    process.WaitForExit();

                    int exitCode = process.ExitCode;
                    Debug.WriteLine($"[ShutdownComputer] ExitCode: {exitCode}");

                    // Успешные коды
                    if (exitCode == 0 || exitCode == 1190)
                        return true;

                    // Известные коды ошибок
                    switch (exitCode)
                    {
                        case 5: // Access denied
                            Debug.WriteLine("Ошибка: Недостаточно прав. Запустите приложение от имени администратора.");
                            break;
                        case 1115: // System shutdown was blocked
                            Debug.WriteLine("Ошибка: Выключение заблокировано (например, установлены обновления).");
                            break;
                        default:
                            Debug.WriteLine($"Неизвестная ошибка shutdown.exe: {exitCode}");
                            break;
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ShutdownComputer] Ошибка: {ex.Message}");
                return false;
            }
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
