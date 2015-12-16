using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{
    public static class ConnectionUtils
    {
        /// <summary>
        /// 发送行情数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="k"></param>
        public static void SendTick(this IConnection conn,Tick k)
        {
            TickNotify ticknotify = new TickNotify();
            ticknotify.Tick = k;
            conn.Send(ticknotify);
        }

        /// <summary>
        /// 发送查询回报
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="response"></param>
        /// <param name="islast"></param>
        public static void SendResponse(this IConnection conn, RspResponsePacket response,bool islast=true)
        {
            response.IsLast = islast;
            conn.Send(response);
        }


    }
}
