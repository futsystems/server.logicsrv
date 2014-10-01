using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using TradingLib.API;

using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    public class SymbolScriptCollection<TSymbolScript> : IEnumerable<TSymbolScript> where TSymbolScript : SymbolStrategyBase, new()
    {
        // Fields
        private StrategyBase _strategybase;
        private Dictionary<Security, TSymbolScript> scriptlist;


        // Methods
        public SymbolScriptCollection()
        {
            this.scriptlist = new Dictionary<Security, TSymbolScript>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TSymbolScript> GetEnumerator()
        {
            return (IEnumerator<TSymbolScript>)this.scriptlist.Values.GetEnumerator();
        }

        private IEnumerator x05b0b83b5e6c5de6()
        {
            return this.GetEnumerator();
        }


        //public IEnumerable GetEnumerator()
        //{
            //return this.GetEnumerator();
        //    return null;
        //}

        void debug(string msg)
        {
            _strategybase.Print(msg);
        }

        TSymbolScript SymbolStraegy = null;
        /// <summary>
        /// SymbolStrategy进行初始化这里为多个symbol创建不同的副本用于接收对应的tick数据以及bar更新
        /// 在单symbol回测过程中会影响回测速度
        /// </summary>
        /// <param name="system"></param>
        public void Initialize(StrategyBase system)
        {
            this._strategybase = system;
            debug("SymbolScriptCollection init....");
            foreach (Security symbol in system.StrategyData.Symbols)
            {
                this.scriptlist[symbol] = Activator.CreateInstance<TSymbolScript>();
            }
            foreach (KeyValuePair<Security, TSymbolScript> pair in this.scriptlist)
            {
                pair.Value.Initialize(system, pair.Key);
            }

            if (_strategybase.StrategyData.IsOneSymbol && scriptlist.Count ==1)
            {
                SymbolStraegy = scriptlist.Values.ToArray()[0];//获得symbolscript的第一个symbolstrategy用于推送Bar与Tick
            }
            //system.PositionManager.OrderFilled += new EventHandler<OrderFilledEventArgs>(this.x4e18f4d87e3d473b);
            //system.PositionManager.OrderUpdated += new EventHandler<OrderUpdatedEventArgs>(this.xd99e14a369c1c3fd);
        }

        public void GotBar(Bar bar)
        {
            //当系统新到一个Tick数据经过时间检查 发现已经越过了一个K线,则把当前K线进行结算(固定成一根K线不在接收数据),
            //然后重新生成一组数据用于处理新的Tick数据
           
            /*
            DateTime minValue = DateTime.MinValue;
            foreach (KeyValuePair<Security, TSymbolScript> pair in this.scriptlist)
            {
                if (pair.Value.Bars.Count != 0)
                {
                    if (this._strategybase.StrategyData.BarFrequency.IsTimeBased && pair.Value.Bars.HasPartialItem)
                    {
                      //  throw new QSQuantError("Not expecting a partial bar at this point. " + pair.Key.ToString() + "  " + this._strategybase.StrategyData.CurrentDate.ToString());
                    }
                    //if (!pair.Value.Bars.HasPartialItem && (pair.Value.Bars.Last.BarStartTime > minValue))
                    {
                        minValue = pair.Value.Bars.Last.BarStartTime;
                    }
                }
            }**/
            //Profiler.Instance.EnterSection("Script2");
            /*
            foreach (KeyValuePair<Security, TSymbolScript> pair2 in this.scriptlist)
            {
                QListBar bars = this._strategybase.StrategyData.Bars[pair2.Value.Symbol];
                if (bars.Count != 0)
                {
                    Bar current = bars.Last;
                    //Bar current = bars.LookBack(1);
                    //if (!(current.BarStartTime != minValue) && this._strategybase.StrategyData.ShouldProcessBar(current, bars))
                    {
                        pair2.Value.OnBar(current);
                    }
                }
            }**/
            //foreach (KeyValuePair<Security, TSymbolScript> pair2 in this.scriptlist)
            //{
            //    pair2.Value.OnBar(bar);
            //}
            if (SymbolStraegy != null)

                SymbolStraegy.OnBar(bar);
            else
                this[bar.Symbol].OnBar(bar);

            //Profiler.Instance.LeaveSection();
        }
        //多symbol回测数据时这里会造成速度减慢 应为需要每次查询键值
        public void GotTick(Security symbol, Tick tick, Bar partialBar)
        {
            //this[symbol].OnTick(tick, partialBar);
            //如果是单symbol模式则加快数据推送
            if (SymbolStraegy != null)
            {
                    SymbolStraegy.OnTick(tick, partialBar);
              
            }
            else
                this[symbol].OnTick(tick, partialBar);
        }

        public void GotOrder(Order order)
        {
            if (SymbolStraegy != null)
                SymbolStraegy.OnOrder(order);
            else
                this[order.symbol].OnOrder(order);
        }

        public void GotTrade(Trade fill)
        {
            if (SymbolStraegy != null)
                SymbolStraegy.OnTrade(fill);
            else
                this[fill.symbol].OnTrade(fill);
            
        }
        public  void GotEntryPosition(Trade fill,PositionDataPair data)
        {
            if (SymbolStraegy != null)
                SymbolStraegy.OnEntryPosition(fill,data);
            else
                this[data.Position.symbol].OnEntryPosition(fill,data);
        }
        public   void GotExitPosiiton(Trade fill,PositionDataPair data)
        {
            if (SymbolStraegy != null)
                SymbolStraegy.OnExitPosition(fill,data);
            else
                this[data.Position.symbol].OnExitPosition(fill,data);
        }
        /*
        private void x4e18f4d87e3d473b(object xe0292b9ed559da7d, OrderFilledEventArgs xfbf34718e704c6bc)
        {
            this[xfbf34718e704c6bc.Position.Symbol].OrderFilled(xfbf34718e704c6bc.Position, xfbf34718e704c6bc.Trade);
        }

        private void xd99e14a369c1c3fd(object xe0292b9ed559da7d, OrderUpdatedEventArgs xfbf34718e704c6bc)
        {
            if ((xfbf34718e704c6bc.Order.OrderState == BrokerOrderState.Cancelled) || (xfbf34718e704c6bc.Order.OrderState == BrokerOrderState.Rejected))
            {
                this[xfbf34718e704c6bc.Position.Symbol].OrderCancelled(xfbf34718e704c6bc.Position, xfbf34718e704c6bc.Order, xfbf34718e704c6bc.Information ?? string.Empty);
            }
        }**/

        // Properties
        public TSymbolScript this[string symbolName]
        {
            get
            {
                return this.scriptlist[this._strategybase.StrategyData.GetSymbolByName(symbolName)];
            }
        }

        public TSymbolScript this[Security symbol]
        {
            get
            {
                return this.scriptlist[symbol];
            }
        }

        /// <summary>
        /// 获得集合中的所有合约
        /// </summary>
        public List<Security> Symbols { get { return scriptlist.Keys.ToList(); } }
    }


}
