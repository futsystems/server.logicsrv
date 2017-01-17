using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace WSServiceHost
{
    public partial class WSServiceHost
    {
        /// <summary>
        /// 客户端连接建立
        /// </summary>
        public event Action<IServiceHost, IConnection> ConnectionCreatedEvent;

        /// <summary>
        /// 客户端连接断开事件
        /// </summary>
        public event Action<IServiceHost, IConnection> ConnectionClosedEvent;

        /// <summary>
        /// 客户端请求事件
        /// </summary>
        public event Action<IServiceHost, IConnection, IPacket> RequestEvent;

        /// <summary>
        /// 客户端服务查询事件
        /// </summary>
        public event Func<IServiceHost, IPacket, IPacket> ServiceEvent;


        /// <summary>
        /// XLRequestEvet事件
        /// </summary>
        public event Action<IServiceHost, IConnection, object, int> XLRequestEvent;
    }
}
