using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Core;
using ZeroMQ;


namespace TradingLib.Contrib
{
    /*
    /// <summary>
    /// 报告中心,所有的报告从这里获得
    /// 这里的报告是静态的,其他组件有实时发送报告,或者定时保存报告的均从这里获得对应的报告数据
    /// 所有组件加载完毕后,再生成report组件,同时将其他组件的报告回调函数绑定到report
    /// </summary>
    public class ReportCentre : BaseSrvObject, IInfoReport
    {
        ClearCentreSrv _clearcentre;//清算中心
        MgrServer _mgrsrv;//管理服务
        TradingServer _tradingsrv;//交易服务
        IFinServiceCentre _fincentre;//配资服务中心

        //public event HealthParamDel SendHealthEvent;
        //public event WebStatisticParamDel SendWebStatisticEvent;
        ConnectionPoll<mysqlDBReport> conn;



        public ReportCentre(TradingServer t, ClearCentreSrv c, MgrServer m, IFinServiceCentre f, string _server, string _user, string _pass)
            : base("Report")
        {

            _tradingsrv = t;
            _clearcentre = c;
            _mgrsrv = m;
            _fincentre = f;

            conn = new ConnectionPoll<mysqlDBReport>(_server, _user, _pass, CoreGlobal.DBName, CoreGlobal.DBPort);

            //注册定时任务
            TaskCentre.RegisterTask(new TaskProc("报告中心采集信息", new TimeSpan(0, 0, 5), Task_CollectReportInfo));
            //TaskCentre.RegisterTask(new TaskProc("报告中心处理日志", new TimeSpan(0, 0, 1), Task_DealLog));
        }

        public bool SaveDebug { get; set; }
        #region 相关统计数据
        
        /// <summary>
        /// web网页需要的统计信息
        /// </summary>
        public IWebStatistic WebStatistic
        {
            get 
            {
                return WebStatisticReport.GetReport(_clearcentre);
            }
            
        }
        /// <summary>
        /// 本地服务器核心组件统计信息
        /// </summary>
        public IHealthInfo HealthInfo
        {
            get 
            {
                return CoreHealthReport.GetReport(_clearcentre, _tradingsrv, _mgrsrv);
            }
        }
        
        #endregion

        #region 配资统计
        /// <summary>
        /// 配资累加统计
        /// </summary>
        public IFinStatistic FinStaTotal
        {
            get
            {
                if (_fincentre == null) return null;
                return _fincentre.GetFinStatForTotal(_clearcentre);
            }
        }
        /// <summary>
        /// 配资实盘统计
        /// </summary>
        public IFinStatistic FinStaLive
        {
            get
            {
                if (_fincentre == null) return null;
                return _fincentre.GetFinStatForLIVE(_clearcentre);
            }
        }

        /// <summary>
        /// 配资模拟统计
        /// </summary>
        public IFinStatistic FinStaSim
        {
            get
            {
                if (_fincentre == null) return null;
                return _fincentre.GetFinStatForSIM(_clearcentre);
            }
        }

        public IFinSummary FinSummarySta
        {
            get
            {
                if (_fincentre == null) return null;
                return _fincentre.GetFinSummary(_clearcentre);// _fincentre.getf FinSummary.GetFinSummary(_clearcentre, _fincentre);
            }
        }

        #endregion


        #region 数据库统计信息

        public DailySummaryList  GenDailySummaryList(string account,DateTime start)
        {
            return GenDailySummaryList(account,start,DateTime.Now);
        }
        public DailySummaryList  GenDailySummaryList(string account, DateTime start, DateTime end)
        {
            DailySummaryList dl= new DailySummaryList();
            mysqlDBReport db = conn.mysqlDB;
            DataSet ds = db.ReTotalDaily(account,start,end);
            conn.Return(db);

            DataTable dt = ds.Tables["settlement"];
            List<long> clist = new List<long>();
            for(int i = 0; i < dt.Rows.Count; i++)
            {
                DailySummary d = new DailySummary();
                DataRow dr = dt.Rows[i];
                d.Account = Convert.ToString(dr["account"]);
                d.DateTime = Convert.ToDateTime(dr["settleday"]);
                d.RealizedPL = Convert.ToDecimal(dr["realizedpl"]);
                d.UnRealizedPL = Convert.ToDecimal(dr["unrealizedpl"]);
                d.Commission = Convert.ToDecimal(dr["commission"]);

                dl.Add(d);
            }
            return dl;
        }

        public WLSideDIS GenWLSideDIS(string account, DateTime start, DateTime end)
        {
            WLSideDIS temp = new WLSideDIS();
            mysqlDBReport db = conn.mysqlDB;
            DataSet ds = db.ReSide_WL(account, start, end);
            conn.Return(db);

            DataTable dt = ds.Tables["viewprofit"];
            for (int i = 0; i < dt.Rows.Count; i++)
            { 
                DataRow dr = dt.Rows[i];
                int side = Convert.ToInt16(dr["side"]);
                int wl = Convert.ToInt16(dr["wl"]);
                decimal profit = Convert.ToDecimal(dr["profit"]);
                decimal commission = Convert.ToDecimal(dr["commission"]);
                decimal netprofit = Convert.ToDecimal(dr["netprofit"]);
            }

            return temp;
        }

        #endregion



        #region 保存相关报表信息
        /// <summary>
        /// 将当日的运营数据写入数据库表,方便形成统计报表进行分析
        /// 当天是交易日才进行保存,非交易日不保存
        /// </summary>
        public void SaveReport()
        {
            try
            {
               

                debug(PROGRAME + ":报表记录中心", QSEnumDebugLevel.INFO);
                //debug("配资费收入:" + FinSummarySta.SumFinFee.ToString() + " 手续费收入:" + FinSummarySta.SumCommissionIn.ToString() + " 手续费支出:" + Commission_out.ToString() + " 入金:" + Deposit.ToString() + " 出金:" + Withdraw.ToString(), QSEnumDebugLevel.INFO);
                mysqlDBReport db = conn.mysqlDB;
                bool re = db.SaveOPReport(DateTime.Now, FinSummarySta.SumFinFee, FinSummarySta.SumCommissionIn, 0, FinSummarySta.SumDeposit, FinSummarySta.SumWithdraw, FinSummarySta.SumFinAmmount,FinSummarySta.SumFinAmmountIntereset, FinSummarySta.SumFinAmmountBonus,this.collect_margin);
                conn.Return(db);
                debug("sql exuction resoult:" + re.ToString(), QSEnumDebugLevel.INFO);
            }
            catch (Exception ex)
            {
                debug("保存运营信息出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }


        //可以考虑建立环形缓冲来记录数据
        public void SaveClientSession(ClientTrackerInfo info, bool login_flag)
        {
            try
            {
                mysqlDBReport db = conn.mysqlDB;
                db.SaveSession(info.Account, info.IPAddress, info.HardWareCode, info.FrontID, login_flag ? "in" : "out", info.API_Type, info.API_Version);
                conn.Return(db);
            }
            catch (Exception ex)
            {
                debug("保存客户端登入注销日志出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }

        }

        #endregion


        #region 固定保证金数据

        public DataTable GetAgentPRSummary(DateTime start, DateTime end)
        {
            mysqlDBReport db = conn.mysqlDB;
            DataTable tb = db.GetAgentPRSummary(start, end);
            conn.Return(db);
            return tb;
        }
        public DataTable GetAgentPRDetails(string agentcode, DateTime start, DateTime end)
        {
            mysqlDBReport db = conn.mysqlDB;
            DataTable tb = db.GetAgentPRDetails(agentcode, start, end);
            conn.Return(db);
            return tb;
        }

        #endregion


        #region 相关历史数据记录
        public DataTable GetSettlement(string account, DateTime start, DateTime end)
        {
            mysqlDBReport db = conn.mysqlDB;
            DataTable tb = db.GetSettelment(account, start, end);
            conn.Return(db);
            return tb;
        }

        public DataTable GetOrders(string account, DateTime start, DateTime end)
        {
            mysqlDBReport db = conn.mysqlDB;
            DataTable tb = db.GetOrders(account, start, end);
            conn.Return(db);
            return tb;
        }
        public DataTable GetTrades(string account, DateTime start, DateTime end)
        {
            mysqlDBReport db = conn.mysqlDB;
            DataTable tb = db.GetTrades(account, start, end);
            conn.Return(db);
            return tb;
        }

        public DataTable GetCash(string account, DateTime start, DateTime end)
        {
            mysqlDBReport db = conn.mysqlDB;
            DataTable tb = db.GetCash(account, start, end);
            conn.Return(db);
            return tb;
        }


        #endregion


        DateTime collectime = DateTime.Now;
        decimal collect_margin = 0;
        int collect_numlogined = 0;

        public void Reset()
        {
            collect_margin = 0;
            collect_numlogined = 0;
        }


        //记录当天的最高值
        void CollectValue()
        {
            //每隔30秒采集一次日内最高数据 并记录
            if (_fincentre == null) return;
            if ((DateTime.Now - collectime).TotalSeconds > 30)
            {
                collectime = DateTime.Now;

                decimal m = this.FinSummarySta.SumMarginUsed;
                collect_margin = collect_margin > m ? collect_margin : m;

                int l = 0;// this.HealthInfo.AccountClientLogedInNum;
                collect_numlogined = collect_numlogined > l ? collect_numlogined : l;
            }
        }

        void Task_CollectReportInfo()
        {
            try
            {
                
                //对外发布服务器状态信息以及网站需要的实时数据
                if (SendHealthEvent != null)
                    SendHealthEvent(this.HealthInfo);

                if (SendWebStatisticEvent != null)
                    SendWebStatisticEvent(this.WebStatistic);
                
                //定时采集数据
                CollectValue();
            }
            catch (Exception ex)
            {
                debug("报告中心循环出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        

        
        public void Start()
        {
            if (_rego) return;
            _rego = true;
            _rethread = new Thread(reproc);
            _rethread.IsBackground = true;
            _rethread.Name = "Report Collect Thread";
            _rethread.Start();
            ThreadTracker.Register(_rethread);
        }

        public void Stop()
        {
            if (!_rego) return;
            _rego = false;
            _rethread.Abort();
            _rethread = null;
        }

        Thread _rethread;
        bool _rego;

        void reproc()
        {
            while (_rego)
            {
                try
                {
                    //对外发布服务器状态信息以及网站需要的实时数据
                    if (SendHealthEvent != null)
                        SendHealthEvent(this.HealthInfo);

                    if (SendWebStatisticEvent != null)
                        SendWebStatisticEvent(this.WebStatistic);

                    //定时采集数据
                    CollectValue();

                    //定时处理日志缓存的记录
                    DealLog();

                    Thread.Sleep(2000);//每隔一个时间间隔 采集相关信息
                }
                catch (Exception ex)
                {
                    debug("循环出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
                finally
                {
                    Thread.Sleep(2000);//每隔一个时间间隔 采集相关信息
                }
                
            }
        }
        
         
    }**/

    /*
    struct LogItem
    {
        public QSEnumDebugLevel DebugLevel;
        public string Message;
        public string ObjName;
        public LogItem(string objname, string msg, QSEnumDebugLevel level)
        {
            ObjName = objname;
            DebugLevel = level;
            Message = msg;
        }
    }**/
}
