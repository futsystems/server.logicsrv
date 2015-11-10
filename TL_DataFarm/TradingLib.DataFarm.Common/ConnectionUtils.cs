using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm;

namespace TradingLib.DataFarm.Common
{
    public static class ConnectionUtils
    {
        /// <summary>
        /// 发送行情数据
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="k"></param>
        public static void SendTick(this DataFarm.API.IConnection conn,Tick k)
        {
        
        }

        /// <summary>
        /// 发送心跳回报
        /// </summary>
        /// <param name="?"></param>
        public static void HeartBeatResponse(this DataFarm.API.IConnection conn)
        {
        
        }

        


    }
}
