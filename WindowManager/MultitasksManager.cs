using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FluidEngine.WindowManager
{
    public static class MultitasksManagger
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        private const uint SPI_SETWINARRANGING = 0x0053; // Команда "Организация окон"
        private const uint SPIF_UPDATEINIFILE = 0x01;
        private const uint SPIF_SENDCHANGE = 0x02;

        public static void KillSnapForever()
        {
            // 1. ПРЯМАЯ ЗАПИСЬ В РЕЕСТР (Для сохранения после перезагрузки)
            // Важно: WindowArrangementActive должен быть СТРОКОЙ "0"
            Registry.SetValue(@"HKEY_CURRENT_USER\Control Panel\Desktop", "WindowArrangementActive", "1", RegistryValueKind.String);

            // Отключаем Snap Assist (чтобы не предлагало окна)
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", "SnapAssist", 1, RegistryValueKind.DWord);

            // 2. ЖИВОЕ ПРИМЕНЕНИЕ ЧЕРЕЗ WINAPI (Чтобы сработало СЕЙЧАС)
            // Этот вызов заставляет Windows ПЕРЕКЛЮЧИТЬ внутренний флаг в памяти
            // Передаем 0 (false) в uiParam, чтобы выключить
            SystemParametersInfo(SPI_SETWINARRANGING, 1, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            // 3. ФИНАЛЬНЫЙ ШТРИХ
            // В Windows 11 иногда требуется "дернуть" проводник, чтобы он сбросил хуки на окна.
            // Если после SystemParametersInfo все еще липнет, выполните:
            //System.Diagnostics.Process.GetProcessesByName("explorer").ToList().ForEach(p => p.Kill());
        }
    }
}
