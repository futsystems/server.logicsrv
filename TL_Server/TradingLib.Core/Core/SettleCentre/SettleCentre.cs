using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public enum QSEnumSettleCentreStatus
    { 
        /// <summary>
        /// 未初始化
        /// </summary>
        UNKNOWN,//未知

        /// <summary>
        /// 结算信息不完整 需要在对应日期上加载历史记录并做结算
        /// </summary>
        HISTSETTLE,//历史结算

        /// <summary>
        /// 当前是交易日 可以进行正常的交易与结算
        /// </summary>
        TRADINGDAY,//交易日

        /// <summary>
        /// 当前非交易日
        /// </summary>
        NOTRADINGDAY,//非交易日
    }

    [CoreAttr(SettleCentre.CoreName, "结算中心", "结算中心,用于执行系统结算生成结算报表等")]
    public partial class SettleCentre : BaseSrvObject, ISettleCentre,ICore
    {
        const string CoreName = "SettleCentre";
        public string CoreId { get { return PROGRAME; } }


        /// <summary>
        /// 结算中心状态
        /// </summary>
        public QSEnumSettleCentreStatus SettleCentreStatus { get; set; }



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
        public int CurrentTradingday { get { return _tradingday; } }

        int _nexttradingday =0;
        /// <summary>
        /// 下一个交易日
        /// 在上个结算日基础上获得下一个交易日
        /// </summary>
        public int NextTradingday { get { return _nexttradingday; } }


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
                if (!IsNormal)
                {
                    return TradingCalendar.SettleTime;
                }
                else
                {
                    return Util.ToTLTime(DateTime.Now);
                }
            }
        }

        ClearCentre _clearcentre = null;
        public void BindClearCentre(ClearCentre cc)
        {
            _clearcentre = cc;
        }

        RiskCentre _riskcentre = null;
        public void BindRiskCentre(RiskCentre rc)
        {
            _riskcentre = rc;
        }

        MsgExchServer _exchsrv = null;
        public void BindExchSrv(MsgExchServer srv)
        {
            _exchsrv = srv;
        }


        ConfigDB _cfgdb;
        int resetTime = 40101;
        bool _cleanTmp = false;
        bool _settleWithLatestPrice = false;
        public SettleCentre()
            :base(SettleCentre.CoreName)
        {
            _cfgdb = new ConfigDB(SettleCentre.CoreName);

            //初始化置结算中心状态为未知
            SettleCentreStatus = QSEnumSettleCentreStatus.UNKNOWN;

            //初始化交易日信息
            InitTradingDay();

            if (!_cfgdb.HaveConfig("SettleTime"))
            {
                _cfgdb.UpdateConfig("SettleTime", QSEnumCfgType.Int, 160000, "执行结算,将当日交易与出入金记录进行结转");
            }
            TradingCalendar.SettleTime = _cfgdb["SettleTime"].AsInt();

            //重置时间
            if (!_cfgdb.HaveConfig("ResetTime"))
            {
                _cfgdb.UpdateConfig("ResetTime", QSEnumCfgType.Int, 170101, "执行重置任务清空日内数据 系统帐户归位");
            }
            resetTime = _cfgdb["ResetTime"].AsInt();

            //结算价
            if (!_cfgdb.HaveConfig("SettleWithLatestPrice"))
            {
                _cfgdb.UpdateConfig("SettleWithLatestPrice", QSEnumCfgType.Bool,false, "是否已最新价来结算持仓盯市盈亏");
            }
            resetTime = _cfgdb["SettleWithLatestPrice"].AsInt();

            //是否清空日内临时表
            if (!_cfgdb.HaveConfig("CleanTmpTable"))
            {
                _cfgdb.UpdateConfig("CleanTmpTable", QSEnumCfgType.Bool,false, "结算后重置系统是否情况日内临时表");
            }
            _settleWithLatestPrice = _cfgdb["CleanTmpTable"].AsBool();

            //注入交易记录转储任务 结算前5分钟 保存交易记录
            DateTime storetime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TradingCalendar.SettleTime)-new TimeSpan(0,5,0);
            TaskProc taskstore = new TaskProc(this.UUID, "交易系统转储交易记录-" + resetTime.ToString(), storetime.Hour, storetime.Minute, storetime.Second, delegate() { Task_DataStore(); });
            TLCtxHelper.Ctx.InjectTask(taskstore);

            //注入结算任务 可以在数据指定重置时间
            DateTime settletime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), TradingCalendar.SettleTime);
            TaskProc tasksettle = new TaskProc(this.UUID, "交易系统结算-" + resetTime.ToString(), settletime.Hour, settletime.Minute, settletime.Second, delegate() { Task_SettleAccount(); });
            TLCtxHelper.Ctx.InjectTask(tasksettle);

            //注入重置任务 可以在数据指定重置时间
            DateTime resttime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), resetTime);
            TaskProc taskreset = new TaskProc(this.UUID, "交易系统重置-" + resetTime.ToString(), resttime.Hour, resttime.Minute, resttime.Second, delegate() { Task_ResetTradingday(); });
            TLCtxHelper.Ctx.InjectTask(taskreset);

        }

        
        /// <summary>
        /// 初始化交易日信息
        /// </summary>
        void InitTradingDay()
        {

            //开发模式每天都有结算,运行模式按照交易日里进行结算
            debug("System running under " + (GlobalConfig.IsDevelop?"develop":"production"), QSEnumDebugLevel.INFO);

            IsInSettle = false;
            //从数据库获得上次结算日
            _lastsettleday = ORM.MSettlement.GetLastSettleday();

            //通过交易日历获得针对结算日的下一个交易日 该交易日会越过休息日与法定假期
            if (!GlobalConfig.IsDevelop)
            {
                _nexttradingday = TradingCalendar.NextTradingDay(_lastsettleday);
            }
            else
            {
                //如果是开发模式则在上一个结算日基础上顺延一天获得下一个交易日 该日期不排除周末和假期主要用于系统开发(周末和节假日也可以正常进行交易)
                DateTime nexdate = Util.ToDateTime(_lastsettleday,0).AddDays(1);
                _nexttradingday = Util.ToTLDate(nexdate);
            }
            //获得当前日期与时间
            int nowdate = Util.ToTLDate();
            int nowtime = Util.ToTLTime();

            //如果当前日期越过了下一个交易日,则表明按结算推算出来的下一个交易日已经被跳过,需要手工进行结算
            if (nowdate > _nexttradingday)
            {
                debug(string.Format("上次结算日:{0} 下一交易日:{1} 当前日期:{2}", _lastsettleday, _nexttradingday, nowdate), QSEnumDebugLevel.INFO);
                debug("当前日期越过了交易日,系统缺少对应交易的结算,请手工进行结算", QSEnumDebugLevel.INFO);
                SettleCentreStatus = QSEnumSettleCentreStatus.HISTSETTLE;
                debug(string.Format("设定当前交易日为下一个交易日:{0}", _nexttradingday), QSEnumDebugLevel.INFO);
                _tradingday = _nexttradingday;
            }
            //如果当前日期<=netxtradingday则正常,比如下午结算后 当前交易日就是夜盘隶属的下一个交易日,当前时间则小于该交易日,遇到周五则会小2天
            else
            {
                
                if (!GlobalConfig.IsDevelop)
                {
                    //运行模式的交易日是通过交易日历来获得当前交易日 当前交易日有可能为0,为0标识当前不是交易日
                    _tradingday = TradingCalendar.GetCurrentTradingday(nowdate,nowtime,_nexttradingday);
                }
                else
                {
                    //开发版 当前交易日就是结算日推算出来的下一个交易日
                    _tradingday = _nexttradingday;
                }
                debug(string.Format("结算中心初始化交易日信息,当前日期:{0} 当前时间:{1}", nowdate, nowtime), QSEnumDebugLevel.INFO);
                debug(string.Format("上次结算日:{0} 下一交易日:{1} 当前交易日:{2}", _lastsettleday, _nexttradingday,_tradingday), QSEnumDebugLevel.INFO);

                if (CurrentTradingday == 0)
                {
                    SettleCentreStatus = QSEnumSettleCentreStatus.NOTRADINGDAY;//如果没有当前交易日信息则为非交易日状态
                }
                else
                {
                    SettleCentreStatus = QSEnumSettleCentreStatus.TRADINGDAY;//如果获得了当前交易日则当前为可交易日状态
                }
            }


        }
        /// <summary>
        /// 重置结算信息
        /// 按照系统记录的上个结算日 当前日期 时间来获得对应的当前交易日和结算状态信息
        /// </summary>
        public void Reset()
        {
            InitTradingDay();
            debug(string.Format("结算中心初始化,上次结算日:{0} 下一交易日:{1} 当前交易日:{2} 结算状态:{3}", _lastsettleday, _nexttradingday, _tradingday, SettleCentreStatus));
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

        string settleheader = "#####SettleAccount:";
        /// <summary>
        /// 结算所有交易账户
        /// 结算分析
        /// 1.单日lastequity + realizedpl + unrealizedpl - commission + cashin - cashout = now equity 数据库检验通过
        /// 2.当日结算完毕后的nowequity即为账户表中的lastequity 数据库检验通过
        /// 3.产生错误就是在某些结算记录中上日权益不等于该账户的nowequity
        /// 4.账户结算时需要检查 账户的上日权益是否是数据库记录的上日权益
        /// </summary>
        public void SettleAccount()
        {
            debug(string.Format(settleheader+"Start Settele Account,Current Tradingday:{0}",CurrentTradingday), QSEnumDebugLevel.INFO);
            foreach (IAccount acc in _clearcentre.Accounts)
            {
                try
                {
                    ORM.MSettlement.SettleAccount(acc);
                }
                catch (Exception ex)
                {
                    debug(string.Format("SettleError,Account:{0} errors:{1}",acc.ID,ex.ToString()), QSEnumDebugLevel.ERROR);
                }
            }

            //更新最近结算日
            debug(string.Format("Update lastsettleday as:{0}", CurrentTradingday), QSEnumDebugLevel.INFO);
            ORM.MSettlement.UpdateSettleday(CurrentTradingday);
            debug("Settlement Done", QSEnumDebugLevel.INFO);
        }


    }
}
