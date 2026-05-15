using Avalonia.Controls;
using SkiaSharp;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace FluidEngine;

public partial class GhostWindow : Window
{

    public GhostWindow()
    {
        InitializeComponent();
    }

    public IntPtr GetHandle ()
    {
        if (this.TryGetPlatformHandle() is { } handle)
        {
            return handle.Handle;
        }

        return IntPtr.Zero;
    }
}