﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class TimeFrequency : FrequencyBase
    {

        TimeSpan _barLength;
        public TimeSpan BarLength { get { return _barLength; } }

        BarFrequency _freq;
        public override BarFrequency BarFrequency { get { return _freq; } }

        int _comparecode;
        public int CompareCode { get { return _comparecode; } }

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

            BarFrequency _freq = freq;
            _comparecode = freq.Interval * 10000 + (int)BarFrequency.Type;//通过这种方式获得为唯一的comparecode
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
                this._generator = new BarGenerator(symbol, this._units, type);
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
                    this._generator.SetBarStartTime(round);
                    this._updated = true;
                }
                //如果roundtime大于PartialBar的起始时间 越过了一个Bar数据
                if (round > this._generator.PartialBar.BarStartTime)
                {
                    //取下一个Bar时间
                    DateTime nextround = TimeFrequency.NextRoundedTime(datetime, this._interval);
                    if (round < nextround)
                    {
                        throw new Exception("Error in time rounding logic");
                    }
                    //发送当前Generator中的Bar数据 同时设定下一个Bar的开始时间
                    this._generator.SendNewBar(nextround);
                    this._generator.SetBarStartTime(nextround);
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
