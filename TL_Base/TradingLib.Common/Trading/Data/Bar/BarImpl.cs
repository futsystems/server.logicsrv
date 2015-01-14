using System;
using System.Collections.Generic;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// A single bar of price data, which represents OHLC and volume for an interval of time.
    /// </summary>
    [Serializable]
    public class BarImpl : GotTickIndicator, TradingLib.API.Bar
    {
        public void GotTick(Tick k) { newTick(k); }
        string _sym = "";
        public string Symbol { get { return _sym; } set { _sym = value; } }
        private double h = double.MinValue;//最高价
        private double l = double.MaxValue;//最低价
        private double o = 0;//开盘价
        private double c = 0;//收盘价
        private double ask = 0;
        private double bid = 0;

        private long v = 0;//成交量
        private long oi = 0;//持仓

        private int tradesinbar = 0;
        private bool _new = false;
        private int units = 300;
        private int _time = 0;//当前更新的真实时间
        private int bardate = 0;
        private bool DAYEND = false;
        public int time { get { return _time; } set { _time = value; } }
        public bool DayEnd { get { return DAYEND; } }
        //ulong lHigh { get { return h; } }
        //ulong lLow { get { return l; } }
        //ulong lOpen { get { return o; } }
        //ulong lClose { get { return c; } }

        //ulong lAsk { get { return ask; } }
        //ulong lBid { get { return bid; } }


        public double High { get { return h; } set { h =value; } }
        public double Low { get { return l; } set { l = value; } }
        public double Open { get { return o; } set { o = value; } }
        public double Close { get { return c; } set { c = value; } }
        public long Volume { get { return v; } set { v = value; } }
        public long OpenInterest { get { return oi; } set { oi = value; } }

        public double Ask { get { return ask; } set { ask = value; } }
        public double Bid { get { return bid; } set { bid = value; } }


        public bool isNew { get { return _new; } set { _new = value; } }
        public bool isValid { get { return (h >= l) && (o != 0) && (c != 0); } }
        public int TradeCount { get { return tradesinbar; } }

        public BarImpl() : this(BarInterval.FiveMin) { }

        public int Interval { get { return units; } set { units = value; } }


        DateTime _starttime = DateTime.MinValue;
        DateTime _endtime = DateTime.MaxValue;
        bool _empty=false;

        public DateTime BarStartTime { get { return _starttime; } set { _starttime = value;bardate = Util.ToTLDate(value); _time = Util.ToTLTime(value); } }
        public DateTime BarEndTime { get { return _endtime; } set { _endtime = value; } }

        public bool EmptyBar { get { return _empty; } set { _empty = value; } }


        //public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, int date, int time, string symbol) : this(open, high, low, close, vol, date, time, symbol) { }
        public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, int date, int time, string symbol, int interval)
            : this(open, high, low, close, vol, 0, date, time, symbol, interval)
        { }
        public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, long oi,int date, int time, string symbol, int interval)
             :this(open, high, low, close, vol, 0,0,0,date, time, symbol, interval)
        {
        }
        public BarImpl(decimal open, decimal high, decimal low, decimal close, long vol, long oi,decimal ask,decimal bid,int date, int time, string symbol, int interval)
        {
            if (open < 0 || high < 0 || low < 0 || close < 0)
            {
                return;
            }
            else
            {
                units = interval;
                h = (double)high;
                o = (double)open;
                l = (double)low;
                c = (double)close;
                this.ask = (double)ask;
                this.bid = (double)bid;
                v = vol;
                bardate = date;
                _time = time;
                _starttime = Util.ToDateTime(date, time);
                _endtime = _starttime.AddSeconds(interval);
                _sym = symbol;
            }
        }
        public Bar Clone()
        {
            return new BarImpl(this);
        }
        public BarImpl(BarImpl b)
        {
            v = b.Volume;
            oi = b.OpenInterest;

            h = b.Open;
            l = b.Low;
            o = b.Open;
            c = b.Close;

            DAYEND = b.DAYEND;
            _time = b._time;
            bardate = b.bardate;

            Interval = b.Interval;
            BarStartTime = b.BarStartTime; ;
            BarEndTime = b.BarEndTime;//BarStartTime
        }

        public BarImpl(Bar b)
        {
            v = b.Volume;
            oi = b.OpenInterest;

            h = b.Open;
            l = b.Low;
            o = b.Open;
            c = b.Close;

            //DAYEND = b.;
            _time = b.time;//Bar的当前实际日期  BarTime是通过b.time计算而来
            bardate = b.Bardate;//Bar的日期

            Interval = b.Interval;//间隔units
            BarStartTime = b.BarStartTime;//结束时间
            BarEndTime = b.BarEndTime;//BarStartTime
        }


        /// <summary>
        /// 生成一个Interval(units)为多少个间隔的Bar
        /// </summary>
        /// <param name="interval"></param>
        public BarImpl(int interval)
        {
            units = interval;
        }
        public BarImpl(BarInterval tu) 
            :this((int)tu)
        {
            
        }
        /// <summary>
        /// 对应的当日Bar的开始时间,通过_time进行计算
        /// </summary>
        public int Bartime 
        { 
            get 
            { 
                // get num of seconds elaps
                int elap = Util.FT2FTS(_time); //计算该时刻的时间间隔
                // get remainder of dividing by interval
                int rem = elap % Interval;//获得对应余数
                // get datetime
                DateTime dt = Util.TLD2DT(bardate);
                // add rounded down result
                dt = dt.AddSeconds(elap-rem);//所有时间间隔-余数 就为Bar的开始时间
                // conver back to normal time
                int bt = Util.ToTLTime(dt);
                return bt;
            }
            //set { _time = value; }
        }
        public int Bardate { get { return bardate; } set { bardate = value; } }
        /// <summary>
        /// bt是用来计算一天中的第几根Bar是用序号来计算的
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        private int bt(int time) 
        {
            // get time elapsed to this point
            int elap = Util.FT2FTS(time);
            // get seconds per bar
            int secperbar = Interval;
            // get number of this bar in the day for this interval
            int bcount = (int)((double)elap / secperbar);
            return bcount;
        }

        /// <summary>
        /// Accepts the specified tick.
        /// </summary>
        /// <param name="t">The tick you want to add to the bar.</param>
        /// <returns>true if the tick is accepted, false if it belongs to another bar.</returns>
        public bool newTick(Tick k)
        {
            TickImpl t = (TickImpl)k;
            if (_sym == "") _sym = t.Symbol;
            if (_sym != t.Symbol) throw new InvalidTick();
            //if (_time == 0) { _time = t.time; bardate = t.date; }
            if (_time == 0) { _time = bt(t.Time); bardate = t.Date; }
            if (bardate != t.Date) DAYEND = true;
            else DAYEND = false;
            // check if this bar's tick//如果该bar不在改时间段中则return false
            if ((bt(t.Time) != _time) || (bardate != t.Date)) return false; 
            // if tick doesn't have trade or index, ignore
            if (!t.isTrade && !t.isIndex) return true; //我们只能通过trade来进行bar的形成，没有成交的ask bid不能作为bar数据
            tradesinbar++; // count it 累计该bar内的trade trade/ask/bid
            _new = tradesinbar == 1;//是否是新bar的标准  tradesinbar==1
            // only count volume on trades, not indicies
            if (!t.isIndex) v += t.Size; // add trade size to bar volume 如果不是质数 则bar的volume通过trades来进行累加
            //更新bar的o h l c 数据
            if (o == 0) o = t._trade;//如果open为0 赋初值
            if (t._trade > h) h = t._trade;
            if (t._trade < l) l = t._trade;
            c = t._trade;
            return true;
        }
        public override string ToString() { return "OHLC (" + bardate +" "+_time + "  " +Bartime.ToString()+") " + Open.ToString("F2") + "," + High.ToString("F2") + "," + Low.ToString("F2") + "," + Close.ToString("F2") + ","+Volume.ToString(); }
        /// <summary>
        /// Create bar object from a CSV file providing OHLC+Volume data.
        /// 从csv获得bar数据
        /// </summary>
        /// <param name="record">The record in comma-delimited format.</param>
        /// <returns>The equivalent Bar</returns>
        public static Bar FromCSV(string record) { return FromCSV(record, string.Empty, (int)BarInterval.Day); }
        public static Bar FromCSV(string record,string symbol,int interval)
        {
            // google used as example
            string[] r = record.Split(',');
            if (r.Length < 6) return null;
            DateTime d = new DateTime();
            try
            {
                d = DateTime.Parse(r[0], System.Globalization.CultureInfo.InvariantCulture);
            }
            catch (System.FormatException) { return null; }
            int date = (d.Year*10000)+(d.Month*100)+d.Day;
            decimal open = Convert.ToDecimal(r[1],System.Globalization.CultureInfo.InvariantCulture);
            decimal high = Convert.ToDecimal(r[2], System.Globalization.CultureInfo.InvariantCulture);
            decimal low = Convert.ToDecimal(r[3], System.Globalization.CultureInfo.InvariantCulture);
            decimal close = Convert.ToDecimal(r[4], System.Globalization.CultureInfo.InvariantCulture);
            long vol = Convert.ToInt64(r[5], System.Globalization.CultureInfo.InvariantCulture);
            return new BarImpl(open,high,low,close,vol,date,0,symbol,interval);
        }

        /// <summary>
        /// 序列化bar
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static string Serialize(Bar b)
        {
            const char d = ',';
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(b.Open.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.High.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.Low.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.Close.ToString(System.Globalization.CultureInfo.InvariantCulture));
            sb.Append(d);
            sb.Append(b.Volume);
            sb.Append(d);
            sb.Append(b.Bardate);
            sb.Append(d);
            sb.Append(b.time);
            sb.Append(d);
            sb.Append(b.Symbol);
            sb.Append(d);
            sb.Append(b.Interval.ToString(System.Globalization.CultureInfo.InvariantCulture));
            
            return sb.ToString();
        }

        /// <summary>
        /// 反序列化bar
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static Bar Deserialize(string msg)
        {
            string[] r = msg.Split(',');
            decimal open = Convert.ToDecimal(r[0], System.Globalization.CultureInfo.InvariantCulture);
            decimal high = Convert.ToDecimal(r[1], System.Globalization.CultureInfo.InvariantCulture);
            decimal low = Convert.ToDecimal(r[2], System.Globalization.CultureInfo.InvariantCulture);
            decimal close = Convert.ToDecimal(r[3], System.Globalization.CultureInfo.InvariantCulture);
            long vol = Convert.ToInt64(r[4]);
            int date = Convert.ToInt32(r[5]);
            int time = Convert.ToInt32(r[6]);
            string symbol = r[7];
            int interval = Convert.ToInt32(r[8]);
            return new BarImpl(open, high, low, close, vol, date, time, symbol,interval);
        }

        /// <summary>
        /// convert a bar into an array of ticks
        /// 将bar转换成Tick数据 通过 4分法将o h l c 形成对应的 trade tick
        /// </summary>
        /// <param name="bar"></param>
        /// <returns></returns>
        
        public static Tick[] ToTick(Bar bar)
        {
            if (!bar.isValid) return new Tick[0];
            List<Tick> list = new List<Tick>();
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, (decimal)bar.Open,
(int)((double)bar.Volume / 4), string.Empty));
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
(decimal)bar.High, (int)((double)bar.Volume / 4), string.Empty));
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime, (decimal)bar.Low,
(int)((double)bar.Volume / 4), string.Empty));
            list.Add(TickImpl.NewTrade(bar.Symbol, bar.Bardate, bar.Bartime,
(decimal)bar.Close, (int)((double)bar.Volume / 4), string.Empty));
            return list.ToArray();
        }
        
        /// <summary>
        /// parses message into a structured bar request
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static BarRequest ParseBarRequest(string msg)
        {
            string[] r = msg.Split(',');
            BarRequest br  = new BarRequest();
            br.Symbol = r[(int)BarRequestField.Symbol];
            br.Interval = Convert.ToInt32(r[(int)BarRequestField.BarInt]);
            br.StartDate = int.Parse(r[(int)BarRequestField.StartDate]);
            br.StartTime = int.Parse(r[(int)BarRequestField.StartTime]);
            br.EndDate= int.Parse(r[(int)BarRequestField.EndDate]);
            br.EndTime = int.Parse(r[(int)BarRequestField.EndTime]);
            br.CustomInterval = int.Parse(r[(int)BarRequestField.CustomInterval]);
            br.ID = long.Parse(r[(int)BarRequestField.ID]);
            br.Client = r[(int)BarRequestField.Client];
            return br;
        }

        /// <summary>
        /// request historical data for today
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string BuildBarRequest(string symbol, BarInterval interval)
        {
            return BuildBarRequest(new BarRequest(symbol, (int)interval, Util.ToTLDate(), 0, Util.ToTLDate(), Util.ToTLTime(),string.Empty));
        }
        /// <summary>
        /// bar request for symbol and interval from previous date through present time
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="startdate"></param>
        /// <returns></returns>
        public static string BuildBarRequest(string symbol, BarInterval interval, int startdate)
        {
            return BuildBarRequest(new BarRequest(symbol, (int)interval, startdate, 0, Util.ToTLDate(), Util.ToTLTime(),string.Empty));
        }
        public static string BuildBarRequest(string symbol, int interval, int startdate)
        {
            return BuildBarRequest(new BarRequest(symbol, interval, startdate, 0, Util.ToTLDate(), Util.ToTLTime(), string.Empty));
        }
        /// <summary>
        /// builds bar request
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static string BuildBarRequest(BarRequest br)
        {
            string[] r = new string[] 
            {
                br.Symbol,
                br.Interval.ToString(),
                br.StartDate.ToString(),
                br.StartTime.ToString(),
                br.EndDate.ToString(),
                br.EndTime.ToString(),
                br.ID.ToString(),
                br.CustomInterval.ToString(),
                br.Client,
            };
            return string.Join(",", r);
            
        }

        //计算多少个bar对应的时间点
        public static DateTime DateFromBarsBack(int barsback, BarInterval intv) { return DateFromBarsBack(barsback, intv, DateTime.Now); }
        public static DateTime DateFromBarsBack(int barsback, BarInterval intv, DateTime enddate) { return DateFromBarsBack(barsback, (int)intv, enddate); }
        public static DateTime DateFromBarsBack(int barsback, int interval) { return DateFromBarsBack(barsback, interval, DateTime.Now); }
        public static DateTime DateFromBarsBack(int barsback, int interval, DateTime enddate)
        {
           return enddate.Subtract(new TimeSpan(0,0,interval*barsback));
        }
        //计算从某个时间以来有多少个bar
        public static int BarsBackFromDate(BarInterval interval, int startdate) { return BarsBackFromDate(interval, startdate, Util.ToTLDate()); }
        public static int BarsBackFromDate(BarInterval interval, int startdate, int enddate) { return BarsBackFromDate(interval, Util.ToDateTime(startdate, 0), Util.ToDateTime(enddate,Util.ToTLTime())); }
        public static int BarsBackFromDate(BarInterval interval, DateTime startdate, DateTime enddate)
        {
            double start2endseconds = enddate.Subtract(startdate).TotalSeconds;
            int bars = (int)((double)start2endseconds / (int)interval);
            return bars;
        }
        /// <summary>
        /// build bar request for certain # of bars back from present 获得自当前时间开始多少个bar
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="barsback"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string BuildBarRequestBarsBack(string sym, int barsback, int interval)
        {
            DateTime n = DateTime.Now;
            return BarImpl.BuildBarRequest(new BarRequest(sym, interval, Util.ToTLDate(BarImpl.DateFromBarsBack(barsback, interval, n)), Util.ToTLTime(BarImpl.DateFromBarsBack(barsback, interval, n)), Util.ToTLDate(n), Util.ToTLTime(n), string.Empty));
        }

        
    }

    public struct BarRequest
    {
        /// <summary>
        /// client making request
        /// </summary>
        public string Client;
        public int StartDate;
        public int EndDate;
        public int StartTime;
        public int EndTime;
        public int CustomInterval;
        public string Symbol;
        public int Interval;
        public long ID;
        public DateTime StartDateTime { get { return Util.ToDateTime(StartDate,StartTime); } }
        public DateTime EndDateTime { get { return Util.ToDateTime(EndDate, EndTime); } }
        public BarRequest(string symbol, int interval, int startdate, int starttime, int enddate, int endtime, string client)
        {
            Client = client;
            Symbol = symbol;
            Interval = interval;
            StartDate = startdate;
            StartTime = starttime;
            EndDate = enddate;
            EndTime = endtime;
            ID = 0;
            CustomInterval = 0;

        }

        public override string ToString()
        {
            return Symbol + " " + Interval + " " + StartDateTime + "->" + EndDateTime;
        }
        
    }


}
