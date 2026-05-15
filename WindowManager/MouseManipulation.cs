
namespace FluidEngine.WindowManager
{
    public static class MouseManipulation
    {
        static POINT lPoint;
        public static bool IsMouseMoving()
        {
            LowLevelHelpers.GetCursorPos(out Core.CursorPos);

            if (lPoint.X != Core.CursorPos.X || lPoint.Y != Core.CursorPos.Y)
            {
                lPoint = Core.CursorPos;
                return true;
            }
            return false;
        }
    }
}
