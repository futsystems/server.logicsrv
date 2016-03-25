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

        //FrequencyManager frequencyManager;
        //ConcurrentDictionary<string, Symbol> subscribeSymbolMap = new ConcurrentDictionary<string, Symbol>();
        //ConcurrentDictionary<BarFrequency, FrequencyPlugin> frequencyPluginMap = new ConcurrentDictionary<BarFrequency, FrequencyPlugin>();


        /// <summary>
        /// 产生Bar数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewBarEvent;

        //public void Add(Symbol symbol)
        //{
        //    if (subscribeSymbolMap.Keys.Contains(symbol.Symbol))
        //    {
        //        logger.Info(string.Format("Symbol:{0} already registed", symbol.Symbol));
        //    }
        //    //添加合约到map
        //    subscribeSymbolMap.TryAdd(symbol.Symbol, symbol);
        //}

        Dictionary<string, FrequencyManager> frequencyMgrMap = new Dictionary<string, FrequencyManager>();

        FrequencyManager GetFrequencyManagerForExchange(string exchange)
        {
            FrequencyManager target = null;
            if (frequencyMgrMap.TryGetValue(exchange, out target))
            {
                return target;
            }
            return null;
        }


        Dictionary<string, FrequencyManager> symbolFrequencyMgrMap = new Dictionary<string, FrequencyManager>();

        FrequencyManager GetFrequencyManagerForSymbol(string symbol)
        {
            FrequencyManager target = null;
            if (symbolFrequencyMgrMap.TryGetValue(symbol, out target))
            {
                return target;
            }
            return null;
        }

        /// <summary>
        /// Bar数据生成器
        /// </summary>
        public FrequencyService()
        {

            //遍历所有交易所 为每个交易所生成频率发生器
            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                FrequencyManager fm = new FrequencyManager(exchange.EXCode,exchange.DataFeed);
                frequencyMgrMap.Add(exchange.EXCode, fm);
                fm.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(OnNewFreqKeyBarEvent);

            }

            //遍历所有合约 并建立合约到FrequencyManager映射
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                if (symbol.Symbol != "IF1604") continue;

                FrequencyManager fm = GetFrequencyManagerForExchange(symbol.SecurityFamily.Exchange.EXCode);
                if (fm != null)
                {
                    //FrequencyManager注册合约并建立直接映射
                    fm.RegisterSymbol(symbol);
                    symbolFrequencyMgrMap.Add(symbol.Symbol, fm);
                }
            }

            //初始化FrequencyPlugin
            //TimeFrequency tm = new TimeFrequency(new BarFrequency(BarInterval.CustomTime,60));
            
            //加载合约
            //Symbol symbol = MDBasicTracker.SymbolTracker["CLK6"];
            //Symbol symbol2 = MDBasicTracker.SymbolTracker["HSIX5"];
            //if (symbol != null)
            //{
            //    logger.Info("~~~~~got symbol:" + symbol.Symbol);
            //    //fm = new FrequencyManager(fb,,false);

            //    Dictionary<Symbol, BarConstructionType> map = new Dictionary<Symbol, BarConstructionType>();
            //    map.Add(symbol, BarConstructionType.Trade);
            //    //map.Add(symbol2, BarConstructionType.Trade);

            //    subscribeSymbolMap.TryAdd(symbol.Symbol, symbol);

            //    frequencyManager = new FrequencyManager();
            //    frequencyManager.RegisterSymbol(symbol);

            //}

            //frequencyManager.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(OnNewFreqKeyBarEvent);
        }

        void OnNewFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
//#if DEBUG
            logger.Warn(string.Format("Bar Generated Key:{0} Bar:{1}", arg1.Settings.BarFrequency, arg2.Bar));
//#endif
            if (NewBarEvent != null)
            {
                NewBarEvent(new FreqNewBarEventArgs() { Bar = new BarImpl(arg2.Bar), BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol });
            }
        }




        /// <summary>
        /// 获得Bar PartialItem
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="freq"></param>
        /// <returns></returns>
        public Bar GetPartialBar(Symbol symbol,BarFrequency freq)
        {
            FrequencyManager fm = GetFrequencyManagerForExchange(symbol.SecurityFamily.Exchange.EXCode);
            if (fm != null)
            {
                Frequency data = fm.GetFrequency(symbol, freq);
                if (data == null) return null;
                if (data.Bars.HasPartialItem) return data.Bars.PartialItem;
            }
            return null;
        }


        /// <summary>
        /// 处理外部行情
        /// </summary>
        /// <param name="k"></param>
        public void ProcessTick(Tick k)
        {
            if (k.Symbol == "IF1604")
            {
                int i = 0;
            }
            FrequencyManager fm = GetFrequencyManagerForSymbol(k.Symbol);
            if (fm == null) return;
            fm.ProcessTick(k);
        }

    }
}
