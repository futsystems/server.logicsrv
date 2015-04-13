using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public partial interface ILogicHandler
    {
        /// <summary>
        /// 行情数据回报
        /// </summary>
        /// <param name="k"></param>
        void OnTick(Tick k);

        /// <summary>
        /// 获得服务端委托回报
        /// </summary>
        /// <param name="o"></param>
        void OnOrder(Order o);

        /// <summary>
        /// 获得服务端昨日持仓回报
        /// </summary>
        /// <param name="pos"></param>
        void OnHoldPosition(PositionDetail pos);

        /// <summary>
        /// 获得服务端成交回报
        /// </summary>
        /// <param name="f"></param>
        void OnTrade(Trade f);

        /// <summary>
        /// 持仓更新回报
        /// </summary>
        /// <param name="pos"></param>
        //void OnPositionUpdate(Position pos);
    }
}
