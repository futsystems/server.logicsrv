using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class BarFrequency
    {

        /// <summary>
        /// 间隔数
        /// </summary>
        public int Interval
        {
            get { return _interval; }
            set { _interval = value; }

        }
        int _interval;

        /// <summary>
        /// 类别
        /// </summary>
        public BarInterval Type
        {
            get
            {
                return _type;
            }
            set { _type = value; }
        }
        BarInterval _type;

        TimeSpan timespan;
        public TimeSpan TimeSpan
        {
            get
            {
                if (timespan != null)
                    return timespan;
                else
                    timespan = new TimeSpan(0, 0, _interval);
                return timespan;
            }
        }

        public BarFrequency(BarInterval interval)
        {
            switch (interval)
            {
                case BarInterval.Day:
                case BarInterval.FifteenMin:
                case BarInterval.FiveMin:
                case BarInterval.Hour:
                case BarInterval.Minute:
                case BarInterval.ThreeMin:
                case BarInterval.ThirtyMin:
                    {
                        _type = BarInterval.CustomTime;
                        _interval = (int)interval;
                    }
                    break;
                default:
                    _type = BarInterval.CustomTime;
                    _interval = 60;
                    break;
            }
            timespan = new TimeSpan(0, 0, _interval);
            return;
        }

        public BarFrequency(BarInterval type, int interval)
        {
            _interval = interval;
            switch (_type)
            {
                case BarInterval.CustomTicks:
                case BarInterval.CustomTime:
                case BarInterval.CustomVol:
                    _type = type;
                    break;
                default:
                    _type = BarInterval.CustomTime;
                    break;
            }
            timespan = new TimeSpan(0, 0, _interval);
            return;
        }

        public override bool Equals(object obj)
        {
            if (obj is BarFrequency)
            {
                BarFrequency freq = obj as BarFrequency;
                return freq.Interval == this.Interval && freq.Type == this.Type;
            }
            return false;
        }

        public override string ToString()
        {
            return "Freq Type:" + this.Type.ToString() + " Interval:" + this.Interval.ToString();
        }
        public string ToUniqueId()
        {
            return string.Format("{0}-{1}", this.Interval, this.Type);
        }

        public override int GetHashCode()
        {
            return this.ToUniqueId().GetHashCode();
        }
    }
}
