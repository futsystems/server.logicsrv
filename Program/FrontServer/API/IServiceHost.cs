using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace FrontServer
{
    public interface IServiceHost
    {
        /// <summary>
        /// 将逻辑服务端的消息进行处理 转换成ServiceHost支持协议内容
        /// 如果不需发送消息到客户端则返回null
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        void HandleLogicMessage(IConnection connection, IPacket packet);

        void ClearIdleSession();
    }
}
