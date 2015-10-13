using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{


    [CoreAttr(SettleCentre.CoreName, "结算中心", "结算中心,用于执行系统结算生成结算报表等")]
    public partial class SettleCentre : BaseSrvObject, IModuleSettleCentre
    {
        const string CoreName = "SettleCentre";
        public string CoreId { get { return PROGRAME; } }


        QSEnumSettleCentreStatus _settleStatus = QSEnumSettleCentreStatus.UNKNOWN;
        /// <summary>
        /// 结算中心状态
        /// </summary>
        public QSEnumSettleCentreStatus SettleCentreStatus { get { return _settleStatus; } set { _settleStatus = value; } }

        int _lastsettleday = 0;
        /// <summary>
        /// 上一个结算日
        /// 系统结算后会将结算日期写入system表 用于记录最近的结算日期
        /// </summary>
        public int LastSettleday { get { return _lastsettleday; } }

        int _tradingday = 0;
        /// <summary>
        /// 当前交易日
        /// 当前交易日是通过当前日期 时间 以及节假日信息综合判定 如果为0则表明当前为非交易日
        /// </summary>
        public int Tradingday { get { return _tradingday; } }

        //int _nexttradingday =0;
        ///// <summary>
        ///// 下一个交易日
        ///// 在上个结算日基础上获得下一个交易日
        ///// </summary>
        //public int NextTradingday { get { return _nexttradingday; } }


        /// <summary>
        /// 返回当前结算中心状态
        /// </summary>
        public bool IsTradingday { get { return SettleCentreStatus == QSEnumSettleCentreStatus.TRADINGDAY; } }


        /// <summary>
        /// 结算中心是否处于正常状态,
        /// 否则处于历史结算
        /// </summary>
        public bool IsNormal { get { return SettleCentreStatus != QSEnumSettleCentreStatus.HISTSETTLE; } }



        public bool IsInSettle { get; set; }

        /// <summary>
        /// 获得结算时间
        /// 如果是历史结算 则返回的结算时间是16:00
        /// 如果是正常结算 则返回系统当前时间
        /// </summary>
        public int SettleTime {

            get
            {
                return _settleTime;
            }
        }

        /// <summary>
        /// 获得重置时间
        /// </summary>
        public int ResetTime
        {
            get
            {
                return _resetTime;
            }
        }

        SettlementPriceTracker _settlementPriceTracker = new SettlementPriceTracker();

        ConfigDB _cfgdb;
        int _settleTime = 160000;

        /// <summary>
        /// 遍历所有可用交易所 获得所有交易所的结算时间 边转换成本地时间 采用最晚的一个时间为下一个结算时间
        /// </summary>
        DateTime _netxSettleTime = DateTime.Now;
        


        int _resetTime = 170000;
        bool _cleanTmp = false;
        bool _settleWithLatestPrice = false;
        bool _tradingEveryDay = false;

        public SettleCentre()
            :base(SettleCentre.CoreName)
        {
            _cfgdb = new ConfigDB(SettleCentre.CoreName);

            //初始化置结算中心状态为未知
            SettleCentreStatus = QSEnumSettleCentreStatus.UNKNOWN;
            IsInSettle = false;

            //初始化交易所结算任务
            InitSettleTask();

            //初始化交易日信息
            InitTradingDay();

            

            //注入交易记录转储任务 结算前2分钟 保存交易记录
            //DateTime storetime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TradingCalendar.SettleTime)-new TimeSpan(0,2,0);
            //TaskProc taskstore = new TaskProc(this.UUID, "系统转储-" + Util.ToTLTime(storetime).ToString(), storetime.Hour, storetime.Minute, storetime.Second, delegate() { Task_DataStore(); });
            //TLCtxHelper.ModuleTaskCentre.RegisterTask(taskstore);

            ////注入结算任务 可以在数据指定重置时间
            //DateTime settletime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TradingCalendar.SettleTime);
            //TaskProc tasksettle = new TaskProc(this.UUID, "系统结算-" + Util.ToTLTime(settletime).ToString(), settletime.Hour, settletime.Minute, settletime.Second, delegate() { Task_SettleAccount(); });
            //TLCtxHelper.ModuleTaskCentre.RegisterTask(tasksettle);

            ////注入重置任务 可以在数据指定重置时间
            //DateTime resettime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), _resetTime);
            //TaskProc taskreset = new TaskProc(this.UUID, "系统重置-" + Util.ToTLTime(resettime).ToString(), resettime.Hour, resettime.Minute, resettime.Second, delegate() { Task_ResetTradingday(); });
            //TLCtxHelper.ModuleTaskCentre.RegisterTask(taskreset);

            //注入关闭清算中心任务
            DateTime closecctime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), _settleTime).AddMinutes(-5);
            TaskProc taskclosecc = new TaskProc(this.UUID, "关闭清算中心" + Util.ToTLTime(closecctime).ToString(), closecctime.Hour, closecctime.Minute, closecctime.Second, delegate() { Task_CloseClearCentre(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(taskclosecc);

            //注入开启清算中心任务
            DateTime opencctime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), _resetTime).AddMinutes(5);
            TaskProc taskopencc = new TaskProc(this.UUID, "开启清算中心" + Util.ToTLTime(opencctime).ToString(), opencctime.Hour, opencctime.Minute, opencctime.Second, delegate() { Task_OpenClearCentre(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(taskopencc);

            
        }

        /// <summary>
        /// 设定当前结算中心日期
        /// 用于回溯到某个交易日
        /// 根据给定的日期 推算到上一个结算日 然后根据上一个结算日推算出下一个结算日
        /// 通过当前日期与下一个交易日是否一致来判定当前日期是否是交易日。
        /// 这里假定系统回溯到交易日结算前 与 实时运行时需要通过时间来判定 处于哪个交易日 逻辑有所不同
        /// </summary>
        /// <param name="tradingday"></param>
        //void SetCurrentDay(int day)
        //{
        //    //计算出某日的上一个交易日
        //    _lastsettleday = TradingCalendar.LastTradingDay(day);

        //    //根据上个交易日计算出下一个交易日
        //    _nexttradingday = TradingCalendar.NextTradingDay(_lastsettleday);

        //    //如果下一个交易日就是设定的当日，则当前交易日就是该日期，否则当前交易日为0 标识非交易日
        //    _tradingday = _nexttradingday == day ? _nexttradingday : 0;

        //    //标记当前状态为历史结算
        //    _settleStatus = QSEnumSettleCentreStatus.HISTSETTLE;
        //    ////设定结算中心状态
        //    //if (CurrentTradingday == 0)
        //    //{
        //    //    SettleCentreStatus = QSEnumSettleCentreStatus.NOTRADINGDAY;//如果没有当前交易日信息则为非交易日状态
        //    //}
        //    //else
        //    //{
        //    //    SettleCentreStatus = QSEnumSettleCentreStatus.TRADINGDAY;//如果获得了当前交易日则当前为可交易日状态
        //    //}

        //    logger.Info(string.Format("设定结算日期信息,上一交易日:{0} 当前交易日:{1} 下一交易日：{2}", _lastsettleday, _tradingday, _nexttradingday));

        //}

        /// <summary>
        /// 初始化交易日信息
        /// </summary>
        void InitTradingDay()
        {
            logger.Info(string.Format("System running mode:{0} trading everyday:{1}", GlobalConfig.IsDevelop ? "develop" : "production", _tradingEveryDay));
            //从数据库获得上次结算日
            _lastsettleday = ORM.MSettlement.GetLastSettleday();
            //取最早结算交易所对应的当前日期为 系统当前结算日

            //如果上个结算日大于等于当前交易日 则交易日设置异常
            if (_lastsettleday >= _tradingday)
            {
                logger.Error("结算日设置不正确,lastsettleday 大于 最早结算交易所对应日期");
                throw new ArgumentException("invlaid settleday");
            }

            logger.Warn(string.Format("SettleCentre lastsettleday:{0} tradingday:{1} next settletime:{2}", _lastsettleday, _tradingday, _netxSettleTime.ToString("yyyyMMdd HH:mm:ss")));
            SettleCentreStatus = QSEnumSettleCentreStatus.TRADINGDAY;//如果获得了当前交易日则当前为可交易日状态

        }

        /// <summary>
        /// 重置结算信息
        /// 按照系统记录的上个结算日 当前日期 时间来获得对应的当前交易日和结算状态信息
        /// </summary>
        public void Reset()
        {
            InitTradingDay();
            //logger.Info(string.Format("结算中心初始化,上次结算日:{0} 下一交易日:{1} 当前交易日:{2} 结算状态:{3}", _lastsettleday, _nexttradingday, _tradingday, SettleCentreStatus));
        }

        public void Start()
        {
            Util.StartStatus(this.PROGRAME);
        }

        public void Stop()
        {
            Util.StopStatus(this.PROGRAME);
        }


        public override void Dispose()
        {
            Util.DestoryStatus(this.PROGRAME);
            base.Dispose();
        }

        /// <summary>
        /// 结算所有交易账户
        /// 结算分析
        /// 1.单日lastequity + realizedpl + unrealizedpl - commission + cashin - cashout = now equity 数据库检验通过
        /// 2.当日结算完毕后的nowequity即为账户表中的lastequity 数据库检验通过
        /// 3.产生错误就是在某些结算记录中上日权益不等于该账户的nowequity
        /// 4.账户结算时需要检查 账户的上日权益是否是数据库记录的上日权益
        /// </summary>
        //public void SettleAccount()
        //{
        //    logger.Info(string.Format("#####SettleAccount: Start Settele Account,Current Tradingday:{0}", CurrentTradingday));
        //    foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
        //    {
        //        try
        //        {
        //            ORM.MSettlement.SettleAccount(acc);
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.Error(string.Format("SettleError,Account:{0} errors:{1}", acc.ID, ex.ToString()));
        //        }
        //    }

        //    //更新最近结算日
        //    logger.Info(string.Format("Update lastsettleday as:{0}", CurrentTradingday));
        //    ORM.MSettlement.UpdateSettleday(CurrentTradingday);
        //    logger.Info("Settlement Done");
        //}


    }
}
