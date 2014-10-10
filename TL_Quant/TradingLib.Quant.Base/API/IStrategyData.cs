using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Quant.Base
{
    public interface IStrategyData:IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable
    {
        /// <summary>
        /// 合约集合
        /// </summary>
        IList<Security> Symbols{get;}

        /// <summary>
        /// 指标集合
        /// </summary>
        IndicatorCollections Indicators{get;}

        /// <summary>
        /// 指标管理器
        /// </summary>
        IIndicatorManager IndicatorManager { get; }

        /// <summary>
        /// 记录交易信息
        /// </summary>
        ITradingInfoTracker TradingInfoTracker { get; }

        /// <summary>
        /// 执行策略支持,下单/取消/获得基础数据等
        /// </summary>
        IStrategySupport StrategySupport { get; }

        /// <summary>
        /// 策略运行的系统参数
        /// </summary>
        StrategyParameters StrategyParameters { get; }

        ITBars Bars { get; }

        bool IgnoreSystemWarnings { get; }
        DateTime DataStartDate { get; set; }
        DateTime TradeStartDate { get; set; }
        DateTime EndDate { get; set; }

        double StartingCapital { get; }

        StrategyHistory StratgyHistory { get; }

        StrategyStatistic StrategyStatistic { get; }

        bool UseTicksForSimulation { get; }

        Dictionary<Security, ChartPaneList> ChartPaneCollections { get; }

        FrequencyPlugin BarFrequency { get; }
        ChartObjectManager ChartObjects { get; }
        Security GetSymbolByName(string name);
        bool IsOneSymbol { get; }

        Frequency GetFrequency(Security symbol, BarFrequency freq);

        //ITBars Bars { get; }
    }
}


