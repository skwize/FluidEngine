using Avalonia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluidEngine.WindowManager
{
    public class ISnapTrigger
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public TriggerPos position { get; set; }
        public ISnapTrigger (int x, int y, int width, int height, TriggerPos position)
        {
            X = x; Y = y; Width = width; Height = height;
            this.position = position;

        }
    }

    public enum TriggerPos
    {
        Left,
        Right,
        Top,
        TopLeft,
        BottomLeft,
        TopRight,
        BottomRight,
        no_one
    }
    public class SnapTrigger
    {
        // Need to add an Array of all triggers, add variable "InvokedTrigger" and optimize function below, by "for" construction and the only one 'if'

        private static List<ISnapTrigger> GenerateTriggers(POINT curPos)
        {
            PixelRect area = ScreenHelper.GetWorkingArea(curPos);
            int areaWidth = area.Width;
            int areaHeight = area.Height;
            int offsetX = area.X; // Смещение, если монитор не основной
            int offsetY = area.Y;

            int _peaceAreaWidth = areaWidth / 5;
            int _peaceAreaHeight = areaHeight / 15;

            return new List<ISnapTrigger>
            {
                new ISnapTrigger(offsetX, offsetY, offsetX + _peaceAreaWidth, offsetY + (_peaceAreaHeight * 2), TriggerPos.TopLeft),
                new ISnapTrigger(offsetX + (areaWidth - _peaceAreaWidth), offsetY, offsetX + areaWidth, offsetY + (_peaceAreaHeight * 2), TriggerPos.TopRight),
                new ISnapTrigger(offsetX + _peaceAreaWidth, offsetY, offsetX + (areaWidth - _peaceAreaWidth), offsetY + (_peaceAreaHeight * 2), TriggerPos.Top),
                new ISnapTrigger(offsetX, offsetY + (_peaceAreaHeight * 2), offsetX + _peaceAreaWidth, offsetY + (_peaceAreaHeight * 11), TriggerPos.Left),
                new ISnapTrigger(offsetX + (areaWidth - _peaceAreaWidth), offsetY + (_peaceAreaHeight * 2), offsetX + areaWidth, offsetY + (_peaceAreaHeight * 11), TriggerPos.Right),
                new ISnapTrigger(offsetX, offsetY - (areaHeight - (_peaceAreaHeight * 2)), offsetX + _peaceAreaWidth, offsetY + areaHeight, TriggerPos.BottomLeft),
                new ISnapTrigger(offsetX + (areaWidth - _peaceAreaWidth), offsetY + (areaHeight - (_peaceAreaHeight * 2)), offsetX + areaWidth, offsetY + areaHeight, TriggerPos.BottomRight)
            };
        }

        public static TriggerPos GetInvokedTrigger (POINT CursorPos)
        {
            List<ISnapTrigger> triggers = GenerateTriggers(CursorPos);

            var invokedTrigger = triggers.FirstOrDefault(t =>
                CursorPos.X >= t.X && CursorPos.X <= t.Width &&
                CursorPos.Y >= t.Y && CursorPos.Y <= t.Height);

            return invokedTrigger?.position ?? TriggerPos.no_one;
        }
    }
}
