using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

using TradingLib.API;


namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 储存了某个symbol对的一个频率的Bar数据
    /// 主要用于对外提供该频率的Bar数据序列
    /// </summary>
    public sealed class Frequency
    {
        // Fields
        
        private DateTime currentbarendtime;

        private QListBar readonlybars;
        
        private bool syncbars;

        private BarDataV writeablebars;
        
        private Dictionary<Frequency, QList<DateTime>> destFrequencyMap;
        private EventHandler<NewTickEventArgs> tickhandler;
        
        private FrequencyPlugin freqsetting;
        
        private Security security;

        public event BarDelegate NewBarEvent;

        // Methods
        public Frequency(FreqKey key, bool synchronizeBars)
        {
            this.Symbol = key.Symbol;
            this.FrequencySettings = key.Settings;
            this.SynchronizeBars = synchronizeBars;
            this.WriteableBars = new BarDataV(key.Symbol, key.Settings.BarFrequency);//new BarDataV(Symbol//null;// new QList<Bar>();
            this.Bars = new QListBar(this.WriteableBars);//.AsReadOnly();
            this.DestFrequencyConversion = new Dictionary<Frequency, QList<DateTime>>();
        }

        public int LookupEndDate(DateTime barEndDate)
        {
            int nBars = this.x954e89ce87b3f10e(barEndDate);
            if (nBars < 0)
            {
                return nBars;
            }
            if (this.Bars.LookBack(nBars).BarStartTime == barEndDate)
            {
                nBars++;
                if (nBars >= this.Bars.Count)
                {
                    return -1;
                }
            }
            return this.x31273f27c463a242(nBars);
        }

        /// <summary>
        /// 通过时间返回对应的序号
        /// </summary>
        /// <param name="barStartTime"></param>
        /// <returns></returns>
        public int LookupStartDate(DateTime barStartTime)
        {
            return writeablebars.BarStartTime2Index(barStartTime);
            /*
            int num = this.x954e89ce87b3f10e(barStartTime);
            if (num >= 0)
            {
                while ((num > 0) && (this.Bars.LookBack(num - 1).BarStartTime == barStartTime))
                {
                    num--;
                }
                return num;
            }
            return num;**/
        }

        //频率数据发送newbar 对外触发newbarevent(有些组件订阅了frequencydata的bar事件)
        public void OnNewBar(SingleBarEventArgs args)
        {
            if (NewBarEvent != null)
                NewBarEvent(args.Bar);
        }
        /*
        //频率数据发送tick/用于对外触发newtick(有些组件订阅了frequencydata的tick事件) tick时间带上对应频率的当前最新的Bar
        internal void SendTick(Tick tick)
        {
            EventHandler<NewTickEventArgs> handler = this.tickhandler;
            if (handler != null)
            {
                NewTickEventArgs e = new NewTickEventArgs
                {
                    Symbol = this.Symbol,
                    Tick = tick,
                    PartialBar = this.WriteableBars.PartialBar
                };
                if (e.PartialBar != null)
                {
                    handler(this, e);
                }
            }
        }
        **/
        
        Dictionary<BarDataType, FrequencyBarElementSeries> tmp = new Dictionary<BarDataType,FrequencyBarElementSeries>();
        private ISeries getBarElementSeries(BarDataType datatype)
        {
            if (!tmp.ContainsKey(datatype))
            {
                tmp.Add(datatype, new FrequencyBarElementSeries(this,datatype));
            }
            return tmp[datatype];
        }
        
        private int x31273f27c463a242(int xe151e765248d06d8)
        {
            DateTime barStartTime = this.Bars.LookBack(xe151e765248d06d8).BarStartTime;
            while ((xe151e765248d06d8 > 0) && (this.Bars.LookBack(xe151e765248d06d8 - 1).BarStartTime == barStartTime))
            {
                xe151e765248d06d8--;
            }
            return xe151e765248d06d8;
        }

        //获得某个时间对应的在序列中的位置
        private int x954e89ce87b3f10e(DateTime barstarttime)
        {
            if (this.Bars.Count != 0)
            {
                int nBars = 0;
                int num2 = Math.Min(this.Bars.Count - 1, 4);

                while ((this.Bars.LookBack(num2).BarStartTime > barstarttime) && (num2 > 0))
                {
                    nBars = num2;
                    num2 *= 2;
                    if (num2 > (this.Bars.Count - 1))
                    {
                        num2 = this.Bars.Count - 1;
                        break;
                    }
                }
                while ((num2 - 1) > nBars)
                {
                    int num3 = (num2 + nBars) / 2;
                    DateTime barStartTime = this.Bars.LookBack(num3).BarStartTime;
                    if (barStartTime == barstarttime)
                    {
                        return num3;
                    }
                    if (barStartTime < barstarttime)
                    {
                        num2 = num3;
                    }
                    else
                    {
                        nBars = num3;
                    }
                }
                if (this.Bars.LookBack(nBars).BarStartTime <= barstarttime)
                {
                    return nBars;
                }
                if (this.Bars.LookBack(num2).BarStartTime <= barstarttime)
                {
                    return num2;
                }
            }
            return -1;
        }

        // Properties
        public QListBar Bars
        {
            get
            {
                return this.readonlybars;
            }
            private set
            {
                this.readonlybars = value;
            }
        }

        public BarDataV WriteableBars
        {

            get
            {
                return this.writeablebars;
            }

            set
            {
                this.writeablebars = value;
            }
        }

        public ISeries this[BarDataType bardatatype]
        {
            get {
                try
                {
                    return this.getBarElementSeries(bardatatype);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    return null;
                }
            }
        }
        /*
        public ISeries Ask
        {
            get
            {
                return this[BarDataType.Close];
            }
        }

        public ISeries Bid
        {
            get
            {
                return this[BarDataType.Close];
            }
        }**/

        public ISeries Close
        {
            get
            {

                return this[BarDataType.Close];
            }
        }

        public ISeries High
        {
            get
            {
                return this[BarDataType.High];
            }
        }

        public ISeries Low
        {
            get
            {
                return this[BarDataType.Low];
            }
        }

        public ISeries Open
        {
            get
            {
                return this[BarDataType.Open];
            }
        }

        public Security Symbol
        {
            
            get
            {
                return this.security;
            }
            
            private set
            {
                this.security = value;
            }
        }

        public ISeries Volume
        {
            get
            {
                return this[BarDataType.Volume];
            }
        }

        public DateTime CurrentBarEndTime
        {
            get
            {
                return this.currentbarendtime;
            }
            set
            {
                this.currentbarendtime = value;
            }
        }

        /// <summary>
        /// 转换成对应的目标频率
        /// </summary>
        public Dictionary<Frequency, QList<DateTime>> DestFrequencyConversion
        {
            get
            {
                return this.destFrequencyMap;
            }
            private set
            {
                this.destFrequencyMap = value;
            }
        }

        public FrequencyPlugin FrequencySettings
        {
            get
            {
                return this.freqsetting;
            }
            private set
            {
                this.freqsetting = value;
            }
        }

        public bool SynchronizeBars
        {

            get
            {
                return this.syncbars;
            }

            private set
            {
                this.syncbars = value;
            }
        }

        
    }


}
