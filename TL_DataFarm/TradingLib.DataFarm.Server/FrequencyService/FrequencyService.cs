using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Common.Logging;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common.DataFarm
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
        /// 实时行情产生Bar数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewRealTimeBarEvent;

        /// <summary>
        /// 历史Tick产生Bar回补数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewHistBarEvent;

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

        FrequencyManager restoreFrequencyMgr = null;

        /// <summary>
        /// Bar数据生成器
        /// </summary>
        public FrequencyService()
        {

            //遍历所有交易所 为每个交易所生成频率发生器
            foreach (var exchange in MDBasicTracker.ExchagneTracker.Exchanges)
            {
                FrequencyManager fm = new FrequencyManager(exchange.EXCode, exchange.DataFeed);
                fm.RegisterAllBasicFrequency();

                frequencyMgrMap.Add(exchange.EXCode, fm);

                fm.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(OnNewRealTimeFreqKeyBarEvent);

            }
            restoreFrequencyMgr = new FrequencyManager("Restore", QSEnumDataFeedTypes.DEFAULT);
            restoreFrequencyMgr.RegisterAllBasicFrequency();
            restoreFrequencyMgr.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(OnNewHistFreqKeyBarEvent);


            //遍历所有合约 并建立合约到FrequencyManager映射
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                if (symbol.Symbol != "rb1610") continue;
                FrequencyManager fm = GetFrequencyManagerForExchange(symbol.SecurityFamily.Exchange.EXCode);
                if (fm != null)
                {
                    //FrequencyManager注册合约并建立直接映射
                    fm.RegisterSymbol(symbol);
                    symbolFrequencyMgrMap.Add(symbol.Symbol, fm);
                }

                restoreFrequencyMgr.RegisterSymbol(symbol);
                
            }

            

        }

        void OnNewHistFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
            logger.Warn(string.Format("Bar ReGenerated Key:{0} Bar:{1}", arg1.Settings.BarFrequency, arg2.Bar));
            if (NewHistBarEvent != null)
            {
                BarImpl b = new BarImpl(arg2.Bar);
                b.TradingDay = 0;
                NewHistBarEvent(new FreqNewBarEventArgs() { Bar = new BarImpl(arg2.Bar), BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol });
            
            }
        }


        /// <summary>
        /// 每天按交易所收盘时间定时更新交易日信息,在Bar数据生成后直接通过Map获得交易日数据 避免每个Bar去运算
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        void OnNewRealTimeFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
#if DEBUG
            logger.Warn(string.Format("Bar Generated Key:{0} Bar:{1}", arg1.Settings.BarFrequency, arg2.Bar));
#endif
            if (NewRealTimeBarEvent != null)
            {
                BarImpl b = new BarImpl(arg2.Bar);
                b.TradingDay = 0;
                NewRealTimeBarEvent(new FreqNewBarEventArgs() { Bar = new BarImpl(arg2.Bar), BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol });
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
        /// 获得某个合约内存中第一个有效Bar时间
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public DateTime GetFirstTickTime(Symbol symbol)
        { 
             FrequencyManager fm = GetFrequencyManagerForExchange(symbol.SecurityFamily.Exchange.EXCode);
             if (fm != null)
             {
                 return fm.GetFirstTickTime(symbol);
             }
             return DateTime.MaxValue;
        }


        /// <summary>
        /// 处理外部行情
        /// </summary>
        /// <param name="k"></param>
        public void ProcessTick(Tick k)
        {
            FrequencyManager fm = GetFrequencyManagerForSymbol(k.Symbol);
            if (fm == null) return;
            fm.ProcessTick(k);
        }


        public void RestoreTick(Tick k)
        {
            restoreFrequencyMgr.ProcessTick(k);
        }

    }
}
