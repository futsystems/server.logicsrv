using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class BarGenerator
    {
        private BarConstructionType _barConstructionType;
        private bool _updated;
        private Bar _partialBar;
        private Symbol _symbol;
        private Bar _currentPartialBar;
        private bool _isTickSent;



        public void ProcessTick(Tick tick)
        {
            if (!tick.isValid) return;
            if (tick.Symbol != _symbol.Symbol) return;

            this._currentPartialBar.EmptyBar = false;
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
                    this._currentPartialBar.Open = value;
                    this._currentPartialBar.High = value;
                    this._currentPartialBar.Low = value;
                    this._updated = true;
                }
                else if (value > this._currentPartialBar.High)
                {
                    this._currentPartialBar.High = value;
                }
                else if (value < this._currentPartialBar.Low)
                {
                    this._currentPartialBar.Low = value;
                }
                this._currentPartialBar.Close = value;

                //标记Tick数据已经处理 进而发送
                this._isTickSent = true;
            }

        }

        public void SendNewBar(DateTime barEndTime)
        {
            bool needSend = this._updated;
            //当前Tick我们无法确认该Bar数据已经结束,需要下一个Tick的时间来判定该Bar是否结束，比如10:30:59秒，在该秒内可能有多个Tick数据，只有当10:31:00这个Tick过来或定时器触发时候才表面10:30分这个Bar结束了

            //
        }
        

        /// <summary>
        /// 结束一个Bar数据
        /// </summary>
        /// <param name="barEndTime"></param>
        private void CloseBar(DateTime barEndTime)
        {
            Bar data = this._currentPartialBar;
            this._isTickSent = false;


        }
    
    }

   
}
