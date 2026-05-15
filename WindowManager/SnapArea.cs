using Avalonia.Controls.ApplicationLifetimes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using System.Diagnostics;
using FluidEngine.DataCore;


namespace FluidEngine.WindowManager
{
    public class ISnapArea
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public TriggerPos position;
        public ISnapArea (int x, int y, int width, int height, TriggerPos position)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.position = position;
        }
    }

    public static class SnapArea
    {
        //public static ISnapArea data = null;

        public static List<ISnapArea> GetSnapAreas (POINT curPos)
        {
            PixelRect area = ScreenHelper.GetWorkingArea(curPos);
            double screenDpi = ScreenHelper.GetScaling(curPos);

            int sWidth = area.Width;
            int sHeight = area.Height;

            int _hWidth = sWidth / 2;
            int _hHeight = sHeight / 2;


            // Смещение, если монитор не основной
            int offsetX = area.X;
            int offsetY = area.Y;

            int wMargin = (int)(HardwareData.Instance.WindowGaps);

            Debug.WriteLine(wMargin);

            return new List<ISnapArea>
            {
                new ISnapArea(offsetX + wMargin, offsetY + wMargin, (_hWidth - (int)(wMargin * 1.5)), (_hHeight - (int)(wMargin * 1.5)), TriggerPos.TopLeft),
                new ISnapArea(offsetX + (_hWidth + wMargin / 2), offsetY + wMargin, (_hWidth - (int)(wMargin * 1.5)), (_hHeight - (int)(wMargin * 1.5)), TriggerPos.TopRight),
                new ISnapArea(offsetX + wMargin, offsetY + (_hHeight + (int)(wMargin / 1.5)), (_hWidth - (int)(wMargin * 1.5)), (_hHeight - wMargin * 2), TriggerPos.BottomLeft),
                new ISnapArea(offsetX + (_hWidth + wMargin / 2), offsetY + (_hHeight + (int)(wMargin / 1.5)), (_hWidth - (int)(wMargin * 1.5)), (_hHeight - wMargin * 2), TriggerPos.BottomRight),
                new ISnapArea(offsetX + wMargin, offsetY + wMargin, (sWidth - wMargin*2), (sHeight - wMargin*2), TriggerPos.Top),
                new ISnapArea(offsetX + wMargin, offsetY + wMargin, (_hWidth - (int)(wMargin * 1.5)), (sHeight - (wMargin * 2)), TriggerPos.Left),
                new ISnapArea(offsetX + (_hWidth + wMargin/2), offsetY + wMargin, (_hWidth - (int)(wMargin * 1.5)), (sHeight - (wMargin * 2)), TriggerPos.Right)
            };
        }
    }
}
