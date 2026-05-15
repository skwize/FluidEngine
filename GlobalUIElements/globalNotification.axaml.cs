using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;
using FluidEngine.NativeImports;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FluidEngine;

public partial class globalNotification : Window
{
    public globalNotification()
    {
        InitializeComponent();
        DataContext = this;

        var screen = Screens.Primary;
        if (screen != null)
        {
            var area = screen.WorkingArea;
            int x = (area.Width / 2) - (350 / 2);
            int y = 0 - (int)this.Height; // Ставим окно по Y в отрицательных координатах

            this.Position = new PixelPoint(x, y); // Window pos manipulation. Replace it by SetWindowPos for Y -150 and more
        }

        this.Opened += async (s, e) =>
        {
            this.Width = 300;
            this.Height = 80;
            await Task.Delay(25);
            ShowWindow_FluidDropAnimation();
        };
    }

    // Анимация появления окна
    public async void ShowWindow_FluidDropAnimation()
    {

        // Параметры
        int targetY = 35;
        int startY = 0 - (int)this.Height;

        ShowNotifyAnimation(targetY, startY, this.Position.X);

        return;
    }

    public async void HideWindow_FluidDropAnimation()
    {

        double durationMs = 400; // Для улета дадим чуть больше времени, чтобы замах был виден
        double fps = 144;
        double interval = 1000 / fps;
        double s = 1.7; // Коэффициент замаха (чем больше, тем ниже присядет окно перед взлетом)

        int targetX = (int)this.Position.X;
        int startY = (int)this.Position.Y; // Текущая позиция (например, 30)
        int endY = -120; // Целевая позиция за экраном

        Stopwatch sw = Stopwatch.StartNew();

        while (sw.ElapsedMilliseconds < durationMs)
        {
            double t = sw.ElapsedMilliseconds / durationMs;
            if (t > 1.0) t = 1.0;

            // Формула BackEaseIn (для замаха):
            // f(t) = (s + 1) * t^3 - s * t^2
            double eased = (s + 1) * Math.Pow(t, 3) - s * Math.Pow(t, 2);

            // Расчет позиции
            int currentY = startY + (int)((endY - startY) * eased);

            this.Position = new PixelPoint(targetX, currentY);

            await Task.Delay((int)interval);
        }

        // Финальная точка и закрытие
        this.Position = new PixelPoint(targetX, endY);
        sw.Stop();
        this.Hide();
        return;
    }
    private async void ShowNotifyAnimation(int targetY, int startY, int wX)
    {
        double duration = 350; // Duration in ms
        double fps = MonitorDisplayHelper.GetRefreshRate(); // Animation fps
        double interval = 1000 / fps; // Interval between frames
        double s = 1.8; // Animation bounce amplitude
                        // Секундомер для точности
        Stopwatch sw = Stopwatch.StartNew();

        while (sw.ElapsedMilliseconds < duration)
        {
            // 1. Считаем прогресс 't' от 0.0 до 1.0 на основе РЕАЛЬНОГО времени
            double t = sw.ElapsedMilliseconds / duration;

            // Защита от перелета (если вдруг последний кадр затянется)
            if (t > 1.0) t = 1.0;

            // 2. Твоя формула BackEaseOut
            double t1 = t - 1;
            double eased = (s + 1) * Math.Pow(t1, 3) + s * Math.Pow(t1, 2) + 1;

            // 3. Расчет позиции
            int currentY = startY + (int)((targetY - startY) * eased);

            // 4. Применяем позицию
            this.Position = new PixelPoint(wX, currentY);

            // 5. Ждем до следующего кадра
            await Task.Delay((int)interval);
        }

        // Финальный аккорд: принудительно ставим в таргет, чтобы не было погрешности в 1 пиксель
        this.Position = new PixelPoint(wX, targetY);
        sw.Stop();

        return;
    }
}
    
    




//private object? _windowTintOpacity;
//public object? WindowTintOpacity
//{
//    get => _windowTintOpacity;
//    set
//    {
//        if (_windowTintOpacity != value)
//        {
//            _windowTintOpacity = value;
//            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WindowTintOpacity)));
//        }
//    }
//}