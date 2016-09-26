using System;
using System.Collections.Generic;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// keep track of bid/ask and last data for symbols
    /// 其实TickTracker维护了一个市场行情快照
    /// 当不同的合约有成交数据 报价数据产生时,用于更新本地行情快照 将最新的数据更新到对应的字段
    /// 当使用时 通过symbol进行索引 获得对应的行情快照
    /// </summary>
    public class TickTracker : GenericTrackerI, GotTickIndicator
    {
        public void GotTick(Tick k)
        {
            newTick(k);
        }
        public Type TrackedType
        {
            get
            {
                return typeof(Tick);
            }
        }

        /// <summary>
        /// gets decimal value of last trade price for given index
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal ValueDecimal(int idx)
        {
            return (decimal)this[idx].Trade;
        }

        /// <summary>
        /// gets decimal value of last trade price given label
        /// </summary>
        /// <param name="txt"></param>
        /// <returns></returns>
        public decimal ValueDecimal(string txt)
        {
            return (decimal)this[txt].Trade;
        }

        public object Value(int idx) { return this[idx]; }
        public object Value(string txt) { return this[txt]; }

        public void Clear()
        {
            bid.Clear();
            ask.Clear();
            bs.Clear();
            be.Clear();
            oe.Clear();
            os.Clear();
            ts.Clear();
            ex.Clear();
            date.Clear();
            time.Clear();
            last.Clear();

            open.Clear();
            high.Clear();
            low.Clear();
            presettle.Clear();
            volume.Clear();
            oi.Clear();
            preoi.Clear();
            upperlimit.Clear();
            lowerlimit.Clear();
            settlement.Clear();
            preclose.Clear();

            ask2.Clear();
            bid2.Clear();
            ask3.Clear();
            bid3.Clear();
            ask4.Clear();
            bid4.Clear();
            ask5.Clear();
            bid5.Clear();

            asksize2.Clear();
            bidsize2.Clear();
            asksize3.Clear();
            bidsize3.Clear();
            asksize4.Clear();
            bidsize4.Clear();
            asksize5.Clear();
            bidsize5.Clear();

        }


        int _estlabels = 100;
        /// <summary>
        /// create ticktracker
        /// </summary>
        public TickTracker() : this(100) { }

        public TickTracker(string name) { _name = name; }


        /// <summary>
        /// create ticktracker with some approximate # of symbols to track
        /// </summary>
        /// <param name="estlabels"></param>
        public TickTracker(int estlabels)
        {
            _estlabels = estlabels;
            bid = new GenericTracker<decimal>(_estlabels);//bid price
            ask = new GenericTracker<decimal>(_estlabels);//ask price
            last = new GenericTracker<decimal>(_estlabels);//last price
            bs = new GenericTracker<int>(_estlabels);//bid size
            be = new GenericTracker<string>(_estlabels);//bid exchange
            oe = new GenericTracker<string>(_estlabels);//ask exchange
            os = new GenericTracker<int>(_estlabels);//asksize 
            ts = new GenericTracker<int>(_estlabels);//last size
            ex = new GenericTracker<string>(_estlabels);//exchange
            date = new GenericTracker<int>(_estlabels);//日期
            time = new GenericTracker<int>(_estlabels);//时间

            open = new GenericTracker<decimal>(_estlabels);
            high = new GenericTracker<decimal>(_estlabels);
            low = new GenericTracker<decimal>(_estlabels);
            presettle = new GenericTracker<decimal>(_estlabels);

            volume = new GenericTracker<int>(_estlabels);
            oi = new GenericTracker<int>(_estlabels);
            preoi = new GenericTracker<int>(_estlabels);

            upperlimit = new GenericTracker<decimal>(_estlabels);
            lowerlimit = new GenericTracker<decimal>(_estlabels);

            settlement = new GenericTracker<decimal>(_estlabels);
            preclose = new GenericTracker<decimal>(_estlabels);

            ask2 = new GenericTracker<decimal>(_estlabels);
            bid2 = new GenericTracker<decimal>(_estlabels);
            ask3 = new GenericTracker<decimal>(_estlabels);
            bid3 = new GenericTracker<decimal>(_estlabels);
            ask4 = new GenericTracker<decimal>(_estlabels);
            bid4 = new GenericTracker<decimal>(_estlabels);
            ask5 = new GenericTracker<decimal>(_estlabels);
            bid5 = new GenericTracker<decimal>(_estlabels);

            asksize2 = new GenericTracker<int>(_estlabels);
            bidsize2 = new GenericTracker<int>(_estlabels);
            asksize3 = new GenericTracker<int>(_estlabels);
            bidsize3 = new GenericTracker<int>(_estlabels);
            asksize4 = new GenericTracker<int>(_estlabels);
            bidsize4 = new GenericTracker<int>(_estlabels);
            asksize5 = new GenericTracker<int>(_estlabels);
            bidsize5 = new GenericTracker<int>(_estlabels);


            // setup generic trackers to track tick information
            last.NewTxt += new TextIdxDelegate(last_NewTxt);
        }
        
        

        /// <summary>
        /// called when new text label is added
        /// </summary>
        public event TextIdxDelegate NewTxt;

        void last_NewTxt(string txt, int idx)
        {
            date.addindex(txt, 0);
            time.addindex(txt, 0);
            bid.addindex(txt, 0);
            ask.addindex(txt, 0);
            bs.addindex(txt, 0);
            os.addindex(txt, 0);
            ts.addindex(txt, 0);
            ex.addindex(txt, string.Empty);
            be.addindex(txt, string.Empty);
            oe.addindex(txt, string.Empty);

            open.addindex(txt, 0);
            high.addindex(txt, 0);
            low.addindex(txt, 0);
            presettle.addindex(txt, 0);


            volume.addindex(txt, 0);
            oi.addindex(txt, 0);
            preoi.addindex(txt, 0);

            upperlimit.addindex(txt, 0);
            lowerlimit.addindex(txt, 0);

            settlement.addindex(txt, 0);
            preclose.addindex(txt, 0);

            ask2.addindex(txt, 0);
            bid2.addindex(txt, 0);
            ask3.addindex(txt, 0);
            bid3.addindex(txt, 0);
            ask4.addindex(txt, 0);
            bid4.addindex(txt, 0);
            ask5.addindex(txt, 0);
            bid5.addindex(txt, 0);

            asksize2.addindex(txt, 0);
            bidsize2.addindex(txt, 0);
            asksize3.addindex(txt, 0);
            bidsize3.addindex(txt, 0);
            asksize4.addindex(txt, 0);
            bidsize4.addindex(txt, 0);
            asksize5.addindex(txt, 0);
            bidsize5.addindex(txt, 0);

            if (NewTxt!=null)
                NewTxt(txt,idx);
        }


        GenericTracker<int> date;
        GenericTracker<int> time;
        GenericTracker<decimal> bid;
        GenericTracker<decimal> ask;
        GenericTracker<decimal> last;
        GenericTracker<int> bs;
        GenericTracker<int> os;
        GenericTracker<int> ts;
        GenericTracker<string> be;
        GenericTracker<string> oe;
        GenericTracker<string> ex;

        GenericTracker<decimal> open;
        GenericTracker<decimal> high;
        GenericTracker<decimal> low;
        GenericTracker<decimal> presettle;
        GenericTracker<int> volume;
        GenericTracker<int> oi;
        GenericTracker<int> preoi;

        GenericTracker<decimal> upperlimit;
        GenericTracker<decimal> lowerlimit;
        GenericTracker<decimal> settlement;
        GenericTracker<decimal> preclose;

        GenericTracker<decimal> ask2;
        GenericTracker<decimal> bid2;
        GenericTracker<int> asksize2;
        GenericTracker<int> bidsize2;

        GenericTracker<decimal> ask3;
        GenericTracker<decimal> bid3;
        GenericTracker<int> asksize3;
        GenericTracker<int> bidsize3;

        GenericTracker<decimal> ask4;
        GenericTracker<decimal> bid4;
        GenericTracker<int> asksize4;
        GenericTracker<int> bidsize4;

        GenericTracker<decimal> ask5;
        GenericTracker<decimal> bid5;
        GenericTracker<int> asksize5;
        GenericTracker<int> bidsize5;


            
        public string Display(int idx) { return this[idx].ToString(); }
        public string Display(string txt) { return this[txt].ToString(); }

        public string getlabel(int idx) { return last.getlabel(idx); }

        string _name = "TICKS";
        public string Name { get { return _name; } set { _name = value; } }

        public int Count { get { return last.Count; } }

        /// <summary>
        /// track a new symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int addindex(string symbol)
        {
            return last.addindex(symbol, 0);
        }
        /// <summary>
        /// get index of an existing symbol
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public int getindex(string symbol)
        {
            return last.getindex(symbol);
        }



        /// <summary>
        /// get the bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Bid(int idx) { return bid[idx]; }
        /// <summary>
        /// get the bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Bid(string sym) { return bid[sym]; }
        /// <summary>
        /// get the ask
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Ask(int idx) { return ask[idx]; }
        /// <summary>
        /// get the ask
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal Ask(string sym) { return ask[sym]; }
        /// <summary>
        /// get the last trade
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public decimal Last(int idx) { return last[idx]; }
        /// <summary>
        /// get the last trade
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public decimal Last(string sym) { return last[sym]; }
        /// <summary>
        /// whether we have a bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasBid(int idx) { if (idx < 0) return false; return bid[idx] != 0; }
        /// <summary>
        /// whether we have a bid
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasBid(string sym) { return bid[sym] != 0; }
        /// <summary>
        /// whether we have a ask
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasAsk(string sym) {  return ask[sym] != 0; }
        /// <summary>
        /// whether we have a ask
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasAsk(int idx) { if (idx < 0) return false; return ask[idx] != 0; }
        /// <summary>
        /// whether we have a last price
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasLast(int idx) { if (idx < 0) return false; return last[idx] != 0; }
        /// <summary>
        /// whether we have a last price
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public bool HasLast(string sym) { return last[sym] != 0; }
        /// <summary>
        /// whether we have a bid/ask and last
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasAll(string sym) { return HasBid(sym) && HasAsk(sym) && HasLast(sym); }
        /// <summary>
        /// whether we have a bid/ask and last
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasAll(int idx) { if (idx < 0) return false; return HasBid(idx) && HasAsk(idx) && HasLast(idx); }
        /// <summary>
        /// whether we have a bid/ask
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasQuote(string sym) { return HasBid(sym) && HasAsk(sym); }
        /// <summary>
        /// whether we have a bid/ask
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public bool HasQuote(int idx) { if (idx < 0) return false; return HasBid(idx) && HasAsk(idx); }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public Tick Tick(int idx)
        {
            return this[idx];
        }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick Tick(string sym)
        {
            return this[sym];
        }
        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick this[int idx]
        {
            get
            {
                Tick k = new TickImpl(last.getlabel(idx));
                k.Date = date[idx];
                k.Time = time[idx];

                k.Trade = last[idx];
                k.Size = ts[idx];
                k.Exchange = ex[idx];

                k.BidPrice = bid[idx];
                k.BidSize = bs[idx];
                k.BidExchange = be[idx];

                k.AskPrice = ask[idx];
                k.AskSize = os[idx];
                k.AskExchange = oe[idx];

                k.Open = open[idx];
                k.High = open[idx];
                k.Low = low[idx];
                k.PreSettlement = presettle[idx];

                k.Vol = volume[idx];
                k.OpenInterest = oi[idx];
                k.PreOpenInterest = preoi[idx];
                k.UpperLimit = upperlimit[idx];
                k.LowerLimit = lowerlimit[idx];
                k.Settlement = settlement[idx];
                k.PreClose = preclose[idx];

                k.AskPrice2 = ask2[idx];
                k.BidPrice2 = bid2[idx];
                k.AskPrice3 = ask3[idx];
                k.BidPrice3 = bid3[idx];
                k.AskPrice4 = ask4[idx];
                k.BidPrice4 = bid4[idx];
                k.AskPrice5 = ask5[idx];
                k.BidPrice5 = bid5[idx];

                k.AskSize2 = asksize2[idx];
                k.BidSize2 = bidsize2[idx];
                k.AskSize3 = asksize3[idx];
                k.BidSize3 = bidsize3[idx];
                k.AskSize4 = asksize4[idx];
                k.BidSize4 = bidsize4[idx];
                k.AskSize5 = asksize5[idx];
                k.BidSize5 = bidsize5[idx];

                
                return k;
            }
        }

        /// <summary>
        /// 返回所有行情Tick
        /// </summary>
        /// <returns></returns>
        public Tick[] GetTicks()
        {
            string[] syms = last.ToLabelArray();
            List<Tick> ticks = new List<Tick>();
                
            foreach(string sym in syms)
            {
                Tick k = this[sym];
                if (k != null && k.IsValid())
                {
                    ticks.Add(k);
                }
            }
            return ticks.ToArray();
        }

        /// <summary>
        /// get a tick in tick format
        /// </summary>
        /// <param name="sym"></param>
        /// <returns></returns>
        public Tick this[string sym]
        {
            get
            {
                int idx = last.getindex(sym);
                if (idx < 0) return null;// new TickImpl();
                return this[idx];
            }
        }

        /// <summary>
        /// update the tracker with a new tick
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        public bool newTick(Tick k)
        {

            //检查是否记录了该symbol
            // get index
            int idx = getindex(k.Symbol);
            // add if unknown
            if (idx < 0)
                idx = addindex(k.Symbol);
            // update date/time
            time[idx] = k.Time;
            date[idx] = k.Date;

            // update bid/ask/last
            if (k.IsTrade())
            {
                last[idx] = k.Trade;
                ex[idx] = k.Exchange;
                ts[idx] = k.Size;
            }
            if (k.HasAsk())
            {
                ask[idx] = k.AskPrice;
                oe[idx] = k.AskExchange;
                os[idx] = k.AskSize;
            }
            if (k.HasBid())
            {
                bid[idx] = k.BidPrice;
                bs[idx] = k.BidSize;
                be[idx] = k.BidExchange;
            }

            //储存tick数据中的扩展内容包含 高开低收，成家量，持仓量等数据 在没有扩展tick数据的情况下,这里通过维护这些数据获得本地扩展数据
            if (k.hasOpen)
            {
                open[idx] = k.Open;
            }

            if (k.hasHigh)
            {
                high[idx] = k.High;
            }

            if (k.hasLow)
            {
                low[idx] = k.Low;
            }
            if (k.hasPreSettle)
            {
                presettle[idx] = k.PreSettlement;
            }

            if (k.hasVol)
            {
                volume[idx] = k.Vol;
            }

            if (k.hasOI)
            {
                oi[idx] = k.OpenInterest;
            }

            if (k.hasPreOI)
            {
                preoi[idx] = k.PreOpenInterest;
            }
            upperlimit[idx] = k.UpperLimit;
            lowerlimit[idx] = k.LowerLimit;
            
            if (k.Settlement != 0)
            {
                settlement[idx] = k.Settlement;
            }

            if (k.PreClose != 0)
            {
                preclose[idx] = k.PreClose;
            }

            //股票行情快照 保存盘口2-5
            if (k.Type == EnumTickType.STKSNAPSHOT)
            {
                ask2[idx] = k.AskPrice2;
                bid2[idx] = k.BidPrice2;
                ask3[idx] = k.AskPrice3;
                bid3[idx] = k.BidPrice3;
                ask4[idx] = k.AskPrice4;
                bid4[idx] = k.BidPrice4;
                ask5[idx] = k.AskPrice5;
                bid5[idx] = k.BidPrice5;

                asksize2[idx] = k.AskSize2;
                bidsize2[idx] = k.BidSize2;
                asksize3[idx] = k.AskSize3;
                bidsize3[idx] = k.BidSize3;
                asksize4[idx] = k.AskSize4;
                bidsize4[idx] = k.BidSize4;
                asksize5[idx] = k.AskSize5;
                bidsize5[idx] = k.BidSize5;


            }
            return true;
        }
    }


    /// <summary>
    /// track only bid price
    /// </summary>
    public class BidTracker : GenericTracker<decimal>, GenericTrackerDecimal, GotTickIndicator
    {
        public BidTracker() : base("BID") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        public int addindex(string txt, decimal v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.HasBid()) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.BidPrice;
        }
    }
    /// <summary>
    /// track only ask price
    /// </summary>
    public class AskTracker : GenericTracker<decimal>, GenericTrackerDecimal, GotTickIndicator
    {
        public AskTracker() : base("ASK") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        public int addindex(string txt, decimal v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.HasAsk()) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.AskPrice;
        }
    }
    /// <summary>
    /// track only last price
    /// </summary>
    public class LastTracker : GenericTracker<decimal>, GenericTrackerDecimal, GotTickIndicator
    {
        public LastTracker() : base("LAST") { }
        public decimal getvalue(int idx) { return this[idx]; }
        public decimal getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, decimal v) { this[idx] = v; }
        public int addindex(string txt, decimal v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.IsTrade()) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.Trade;
        }
    }

    /// <summary>
    /// track only last trade size
    /// </summary>
    public class SizeTracker : GenericTracker<int>, GenericTrackerInt, GotTickIndicator
    {
        public SizeTracker() : base("SIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { this[idx] = v; }
        public int addindex(string txt, int v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.IsTrade()) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.Size;
        }
    }

    /// <summary>
    /// track only last bid size
    /// </summary>
    public class BidSizeTracker : GenericTracker<int>, GenericTrackerInt, GotTickIndicator
    {
        public BidSizeTracker() : base("BIDSIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { this[idx] = v; }
        public int addindex(string txt, int v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.HasBid()) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.BidSize;
        }
    }

    /// <summary>
    /// track only last ask size
    /// </summary>
    public class AskSizeTracker : GenericTracker<int>, GenericTrackerInt, GotTickIndicator
    {
        public AskSizeTracker() : base("ASKSIZE") { }
        public int getvalue(int idx) { return this[idx]; }
        public int getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, int v) { this[idx] = v; }
        public int addindex(string txt, int v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            if (!k.HasAsk()) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.AskSize;
        }
    }
    /// <summary>
    /// whether last tick in given symbol was a trade
    /// </summary>
    public class IsTradeTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsTradeTracker() : base("ISTRADE") {}
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.Symbol);
            this[idx] = k.IsTrade();
        }
    }
    /// <summary>
    /// whether last tick in given symbol had bid information
    /// </summary>
    public class IsBidTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsBidTracker() : base("ISBID") { }
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.Symbol);
            this[idx] = k.HasBid();
        }
    }
    /// <summary>
    /// whether last tick in given symbol had ask information
    /// </summary>
    public class IsAskTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsAskTracker() : base("ISASK") { }
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.Symbol);
            this[idx] = k.HasAsk();
        }
    }

}
