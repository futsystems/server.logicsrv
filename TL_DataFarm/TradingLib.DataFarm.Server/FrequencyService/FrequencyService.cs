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

        //public event Action<Bar> NewBarEvent;


        FrequencyManager frequencyManager;
        ConcurrentDictionary<string, Symbol> subscribeSymbolMap = new ConcurrentDictionary<string, Symbol>();
        ConcurrentDictionary<BarFrequency, FrequencyPlugin> frequencyPluginMap = new ConcurrentDictionary<BarFrequency, FrequencyPlugin>();


        /// <summary>
        /// 产生Bar数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewBarEvent;

        public FrequencyService()
        {

            //初始化FrequencyPlugin
            //TimeFrequency tm = new TimeFrequency(new BarFrequency(BarInterval.CustomTime,60));
            
            //加载合约
            Symbol symbol = MDBasicTracker.SymbolTracker["CLK6"];
            //Symbol symbol2 = MDBasicTracker.SymbolTracker["HSIX5"];
            if (symbol != null)
            {
                logger.Info("~~~~~got symbol:" + symbol.Symbol);
                //fm = new FrequencyManager(fb,,false);

                Dictionary<Symbol, BarConstructionType> map = new Dictionary<Symbol, BarConstructionType>();
                map.Add(symbol, BarConstructionType.Trade);
                //map.Add(symbol2, BarConstructionType.Trade);

                subscribeSymbolMap.TryAdd(symbol.Symbol, symbol);

                frequencyManager = new FrequencyManager(map);

            }


            //绑定事件
            //frequencyManager.FreqKeyRegistedEvent += new Action<FrequencyManager.FreqKey>(OnFreqKeyRegistedEvent);

            //注册频率发生器
            //frequencyManager.RegisterFrequencies(tm);

            frequencyManager.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(frequencyManager_NewFreqKeyBarEvent);
        }

        void frequencyManager_NewFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
            logger.Warn(string.Format("Bar Generated Key:{0} Bar:{1}", arg1.Settings.BarFrequency, arg2.Bar));


            //string key = string.Format("{0}-{1}",arg1.Symbol.GetContinuousKey(),arg1.Settings.BarFrequency.ToUniqueId());
            //Bar tmp = new BarImpl(arg2.Bar);
            //tmp.Symbol = arg1.Symbol.GetContinuousSymbol();//IF01,IF02;
            if (NewBarEvent != null)
            {
                NewBarEvent(new FreqNewBarEventArgs() { Bar = new BarImpl(arg2.Bar), BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol });
            }
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

            //if (NewBarEvent != null)
            //{
            //    NewBarEvent(obj.Bar);
            //}

        }


        /// <summary>
        /// 获得Bar PartialItem
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="freq"></param>
        /// <returns></returns>
        public Bar GetPartialBar(Symbol symbol,BarFrequency freq)
        {
            Frequency data = frequencyManager.GetFrequency(symbol, freq);
            if (data == null) return null;
            if (data.Bars.HasPartialItem) return data.Bars.PartialItem;
            return null;
        }
        /// <summary>
        /// 处理外部行情
        /// </summary>
        /// <param name="k"></param>
        public void ProcessTick(Tick k)
        {
            Symbol target = null;
            if (subscribeSymbolMap.TryGetValue(k.Symbol, out target))
            {
                //不同行情源混合
                //logger.Info(string.Format("date:{0} time:{1} datafeed:{2}", k.Date, k.Time, k.DataFeed));
                if (k.DataFeed == QSEnumDataFeedTypes.IQFEED)
                {
                    frequencyManager.ProcessTick(target, k);
                }
            }
        }

    }
}
