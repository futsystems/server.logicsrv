using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// keep track of bid/ask and last data for symbols
    /// 其实TickTracker维护了一个市场行情快照
    /// 当不同的合约有成交数据 报价数据产生时,用于更新本地行情快照 将最新的数据更新到对应的字段
    /// 当使用时 通过symbol进行索引 获得对应的行情快照
    /// 
    /// 这里直接使用S类型的Tick 是否会更理想，在这个数据维护器中，获得一个行情快照 需要从多个数据维护期中查找数据 效率明显比直接获取Tick对象要慢很多
    /// </summary>
    public class TickTracker: GotTickIndicator
    {
        public void GotTick(Tick k)
        {
            this.UpdateTick(k);
        }

        public void Clear()
        {
            tickSnapMap.Clear();
        }

        /// <summary>
        /// create ticktracker
        /// </summary>
        public TickTracker()
        {
        }

        public int Count { get { return tickSnapMap.Count; } }

        /// <summary>
        /// 返回所有行情Tick
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tick> GetTicks()
        {
            return tickSnapMap.Values;
        }

        ConcurrentDictionary<string, Tick> tickSnapMap = new ConcurrentDictionary<string, Tick>();

        //TODO SymbolKey
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick this[string exchange,string sym]
        {
            get
            {
                string key = string.Format("{0}-{1}", exchange, sym);
                Tick snapshot = null;
                if (tickSnapMap.TryGetValue(key, out snapshot))
                {
                    return snapshot;
                }
                return null;
            }
        }

        /// <summary>
        /// 更新Tick数据到Tick快照维护器
        /// </summary>
        /// <param name="k"></param>
        public void UpdateTick(Tick k)
        {
            if(k == null) return;
            if(string.IsNullOrEmpty(k.Symbol) || string.IsNullOrEmpty(k.Exchange)) return;
            
            string key = k.GetSymbolUniqueKey();

            Tick snapshot = null;
            if (!tickSnapMap.TryGetValue(key, out snapshot))
            {
                snapshot = new TickImpl();
                snapshot.UpdateType = "S";
                snapshot.Symbol = k.Symbol;
                snapshot.Exchange = k.Exchange;
                tickSnapMap.TryAdd(key, snapshot);
            }
            snapshot.DataFeed = k.DataFeed;

            switch (k.UpdateType)
            {
                case "X":
                    {
                        snapshot.Date = k.Date;
                        snapshot.Time = k.Time;
                        snapshot.Trade = k.Trade;
                        snapshot.Size = k.Size;
                        snapshot.Vol = k.Vol;
                        snapshot.Exchange = k.Exchange;
                        break;
                    }
                case "A":
                    {
                        snapshot.AskPrice = k.AskPrice;
                        snapshot.AskSize = k.AskSize;
                        snapshot.AskExchange = k.AskExchange;
                        snapshot.Exchange = k.Exchange;
                        break;
                    }
                case "B":
                    {
                        snapshot.BidPrice = k.BidPrice;
                        snapshot.BidSize = k.BidSize;
                        snapshot.BidExchange = k.BidExchange;
                        snapshot.Exchange = k.Exchange;
                        break;
                    }
                case "Q":
                    {
                        snapshot.AskPrice = k.AskPrice;
                        snapshot.AskExchange = k.AskExchange;
                        snapshot.AskSize = k.AskSize;
                        snapshot.BidPrice = k.BidPrice;
                        snapshot.BidSize = k.BidSize;
                        snapshot.BidExchange = k.BidExchange;
                        snapshot.Exchange = k.Exchange;
                        break;
                    }
                case "F":
                    {
                        snapshot.Open = k.Open;
                        snapshot.High = k.High;
                        snapshot.Low = k.Low;
                        snapshot.PreClose = k.PreClose;
                        snapshot.OpenInterest = k.OpenInterest;
                        snapshot.PreOpenInterest = k.PreOpenInterest;
                        snapshot.Settlement = k.Settlement;
                        snapshot.PreSettlement = k.PreSettlement;
                        snapshot.Exchange = k.Exchange;
                        break;
                    }
                case "S":
                    {
                        snapshot.Date = k.Date;
                        snapshot.Time = k.Time;
                        snapshot.Trade = k.Trade;
                        snapshot.Size = k.Size;
                        snapshot.Vol = k.Vol;
                        snapshot.Exchange = k.Exchange;

                        snapshot.AskPrice = k.AskPrice;
                        snapshot.AskSize = k.AskSize;
                        snapshot.AskExchange = k.AskExchange;

                        snapshot.BidPrice = k.BidPrice;
                        snapshot.BidSize = k.BidSize;
                        snapshot.BidExchange = k.BidExchange;

                        snapshot.AskPrice2 = k.AskPrice2;
                        snapshot.BidPrice2 = k.BidPrice2;
                        snapshot.AskPrice3 = k.AskPrice3;
                        snapshot.BidPrice3 = k.BidPrice3;
                        snapshot.AskPrice4 = k.AskPrice4;
                        snapshot.BidPrice4 = k.BidPrice4;
                        snapshot.AskPrice5 = k.AskPrice5;
                        snapshot.BidPrice5 = k.BidPrice5;

                        snapshot.AskSize2 = k.AskSize2;
                        snapshot.BidSize2 = k.BidSize2;
                        snapshot.AskSize3 = k.AskSize3;
                        snapshot.BidSize3 = k.BidSize3;
                        snapshot.AskSize4 = k.AskSize4;
                        snapshot.BidSize4 = k.BidSize4;
                        snapshot.AskSize5 = k.AskSize5;
                        snapshot.BidSize5 = k.BidSize5;


                        snapshot.Open = k.Open;
                        snapshot.High = k.High;
                        snapshot.Low = k.Low;
                        snapshot.PreClose = k.PreClose;
                        snapshot.OpenInterest = k.OpenInterest;
                        snapshot.PreOpenInterest = k.PreOpenInterest;
                        snapshot.Settlement = k.Settlement;
                        snapshot.PreSettlement = k.PreSettlement;

                        snapshot.UpperLimit = k.UpperLimit;
                        snapshot.LowerLimit = k.LowerLimit;

                        break;

                    }
                default:
                    break;
            }
        }
    }
}
