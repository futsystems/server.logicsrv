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

        QSEnumSettleMode _settlemode = QSEnumSettleMode.StandbyMode;
        /// <summary>
        /// 结算中心工作模式
        /// 历史结算
        /// 结算
        /// 待机
        /// </summary>
        public QSEnumSettleMode SettleMode { get { return _settlemode; } set { _settlemode = value; } }

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

        /// <summary>
        /// 下一个结算时间
        /// </summary>
        public long NextSettleTime { get { return _nextSettleTime.ToTLDateTime(); } }


        int _settleTime = 160000;
        /// <summary>
        /// 获得结算时间
        /// 如果是历史结算 则返回的结算时间是16:00
        /// 如果是正常结算 则返回系统当前时间
        /// </summary>
        public int SettleTime { get { return _settleTime; } }

        int _resetTime = 170000;
        /// <summary>
        /// 重置时间
        /// </summary>
        public int ResetTime { get { return _resetTime; } }


        SettlementPriceTracker _settlementPriceTracker = new SettlementPriceTracker();

        ConfigDB _cfgdb;
        

        /// <summary>
        /// 遍历所有可用交易所 获得所有交易所的结算时间 边转换成本地时间 采用最晚的一个时间为下一个结算时间
        /// </summary>
        DateTime _nextSettleTime = DateTime.Now;
        DateTime _nextSettleResetTime = DateTime.Now;
        

        public SettleCentre()
            :base(SettleCentre.CoreName)
        {
            _cfgdb = new ConfigDB(SettleCentre.CoreName);

            //初始化交易所结算任务
            InitSettleTask();

            //初始化交易日信息
            InitTradingDay();

            //加载当日结算价数据
            _settlementPriceTracker.LoadSettlementPrice(this.Tradingday);
            //结算中心创建当前交易日汇率数据
            BasicTracker.ExchangeRateTracker.CreateExchangeRates(this.Tradingday);

            logger.Info(string.Format("LastSettleday:{0} Current Tradingday:{1} Next SettleTime:{2}", _lastsettleday, _tradingday, _nextSettleTime.ToString("yyyyMMdd HH:mm:ss")));
            
        }


        
        /// <summary>
        /// 初始化交易日信息
        /// </summary>
        void InitTradingDay()
        {
            //从数据库获得上次结算日
            _lastsettleday = ORM.MSettlement.GetLastSettleday();

            //如果上个结算日大于等于当前交易日 则交易日设置异常
            if (_lastsettleday >= _tradingday)
            {
                logger.Error("结算日设置不正确,lastsettleday 大于 最早结算交易所对应日期");
                throw new ArgumentException("invlaid settleday");
            }
        }

        /// <summary>
        /// 滚动交易日
        /// </summary>
        void RollTradingDay()
        {
            //更新结算日
            logger.Info(string.Format("Update lastsettleday as:{0}", Tradingday));
            ORM.MSettlement.UpdateSettleday(this.Tradingday);
            //更新交易日
            _lastsettleday = this.Tradingday;
            _tradingday = Util.ToDateTime(this.Tradingday, DateTime.Now.ToTLTime()).AddDays(1).ToTLDate();
            BasicTracker.ExchangeRateTracker.CreateExchangeRates(_tradingday);//创建下一个交易日的汇率数据
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
    }
}
