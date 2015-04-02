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

            if (NewTxt != null)
                NewTxt(txt, idx);
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
        public bool HasAsk(string sym) { return ask[sym] != 0; }
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

            foreach (string sym in syms)
            {
                Tick k = this[sym];
                if (k != null && k.isValid)
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
                if (idx < 0) return new TickImpl();
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
            if (k.isTrade)
            {
                last[idx] = k.Trade;
                ex[idx] = k.Exchange;
                ts[idx] = k.Size;
            }
            if (k.hasAsk)
            {
                ask[idx] = k.AskPrice;
                oe[idx] = k.AskExchange;
                os[idx] = k.AskSize;
            }
            if (k.hasBid)
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
            if (!k.hasBid) return;
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
            if (!k.hasAsk) return;
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
            if (!k.isTrade) return;
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
            if (!k.isTrade) return;
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
            if (!k.hasBid) return;
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
            if (!k.hasAsk) return;
            int idx = addindex(k.Symbol);
            this[idx] = k.AskSize;
        }
    }
    /// <summary>
    /// whether last tick in given symbol was a trade
    /// </summary>
    public class IsTradeTracker : GenericTracker<bool>, GenericTrackerBool, GotTickIndicator
    {
        public IsTradeTracker() : base("ISTRADE") { }
        public bool getvalue(int idx) { return this[idx]; }
        public bool getvalue(string txt) { return this[txt]; }
        public void setvalue(int idx, bool v) { this[idx] = v; }
        public int addindex(string txt, bool v) { return getindex(txt); }

        public void GotTick(Tick k)
        {
            int idx = addindex(k.Symbol);
            this[idx] = k.isTrade;
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
            this[idx] = k.hasBid;
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
            this[idx] = k.hasAsk;
        }
    }

}