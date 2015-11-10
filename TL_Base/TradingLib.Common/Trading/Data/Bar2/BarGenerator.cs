using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// Bar生成器 用于处理行情数据更新Bar数据
    /// 具体Bar开始和结束由IFrequencyGeneratro进行管理
    /// </summary>
    public class BarGenerator
    {
        private BarConstructionType _barConstructionType;
        public BarConstructionType BarConstructionType { get { return _barConstructionType; } }

        private bool _updated;//是否更新过OHLC
        /// <summary>
        /// Tick数据不进行更新 在处理Bar数据时进行更新
        /// </summary>
        private Bar _partialBar;
        public Bar BarPartialBar { get { return _partialBar; } }

        private Bar _currentPartialBar;
        public Bar PartialBar { get { return _partialBar; } }


        private Symbol _symbol;
        public Symbol Symbol { get { return _symbol; } }

        private bool _isTickSent;

        public DateTime BarStartTime { get {return _currentPartialBar.BarStartTime; } }


        public BarGenerator(Symbol symbol,BarConstructionType type)
        {
            this._symbol = symbol;
            this._barConstructionType = type;
            this.CloseBar(DateTime.MinValue);

        }


        public void ProcessBar(Bar bar)
        { 
        
        }





        public void ProcessTick(Tick tick)
        {
            if (tick.hasAsk)
            {
                this._currentPartialBar.Ask = (double)tick.AskPrice;
                this._currentPartialBar.EmptyBar = false;
            }
            if (tick.hasBid)
            {
                this._currentPartialBar.Bid = (double)tick.BidPrice;
                this._currentPartialBar.EmptyBar = false;
            }
            if (tick.isTrade)
            {
                this._currentPartialBar.Volume += tick.Size;
                this._currentPartialBar.EmptyBar = false;
            }
            this._currentPartialBar.OpenInterest = tick.OpenInterest;

            this._currentPartialBar.time = tick.Time;

            if (tick.isTrade)
            {
                this._currentPartialBar.Volume += tick.Size;
                this._currentPartialBar.OpenInterest = tick.OpenInterest;
            }

            decimal value = 0;
            bool needUpdate = false;
            switch (this._barConstructionType)
            {
                case BarConstructionType.Default:
                case BarConstructionType.Trade:
                    {
                        if (tick.isTrade)
                        {
                            value = tick.Trade;
                            needUpdate = true;
                        }
                        break;
                    }
                case BarConstructionType.Ask:
                    {
                        if (tick.hasAsk)
                        {
                            value = tick.AskPrice;
                            needUpdate = true;
                        }
                        break;
                    }
                case BarConstructionType.Bid:
                    {
                        if (tick.hasBid)
                        {
                            value = tick.BidPrice;
                            needUpdate = true;
                        }
                        break;
                    }
                default:
                    throw new ArgumentException("not supported contructiontype");
            }

            if (needUpdate)
            {
                if (!this._updated)
                {
                    this._currentPartialBar.Open = (double)value;
                    this._currentPartialBar.High = (double)value;
                    this._currentPartialBar.Low = (double)value;
                    this._updated = true;
                }
                else if ((double)value > this._currentPartialBar.High)
                {
                    this._currentPartialBar.High = (double)value;
                }
                else if ((double)value < this._currentPartialBar.Low)
                {
                    this._currentPartialBar.Low = (double)value;
                }
                this._currentPartialBar.Close = (double)value;

                //标记Tick数据已经处理 进而发送
                this._isTickSent = true;
            }

            if (NewTick != null)
            { 
                NewTick(new NewTickEventArgs(this._symbol,tick,this._currentPartialBar));
            }

        }

        public event Action<SingleBarEventArgs> NewBar;

        public event Action<NewTickEventArgs> NewTick;
        /// <summary>
        /// 触发新的Bar
        /// </summary>
        /// <param name="barEndTime"></param>
        public void SendNewBar(DateTime barEndTime)
        {
            bool flag = !this._updated && this._currentPartialBar.Close == 0;
            //当前Tick我们无法确认该Bar数据已经结束,需要下一个Tick的时间来判定该Bar是否结束，比如10:30:59秒，在该秒内可能有多个Tick数据，只有当10:31:00这个Tick过来或定时器触发时候才表面10:30分这个Bar结束了

            SingleBarEventArgs e = new SingleBarEventArgs(this._symbol,new BarImpl(this._currentPartialBar),barEndTime,this._isTickSent);

            this.CloseBar(barEndTime);

            //如果没有更新过数据 则手工更新
            if (flag)
            {
                bool flag2 = (e.Bar.Bid != 0.0) && !double.IsNaN((double)e.Bar.Bid);
                bool flag3 = (e.Bar.Ask != 0.0) && !double.IsNaN((double)e.Bar.Ask);
                if (flag2 && flag3)
                {
                    e.Bar.Open = e.Bar.High = e.Bar.Low = e.Bar.Close = ((e.Bar.Bid + e.Bar.Ask) / 2.0);
                }
                else if (flag2)
                {
                    e.Bar.Open = e.Bar.High = e.Bar.Low = e.Bar.Close = e.Bar.Bid;
                }
                else if (flag3)
                {
                    e.Bar.Open = e.Bar.High = e.Bar.Low = e.Bar.Close = e.Bar.Ask;
                }
            }

            if (e.Bar.BarStartTime != System.DateTime.MinValue && !e.Bar.EmptyBar)
            {
                if (NewBar != null && !e.Bar.EmptyBar)
                {
                    NewBar(e);
                }
            }
        }

        /// <summary>
        /// 设定Bar起始时间
        /// </summary>
        /// <param name="barStartTime"></param>
        public void SetBarStartTime(DateTime barStartTime)
        {
            this._currentPartialBar.BarStartTime = barStartTime;
            this._partialBar.BarStartTime = barStartTime;
        }

        /// <summary>
        /// 结束一个Bar数据
        /// </summary>
        /// <param name="barEndTime"></param>
        private void CloseBar(DateTime barEndTime)
        {
            Bar data = this._currentPartialBar;
            this._isTickSent = false;

            this._currentPartialBar = new BarImpl();

            this._currentPartialBar.BarStartTime = barEndTime;
            this._currentPartialBar.EmptyBar = false;
            this._updated = false;

            if (data != null)
            {
                this._currentPartialBar.Open = this._currentPartialBar.Close = this._currentPartialBar.High = this._currentPartialBar.Low = data.Close;
                this._currentPartialBar.Bid = data.Bid;
                this._currentPartialBar.Ask = data.Ask;
            }

            this._partialBar = this._currentPartialBar.Clone();
        }
    
    }

   
}
