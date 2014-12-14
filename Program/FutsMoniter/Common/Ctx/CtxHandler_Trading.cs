using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {
        public TradingInfoTracker TradingInfoTracker { get { return infotracker; } }

        TradingInfoTracker infotracker = new TradingInfoTracker();

       
        /// <summary>
        /// 行情数据回报
        /// </summary>
        /// <param name="k"></param>
        public void OnTick(Tick k)
        {
            infotracker.GotTick(k);
            if (GotTickEvent != null)
                GotTickEvent(k);
        }

        /// <summary>
        /// 获得服务端委托回报
        /// </summary>
        /// <param name="o"></param>
        public void OnOrder(Order o)
        {
            //Globals.Debug("got order :" + o.GetOrderInfo());
            infotracker.GotOrder(o);
            if (GotOrderEvent != null)
                GotOrderEvent(o);
        }

        /// <summary>
        /// 获得服务端昨日持仓回报
        /// </summary>
        /// <param name="pos"></param>
        public void OnHoldPosition(PositionDetail pos)
        {
            infotracker.GotHoldPosition(pos);
        }

        /// <summary>
        /// 获得服务端成交回报
        /// </summary>
        /// <param name="f"></param>
        public void OnTrade(Trade f)
        {
            infotracker.GotFill(f);
            if (GotFillEvent != null)
                GotFillEvent(f);
        }

        /// <summary>
        /// 持仓更新回报
        /// </summary>
        /// <param name="pos"></param>
        //void OnPositionUpdate(Position pos);

    }
}
