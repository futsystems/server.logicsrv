using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 维护Bar数据
    /// </summary>
    public partial class FrequencyService
    {

        ILog logger = LogManager.GetLogger("FrequencyService");

        public event Action<Bar> NewBarEvent;


        FrequencyManager frequencyManager;
        ConcurrentDictionary<string, Symbol> subscribeSymbolMap = new ConcurrentDictionary<string, Symbol>();
        ConcurrentDictionary<BarFrequency, FrequencyPlugin> frequencyPluginMap = new ConcurrentDictionary<BarFrequency, FrequencyPlugin>();


        public FrequencyService()
        {

            //初始化FrequencyPlugin
            TimeFrequency tm = new TimeFrequency(new BarFrequency(BarInterval.CustomTime,30));
            
            //加载合约
            Symbol symbol = MDBasicTracker.SymbolTracker["HGZ5"];
            //Symbol symbol2 = MDBasicTracker.SymbolTracker["HSIX5"];
            if (symbol != null)
            {
                logger.Info("~~~~~got symbol:" + symbol.Symbol);
                //fm = new FrequencyManager(fb,,false);

                Dictionary<Symbol, BarConstructionType> map = new Dictionary<Symbol, BarConstructionType>();
                map.Add(symbol, BarConstructionType.Trade);
                //map.Add(symbol2, BarConstructionType.Trade);

                subscribeSymbolMap.TryAdd(symbol.Symbol, symbol);

                frequencyManager = new FrequencyManager(tm, map);
            }


            //绑定事件
            frequencyManager.FreqKeyRegistedEvent += new Action<FrequencyManager.FreqKey>(OnFreqKeyRegistedEvent);

            //注册频率发生器
            frequencyManager.RegisterFrequencies(tm);
        }

        void OnFreqKeyRegistedEvent(FrequencyManager.FreqKey obj)
        {
            Frequency frequency = frequencyManager[obj];
            //如果Frequency不为空则订阅该Frequency的Bar事件
            if (frequency != null)
            {
                frequency.SingleBarEvent += (e) => { OnFrequencySingleBarEvent(frequency, e); };
            }
        }

        void OnFrequencySingleBarEvent(Frequency frequency,SingleBarEventArgs obj)
        {
            logger.Info(string.Format("Bar Generated:{0} frequency bar count:{1}", obj.Bar,frequency.Bars.Count));
            foreach (var b in frequency.Bars.Items)
            {
                logger.Info("Bar:" + b.ToString());
            }

            if (NewBarEvent != null)
            {
                NewBarEvent(obj.Bar);
            }

        }


        /// <summary>
        /// 处理外部行情
        /// </summary>
        /// <param name="k"></param>
        public void ProcessTick(Tick k)
        {
            Symbol target = null;
            if (subscribeSymbolMap.TryGetValue(k.Symbol,out target))
            {
                frequencyManager.ProcessTick(target,k);
            }
        }

    }
}
