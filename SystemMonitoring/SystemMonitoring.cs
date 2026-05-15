
using Avalonia.Threading;
using FluidEngine.DataCore;
using FluidEngine.NativeImports;
using System;
using System.Management;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static FluidEngine.NativeImports.NativeStructures;

namespace FluidEngine.SystemMonitoring
{
    internal class SystemMonitoring
    {
        private static Thread? _SystemMonitoringThread;
        private static Thread? _GpuMonitoringThread;
        private static bool _isMonitoringRunning;
        private static bool _isGpuMonitoringRunning;
        private const int UpdateIntervalMs = 820;

        public static void Start()
        {
            if (_isMonitoringRunning) return;

            _isMonitoringRunning = true;
            _isGpuMonitoringRunning = true;

            // Создаем поток. Background = true, чтобы он не мешал приложению закрыться.
            _SystemMonitoringThread = new Thread(SystemMonitoringLoop)
            {
                Name = "FluidEngine_System_Monitor",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal // Даем чуть больше приоритета для точности
            };

            _SystemMonitoringThread.Start();

            _GpuMonitoringThread = new Thread(GpuMonitoringLoop)
            {
                Name = "FluidEngine_GPU_Monitor",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal // Даем чуть больше приоритета для точности
            };

            _GpuMonitoringThread.Start();
        }

        public static void Stop()
        {
            _isMonitoringRunning = false;
            _isGpuMonitoringRunning = false;

            _SystemMonitoringThread?.Interrupt(); // Ждем завершения, если нужно
            _GpuMonitoringThread?.Interrupt();

            _SystemMonitoringThread?.Join(); // Ждем завершения, если нужно
            _GpuMonitoringThread?.Join(); // Ждем завершения, если нужно
        }

        private static void SystemMonitoringLoop()
        {
            // Инициализируем первый замер (прогрев)
            CpuMonitoring.GetCpuUsage();
            var memStatus = new MEMORYSTATUSEX();
            memStatus.Init();

            while (_isMonitoringRunning)
            {
                // Берем актуальные данные из WinAPI
                var currentCpuLoad = CpuMonitoring.GetCpuUsage();
                bool haveRamLoad = false;
                if (NativeMethods.GlobalMemoryStatusEx(ref memStatus))
                {
                    haveRamLoad = true;
                }
                // Прокидываем в UI Авалонии. 
                // Используем Post, чтобы не блокировать поток мониторинга отрисовкой.
                Dispatcher.UIThread.Post(() =>
                {
                    // Здесь твоя привязка к ViewModel (например, через паттерн синглтона или событие)
                    HardwareData.Instance.CpuValue = (int)currentCpuLoad;

                    if (haveRamLoad)
                    {
                        HardwareData.Instance.RamValue = (int)memStatus.dwMemoryLoad;
                    }
                });

                // "Засыпаем" ровно на интервал
                Thread.Sleep(UpdateIntervalMs);
            }
        }

        private static void GpuMonitoringLoop ()
        {

            // Создаем поисковик один раз, чтобы не спамить объектами в памяти
            using (var searcher = new ManagementObjectSearcher("SELECT UtilizationPercentage FROM Win32_PerfFormattedData_GPUPerformanceCounters_GPUEngine"))
            while (_isGpuMonitoringRunning)
            {

                float currentUsage = 0;
                var results = searcher.Get();

                try
                {
                    foreach (ManagementObject obj in results)
                    {
                        float val = Convert.ToSingle(obj["UtilizationPercentage"]);
                        if (val > currentUsage) currentUsage = val;
                        obj.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    // Если WMI лаганул, просто пропускаем тик
                    Console.WriteLine($"WMI Error: {ex.Message}");
                }

                Dispatcher.UIThread.Post(() =>
                {
                    HardwareData.Instance.GpuValue = (int)currentUsage;
                });

                Thread.Sleep(1500);
            }
        }
    }
}
