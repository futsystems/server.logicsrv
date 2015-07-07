using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TradingLib.API;

namespace TradingLib.Common
{
    public class Performance
    {
        /// <summary>
        /// 从开始到现在CPU消耗比例
        /// </summary>
        public double CPUUsageTotal
        {
            get;
            private set;
        }

        /// <summary>
        /// 上个采样周期内的CPU时间
        /// </summary>
        public double CPUUsageLastMinute
        {
            get;
            private set;
        }

        /// <summary>
        /// 内存数量
        /// </summary>
        public double MemorySize
        {
            get;
            private set;
        }


        TimeSpan start;
        TimeSpan oldCPUTime = new TimeSpan(0);
        DateTime lastMonitorTime = DateTime.UtcNow;
        public DateTime StartTime = DateTime.UtcNow;

        // Call it once everything is ready
        public void OnStartup()
        {
            start = Process.GetCurrentProcess().TotalProcessorTime;
        }

        // Call this before get data
        void CallCPU()
        {
            //当前进程CPU时间消耗
            TimeSpan newCPUTime = Process.GetCurrentProcess().TotalProcessorTime - start;

            //单个采样间隔内的时间比例(当前时间-上次采样时间)
            CPUUsageLastMinute = (newCPUTime - oldCPUTime).TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(lastMonitorTime).TotalSeconds);
            lastMonitorTime = DateTime.UtcNow;
            //所有CPU时间比例
            CPUUsageTotal = newCPUTime.TotalSeconds / (Environment.ProcessorCount * DateTime.UtcNow.Subtract(StartTime).TotalSeconds);
            oldCPUTime = newCPUTime;
        }

        void CallMemroy()
        {
            Process proc = Process.GetCurrentProcess();
            this.MemorySize = Convert.ToDouble(proc.WorkingSet64 / (1024 * 1024));
        }

        /// <summary>
        /// 采样计算
        /// </summary>
        public void DoPerformance()
        {
            CallCPU();
            CallMemroy();
        }
    }
}