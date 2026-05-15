using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using FluidEngine.DataCore;
using FluidEngine.SystemMonitoring;
using FluidEngine.WindowManager;

namespace FluidEngine;

public partial class GeneralPage : UserControl
{
    public GeneralPage()
    {
        InitializeComponent();
        this.DataContext = HardwareData.Instance;
        SystemMonitoring.SystemMonitoring.Start();
    }

    private readonly globalNotification notification = new globalNotification();

    private void switchBtn_KeysLock_Checked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        KeyboardCleaning.KeyboardCleaning.LockKeyboard();
        notification.Show();
    }

    private void switchBtn_KeysLock_Unchecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        KeyboardCleaning.KeyboardCleaning.UnlockKeyboard();
        notification.HideWindow_FluidDropAnimation();
    }

    private void switchBtn_Snap_Checked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowManager.Core.EnableWindowManager();
    }

    private void switchBtn_Snap_Unchecked(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowManager.Core.DisableWindowManager();
    }
}