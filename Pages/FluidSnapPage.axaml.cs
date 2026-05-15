using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluidEngine.DataCore;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FluidEngine;

public partial class FluidSnapPage : UserControl
{
    public FluidSnapPage()
    {
        InitializeComponent();
        _windowGapValueSlider.Value = HardwareData.Instance.WindowGaps;
        _windowGapValueLabel.Content = HardwareData.Instance.WindowGaps;
    }

    private Stopwatch sw = null;
    private async void GapAnimation ()
    {
        double durationMs = 200;
        double fps = 144;
        double interval = 1000 / fps;
        //asdouble s = 1.8;
        int targetValue = HardwareData.Instance.WindowGaps;

        if (sw != null)
        {
            sw.Stop();
            sw = null;
        }

        sw = Stopwatch.StartNew();

        while (sw != null & sw?.ElapsedMilliseconds < durationMs)
        {
            double t = sw.ElapsedMilliseconds / durationMs;
            if (t > 1.0) t = 1.0;

            double t1 = t - 1;
            double eased = t;

            // Расчет позиции
            int currentRowGapValue = (int)(_snapOverlayView.RowSpacing + (int)((targetValue - _snapOverlayView.RowSpacing) * eased));
            int currentColumnGapValue = (int)(_snapOverlayView.ColumnSpacing + (int)((targetValue - _snapOverlayView.ColumnSpacing) * eased));
            Avalonia.Thickness currentMarginValue = (_snapOverlayView.Margin + ((new Avalonia.Thickness(targetValue) - _snapOverlayView.Margin) * eased));

            _snapOverlayView.RowSpacing = currentRowGapValue;
            _snapOverlayView.ColumnSpacing = currentColumnGapValue;
            _snapOverlayView.Margin = currentMarginValue;

            await Task.Delay((int)interval);
        };

        if (sw != null) sw.Stop();
        sw = null;
        return;
    }

    private void Slider_ValueChanged(object? sender, Avalonia.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        HardwareData.Instance.WindowGaps = (int)_windowGapValueSlider.Value;

        if (HardwareData.Instance.WindowGaps == 1)
        {
            _windowGapValueLabel.Content = 0;
        }
        else
        {
            _windowGapValueLabel.Content = HardwareData.Instance.WindowGaps;
        }
        GapAnimation();
    }
}