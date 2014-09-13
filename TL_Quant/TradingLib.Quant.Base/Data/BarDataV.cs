using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Quant.Base
{

    /// <summary>
    /// 以Bar为单位重新组织了数据
    /// </summary>
    [Serializable]
    public class QListBar:IBarData,IEnumerable,IEnumerable<Bar>
    {
        BarDataV _data;
        public QListBar(BarDataV data)
        {
            _data = data;
        }

        /// <summary>
        /// 获得某个序号的Bar
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Bar this[int index]
        {
            get { return _data.getBar(index);}
        }
        public Bar this[DateTime time]
        {
            get{return _data[time];}
        }
        public int Count { get { return _data.Count; } }

        public Bar First { get { return _data.getBar(0); } }

        public Bar Last { get { 
            
            if(_data.PartialBar!=null) return _data.PartialBar;
            
            else
                return _data.getBar(_data.Count - 1); } 
        }

        public Bar LookBack(int nBars)
        { 
            if (nBars < 0)
                return Last;
            if (nBars > (Count - 1))
                return First;
            return _data.getBar((Count - 1) - nBars);
        }

        public bool HasPartialItem
        {
            get { return _data.PartialBar !=null; }
        }

        #region IBarData接口

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Bar> GetEnumerator()
        {
            int max =_data.Count;
           
            for (int i = 0; i < max; i++)
                yield return _data.getBar(i);
        }
        public Security Security { get { return _data.Security; } }



        /// <summary>
        /// 通过字段open,high,low,close等返回ISeries用于指标计算的Input
        /// 速度慢于直接通过DataType进行访问 这里可以通过修改 生成hashtable的形式来储存
        /// </summary>
        /// <param name="seriesname"></param>
        /// <returns></returns>
        public ISeries this[string seriesname]
        {
            get
            {
                string upname = seriesname.ToUpper();

                switch (upname)
                {
                    case "DATE":
                        return this[BarDataType.Date];
                    case "OPEN":
                        return this[BarDataType.Open];
                    case "HIGH":
                        return this[BarDataType.High];
                    case "LOW":
                        return this[BarDataType.Low];
                    case "CLOSE":
                        return this[BarDataType.Close];
                    case "VOLUME":
                        return this[BarDataType.Volume];
                    default:
                        throw new Exception();
                }
            }
        }

        //将bardataseries进行内存缓存，这样可以防止多次请求多次生成
        Dictionary<BarDataType,ISeries> tmp = new Dictionary<BarDataType,ISeries>();
        public ISeries this[BarDataType type]
        {
            get
            {
                ISeries outseries = null;
                if (tmp.TryGetValue(type, out outseries))
                {
                    return outseries;
                }
                else
                {
                    tmp.Add(type, new BarDataV2BarElementISeries(_data, type));
                    return tmp[type];
                }
                

                
            }
        }

        #endregion

    }
    /// <summary>
    /// 中间数据转换接口
    /// </summary>
    [Serializable]
    public class BarDataV2BarElementISeries:ISeries
    {
        BarDataV _data;
        BarDataType _datatype;

        public Security Secuirty { get { return _data.Security; } }
        QList<double> _datalist = null;//封装了QList 进行的数据检查 这里的数据封装就不用再进行数据检查
        public BarDataV2BarElementISeries(BarDataV data,BarDataType type)
        {
            _data = data;
            _datatype = type;
            //在进行封装的时候 初始化就根据datatype获得对应的数据列 避免数据访问是进行switch datatype
            switch (_datatype)
                {
                    case BarDataType.Date:
                        _datalist = _data.oadatetime;
                        break;
                    case BarDataType.Open:
                        _datalist = _data.open;
                        break;
                    case BarDataType.High:
                        _datalist = _data.high;
                        break;
                    case BarDataType.Low:
                        _datalist =  _data.low;
                        break;
                    case BarDataType.Close:
                        _datalist =  _data.close;
                        break;
                    case BarDataType.Volume:
                        _datalist = _data.vol;
                        break;
                    default:
                        break;
                }
            //初始化Bar数据对应的ISeries信息
           
        }

        public ISeriesChartSettings ChartSettings
        {
            get
            {
                throw new NotSupportedException();
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public double LookBack(int nBars)
        {
            return _datalist.LookBack(nBars);
        }

        public double this[int index]
        {
            get
            {
                return _datalist[index];
            }
        }
     
        public int Count
        {
            get
            {
                return _datalist.Count;
            }
        }
        public double Last
        {
            get
            {
                return _datalist.Last;
            }
        }
        //请求较少
        public double First
        {
            get
            {
                return _datalist.First;
            }
        }
        //数据绘图时进行请求
        public double[] Data
        {
            get
            {
                return this._datalist.ToArray();
            }
        }
        
    }
    /// <summary>
    /// 向量化的BarData数据储存 这里需要实现一些方便简单的接口进行封装
    /// ...等待系统总结时,进行完善
    /// BarDataV向量化储存数据是系统底层储存Bar数据的数据结构,外层绘图,指标运算均调用这里的数据
    /// 
    /// </summary>
    [Serializable]
    public class BarDataV:IOwnedDataSerializable
    {
        public BarDataV(Security sec,BarFrequency frequency)
        {
            security = sec;
            freq = frequency;
            //this.Add();//增加一条空的数据条目 用于储存partialbar
        }
        #region 数据结构
        //合约
        Security security;
        public Security Security { get { return security; } }
       

        BarFrequency freq;
        public BarFrequency BarFrequency { get { return freq; } }

        public QList<Double> open = new QList<double>();//
        public QList<Double> high = new QList<double>();//
        public QList<Double> low = new QList<double>();
        public QList<Double> close = new QList<double>();
        public QList<Double> ask = new QList<double>();
        public QList<Double> bid = new QList<double>();
        public QList<Double> vol = new QList<double>();
        public QList<Double> oi = new QList<double>();
        public QList<DateTime> datetime = new QList<DateTime>();//DateTime格式的时间
        public QList<double> oadatetime = new QList<double>();//OADateTime格式的时间
        //Fast Time Value
        public QList<Int32> _time = new QList<int>();//Bar的当前时间
        public QList<Int32> date = new QList<int>();//Bar的日期
        public QList<Int32> time = new QList<int>();//Bar的开始时间

        public Dictionary<DateTime, int> datetimeBarMap = new Dictionary<DateTime, int>();

        public QList<double> OpenList { get { return open; } }
        public QList<double> HighList { get { return high; } }
        public QList<double> LowList { get { return low; } }
        public QList<double> CloseList { get { return close; } }
        public QList<double> AskList { get { return ask; } }
        public QList<double> BidList { get { return bid; } }
        public QList<double> VolList { get { return vol; } }
        public QList<double> OpenInterestList { get { return oi; } }
        public QList<DateTime> DateTimeList { get { return datetime; } }
        public QList<double> OADateTimeList { get { return oadatetime; } }
        #endregion

        #region SerializeOwnedData接口
        public void SerializeOwnedData(SerializationWriter writer, object context)
        {
            writer.WriteObject(security);
            writer.WriteObject(freq);
            writer.WriteObject(open);
            writer.WriteObject(high);
            writer.WriteObject(low);
            writer.WriteObject(close);
            writer.WriteObject(vol);
            writer.WriteObject(oi);
            writer.WriteObject(datetime);
            writer.WriteObject(oadatetime);
            writer.WriteObject(_time);
            writer.WriteObject(date);
            writer.WriteObject(time);
            writer.WriteObject(datetimeBarMap);
        }

        public void DeserializeOwnedData(SerializationReader reader, object context)
        {

            security = (Security)reader.ReadObject();
            freq = (BarFrequency)reader.ReadObject();
            open = (QList<Double>)reader.ReadObject();
            high = (QList<Double>)reader.ReadObject();
            low = (QList<Double>)reader.ReadObject();
            close = (QList<Double>)reader.ReadObject();
            open = (QList<Double>)reader.ReadObject();
            vol = (QList<Double>)reader.ReadObject();
            oi = (QList<Double>)reader.ReadObject();
            datetime = (QList<DateTime>)reader.ReadObject();
            oadatetime = (QList<Double>)reader.ReadObject();
            _time = (QList<Int32>)reader.ReadObject();
            date = (QList<Int32>)reader.ReadObject();
            time = (QList<Int32>)reader.ReadObject();
            datetimeBarMap = (Dictionary<DateTime, int>)reader.ReadObject();
        }

        #endregion


        public QList<double> GetBarElement(BarDataType type)
        {
            switch (type)
            {
                case BarDataType.Close:
                    return close;
                case BarDataType.Date:
                    return oadatetime;
                case BarDataType.High:
                    return high;
                case BarDataType.Low:
                    return low;
                case BarDataType.Open:
                    return open;
                case BarDataType.Volume:
                    return vol;
                default:
                    return null;
            }
        }
        public double[] Open() { return open.ToArray(); }
        public double[] High() { return high.ToArray(); }
        public double[] Low() { return low.ToArray(); }
        public double[] Close() { return close.ToArray(); }
        public double[] Ask() { return ask.ToArray(); }
        public double[] Bid() { return bid.ToArray(); }
        public double[] Vol() { return vol.ToArray(); }
        //public double[] DoubleVol() { return Calc.taprep(Vol(), true); }
        public int[] Date() { return date.ToArray(); }
        public int[] Time() { return time.ToArray(); }

        //新增日期
        public double[] oaDateTime() { return oadatetime.ToArray(); }

        /// <summary>
        /// 遍历生成所有对应的Bar
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            int max = close.Count;
            string sym = security.Symbol;
            for (int i = 0; i < max; i++)
                yield return getBar(i);
        }

        public Bar getBar(int i)
        {
            return new BarImpl((decimal)open[i], (decimal)high[i], (decimal)low[i], (decimal)close[i], (long)vol[i], (long)oi[i], (decimal)ask[i], (decimal)bid[i], date[i], time[i], security.Symbol, freq.Interval);
        }

        /// <summary>
        /// 新增加一个数据条目
        /// </summary>
        public void Add()
        {
            open.Add(0);
            high.Add(0);
            low.Add(0);
            close.Add(0);

            ask.Add(0);
            bid.Add(0);

            vol.Add(0);
            oi.Add(0);
            datetime.Add(DateTime.Now);
            oadatetime.Add(0);

            _time.Add(0);
            time.Add(0);
            date.Add(0);
            
        }
        /// <summary>
        /// 数据是否有效
        /// </summary>
        public bool isValid { get { return (security.isValid) && (close.Count() > 0); } }

        /// <summary>
        /// 最老的一个数据
        /// </summary>
        public Bar First
        {
            get { return getBar(0); }
        }
        /// <summary>
        /// 最近一个Bar
        /// </summary>
        public Bar Last
        {
            get { return getBar(this.Count - 1); }
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int Count
        {
            get { return close.Count; }
        }
        /// <summary>
        /// 回溯多少个间隔
        /// </summary>
        /// <param name="nBars"></param>
        /// <returns></returns>
        public Bar LookBack(int nBars)
        {
            if (nBars < 0)
                return Last;
            if (nBars > (Count - 1))
                return First;
            return getBar((Count - 1) - nBars);
        }
        public Bar this[int index]
        {
            get {
                return getBar(index);
            }
        }
        public void Add(Bar bar)
        {
            open.Add(bar.Open);
            high.Add(bar.High);
            low.Add(bar.Low);
            close.Add(bar.Close);

            ask.Add(bar.Ask);
            bid.Add(bar.Bid);

            vol.Add(bar.Volume);
            oi.Add(bar.OpenInterest);
            datetime.Add(bar.BarStartTime);
            oadatetime.Add(bar.BarStartTime.ToOADate());

            _time.Add(bar.time);
            time.Add(bar.Bartime);
            date.Add(bar.Bardate);

            datetimeBarMap.Add(bar.BarStartTime, date.Count - 1);
        }

        public void FillBars(List<Bar> barlist)
        {
            foreach (Bar b in barlist)
            {
                this.Add(b);
            }
            
        }

        public void UpdateLastBar(Bar bar)
        {
            int last = this.Count - 1;
            open[last] = bar.Open;
            high[last] = bar.High;
            low[last] = bar.Low;
            close[last] = bar.Close;

            ask[last] = bar.Ask;
            bid[last] = bar.Bid;

            vol[last] = bar.Volume;
            oi[last] = bar.OpenInterest;

            //datetime[last] =
            //oadatetime[last] = bar.oa

            _time[last] = bar.time;
            time[last] = bar.Bartime;
            date[last] = bar.Bardate;

            
        }

        public int BarStartTime2Index(DateTime starttime)
        { 
            int id = 0;
            if (datetimeBarMap.TryGetValue(starttime, out id))
            {
                return id;
            }
            else
                return -1;
        }
        public Bar this[DateTime time]
        {
            get
            {
                int id = 0;
                if (datetimeBarMap.TryGetValue(time, out id))
                {
                    return this[id];
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 当前最新的Bar该Bar没有结束,正在更新中
        /// </summary>
        Bar partialbar = null;
        public Bar PartialBar
        {
            get
            {
                return partialbar;
            }
            set { partialbar = value; }
        }
    }
    [Serializable]
    public class BarDataV2PartialBar : Bar
    {
        BarDataV _data;
        /// <summary>
        /// 将BarDataV的最后一格数据封装成Bar接口 用于BarGenerator进行数据更新 这样可以避免频繁更新BarDataV
        /// </summary>
        /// <param name="bardatav"></param>
        public BarDataV2PartialBar(BarDataV bardatav)
        {
            _data = bardatav;
        }

        public long Volume {

            get { return (long)_data.vol.Last; }
            set {_data.vol.Last = (double)value;}
        }

        public double Open {
            get { return _data.open.Last; }
            set { _data.open.Last = value; }
        }

        public double High {
            get { return _data.high.Last; }
            set { _data.high.Last = value; }
        }

        public double Low {
            get { return _data.low.Last; }
            set { _data.low.Last = value; ; }
        }
        public double Close {
            get { return _data.close.Last; }
            set { _data.close.Last = value; }
        }

        public long OpenInterest {
            get { return (long)_data.oi.Last; }
            set { _data.oi.Last = value; }
        }
        public bool isValid { get { return true; } }

        public int Bardate { get { return _data.date.Last; } set { _data.date.Last = value; } }
        public int Bartime { get{return _data.time.Last;} set { throw new NotFiniteNumberException(); } }

        public int time { get { return _data._time.Last; } set { _data._time.Last = value; } }

        DateTime _starttime=DateTime.MinValue;
        public DateTime BarStartTime {
            get
            {
                return _starttime;
            }

            set
            {
                _starttime = value;
                _data.date.Last = Util.ToTLDate(_starttime);
                _data.time.Last = Util.ToTLTime(_starttime);
                _data.oadatetime.Last = BarStartTime.ToOADate();
                _data._time.Last = Util.ToTLTime(_starttime);
                if (_data.datetimeBarMap.Keys.Contains(_starttime))
                {

                }
                else
                {
                    _data.datetimeBarMap.Add(_starttime, _data.Count - 1);
                }
            }
            }
        DateTime _endtime;
        public DateTime BarEndTime {
            get
            {
                return _endtime;
            }
            set
            {
                throw  new NotImplementedException();
            }
            }

        bool _empty = true;
        public bool EmptyBar
        {
            get
            {
                return _empty;
            }
            set
            {
                _empty = value;
            }
        }
        public bool isNew { get; set; }

        public int Interval {
            get
            {
                return _data.BarFrequency.Interval;
            }
            set
            {
                throw new NotImplementedException();
            }
            }

        public double Ask { get; set; }
        public double Bid { get; set; }

        public string Symbol {
            get
            {
                return _data.Security.Symbol;
            }
            set
            {
                throw new NotImplementedException();
            }
            
            }

        public Bar Clone()
        {
            return new BarImpl();
        }

        public override string ToString() { return "OHLC (" + Bardate + "  " + Bartime.ToString() +" " + _data._time.Last + ") " + Open.ToString("F2") + "," + High.ToString("F2") + "," + Low.ToString("F2") + "," + Close.ToString("F2") + "," + Volume.ToString(); }
        
    }
}
