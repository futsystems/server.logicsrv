using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace FrontServer.TLServiceHost
{
    public partial class TLServiceHost
    {
        public void HandleLogicMessage(FrontServer.IConnection connection, IPacket packet)
        {
            //string hex = string.Empty;
            //TLConnection conn = GetConnection(connection.SessionID);
            //if (connection == null)
            //{
            //    logger.Warn(string.Format("Session:{0} TLConnection do not exist", connection.SessionID));
            //    return;
            //}

            //将逻辑服务器发送过来的数据转发到对应的连接上去
            connection.Send(packet.Data);
        }
    }
}
