using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static FluidEngine.NativeImports.NativeMethods;

namespace FluidEngine.KeyboardCleaning
{
    public class KeyboardCleaning
    {

        private static LowLevelKeyboardProc? _proc;
        private static IntPtr _hookID = IntPtr.Zero;

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process? curProcess = Process.GetCurrentProcess())
            using (ProcessModule? curModule = curProcess?.MainModule)
            {
                if (curModule != null)
                {
                    return SetWindowsHookEx(WindowVariables.WH_KEYBOARD_LL, proc,
                        GetModuleHandle(curModule.ModuleName), 0);
                }
                return IntPtr.Zero;
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && (wParam == (IntPtr)WindowVariables.WM_KEYDOWN || wParam == (IntPtr)WindowVariables.WM_KEYUP))
            {
                int vkCode = Marshal.ReadInt32(lParam);
                // Логика блокировки клавиатуры
                return (IntPtr)1; // блокирует ввод клавиши
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public static void LockKeyboard ()
        {
            _proc = HookCallback; // Хранение делегата в поле класса
            _hookID = SetHook(_proc);
        }

        public static void UnlockKeyboard()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}
