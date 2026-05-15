using FluidEngine.WindowManager;
using System;
using System.Runtime.InteropServices;
using static FluidEngine.NativeImports.NativeStructures;


namespace FluidEngine.NativeImports
{
    internal class NativeMethods
    {
        #region WinAPI Imports
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetDpiForWindow(IntPtr hwnd);

        // Commonly used command constants
        public const int SW_HIDE = 0;
        public const int SW_NORMAL = 1;
        public const int SW_MAXIMIZE = 3;

        [DllImport("user32.dll")]
        public static extern bool UpdateWindow(IntPtr hWnd);
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool GetMessage(ref MSG lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

        [DllImport("user32.dll")] public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll")] public static extern IntPtr DispatchMessage(ref MSG lpMsg);

        [DllImport("user32.dll")] public static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref NativeStructures.DEVMODE devMode);
        [DllImport("user32.dll", CharSet = CharSet.Auto)] public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")] public static extern bool InvalidateRect(IntPtr hWnd, IntPtr lpRect, bool bErase);

        // Константы для удобства
        public const int WM_SETREDRAW = 0x000B;
        public const uint SWP_NOZORDER = 0x0004;
        public const uint SWP_NOACTIVATE = 0x0010;
        public const uint SWP_NOMOVE = 0x0002;
        public const nint HWND_BOTTOM = 1;
        public const nint HWND_TOP = 0;
        public const uint SWP_NOSIZE = 0x0001;
        public const uint SWP_NOREDRAW = 0x0008;
        public const uint SWP_FRAMECHANGED = 0x0020; // Не посылать WM_NCCALCSIZE (отключает пересчет рамки)
        public const uint SWP_NOSENDCHANGING = 0x0400; // Не посылать WM_WINDOWPOSCHANGING (блокирует реакцию окна)
        public const uint SWP_DEFERERASE = 0x2000; // Не очищать фон
        public const uint SWP_ASYNCWINDOWPOS = 0x4000; // Не ждать завершения (полезно для тяжелых окон типа VS)
        public const uint SWP_NOCOPYBITS = 0x0100;
        #endregion

        #region Kernel32 Imports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetSystemTimes(out NativeStructures.FILETIME lpIdleTime, out NativeStructures.FILETIME lpKernelTime, out NativeStructures.FILETIME lpUserTime);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
        #endregion
    }
}
