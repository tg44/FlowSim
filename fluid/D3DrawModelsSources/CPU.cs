using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace fluid.D3DrawModelsSources
{
    class CPU
    {
        PerformanceCounter counter;
        Stopwatch watch;
        bool CanReadCpu;
        float CpuUsage;
        public float Value { get { return CanReadCpu ? CpuUsage : 0; } }

        public bool Initialize()
        {
            CpuUsage = 0;
            watch = Stopwatch.StartNew();

            CanReadCpu = true;
            try
            {
                // Create performance counter.
                counter = new PerformanceCounter();
                counter.CategoryName = "Processor";
                counter.CounterName = "% Processor Time";
                counter.InstanceName = "_Total";
                CpuUsage = 0;
            }
            catch
            {
                CanReadCpu = false;
            }

            return true;
        }

        public void Frame()
        {
            if (CanReadCpu)
            {
                if (watch.ElapsedMilliseconds >= 1000)
                {
                    watch.Restart();
                    CpuUsage = counter.NextValue();
                }
            }
        }
        public void Dispose()
        {
            watch.Stop();
            if (CanReadCpu)
                counter.Close();
        }
    }
}
