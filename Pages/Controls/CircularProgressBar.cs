using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System;

namespace FluidEngine.Pages.Controls
{
    public class CircularProgressBar : Control
    {
        // Свойство для значения (0-100)
        public static readonly StyledProperty<double> ValueProperty =
            AvaloniaProperty.Register<CircularProgressBar, double>(nameof(Value), 0);

        public double Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        // Свойство для толщины линии
        public static readonly StyledProperty<double> StrokeThicknessProperty =
            AvaloniaProperty.Register<CircularProgressBar, double>(nameof(StrokeThickness), 10);

        public double StrokeThickness
        {
            get => GetValue(StrokeThicknessProperty);
            set => SetValue(StrokeThicknessProperty, value);
        }

        // Статический конструктор для подписки на изменения
        static CircularProgressBar()
        {
            // Перерисовывать только визуальную часть при изменении Value
            ValueProperty.Changed.AddClassHandler<CircularProgressBar>((x, _) => x.InvalidateVisual());
            AffectsRender<CircularProgressBar>(ValueProperty);
        }

        public override void Render(DrawingContext context)
        {
            var bounds = Bounds;
            var thickness = StrokeThickness;
            var radius = (Math.Min(bounds.Width, bounds.Height) - thickness) / 2;
            var center = new Point(bounds.Width / 2, bounds.Height / 2);

            if (radius <= 0) return;

            // 1. Фон (темное кольцо)
            var backPen = new Pen(new SolidColorBrush(Color.Parse("#60428A")), thickness);
            context.DrawEllipse(null, backPen, center, radius, radius);

            // 2. Прогресс (светлая дуга)
            double percent = Math.Clamp(Value / 100.0, 0, 0.9999); // 0.9999 чтобы дуга не схлопнулась при 100%
            double angle = percent * 360;

            var progressPen = new Pen(new SolidColorBrush(Color.Parse("#E6C0FF")), thickness+1, lineCap: PenLineCap.Round);

            // Создаем объект геометрии
            var geometry = new StreamGeometry();

            // Открываем контекст для записи команд рисования
            using (var ctx = geometry.Open())
            {
                double startAngle = -90; // 12 часов
                double endAngle = startAngle + angle;

                Point startPoint = GetPointOnCircle(center, radius, startAngle);
                Point endPoint = GetPointOnCircle(center, radius, endAngle);

                ctx.BeginFigure(startPoint, false); // false = фигура не замкнута

                // Рисуем дугу
                ctx.ArcTo(endPoint, new Size(radius, radius), 0, angle > 180, SweepDirection.Clockwise);
            }

            // Рисуем готовую геометрию
            context.DrawGeometry(null, progressPen, geometry);
        }

        private Point GetPointOnCircle(Point center, double radius, double angleDegree)
        {
            double angleRad = angleDegree * Math.PI / 180.0;
            return new Point(
                center.X + radius * Math.Cos(angleRad),
                center.Y + radius * Math.Sin(angleRad));
        }
    }
}
