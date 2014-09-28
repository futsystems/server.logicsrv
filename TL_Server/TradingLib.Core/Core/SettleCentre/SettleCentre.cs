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
        UNKNOWN,//未知
        HISTSETTLE,//历史结算
        TRADINGDAY,//交易日
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
        /// </summary>
        public int LastSettleday { get { return _lastsettleday; } }

        int _tradingday = 0;
        /// <summary>
        /// 当前交易日
        /// </summary>
        public int CurrentTradingday { get { return _tradingday; } }

        int _nexttradingday =0;
        /// <summary>
        /// 下一个交易日
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
        /// 获得结算时间 如果是历史结算 则返回的结算时间是16:00
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


        public SettleCentre()
            :base(SettleCentre.CoreName)
        {
            //TradingCalendar.SendDebugEvent +=new DebugDelegate(msgdebug);

            //初始化置结算中心状态为未知
            SettleCentreStatus = QSEnumSettleCentreStatus.UNKNOWN;

            //初始化交易日信息
            InitTradingDay();
        }



        void InitTradingDay()
        {
            //开发模式每天都有结算,运行模式按照交易日里进行结算
            debug("结算系统工作模式:" + (GlobalConfig.IsDevelop?"开发模式":"运行模式"), QSEnumDebugLevel.INFO);

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
                _tradingday = _nexttradingday;
                debug(string.Format("设定当前交易日为下一个交易日:{0}", _nexttradingday),QSEnumDebugLevel.INFO);

            }
            //如果当前日期<=netxtradingday则正常,比如下午结算后 当前交易日就是夜盘隶属的下一个交易日,当前时间则小于该交易日,遇到周五则会小2天
            else
            {
                
                if (!GlobalConfig.IsDevelop)
                {
                    //运行模式的交易日是通过交易日里来获得当前交易日 当前交易日有可能为0,为0标识当前不是交易日
                    _tradingday = TradingCalendar.GetCurrentTradingday(nowtime, nowdate, _nexttradingday);
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
        /// </summary>
        public void Reset()
        {
            InitTradingDay();
            debug(string.Format("结算中心初始化,上次结算日:{0} 下一交易日:{1} 当前交易日:{2} 结算状态:{3}", _lastsettleday, _nexttradingday, _tradingday, SettleCentreStatus));
        }

        public void Start()
        { 
        
        }

        public void Stop()
        { 
        
        }


        public override void Dispose()
        {
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
        public void SettleAccount()
        {
            //Status = QSEnumClearCentreStatus.CCSETTLE;
            debug(string.Format("结算系统开始结算交易账户 当前交易日{0}",CurrentTradingday), QSEnumDebugLevel.INFO);
            foreach (IAccount acc in _clearcentre.Accounts)
            {
                try
                {
                    ORM.MSettlement.SettleAccount(acc);
                }
                catch (Exception ex)
                {
                    debug(acc.ID + "结算出错:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                }
            }

            //更新最近结算日
            debug(string.Format("更新上次结算日为当前交易日{0}", CurrentTradingday), QSEnumDebugLevel.INFO);
            ORM.MSettlement.UpdateSettleday(CurrentTradingday);

            //Status = QSEnumClearCentreStatus.CCSETTLEFINISH;
            debug("交易系统结算完毕=====================================", QSEnumDebugLevel.INFO);
        }


    }
}
