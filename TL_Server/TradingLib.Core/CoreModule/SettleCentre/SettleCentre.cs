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

            //初始化交易所结算任务 程序启动时 初始化任务过程会自动判定当前交易日 系统执行结算后 结算过程会回滚交易日 因此 初始化数据库过程只要从数据库加载记录的上个结算日并于当前交易日确认
            InitSettleTask();

            //初始化交易日信息
            InitData();

            logger.Info(string.Format("LastSettleday:{0} Current Tradingday:{1} Next SettleTime:{2}", _lastsettleday, _tradingday, _nextSettleTime.ToString("yyyyMMdd HH:mm:ss")));
            
        }


        
        /// <summary>
        /// 初始化结算中心数据
        /// 与结算相关的数据
        /// 上个结算日 当前交易日 结算价 汇率 等数据
        /// </summary>
        void InitData()
        {
            //从数据库获得上次结算日
            _lastsettleday = ORM.MSettlement.GetLastSettleday();

            //如果上个结算日大于等于当前交易日 则交易日设置异常
            if (_lastsettleday >= _tradingday)
            {
                logger.Error("结算日设置不正确,lastsettleday 大于 最早结算交易所对应日期");
                throw new ArgumentException("invlaid settleday");
            }

            //加载当日结算价数据
            _settlementPriceTracker.LoadSettlementPrice(this.Tradingday);

            //结算中心创建当前交易日汇率数据
            BasicTracker.ExchangeRateTracker.CreateExchangeRates(this.Tradingday);
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
