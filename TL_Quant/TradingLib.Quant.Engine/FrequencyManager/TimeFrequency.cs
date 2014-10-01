using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Engine
{
    //[Serializable, DisplayName("Time-based Bar Frequency"), PluginEditor(typeof(TimeFrequencyEditor))]
    /// <summary>
    /// 以时间为基础的Frequency 基于时间间隔生成对应的Bar数据
    /// 需要实现xml读写,在生成pluginsetting->plugintoken(定义了类/接口/dll文件) pluginxml定义了如何生成xml以及从xml生成对应的对象
    /// </summary>
    [Serializable]
    public sealed class TimeFrequency : FrequencyPlugin, IXmlSerializable
    {

        // Methods
        public TimeFrequency()
        {
            this.BarLength = TimeSpan.FromMinutes(1.0);
        }

        public override BarFrequency BarFrequency { get; set; }
        public int CompareCode = 0;
        public TimeFrequency(BarFrequency freq)
        {
            if (freq.Type != BarInterval.CustomTime)
            {
                throw new QSQuantError("TimeFrequency need Time Based BarFrequency");
            }
            this.BarLength = new TimeSpan(0,0,freq.Interval);
            BarFrequency = freq;
            CompareCode = freq.Interval * 100 + (int)BarFrequency.Type;//通过这种方式获得为唯一的comparecode
        }
        /*
        public TimeFrequency(TimeSpan barLength)
        {
            this.BarLength = barLength;
        }
        **/
        public override FrequencyPlugin Clone()
        {
            return (TimeFrequency)base.MemberwiseClone();
        }

        /// <summary>
        /// 获得对应的Bar生成器
        /// </summary>
        /// <returns></returns>
        public override IFrequencyGenerator CreateFrequencyGenerator()
        {
            return new TimeFreqGenerator(this.BarLength);
        }

        public override bool Equals(object obj)
        {
            TimeFrequency frequency = obj as TimeFrequency;
            if (frequency == null)
            {
                return false;
            }
            return (this.BarLength == frequency.BarLength);
        }

        public override int GetHashCode()
        {
            return this.BarLength.GetHashCode();
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

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    string str2;
                    if (((str2 = reader.LocalName) != null) && (str2 == "BarLengthSecends"))
                    {
                        int _interval = int.Parse(reader.ReadElementContentAsString(), CultureInfo.InvariantCulture);
                        //this.BarLength = new TimeSpan(ticks);
                        //从xml得到对应的barFrequency
                        this.BarFrequency = new BarFrequency(BarInterval.CustomTime, _interval);
                        this.BarLength = new TimeSpan(0, 0,this.BarFrequency.Interval);
                        this.CompareCode = this.BarFrequency.Interval * 100 + (int)BarFrequency.Type;
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    reader.Read();
                    return;
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteElementString("BarLengthSecends", this.BarFrequency.Interval.ToString(CultureInfo.InvariantCulture));
        }

        public override string ToString()
        {
            return "Time:" + this.BarLength.TotalSeconds.ToString()+"s";//this.BarLength.ToString();
        }

        // Properties
        [Description("Indicates the timespan or period for each bar."), DisplayName("Bar Length")]
        public TimeSpan BarLength { get; set; }

        public override bool IsTimeBased
        {
            get
            {
                return true;
            }
        }

      
        // Nested Types
        /// <summary>
        /// 时间间隔为基础的Bar生成器
        /// </summary>
        private class TimeFreqGenerator : IFrequencyGenerator
        {
            // Fields
            private bool _updated=false;
            private TimeSpan _interval;
            private BarGenerator _generator;
            public event NewTickEventArgsDel SendNewTickEvent;
            public event SingleBarEventArgsDel SendNewBarEvent;
           

            int _units = 0;
            // Methods
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
            /// <param name="barConstruction"></param>
            public void Initialize(Security symbol, BarConstructionType barConstruction)
            {
                this._generator = new BarGenerator(symbol,(int)_interval.TotalSeconds, barConstruction,_barstore);
                
                //this._generator.NewBar += new EventHandler<SingleBarEventArgs>(this.handlebar);
                this._generator.SendSingleBarEvent += new SingleBarEventArgsDel(_generator_SendSingleBarEvent);
                this._generator.SendNewTickEvent += new NewTickEventArgsDel(_generator_SendNewTickEvent);

            }

            BarDataV _barstore = null;
            public void SetBarStore(BarDataV Barstore)
            {
                _barstore = Barstore;
                //this._generator.SetDataStore(_barstore);
            }

            void _generator_SendNewTickEvent(NewTickEventArgs tickEventArgs)
            {
                if (SendNewTickEvent != null)
                    SendNewTickEvent(tickEventArgs);
            }

            void _generator_SendSingleBarEvent(SingleBarEventArgs barEventArgs)
            {
                if (SendNewBarEvent != null)
                    SendNewBarEvent(barEventArgs);
            }
            /// <summary>
            /// 处理Bar数据
            /// </summary>
            /// <param name="args"></param>
            public void ProcessBar(SingleBarEventArgs args)
            {
                //如果Tick数据已经发送,则我们直接更新barendtime
                if (args.TicksWereSent)
                {
                    //this.UpdateTime(args.BarEndTime);
                }
                else
                {
                    //如果当前Bar的开始时间小于generator的bar开始时间 报错 该bar不属于当前时间周期
                    if (args.Bar.BarStartTime < this._generator.BarStartTime)
                    {
                        throw new QSQuantError(string.Concat(new object[] { "Bar data error.  Received data for ", args.Bar.BarStartTime, " when current time is ", this._generator.BarStartTime }));
                    }
                    //如果没有更新 则更新当前时间为bar的开始时间
                    if (!this._updated)
                    {
                        //this.UpdateTime(args.Bar.BarStartTime);
                    }
                    //generator处理bar
                    this._generator.ProcessBar(args);
                    //更新该bar的结束时间
                    //this.UpdateTime(args.BarEndTime);
                }
            }
            /// <summary>
            /// 处理Tick数据 由FrequencyManager中的FreqProcessTick直接进行调用
            /// 1.更新当前时间,根据计算当前时间对应的Bar的starttime来判断,是否产生了新的Bar 如果产生新的Bar就sendnewbar
            /// 2.调用generator处理Tick数据
            /// 
            /// </summary>
            /// <param name="tick"></param>
            public void ProcessTick(Tick tick)//空载72
            {
                //Profiler.Instance.EnterSection("gen processtick");
                this.UpdateTime(tick.date,tick.time);//单独运行updaedate 66万/s(不对外触发tick 57-58万)
                this._generator.ProcessTick(tick);//单独运行processtick50万
                //Profiler.Instance.LeaveSection();
            }

            /// <summary>
            /// 通过时间来获得bar的序号
            /// </summary>
            /// <param name="time"></param>
            /// <param name="date"></param>
            /// <param name="intervallength"></param>
            /// <returns></returns>
            static internal long getbarid(int time, int date, int intervallength)
            {
                // get time elapsed to this point
                int elap = Util.FT2FTS(time);
                // get number of this bar in the day for this interval(获得该bar所在当天时间中的排序)
                long bcount = (int)((double)elap / intervallength);
                // add the date to the front of number to make it unique //某天的序号加上日期就为该Bar的唯一序号
                bcount += (long)date * 10000;
                return bcount;
            }

            int curr_barid = 0;

            
            /// <summary>
            /// 更新当前时间逻辑
            /// 关于Bar数据 设计到了时间转换,我们这里是否可以把速度进一步提高
            /// 判断时间然后对外发送产生的Bar
            /// </summary>
            /// <param name="time"></param>
            private void UpdateTime(int date,int time)
            {

                long barid = getbarid(time, date, this._units);//获得某个时间频率的Bar在当天的bar序号
                //通过时间获得当前时间锁对应的BarStartTime
                if (!this._updated)//如果没有更新过 这设定当前barstarttime为当前bar开始时间
                {
                    
                    //Profiler.Instance.EnterSection("updatecheck");
                    DateTime t = Util.ToDateTime(date, time);//关于Bar数据 设计到了时间转换,我们这里是否可以把速度进一步提高
                    DateTime barStartTime = TimeFrequency.RoundTime(t, this._interval);
                    this._generator.SetBarStartTime(barStartTime);
                    this._generator.CurrentBarID = barid;
                    this._updated = true;
                    //Profiler.Instance.LeaveSection();
                }
                //如果当前Bar的开始时间大于generator的Bar开始时间则我们需要触发一个新的Bar,并且重新设定Bar的开始时间
                if(barid > this._generator.CurrentBarID)
                {
                    
                    //Profiler.Instance.EnterSection("bar gener");
                    DateTime t = Util.ToDateTime(date, time);
                    DateTime barStartTime = TimeFrequency.RoundTime(t, this._interval);

                    DateTime barEndTime = TimeFrequency.NextRoundedTime(this._generator.BarStartTime, this._interval);
                    if (barStartTime < barEndTime)
                    {
                        throw new QSQuantError("Error in time rounding logic");
                    }
                    this._generator.SendNewBar(barEndTime);//调用BarGenerator发送Bar通过时间回路到这里然后再被外层对象订阅
                    //sendNewBar的过程中就结算了当前的Bar并生成了新的partialBar
                    this._generator.SetBarStartTime(barStartTime);
                    
                    this._generator.CurrentBarID = barid;
                    
                    //Profiler.Instance.LeaveSection();
                }
            }

            // Properties
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
