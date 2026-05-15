using Avalonia;
using Avalonia.Threading;
using FluidEngine.NativeImports;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FluidEngine.WindowManager
{
    public class Core
    {
        private static Thread? WinManagerThread;
        private static bool isWindowManagerRunning;

        private static IntPtr ForegroundWindowHandle;
        public static POINT CursorPos;
        private static RECT oldWndRect;
        private static bool isReadyToSnap = false;
        private static bool isOverlayShowing = false;
        private readonly static GhostWindow overlay = new();



        // Main WM Thread logic
        private static async void WinManagerThreadFunc()
        {

            try
            {
                while (isWindowManagerRunning)
                {
                    //Debug.WriteLine(isReadyToSnap);
                    if ((LowLevelHelpers.GetKeyState(0x01) & 0x8000) != 0)
                    {
                        GetWindowForeground();
                        if (MouseManipulation.IsMouseMoving() && DetectWindowDragging(ForegroundWindowHandle))
                        {
                            await Task.Delay(50);
                            if (IsWindowReadyToSnap())
                            {
                                ShowSnapOverlay();
                            }
                            else
                            {
                                HideSnapOverlay();
                            }
                        }
                    }
                    else
                    {
                        HideSnapOverlay();
                        if (ForegroundWindowHandle != IntPtr.Zero && isReadyToSnap)
                        {
                            SnapWindow(ForegroundWindowHandle);
                            continue;
                        }
                    }

                    Thread.Sleep(120);
                }
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }
        }

        public static void EnableWindowManager ()
        {
            isWindowManagerRunning = true;
            // MultitasksManagger.KillSnapForever();
            WinManagerThread = new Thread(WinManagerThreadFunc)
            {
                Name = "FluidEngine_WindowManager",
                IsBackground = true
            };
            WinManagerThread.Start();
        }

        public static void DisableWindowManager ()
        {
            isWindowManagerRunning = false;
            if (WinManagerThread != null && WinManagerThread.IsAlive)
            {
                WinManagerThread.Interrupt();
                WinManagerThread.Join();
                WinManagerThread = null;
            }
        }

        public static IntPtr GetWindowForeground ()
        {
            IntPtr hwnd = LowLevelHelpers.GetForegroundWindow();

            if (hwnd == IntPtr.Zero) return IntPtr.Zero;

            RECT rect;
            LowLevelHelpers.GetWindowRect(hwnd, out rect);
            // Делаем проверку, что окно не является системным
            StringBuilder className = new StringBuilder(256); // Создаем строку для имени класса окна
            LowLevelHelpers.GetClassName(hwnd, className, 256); // Получаем имя класса окна
            if (!string.IsNullOrWhiteSpace(className.ToString()) && !className.ToString().Contains("Sys"))
            {
                return ForegroundWindowHandle = hwnd;
            }
            return IntPtr.Zero;
        }

        public static bool DetectWindowDragging (IntPtr hwnd)
        {

            RECT wndRect;

            if (hwnd != IntPtr.Zero)
            {
                LowLevelHelpers.GetWindowRect(hwnd, out wndRect);
                
                if (wndRect.Left != oldWndRect.Left && wndRect.Bottom != oldWndRect.Bottom)
                {
                    oldWndRect = wndRect;
                    return true;
                }

                return false;
            }

            return false;
        }


        private static void ShowSnapOverlay ()
        {
            
            Dispatcher.UIThread.Post( async () =>
            {
                var snapArea = SnapArea.GetSnapAreas(CursorPos).FirstOrDefault(area => area.position == SnapTrigger.GetInvokedTrigger(CursorPos));
                if (isOverlayShowing || snapArea == null) return;

                overlay.Show();
                LowLevelHelpers.SetWindowPos(overlay.GetHandle(), IntPtr.Zero, snapArea.X, snapArea.Y, snapArea.Width, snapArea.Height, NativeMethods.SWP_NOACTIVATE);
                
                await Task.Delay(50);

                overlay.OverlayBody.Opacity = 1;
                isOverlayShowing = true;
                return;
            }, DispatcherPriority.Background);
        }

        private static void HideSnapOverlay ()
        {
            if (!isOverlayShowing) return;
            isOverlayShowing = false;
            Dispatcher.UIThread.Post(() =>
            {
                overlay.OverlayBody.Opacity = 0;
                overlay.Hide();
            }, DispatcherPriority.Background);
        }
        
        public static bool IsWindowReadyToSnap ()
        {
            if (SnapTrigger.GetInvokedTrigger(CursorPos) == TriggerPos.no_one)
            {
                HideSnapOverlay();
                Debug.WriteLine("NOT READY");
                isReadyToSnap = false;
                return false;
            }

            Debug.WriteLine("READY");
            isReadyToSnap = true;
            return true;

        }


        public static void SnapWindow (IntPtr hwnd)
        {
            isReadyToSnap = false;

            if (hwnd == IntPtr.Zero)
            {
                return;
            }

            RECT winRectOffset = new RECT();
            
            LowLevelHelpers.GetWindowRect(hwnd, out RECT frameRect);
            LowLevelHelpers.DwmGetWindowAttribute(hwnd, LowLevelHelpers.DWMWA_EXTENDED_FRAME_BOUNDS, out var extendedRect, Marshal.SizeOf(typeof(RECT)));

            winRectOffset.Left = extendedRect.Left - frameRect.Left;
            winRectOffset.Right = extendedRect.Top - frameRect.Top;
            winRectOffset.Top = frameRect.Right - extendedRect.Right;
            winRectOffset.Bottom = frameRect.Bottom - extendedRect.Bottom;

            var snapArea = SnapArea.GetSnapAreas(CursorPos).FirstOrDefault(area => area.position == SnapTrigger.GetInvokedTrigger(CursorPos));

            if (snapArea == null) return;

            try
            {
                LowLevelHelpers.SetWindowPos(hwnd, IntPtr.Zero, snapArea.X - winRectOffset.Left, snapArea.Y - winRectOffset.Top,
                                                                snapArea.Width - (winRectOffset.Right - winRectOffset.Left),
                                                                snapArea.Height - (winRectOffset.Bottom - winRectOffset.Top), 0x0040);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                return;
            }

            return;
        }
    }
}
