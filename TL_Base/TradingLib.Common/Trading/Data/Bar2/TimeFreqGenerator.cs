using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using Common.Logging;

namespace TradingLib.Common
{
    /// <summary>
    /// 以时间为间隔的Bar数据生成器 通过CreateFrequencyGenerator获得具体的Bar生成器
    /// </summary>
    public class TimeFrequency : FrequencyPlugin
    {
        ILog logger = LogManager.GetLogger("TimeFrequency");
        TimeSpan _barLength;
        public TimeSpan BarLength { get { return _barLength; } }

        BarFrequency _freq;
        public override BarFrequency BarFrequency { get { return _freq; } }


        /// <summary>
        /// Determines whether two FrequencyPlugin instances are equal
        /// </summary>
        /// <param name="obj">FrequencyPlugin used for comparison</param>
        /// <returns>true if they are equal, otherwise false.</returns>
        public override bool Equals(object obj)
        {
            TimeFrequency timeFrequency = obj as TimeFrequency;
            return timeFrequency != null && this.BarLength == timeFrequency.BarLength;
        }

        /// <summary>
        /// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
        /// </summary>
        /// <returns>The hash code for this object.</returns>
        public override int GetHashCode()
        {
            return this.BarLength.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("Time:{0}s", this._freq.Interval);
        }
        /// <summary>
        /// 全复制
        /// </summary>
        /// <returns></returns>
        public override FrequencyPlugin Clone()
        {
            return (TimeFrequency)base.MemberwiseClone();
        }


        //int _comparecode;
        //public int CompareCode { get { return _comparecode; } }

        /// <summary>
        /// 初始化一个TimeFrequency对象
        /// </summary>
        /// <param name="freq"></param>
        public TimeFrequency(BarFrequency freq)
        {
            if (freq.Type != BarInterval.CustomTime)
            {
                throw new ArgumentException("TimeFrequency need Time Based BarFrequency");
            }
            _barLength = new TimeSpan(0, 0, freq.Interval);

            _freq = freq;
            //_comparecode = freq.Interval * 10000 + (int)BarFrequency.Type;//通过这种方式获得为唯一的comparecode
        }

        /// <summary>
        /// 获得对应的Bar生成器
        /// </summary>
        /// <returns></returns>
        public override IFrequencyGenerator CreateFrequencyGenerator()
        {
            return new TimeFreqGenerator(this.BarLength);
        }


        public static DateTime NextRoundedTime(DateTime date, TimeSpan period)
        {
            DateTime time = RoundTime(date, period);
            TimeSpan span = period;
            if (span.TotalDays > 1.0)
            {
                span = TimeSpan.FromDays(1.0);
            }
            while (RoundTime(time, period) <= date)
            {
                time = time.Add(span);
            }
            return RoundTime(time, period);
        }

        public static DateTime RoundTime(DateTime date, TimeSpan period)
        {
            DateTime time;
            if (period.TotalDays < 7.0)
            {
                time = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
            }
            else if (period.TotalDays < 30.0)
            {
                time = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0);
                while (time.DayOfWeek != DayOfWeek.Monday)
                {
                    time = time.AddDays(-1.0);
                }
            }
            else if (period.TotalDays < 365.0)
            {
                time = new DateTime(date.Year, date.Month, 1, 0, 0, 0);
                if (period.TotalDays == 30.0)
                {
                    return time;
                }
            }
            else
            {
                time = new DateTime(date.Year, 1, 1, 0, 0, 0);
                if (period.TotalDays == 365.0)
                {
                    return time;
                }
            }
            long num = date.Ticks - time.Ticks;
            long num2 = 0;
            if (period.Ticks != 0)
            {
                num2 = num % period.Ticks;
            }
            return new DateTime(date.Ticks - num2);
        }


        public override bool IsTimeBased { get { return true; } }

        /// <summary>
        /// 以时间为间隔的Bar数据生成器
        /// </summary>
        internal class TimeFreqGenerator:IFrequencyGenerator
        {
            ILog logger = LogManager.GetLogger("TimeFreqGenerator");

            bool _updated = false;
            TimeSpan _interval;
            BarGenerator _generator;

            int _units = 0;
            public TimeFreqGenerator(TimeSpan barLength)
            {
                this._interval = barLength;
                this._updated = false;
                this._units = (int)barLength.TotalSeconds;
            }

            /// <summary>
            /// 初始化
            /// </summary>
            /// <param name="symbol"></param>
            /// <param name="type"></param>
            public void Initialize(Symbol symbol, BarConstructionType type)
            {
                this._generator = new BarGenerator(symbol, new BarFrequency(BarInterval.CustomTime, this._units), type);
                this._generator.NewBar += new Action<SingleBarEventArgs>(_generator_NewBar);
                this._generator.NewTick += new Action<NewTickEventArgs>(_generator_NewTick);
            }

            public event Action<NewTickEventArgs> NewTickEvent;
            public event Action<SingleBarEventArgs> NewBarEvent;

            void _generator_NewTick(NewTickEventArgs obj)
            {
                if (NewTickEvent != null)
                    NewTickEvent(obj);
            }

            void _generator_NewBar(SingleBarEventArgs obj)
            {
                if (NewBarEvent != null)
                    NewBarEvent(obj);
            }


            public void ProcessBar(Bar bar)
            { 
            
            }

            public void ProcessTick(Tick k)
            {
                this.UpdateTime(k.Datetime);
                this._generator.ProcessTick(k);
            }

            public void UpdateTime(DateTime datetime)
            {
                DateTime round = TimeFrequency.RoundTime(datetime, this._interval);
                //没有处理过tick数据 则更新当前的round时间为当前Bar的开始时间
                if (!this._updated)
                {
                    logger.Debug(string.Format("DateTime:{0} SetBarStartTime:{1}", datetime, round));
                    this._generator.SetBarStartTime(round);
                    this._updated = true;
                }
                //如果roundtime大于PartialBar的起始时间 越过了一个Bar数据 调用generator发送Bar同时设定BarStartTime
                if (round > this._generator.PartialBar.BarStartTime)
                {
                    //取下一个Bar时间 根据当前BarStartTime计算下一个BarStarTime
                    DateTime nextround = TimeFrequency.NextRoundedTime(this._generator.BarStartTime, this._interval);
                    if (round < nextround)
                    {
                        throw new Exception("Error in time rounding logic");
                    }
                    //发送当前Generator中的Bar数据 同时设定下一个Bar的开始时间
                    this._generator.SendNewBar(nextround);//结束时间按Bar的开始时间以及间隔计算获得
                    this._generator.SetBarStartTime(round);//Bar的开始时间按当前实际时间Round获得
                }
            }

            public DateTime NextTimeUpdateNeeded
            {
                get
                {
                    if (!this._updated)
                    {
                        return DateTime.MinValue;
                    }
                    return TimeFrequency.NextRoundedTime(this._generator.BarStartTime, this._interval);
                }
            }

        }
    }
}
