using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.DataFarm.API
{
    public interface IServiceHost
    {
        /// <summary>
        /// ServiceHost名称
        /// </summary>
        string Name { get;}

        /// <summary>
        /// 启动
        /// </summary>
        void Start();

        /// <summary>
        /// 停止
        /// </summary>
        void Stop();


        /// <summary>
        /// 客户端连接建立
        /// </summary>
        event Action<IServiceHost, IConnection> ConnectionCreatedEvent;

        /// <summary>
        /// 客户端连接断开事件
        /// </summary>
        event Action<IServiceHost, IConnection> ConnectionClosedEvent;

        /// <summary>
        /// 客户端服务查询事件
        /// 客户端在连接服务端时,会进行服务查询,该操作为同步操作需要进行逻辑处理后直接回报给客户端
        /// 客户端收到服务查询汇报后 判断该服务节点是否可用/超时则服务不可用
        /// 请求为QryServiceRequest,回报为RspQryServiceResponse
        /// </summary>
        event Func<IServiceHost, IPacket,IPacket> ServiceEvent;

        /// <summary>
        /// ServiceHost客户端发送上来的请求事件
        /// 绑定该事件后进行逻辑处理然后发送给客户端
        /// 发送给客户端通过Connection.Send进行操作
        /// 客户端请求均通过异步方式进行处理
        /// </summary>
        event Action<IServiceHost, IConnection,IPacket> RequestEvent;

        /// <summary>
        /// XLRequestEvet事件
        /// </summary>
        event Action<IServiceHost, IConnection, object, int> XLRequestEvent;
    }
}
