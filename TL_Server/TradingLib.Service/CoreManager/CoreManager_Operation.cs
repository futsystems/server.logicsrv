using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.ServiceManager
{
    public partial class CoreManager
    {

        bool validmode(string modelname,object obj)
        { 
            if(obj==null)
            {
                //fmMessage.Show("没有" + modelname + "模块,无法执行该操作!");
                return false;
            }
            return true;
        }

        #region 对外公开操作

        public void OpenClearCentre()
        {
            _clearCentre.OpenClearCentre();
        }
        #region 盘中参数调整

        //是否输出全局日志
        public bool SaveDebug { get;set;} //{ return _reportcentre.SaveDebug; } set { _reportcentre.SaveDebug = value; } }

        #endregion

        ////[CLICommandAttr("startl","start tradingserver",QSEnumCLIGroup.Other,"start tradingserver,system will start transport service",false)]
        //public void StartTL()
        //{
        //    new Thread(_messageExchagne.Start).Start();
        //}

        ////[CLICommandAttr("stoptl","stop tradingserver",QSEnumCLIGroup.Other,"stop tradingserver,system will stop transport service",false)]
        //public void StopTL()
        //{
        //    new Thread(_messageExchagne.Stop).Start();
        //}
        /// <summary>
        /// 数据路由连接成功后,将注册所有客户端订阅的合约组
        /// </summary>
        //public void RegisterSymbols()
        //{
        //    _srv.RegisterSymbols();
        //
        //}
        /// <summary>
        /// 开启清算中心
        /// </summary>
        ////[CLICommandAttr("opencc","open clearcentre",QSEnumCLIGroup.ClearCentre,"open cc, accept trading request from client",false)]
        //public void OpenClearCentre()
        //{
        //    debug("Try to OpenClearCentre....");
        //    _clearCentre.OpenClearCentre();
        //}
        ///// <summary>
        ///// 关闭清算中心
        ///// </summary>
        ////[CLICommandAttr("closecc","close clearcentre",QSEnumCLIGroup.ClearCentre,"close cc, drop trading request from client",false)]
        //public void CloseClearCentre()
        //{
        //    _clearCentre.CloseClearCentre();
        //}

        //public void RollLossAccount(DateTime day)
        //{
        //    _clearCentre.RollLossAccount(day);
        //}


        #region 信息显示

        


        //public void DisplayOrder(long ordid, out string msg)
        //{
        //    _clearCentre.DisplayOrder(ordid, out msg);
        //}

        //public void DisplayPosition(out string msg)
        //{
        //   // _clearCentre.DisplayPosition(out msg);
        //}

        //public void DisplayState(out string msg)
        //{
        //    _clearCentre.DisplayState(out msg);
        //}

        #endregion


        /// <summary>
        /// 调整日期后 回复当日历史交易数据
        /// </summary>
        public void RestoreHist()
        {
            //_clearCentre.RestoreHist();
        }
        /// <summary>
        /// 恢复报名数据
        /// </summary>
        public void RestoreSignup()
        {
            try
            {
                //实例化一个文件流--->与写入文件相关联  
                FileStream fs = new FileStream(@"signup.txt", FileMode.Open);
                //实例化一个StreamWriter-->与fs相关联  
                StreamReader sw = new StreamReader(fs);
                string msg = string.Empty;

                while (sw.Peek() > 0)
                {
                    string str = sw.ReadLine();
                    //_racecentre.Signup(_clearCentre[str], out msg);
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                debug(":error reading singup data" + ex.ToString());

            }
        }

        #region 获得相关对象数据
        /// <summary>
        /// 获得Account账户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IAccount GetAccount(string account)
        {
            return _clearCentre[account];
        }

        /*
        /// <summary>
        /// 获得某个帐户的配资服务
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public IFinService GetFinService(string account)
        {
            if (_fincentre != null)
                return _fincentre[account];
            return null;
        }
         * **/
        /// <summary>
        /// 获得client tracker info
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ClientTrackerInfo GetClientTracker(string account)
        {
            return _riskCentre.GetClientTracker(account);
        }
        /*
        public Security GetMasterSecurity(string symbol)
        {
            return _clearCentre.getMasterSecurity(symbol);
        }**/

        #endregion


        #region 发送委托 或取消委托

        public void SendOrder(Order o)
        {
            //_clearCentre.SendOrder(o);

        }

        public void CancelOrder(long ordid)
        {
            //_clearCentre.CancelOrder(ordid);
        }

        #endregion

        #region 平仓操作
        ///// <summary>
        ///// 全平商品委托
        ///// </summary>
        ////[CLICommandAttr("cancelcom","cancel all comunity ordres",QSEnumCLIGroup.ClearCentre,"cancel all community orders",false)]
        //public void Clear_Community()
        //{
        //    new Thread(_riskCentre.Task_ClearOrders_Community).Start();
        //}
        ///// <summary>
        ///// 全撤商品持仓
        ///// </summary>
        //public void Flat_Community()
        //{
        //    new Thread(_riskCentre.Task_FlatPosition_Community).Start();
        //}
        ///// <summary>
        ///// 全撤股指委托
        ///// </summary>
        //public void Clear_IF()
        //{
        //    new Thread(_riskCentre.Task_ClearOrders_IF).Start();
        //}
        ///// <summary>
        ///// 全平股指仓位
        ///// </summary>
        //public void Flat_IF()
        //{
        //    new Thread(_riskCentre.Task_FlatPosition_IF).Start();
        //}
        ///// <summary>
        ///// 平掉模拟的未平日内持仓
        ///// </summary>
        ////public void Flat_Residue()
        ////{
        ////    _clearCentre.Task_FlatResidue();
        ////}
        #endregion


        #region 比赛
        /*
        /// <summary>
        /// 新开比赛
        /// </summary>
        [CLICommandAttr("newrace","newrace - Open new prerace",QSEnumCLIGroup.Race,"Prerace rolled from time to time,when one prerace's sigup time finished,we need to open an new preprace for signup to client")]
        public void OpenNewRace()
        {
            _racecentre.OpenNewPrerace();
        }
        /// <summary>
        /// 比赛考核
        /// </summary>
        [CLICommandAttr("checkrace","checkrace - Check race",QSEnumCLIGroup.Race,"The checkrace command will caculate accounts in race and check if prompted or elimiated,also change the race status and risk rule of account")]
        public void CheckRace()
        {
            new Thread(_racecentre.Task_CheckRace).Start();
        }

        /// <summary>
        /// 晋级账户
        /// </summary>
        /// <param name="account"></param>
        [CLICommandAttr("promptac","promptac [account] - Prompt account to next race level",QSEnumCLIGroup.Race,"The promptac command will prompte the account into next race level,such as from prerace to simrace,from simrace to leve1 and so on")]
        public void PromptAccount(string account)
        {
            debug("prompte account is running");
            try
            {
                _racecentre.PromptAccount(_clearCentre[account]);
            }
            catch (Exception ex)
            {
                debug("PromptAccount Error");
            }
        }

        /// <summary>
        /// 复赛帐户重置资金与风控规则
        /// </summary>
        /// <param name="account"></param>
        [CLICommandAttr("resetacc","resetacc [account] - Reset account to initial state",QSEnumCLIGroup.Race,"The resetacc command will reset equity of account,and set the default risk rule for his race level")]
        public void ResetSemiRaceAccount(string account)
        {
            _clearCentre.ResetEquity(account, TradingLib.Core.CoreGlobal.DefaultSimAmmount);
            _riskCentre.OnAccountEntryRace(_clearCentre[account], QSEnumRaceType.SEMIRACE);
        }
        /// <summary>
        /// 淘汰账户
        /// </summary>
        /// <param name="account"></param>
        [CLICommandAttr("eliminateacc","eliminateacc [account]- Eliminate account ",QSEnumCLIGroup.Race,"The eliminateacc command will eliminate account to eliminate status")]
        public void EliminateAccount(string account)
        {
            _racecentre.EliminateAccount(_clearCentre[account]);
        }
        /// <summary>
        /// 生成比赛统计信息,用于生成对应的排名数据
        /// </summary>
        [CLICommandAttr("genracestat","genracestat - Generate Race Statistic",QSEnumCLIGroup.Race,"The genracestat command will caculate the race statistic for accounts are in race,such win percent,avg win,avg loss and so on")]
        public void GenRaceStatistic()
        {

            new Thread(_racecentre.Task_GenRaceStatistic).Start();
        }
        /// <summary>
        /// 清空比赛统计表
        /// </summary>
        [CLICommandAttr("clearracestat","clearracestat - Clear race statistic",QSEnumCLIGroup.Race,"The clearracestat command will clear the race statistic in database")]
        public void ClearRaceStatistic()
        {
            _racecentre.Task_ClearRaceStatistic();
        }
         * **/
        #endregion

        #region 服务器信息输出
        //[CLICommandAttr("print", "print[type] - Print status of object", QSEnumCLIGroup.Other, "The print command will print the status information for some SrvObject")]
        public string Print(string type)
        {

            return "";
        }

        /// <summary>
        /// 输出线程列表
        /// </summary>
        /// <returns></returns>
        string Print_Thread()
        {

            return "";
        }
        /// <summary>
        /// 输出服务端对象列表
        /// </summary>
        /// <returns></returns>
        string Print_SrvObject()
        {

            return "";
        }

        /// <summary>
        /// 输出任务列表
        /// </summary>
        /// <returns></returns>
        //[CLICommandAttr("ptask","ptask - Print Task Lists",QSEnumCLIGroup.Other,"The ptask command will print the task hold in taskcentre")]
        public string PrintTask()
        {
            return "";// _taskcentre.PrintTasks();
        }
        //[CLICommandAttr("pthread", "pthread - Print Thread Lists", QSEnumCLIGroup.Other, "The pthread command will print the thread running in system")]
        public string PrintThreads()
        {
            return ThreadTracker.PrintThreads();
        }

        //[CLICommandAttr("psrvobj","psrvobj - Print SrvObject List",QSEnumCLIGroup.Other,"The psrvobj command will print all srvobject in server")]
        public string PrintObjects()
        {
            //return BaseSrvObject.PrintObjects();
            return "";
        }

        public event StringDelegate GetConnectorStatusEvent;
        //[CLICommandAttr("pconnector", "pconnector - Print Connector List", QSEnumCLIGroup.Other, "The pconnector command will connector's status in system")]
        public string PrintConnectors()
        {
            if (GetConnectorStatusEvent != null)
                return GetConnectorStatusEvent();
            return "Not Avabile Now";
        }
        #endregion

        #region 数据与交易路由

        public event FindBrokerDel FindBrokerEvent;
        public event FindDataFeedDel FindDataFeedEvent;
        /// <summary>
        /// 启动成交路由
        /// </summary>
        [ContribCommandAttr(QSEnumCommandSource.CLI,"startbroker","startbroker - 启动某个成交通道","用于命令行启动某个交易通道")]
        public void StartBroker()
        {
            try
            {
                string fullname = "Broker.SIM.SIMTrader";
                debug("启动数据通道[SIMTrader]", QSEnumDebugLevel.INFO);
                if (FindBrokerEvent != null)
                {
                    IBroker b = FindBrokerEvent(fullname);
                    if (b != null && !b.IsLive)
                    {
                        b.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                debug("start broker error:" + ex.ToString());
            }
            
        }

        
        
        /// <summary>
        /// 停止成交路由
        /// </summary>
        //[CLICommandAttr("stopbroker", "stopbroker - Stop Broker", QSEnumCLIGroup.Other, "The stopbroker command will stop one broker, and make it unworkable",true)]
        
        public void StopBroker()
        {
            try
            {
                string fullname = "Broker.SIM.SIMTrader";
                if (FindBrokerEvent != null)
                {
                    IBroker b = FindBrokerEvent(fullname);
                    if (b != null && b.IsLive)
                    {
                        b.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                debug("stop broker error:" + ex.ToString());
            }

        }

        /// <summary>
        /// 启动数据路由
        /// </summary>
        //[CLICommandAttr("startdata", "startdata - Start DataFeed", QSEnumCLIGroup.Other, "The stopbroker command will stop one broker, and make it unworkable", true)]
        [ContribCommandAttr(QSEnumCommandSource.CLI, "startdatafeed", "startdatafeed - 启动某个数据通道", "用于命令行启动某个数据通道")]
        public void StartDataFeed()
        {
            debug("启动数据通道[FastTick]", QSEnumDebugLevel.INFO);
            try
            {
                string fullname = "DataFeed.FastTick.FastTick";
                if (FindDataFeedEvent != null)
                {
                    //debug("xxxxxxxxxxxxxxxxx", QSEnumDebugLevel.INFO);
                    IDataFeed d = FindDataFeedEvent(fullname);
                    if (d != null && !d.IsLive)
                    {
                        d.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                debug("start datafeed error:" + ex.ToString());
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.CLI, "startdatafeedsim", "startdatafeedsim - 启动模拟数据通道", "用于命令行启动模拟数据通道")]
        public void StartDataFeedSim()
        {
            debug("启动数据通道[SimTick]", QSEnumDebugLevel.INFO);
            try
            {
                string fullname = "DataFeed.SimTick.SimTick";
                if (FindDataFeedEvent != null)
                {
                    //debug("xxxxxxxxxxxxxxxxx", QSEnumDebugLevel.INFO);
                    IDataFeed d = FindDataFeedEvent(fullname);
                    if (d != null && !d.IsLive)
                    {
                        d.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                debug("start datafeed error:" + ex.ToString());
            }
        }


        /// <summary>
        /// 停止数据路由
        /// </summary>
        //[CLICommandAttr("stopdata", "stopdata - Stop DataFeed", QSEnumCLIGroup.Other, "The stopbroker command will stop one broker, and make it unworkable", true)]
        public void StopDataFeed()
        { 
        
        }
        /// <summary>
        /// 重置交易路由
        /// </summary>
        //[CLICommandAttr("resetbr", "resetbr - Reset BrokerRouter", QSEnumCLIGroup.Other, "The resetbr command will reset broker router,reset the order engine,and restart SimBroker auto", true)]
        public void ResetBrokerRouter()
        {
            _brokerRouter.Reset();
        }

        /// <summary>
        /// 重置数据路由
        /// </summary>
        //[CLICommandAttr("resetdr", "resetdr - Reset DataFeedRouter", QSEnumCLIGroup.Other, "The resetdr command will reset datafeed router,clear ticktracker,baseket registed,and restart FastTickDataFeed auto", true)]
        public void ResetDataRouter()
        {
            _datafeedRouter.Reset();
        }

        
        public void RestartCTPDataFeed()
        {
            if (_ftmgrclient == null) return;
            new Thread(delegate()
                {
                    _ftmgrclient.StopDataFeed(QSEnumDataFeedTypes.CTP);
                    Thread.Sleep(2000);
                    _ftmgrclient.StartDataFeed(QSEnumDataFeedTypes.CTP);
                }).Start();
        }
       

        /// <summary>
        /// 重启模拟交易 与 数据 connecter
        /// 重新建立数据源连接,清空当前Tick缓存
        /// 重启模拟交易连接,清空无用委托数据等
        /// </summary>
        public void RouterReset()
        {
            new Thread(Task_Ready_Reset).Start();
        }

        #endregion

        #region 清算中心事务


        //保存PositionRound数据
        /// <summary>
        /// 保存日内PositionRound信息
        /// </summary>
        public void SavePositionRound()
        {
            //new Thread(_clearCentre.SavePositionRound).Start();
        }
        /// <summary>
        /// 保存比赛选手的日内PositionRound到比赛临时表
        /// </summary>
        public void SavePR2RaceTable()
        {
            //new Thread(_clearCentre.SavePR2Race).Start();
        }



        /// <summary>
        /// 检查账户内存与数据库上日权益一致性
        /// </summary>
        public void CheckAccountLastEquity()
        {
            //new Thread(_clearCentre.CheckAccountLastEquity).Start();
        }
        /// <summary>
        /// 数据一致性检验 检验当日交易信息的完备性
        /// </summary>
        public void CheckTradingInfo()
        {
            //new Thread(_clearCentre.CheckTradingInfo).Start();

        }
        /// <summary>
        /// 保存PR数据1.将PR数据保存到log记录 2.将比赛对应的PR数据保存到对应的数据库表
        /// 该操作包含了保存posiitonround信息以及保存PR信息到比赛数据表
        /// </summary>
        public void StoreData()
        {
            //_clearCentre.Task_DataStore();
        }

        /// <summary>
        /// 清算中心结算
        /// </summary>
        public void SettleAccount()
        {
            //new Thread(_clearCentre.Task_SettleAccount).Start();
        }

        /// <summary>
        /// 结算重置,结算完毕后 将账户重置,调整昨日权益,归零 出入金 ,调整结算时间等
        /// </summary>
        public void SettleReset()
        {
            new Thread(Task_Settle_Reset).Start();
        }

        #endregion






        public void SaveReportData()
        {
            //_opreport.SaveReport();
            //_reportcentre.SaveReport();
        }





       // public DateTime LastSettleDay { get { return _clearCentre.LastSettleDay; } }
        //public bool IsTradingDay { get { return _clearCentre.IsTradeDay; } }
        //public string GetHolidayInfo()
        //{
        //    return _clearCentre.GetHolidayInfo();
        //}


        /// <summary>
        /// 加载全局合约
        /// </summary>
        //public void ApplySecuirtyUpdate()
        //{
        //    _clearCentre.LoadXMLTable();//加载交易所信息以及全局合约信息
        //    _clearCentre.OverWriteAccountSecurity();//重新加载账户合约信息


        //}
        /// <summary>
        /// 加载热门合约列表
        /// </summary>
        public void ApplyHotSymCheck()
        {
            _riskCentre.InitHotBasket();
        }

        /*
        /// <summary>
        /// 从数据库加载配资服务，加载所有账户的配资服务
        /// </summary>
        public void ReloadFinService()
        {
            _fincentre.Reset();
        }
        /// <summary>
        /// 从数据库加载某个账户的配资服务
        /// </summary>
        /// <param name="account"></param>
        public void ReloadFinServiceAccount(string account)
        {
            _fincentre.LoadFinServices(account);
        }
        /// <summary>
        /// 结算配资服务
        /// </summary>
        public void SettleFinService()
        {
            _fincentre.SettleFinServices();
        }

        public IFinStatistic GetFinStatisticTotal()
        {
            return _fincentre.GetFinStatForTotal(_clearCentre);//FinStatistic.GetFinStatForTotal(_clearCentre, _fincentre);

        }
        public IFinStatistic GetFinStatisticLive()
        {
            return _fincentre.GetFinStatForLIVE(_clearCentre);// FinStatistic.GetFinStatForLIVE(_clearCentre, _fincentre);
        }

        public IFinStatistic GetFinStatisticSim()
        {
            return _fincentre.GetFinStatForSIM(_clearCentre);// FinStatistic.GetFinStatForSIM(_clearCentre, _fincentre);
        }
        **/

        public void Test()
        {
            //new Thread(_clearCentre.TestWhereForeach).Start();
        }
        #endregion

    }
}
