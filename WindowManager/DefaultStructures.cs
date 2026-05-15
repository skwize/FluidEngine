using Avalonia;
using System.Runtime.InteropServices;

namespace FluidEngine.WindowManager
{
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator PixelPoint(POINT point)
        {
            return new PixelPoint(point.X, point.Y);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public static implicit operator PixelRect(RECT rect)
        {
            return new PixelRect(rect.Left, rect.Top, rect.Right, rect.Bottom);
        }
    }
}
