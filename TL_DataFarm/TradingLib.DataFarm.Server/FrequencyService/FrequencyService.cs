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
    /// 
    /// </summary>
    public partial class FrequencyService
    {

        ILog logger = LogManager.GetLogger("FrequencyService");
        /// <summary>
        /// 实时行情产生Bar数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewRealTimeBarEvent;

        /// <summary>
        /// 历史Tick产生Bar回补数据
        /// </summary>
        public event Action<FreqNewBarEventArgs> NewHistBarEvent;


        /// <summary>
        /// 某个交易所的所有合约由同一个FrequencyManager进行维护
        /// </summary>
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

        /// <summary>
        /// 建立Symbol到FrequencyManager映射 用于快速查找某个合约的FrequencyManager
        /// </summary>
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
        /// Bar数据生成服务
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

            //恢复历史Tick所用的FrequencyManager
            restoreFrequencyMgr = new FrequencyManager("Restore", QSEnumDataFeedTypes.DEFAULT);
            restoreFrequencyMgr.RegisterAllBasicFrequency();
            restoreFrequencyMgr.NewFreqKeyBarEvent += new Action<FrequencyManager.FreqKey, SingleBarEventArgs>(OnNewHistFreqKeyBarEvent);


            //遍历所有合约 建立合约到FrequencyManager映射 同时将合约注册到FrequencyManager
            foreach (var symbol in MDBasicTracker.SymbolTracker.Symbols)
            {
                if (symbol.Symbol != "CLX6") continue;
                FrequencyManager fm = GetFrequencyManagerForExchange(symbol.SecurityFamily.Exchange.EXCode);
                if (fm != null)
                {
                    fm.RegisterSymbol(symbol);
                    symbolFrequencyMgrMap.Add(symbol.Symbol, fm);
                }
                //同时向数据恢复FrequencyManager注册
                restoreFrequencyMgr.RegisterSymbol(symbol);
            }
        }

        void OnNewHistFreqKeyBarEvent(FrequencyManager.FreqKey arg1, SingleBarEventArgs arg2)
        {
            //if (arg1.Symbol.Symbol == "GC04")
            //{
            //    int i = 1;
            //}
#if DEBUG
            logger.Warn(string.Format("Bar ReGenerated Key:{0} Bar:{1}", arg1.Settings.BarFrequency, arg2.Bar));
#endif

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
                FrequencyManager fm = GetFrequencyManagerForExchange(arg1.Symbol.SecurityFamily.Exchange.EXCode);
                Frequency data = null;
                if (fm != null)
                {
                    data = fm.GetFrequency(arg1.Symbol, arg1.Settings.BarFrequency);
                    if (data == null) return;
                }

                BarImpl b = new BarImpl(arg2.Bar);
                b.TradingDay = 0;
                NewRealTimeBarEvent(new FreqNewBarEventArgs() { Bar = new BarImpl(arg2.Bar), BarFrequency = arg1.Settings.BarFrequency, Symbol = arg1.Symbol,Frequency=data });
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

        /// <summary>
        /// 恢复历史Tick
        /// </summary>
        /// <param name="k"></param>
        public void RestoreTick(Tick k)
        {
            restoreFrequencyMgr.ProcessTick(k);
        }

    }
}
