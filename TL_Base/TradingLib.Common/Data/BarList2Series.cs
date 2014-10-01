using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

using TradingLib.Common;

namespace TradingLib.Common
{
    /*
    //将barlist wrapper成 ISeries便于指标系统进行运算与调用
    public class BarList2Series : ISeries
    {
        //barlist 已经包含了对应的symbol barlisttracker则是对不同symbol的bar数据的封装
        private BarListImpl _barlist;
        //默认为分钟数据
        private BarInterval _defaultinterval = BarInterval.Minute;
        //默认为收盘价
        private BarDataType _datatype = BarDataType.Close;

        public BarList2Series(BarList bl)
        {
            _barlist = bl as BarListImpl;
        }

        public BarList2Series(BarList bl, BarDataType datatype)
        {
            _barlist = bl as BarListImpl;
            _datatype = datatype;
        }
        public BarList2Series(BarList bl, BarDataType datatype, BarInterval interval)
        {
            _barlist = bl as BarListImpl;
            _datatype = datatype;
            _defaultinterval = interval;
        }

        public double LookBack(int n)
        {
            //return _barlist
            //int count = 
            return this[Count - n - 1];
        }

        public int Count
        {
            get { return _barlist.Close(_defaultinterval).Length; }
        }

        public double[] Data
        {
            get
            {

                switch (_datatype)
                {
                    case BarDataType.Close:
                        return Calc.Decimal2Double(_barlist.Close(_defaultinterval));

                    case BarDataType.High:
                        return Calc.Decimal2Double(_barlist.High(_defaultinterval));

                    case BarDataType.Low:
                        return Calc.Decimal2Double(_barlist.Low(_defaultinterval));

                    case BarDataType.Open:
                        return Calc.Decimal2Double(_barlist.Open(_defaultinterval));

                    //case BarDataType.Volumne:
                    //    return (_barlist.Vol(_defaultinterval));

                    default:
                        return new double[] { 0 };
                    //break;
                }
            }

        }
        public double this[int index]
        {
            get
            {
                switch (_datatype)
                {
                    case BarDataType.Close:
                        return (double)_barlist[index, _defaultinterval].Close;

                    case BarDataType.High:
                        return (double)_barlist[index, _defaultinterval].High;

                    case BarDataType.Low:
                        return (double)_barlist[index, _defaultinterval].Low;

                    case BarDataType.Open:
                        return (double)_barlist[index, _defaultinterval].Open;

                    case BarDataType.Volume:
                        return (double)_barlist[index, _defaultinterval].Volume;

                    default:
                        return 0.0;
                    //break;
                }
            }
        }

        public double Last
        {
            get
            {
                if (Count > 0)
                    return this[Count - 1];
                return 0;
            }
        }

        public double First
        {
            get
            {
                if (Count > 0)
                    return this[0];
                return 0;

            }
        }

    }

    /// <summary>
    /// 将barlist转换成ISeries封装Bar格式
    /// </summary>
    public class BarSeries:ISeries
    {
        private BarDataType _datatype = BarDataType.Close;

        public BarSeries(BarList bl) : this(bl, BarInterval.Minute) { }
        public BarSeries(BarList bl, BarInterval interval) : this(new BarList2Series(bl, BarDataType.Open, interval), new BarList2Series(bl, BarDataType.High, interval), new BarList2Series(bl, BarDataType.Low, interval), new BarList2Series(bl, BarDataType.Close, interval), new BarList2Series(bl, BarDataType.Volume, interval)) { }
        public BarSeries(ISeries open,ISeries high,ISeries low,ISeries close,ISeries volume)
        {
            this.Open = open;
            this.High = high;
            this.Low = low;
            this.Close = close;
            this.Volume = volume;
        }
        public ISeries Open;
        public ISeries High;
        public ISeries Low;
        public ISeries Close;
        public ISeries Volume;

        //public string ToString()

        public double LookBack(int n)
        {
            //return _barlist
            //int count = 
            return this[Count - n - 1];
        }

        public int Count
        {
            get { return Close.Count; }
        }
        public double this[int index]
        {
            get
            {
                switch (_datatype)
                {
                    case BarDataType.Close:
                        return Close[index];
                    case BarDataType.High:
                        return High[index];
                    case BarDataType.Low:
                        return Low[index];
                    case BarDataType.Open:
                        return Open[index];
                    case BarDataType.Volume:
                        return Volume[index];
                    default:
                        return 0.0;
                }
            }
        
        }
        public double[] Data
        {
            get
            {
                switch (_datatype)
                {
                    case BarDataType.Close:
                        return Close.Data;
                    case BarDataType.High:
                        return High.Data;
                    case BarDataType.Low:
                        return Low.Data;
                    case BarDataType.Open:
                        return Open.Data;
                    case BarDataType.Volume:
                        return Volume.Data;
                    default:
                        return new double[] { 0 };
                }
            }

        }
        public double Last
        {
            get
            {
                if (Count > 0)
                    return this[Count - 1];
                return 0;
            }
        }

        public double First
        {
            get
            {
                if (Count > 0)
                    return this[0];
                return 0;

            }
        }
        //public ISeries
    }
        
        
    **/
}
