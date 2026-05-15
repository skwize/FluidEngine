using FluidEngine.NativeImports;
using System;
using System.Collections.Generic;
using System.Text;

namespace FluidEngine.SystemMonitoring
{
    internal class CpuMonitoring
    {
        private static ulong _lastIdleTime;
        private static ulong _lastKernelTime;
        private static ulong _lastUserTime;

        public static double GetCpuUsage()
        {
            if (!NativeMethods.GetSystemTimes(out var idleTime, out var kernelTime, out var userTime)) return 0;

            ulong currentIdle = idleTime.ToLong();
            ulong currentKernel = kernelTime.ToLong();
            ulong currentUser = userTime.ToLong();

            // Считаем разницу между текущим и прошлым замером
            ulong idleDelta = currentIdle - _lastIdleTime;
            ulong kernelDelta = currentKernel - _lastKernelTime;
            ulong userDelta = currentUser - _lastUserTime;

            // Общее системное время = Kernel + User
            ulong totalDelta = kernelDelta + userDelta;

            _lastIdleTime = currentIdle;
            _lastKernelTime = currentKernel;
            _lastUserTime = currentUser;

            if (totalDelta == 0) return 0;

            // Формула: 100% минус процент бездействия
            // (Система считает Kernel время ВКЛЮЧАЯ Idle, поэтому вычитать Idle из Kernel не нужно)
            double usage = 100.0 - (idleDelta * 100.0 / totalDelta);

            return Math.Clamp(usage, 0, 100);
        }
    }
}
