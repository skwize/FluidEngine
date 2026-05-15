using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using FluidEngine.NativeImports;
using System;
using System.Linq;

namespace FluidEngine.WindowManager
{
    public static class ScreenHelper
    {

        public static double GetWindowScaling(IntPtr hwnd)
        {
            int dpi = 0;

            // Try using the modern GetDpiForWindow API
            if (Environment.OSVersion.Version.Major >= 10) // Windows 10 or later
            {
                dpi = NativeMethods.GetDpiForWindow(hwnd);
            }

            if (dpi == 0)
            {
                dpi = 96;
            }

            return (double)dpi / 96.0;

        }

        public static double GetScaling(POINT cPos)
        {
            var screens = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
                             ? desktop.Windows.FirstOrDefault()?.Screens
                             : null;

            if (screens == null) return 1.0;

            var screen = screens.ScreenFromPoint(cPos);
            return screen?.Scaling ?? 1.0;
        }

        public static PixelRect GetWorkingArea (POINT cPos)
        {
            var screens = Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
         ? desktop.Windows.FirstOrDefault()?.Screens
         : null;

            if (screens == null) return new PixelRect(0, 0, 1920, 1020);

            var screen = screens.ScreenFromPoint(cPos);
            return screen?.WorkingArea ?? new PixelRect(0, 0, 1920, 1020);
        }
    }
}
