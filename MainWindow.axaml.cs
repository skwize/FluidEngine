using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform;
using FluidEngine.NativeImports;
using System;
using System.ComponentModel;
using System.Threading;

namespace FluidEngine
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private TrayIcon? _trayIcon;
        private bool showFirstTime = false;
        private bool _isRealExit = false;
        private readonly GeneralPage _generalPage = new GeneralPage();
        private readonly FluidSnapPage _fluidsnapPage = new FluidSnapPage();

        public static IntPtr WinMenuHandle;
        public static bool isGlobalThreadRunning;
        public static Thread? GlobalThread = new Thread(FluidEngineThread);
        public MainWindow()
        {
            InitializeComponent();
            InitializeTray();
            InitGlobalThread();

            this.DataContext = this;
            CurrentPage = _generalPage;
        }

        private void InitializeTray()
        {
            // 1. Загружаем иконку из ресурсов (Берем твою неоновую каплю)
            var assetLoader = AssetLoader.Open(new Uri("avares://FluidEngine/Assets/fluid_engine.ico"));
            var icon = new WindowIcon(assetLoader);

            // 2. Создаем само меню трея
            var menu = new NativeMenu();

            var exitItem = new NativeMenuItem("Выход");
            exitItem.Click += (s, e) =>
            {
                if(Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                {
                    desktop.Shutdown();
                }
            };

            menu.Items.Add(exitItem);

            // 3. Инициализируем иконку в трее
            _trayIcon = new TrayIcon
            {
                Icon = icon,
                ToolTipText = "Fluid Engine — Плавность в каждом движении", // Здесь кириллица будет работать!
                Menu = menu,
                IsVisible = true
            };

            // Клик по самой иконке (левой кнопкой)
            _trayIcon.Clicked += (s, e) => { this.Show(); this.Activate(); };
        }

        private void InitGlobalThread()
        {
            if (GlobalThread == null) return;
            // Init Threading Functions
            isGlobalThreadRunning = true;
            GlobalThread.IsBackground = true;
            GlobalThread.Start();
        }

        // Main thread for lisening commands from WINAPI
        private static void FluidEngineThread()
        {
            NativeStructures.MSG msg = default;

            while (isGlobalThreadRunning)
            {
                if (NativeMethods.GetMessage(ref msg, IntPtr.Zero, 0, 0))
                {
                    NativeMethods.TranslateMessage(ref msg);
                    NativeMethods.DispatchMessage(ref msg);
                }
                Thread.Sleep(350);
            }
        }

        private object? _currentPage;
        public new event PropertyChangedEventHandler? PropertyChanged;
        public object? CurrentPage
        {
            get => _currentPage;
            set
            {
                if (_currentPage != value)
                {
                    _currentPage = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentPage)));
                }
            }
        }

        private void general_p_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CurrentPage = _generalPage;
        }

        private void fluidSnap_p_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            CurrentPage = _fluidsnapPage;
        }

        private void Window_Closing(object? sender, WindowClosingEventArgs e)
        {
            if (this.IsVisible == false) return;

            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }
    }
}