using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using System.Diagnostics;

namespace TradingLib.Core
{
    internal class HealthReport
    {

        ClearCentre _clearcentre;
        RiskCentre _riskcentre;
        MsgExchServer _srv;

        PerformanceCounter pc1 = new PerformanceCounter();
        PerformanceCounter pc2 = new PerformanceCounter();
        PerformanceCounter pc3 = new PerformanceCounter();
        Process srvprocess;
        Process[] mysqlprocess = Process.GetProcessesByName("mysqld");

        public HealthReport(MsgExchServer s, ClearCentre c, RiskCentre r)
        {
            _srv = s;
            _clearcentre = c;
            _riskcentre = r;

            srvprocess = Process.GetCurrentProcess();

            pc2.CategoryName = "Processor";
            pc2.CounterName = "% Processor Time";
            pc2.InstanceName = "_Total";
            pc2.MachineName = ".";

            pc1.CategoryName = "Process";
            pc1.CounterName = "% Processor Time";
            pc1.InstanceName = srvprocess.ProcessName;
            pc1.MachineName = ".";

            pc3.CategoryName = "Process";
            pc3.CounterName = "% Processor Time";
            pc3.InstanceName = "mysqld";
            pc3.MachineName = ".";

        }

        //缓存中的交易记录
        public int CacheOrderNum { get { return _clearcentre.SqlLog.OrderInCache; } }
        public int CacheCancelNum { get { return _clearcentre.SqlLog.CancleInCache; } }
        public int CacheTradeNum { get { return _clearcentre.SqlLog.TradeInCache; } }
        public int CachePosTransNum { get { return _clearcentre.SqlLog.PosTransInCache; } }
        public int CacheOrderUpdateNum { get { return _clearcentre.SqlLog.OrderUpdateInCache; } }


        //交易客户端
        public int ClientNum { get { return _srv.ClientNum; } }
        public int ClientLoginNum { get { return _srv.ClientLoggedInNum; } }


        public double CPUTotal { get { return Math.Round(pc2.NextValue(), 2); } }
        public double CPUPlatform { get { return Math.Round(pc1.NextValue(), 2); } }
        public double CPUMySql { get { return Math.Round(pc3.NextValue(), 2); } }

        public double MemoryPlatform { get { return Convert.ToInt64(srvprocess.WorkingSet64.ToString()) / 1024; } }
        public double MemoryMySql { get { return Convert.ToInt64(mysqlprocess[0].WorkingSet64.ToString()) / 1024; } }

    }
}
