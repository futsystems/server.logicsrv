using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Windows.Forms;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Quant.Base;


namespace TradingLib.Quant.Engine
{
    /// <summary>
    /// 以另外一种人性化的方式操作委托发送以取消
    /// 不过最终所有的委托都要以内部标准格式在各个组件之间进行回转与沟通
    /// </summary>
    public class StrategySupport
    {
        public event DebugDelegate SendDebugEvent;
        void debug(string msg)
        {
            if (SendDebugEvent != null)
                SendDebugEvent(msg);
        }

        StrategyData strategydata = null;

        public StrategySupport(StrategyData data)
        {
            this.strategydata = data;
        }

        
        public PositionTracker Positions {
            get
            {
                return strategydata.TradingInfoTracker.PositionManager;
            }
        }


        /// <summary>
        /// 是否有持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasPosition(string symbol)
        {
            return !Positions[symbol].isFlat;
        }
        /// <summary>
        /// 是否有多头持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasLongPositon(string symbol)
        {
            return Positions[symbol].isLong;
        }
        /// <summary>
        /// 是否有空头持仓
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public bool HasShortPosition(string symbol)
        {
            return Positions[symbol].isShort;
        }
    

    

        #region 仓位操作
        
        /// <summary>
        /// 反手某个持仓
        /// </summary>
        /// <param name="symbol"></param>
        public void ReversalPosition(string symbol)
        { 
        
        }
        /// <summary>
        /// 平掉某个symbol的仓位
        /// </summary>
        /// <param name="symbol"></param>
        public void FlatPosition(string symbol,string comment="")
        {
            Position pos = Positions[symbol];
            if (!pos.isFlat)
            {
                Order o = new MarketOrderFlat(pos);
                o.comment = comment;
                SendOrder(o);
            }
        }
        /// <summary>
        /// 平掉所有持仓
        /// </summary>
        public void FlatAllPositions()
        {
            foreach (Position pos in Positions)
            {
                if (!pos.isFlat)
                {
                    SendOrder(new MarketOrderFlat(pos));
                }
            }
            
        }


        #endregion

        /// <summary>
        /// 记录了定时发送的委托,按照指定的方式定时发送
        /// </summary>
        List<Order> timeSender = new List<Order>();

        #region 定时发送委托

        /// <summary>
        /// 下个Bar开始市价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        public void BuyMarketNextBar(string symbol, int size, string comment = "")
        { 
            
        }
        /// <summary>
        /// 下个Bar开始市价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        public void SellMarketNextBar(string symbol, int size, string comment = "")
        {
        }

        #endregion



        #region 委托操作 这里的操作都是即时操作 执行操作后委托会立刻发送

        /// <summary>
        /// 市价买入 买入前平掉空头仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        public void EntryLongPosition(string symbol, int size, string comment = "")
        {
            //MessageBox.Show("建立多头仓位");
            Position pos = Positions[symbol];
            if (pos.isShort)
            {
                //MessageBox.Show("有空仓 先平空 然后再 建立多头仓位");
                //FlatPosition("建多前平空");
                //Order o = new MarketOrderFlat(pos);
                //o.comment = comment;
                //SendOrder(o);
                BuyMarket(symbol, pos.UnsignedSize, "建多前平空");

            }
            BuyMarket(symbol, size, comment);
            
        }

        /// <summary>
        /// 市价建立空头仓位 卖出前平掉多头仓位
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="comment"></param>
        public void EntryShortPosition(string symbol, int size, string comment = "")
        {
            Position pos = Positions[symbol];
            if (pos.isLong)
            {
                //FlatPosition("建空前平多");
                SellMarket(symbol, pos.UnsignedSize, "建空前平多");
            }
            SellMarket(symbol, size, comment);
        
        }
        /// <summary>
        /// 市价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        public void BuyMarket(string symbol, int size,string comment="")
        {
            SendOrder(new BuyMarket(symbol, size,comment));
            
        }
        /// <summary>
        /// 市价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        public void SellMarket(string symbol, int size,string comment="")
        {
            SendOrder(new SellMarket(symbol, size,comment));
        }
        /// <summary>
        /// 限价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        public void BuyLimit(string symbol, int size, double price,string comment="")
        {
            SendOrder(new BuyLimit(symbol, size, (decimal)price,comment));
        }
        /// <summary>
        /// 限价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="price"></param>
        public void SellLimit(string symbol, int size, double price,string comment="")
        {
            SendOrder(new SellLimit(symbol, size, (decimal)price,comment));
        }
        /// <summary>
        /// 追价买入
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="stop"></param>
        public void BuyStop(string symbol, int size, double stop,string comment="")
        {
            SendOrder(new BuyStop(symbol, size, (decimal)stop,comment));
        }

        /// <summary>
        /// 追价卖出
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="size"></param>
        /// <param name="stop"></param>
        public void SellStop(string symbol, int size, double stop,string comment="")
        {
            SendOrder(new SellStop(symbol, size, (decimal)stop,comment));
        }

        #endregion





        //所有委托均通过如下方式 最终发送的IEXBroker.SendOrder(Order o)接口上 然后转发到Broker实体
        public void SendOrder(Order order)
        {
            order.date = strategydata.TickDate;//Util.ToTLDate();
            order.time = strategydata.TickTime;//Util.ToTLTime();
            order.VirtualOwner = 0;//该委托来自与哪个策略
            order.LocalID = 0;//本地委托编号 用于通过本地编号来查询委托
            order.Account = "180001";
            _broker.SendOrder(order);
        }

        public void CancelOrder(long val)
        {
            _broker.CancelOrder(val);
        }

        IExBroker _broker=null;
        public void SetBroker(IExBroker broker)
        {
            _broker = broker;
        }

        public void Print(string msg)
        {
            debug(msg);
        }

    }

    public enum QSEnumSendTime
    { 
        NextBar,//下个Bar开始
        NextBarClose,//下个Bar结束
        Time,//延迟几秒发送

    }
}
