

using System.Runtime.InteropServices;

namespace FluidEngine.NativeImports
{
    internal class MonitorDisplayHelper
    {  
        public const int ENUM_CURRENT_SETTINGS = -1;

        public static int GetRefreshRate()
        {
            NativeStructures.DEVMODE dm = new NativeStructures.DEVMODE();
            dm.dmSize = (short)Marshal.SizeOf(typeof(NativeStructures.DEVMODE));

            // Передаем null, чтобы получить настройки основного монитора
            if (NativeMethods.EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
            {
                return dm.dmDisplayFrequency;
            }

            return 60; // Дефолт, если что-то пошло не так
        }
    }
}
