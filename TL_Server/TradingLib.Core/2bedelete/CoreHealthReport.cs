using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using System.Threading;
using System.Diagnostics;
using TradingLib.Core;

namespace TradingLib.Contrib
{
    /*
    /// <summary>
    /// 核心组件的相关数据统计
    /// </summary>
    public class CoreHealthReport:IHealthInfo
    {
        private PerformanceCounter PC1 = new PerformanceCounter();
        private PerformanceCounter PC2 = new PerformanceCounter();
        private PerformanceCounter PC3 = new PerformanceCounter();
        Process srvprocess;
        Process[] p1 = Process.GetProcessesByName("mysqld");//获取指定进程信息

        MgrServer _mgrsrv;//管理服务
        TradingServer _tradingsrv;//交易服务
        ClearCentreSrv _clearcentre;//清算中心

        static object lockobj = new object();
        static IHealthInfo _hinfo = null;
        public static IHealthInfo GetReport(ClearCentreSrv c, TradingServer t,MgrServer m)
        {
            lock (lockobj)
            {
                if (_hinfo == null)
                {
                    _hinfo = new CoreHealthReport(c, t, m);
                }
                return _hinfo;
            }
            
        }
        public CoreHealthReport(ClearCentreSrv c, TradingServer t, MgrServer m)
        {

            srvprocess = Process.GetCurrentProcess();//获取指定进程信息

            PC2.CategoryName = "Processor";//指定获取计算机进程信息  如果传Processor参数代表查询计算机CPU 
            PC2.CounterName = "% Processor Time";
            //如果pp.CategoryName="Processor",那么你这里赋值这个参数 pp.InstanceName = "_Total"代表查询本计算机的总CPU。
            PC2.InstanceName = "_Total";//指定进程 "_Total";
            PC2.MachineName = ".";       //本机

            PC1.CategoryName = "Process";//指定获取计算机进程信息  如果传Processor参数代表查询计算机CPU 
            PC1.CounterName = "% Processor Time";
            //如果pp.CategoryName="Processor",那么你这里赋值这个参数 pp.InstanceName = "_Total"代表查询本计算机的总CPU。
            PC1.InstanceName = srvprocess.ProcessName;//指定进程 "_Total";
            PC1.MachineName = ".";       //本机

            PC3.CategoryName = "Process";//指定获取计算机进程信息  如果传Processor参数代表查询计算机CPU 
            PC3.CounterName = "% Processor Time";
            //如果pp.CategoryName="Processor",那么你这里赋值这个参数 pp.InstanceName = "_Total"代表查询本计算机的总CPU。
            PC3.InstanceName = "mysqld";//指定进程 "_Total";
            PC3.MachineName = ".";       //本机
            _clearcentre = c;
            _tradingsrv = t;
            _mgrsrv = m;
        }
        public double CPUTotal 
        {
            get
            {
                return Math.Round(PC2.NextValue(), 2);
            }
            set
            {
            }
        }

        public double CPUQS
        {
            get
            {
                return Math.Round(PC1.NextValue(), 2);
            }
            set
            {
            }
        }

        

        public double CPUMysql
        {
            get 
            {
                return Math.Round(PC3.NextValue(), 2);
            }
            set
            {
            }
        }

        public double MemoryQS
        {
            get
            {
                return (Convert.ToInt64(srvprocess.WorkingSet64.ToString()) / 1024);
            }
            set
            {
            }
        }


        public double MemoryMysql
        {
            get
            {
                return (Convert.ToInt64(p1[0].WorkingSet64.ToString()) / 1024);
            }
            set
            {
            }
        }

        public int CustomerClientNum
        {
            get
            {
                return _mgrsrv!=null ?_mgrsrv.CustomerNum:0;
            }
            set
            {
            }
        }

        public int CustomerClientLogedInNum
        {
            get
            {
                return _mgrsrv != null ? _mgrsrv.CustomerLoggedInNum : 0;
            }
            set
            {
            }
        }

        public int AccountClientNum
        {
            get
            {
                return _tradingsrv!=null ? _tradingsrv.ClientNum:0;
            }
            set
            {
            }
        }

        public int AccountClientLogedInNum
        {
            get
            {
                return _tradingsrv!=null ? _tradingsrv.ClientLoggedInNum:0;
            }
            set
            {
            }
        }

        public int CacheOrder
        {
            get
            {
                return _clearcentre != null ? _clearcentre.SqlLog.OrderInCache : 0;
            }
            set
            {
            }
        }

        public int CacheCancel
        {
            get
            {
                return _clearcentre != null ? _clearcentre.SqlLog.CancleInCache : 0;
            }
            set
            {
            }
        }

        public int CacheTrade
        {
            get
            {
                return _clearcentre != null ? _clearcentre.SqlLog.TradeInCache : 0;
            }
            set
            {
            }
        }

        public int CachePosTrans
        {
            get
            {
                return _clearcentre != null ? _clearcentre.SqlLog.PosTransInCache : 0;
            }
            set
            {
            }
        }

        public int CacheOrderUpdate
        {
            get
            {
                return _clearcentre != null ? _clearcentre.SqlLog.OrderUpdateInCache : 0;
            }
            set
            { 
            }
        }

        public double BandWidthUsage { get; set; }//网络带宽使用量
        public int MysqlConnection { get; set; }//数据库连接个数
        public int MysqlQueryNum { get; set; }//数据库平均查询数量
    }**/
}
