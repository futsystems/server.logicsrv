using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 策略运行的配置文件,设定了策略将以什么样的方式进行运行
    /// </summary>
    [Serializable]
    public class StrategyRunSettings
    {
            //private FrequencyPlugin _barFrequency;

        // Methods
        public StrategyRunSettings()
        {
            this.EndDate = DateTime.MaxValue;
            this.CreateTicksFromBars = true;
        }

        public StrategyRunSettings Clone()
        {
            StrategyRunSettings settings = (StrategyRunSettings)base.MemberwiseClone();
            if (this.Symbols != null)
            {
                settings.Symbols = (from s in this.Symbols select s.Clone()).ToList<SymbolSetup>();
            }
            if (this.BarFrequency != null)
            {
                settings.BarFrequency = this.BarFrequency.Clone();
            }
            if (this.StrategyParameters != null)
            {
                //settings.StrategyParameters = new Dictionary<string, double>(this.StrategyParameters);
                settings.StrategyParameters = new List<KeyValuePair<string, double>>(this.StrategyParameters);
            }
            return settings;
        }

        // Properties
        /// <summary>
        /// 账户货币类别
        /// </summary>
        public CurrencyType AccountCurrency { get; set; }

        //public double AllocationPerPosition { get; set; }

        //public PositionAllocationType AllocationType { get; set; }

        //public bool ApplyForexInterest { get; set; }

        //public int BarCountExit { get; set; }

        /// <summary>
        /// 频率信息
        /// </summary>
        public FrequencyPlugin BarFrequency { get; set; }
        /// <summary>
        /// 利用Bar数据 生成Tick数据
        /// </summary>
        public bool CreateTicksFromBars { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime DataStartDate { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndDate { get; set; }

        //public bool ForceRoundLots { get; set; }

        //public TimeSpan ForexRolloverTime { get; set; }

        /// <summary>
        /// Bar产生的Tick数据发送方式 o h l c 4个模拟Tick
        /// </summary>
        public bool HighBeforeLowDuringSimulation { get; set; }
        /// <summary>
        /// 忽略系统错误信息
        /// </summary>
        public bool IgnoreSystemWarnings { get; set; }

        public int LeadBars { get; set; }
        /// <summary>
        /// 最大开仓
        /// </summary>
        public int MaxOpenPositions { get; set; }

        /// <summary>
        /// 单个symbol最大开仓数量
        /// </summary>
        public int MaxOpenPositionsPerSymbol { get; set; }
        /// <summary>
        /// 目标盈利
        /// </summary>
        //public double ProfitTarget { get; set; }

        //public TargetPriceType ProfitTargetType { get; set; }

        public bool RestrictOpenOrders { get; set; }

        public int RunNumber { get; set; }

        public bool SaveOptimizationResults { get; set; }
        /// <summary>
        /// 初始资金
        /// </summary>
        public double StartingCapital { get; set; }
        /// <summary>
        /// 止损额度
        /// </summary>
        //public double StopLoss { get; set; }

        //public TargetPriceType StopLossType { get; set; }
        /// <summary>
        /// 交易的合约
        /// </summary>
        public List<SymbolSetup> Symbols { get; set; }

        

        public bool SynchronizeBars { get; set; }

        /// <summary>
        /// 系统参数
        /// </summary>
        public List<KeyValuePair<string,double>> StrategyParameters { get; set; }

        /// <summary>
        /// 交易开始时间
        /// </summary>
        public DateTime TradeStartDate { get; set; }

        /// <summary>
        /// 是否用原始Tick数据进行模拟
        /// </summary>
        public bool UseTicksForSimulation { get; set; }

        }
}
