using System;
using System.Collections.Generic;
using System.Text;

namespace FluidEngine.DataCore
{
    public class HardwareData: System.ComponentModel.INotifyPropertyChanged
    {
        // Реализация синглтона
        public static HardwareData Instance { get; } = new HardwareData();

        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(name));

        private int _cpuValue;
        private int _ramValue;
        private int _gpuValue;
        private int _diskFullnessValue;
        private int _windowGaps = 10;
        public int CpuValue
        {
            get => _cpuValue;
            set
            {
                _cpuValue = value;
                OnPropertyChanged(nameof(CpuValue));
            }
        }
        public int RamValue
        {
            get => _ramValue;
            set
            {
                _ramValue = value;
                OnPropertyChanged(nameof(RamValue));
            }
        }
        public int GpuValue
        {
            get => _gpuValue;
            set
            {
                _gpuValue = value;
                OnPropertyChanged(nameof(GpuValue));
            }
        }
        public int DiskFullnessValue
        {
            get => _diskFullnessValue;
            set
            {
                _diskFullnessValue = value;
                OnPropertyChanged(nameof(DiskFullnessValue));
            }
        }

        public int WindowGaps
        {
            get => _windowGaps;
            set
            {
                _windowGaps = value;
                OnPropertyChanged(nameof(WindowGaps));
            }
        }
    }
}
