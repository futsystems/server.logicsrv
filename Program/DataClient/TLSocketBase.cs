using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using TradingLib.API;
using TradingLib.Common;

namespace DataClient
{
    public abstract class TLSocketBase
    {
        /// <summary>
        /// Socket收到消息后事件
        /// </summary>
        public event Action<MessageTypes, string> MessageEvent;

        protected void HandleMessage(Message message)
        {
            if (MessageEvent != null)
            {
                MessageEvent(message.Type, message.Content);
            }
        }

        protected IPEndPoint _server;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="server"></param>
        public TLSocketBase(IPEndPoint server)
        {
            _server = server;
        }

        /// <summary>
        /// Socket是否处于连接状态
        /// </summary>
        public abstract bool IsConnected { get; }

        /// <summary>
        /// 查询服务
        /// </summary>
        /// <param name="apiType"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public abstract RspQryServiceResponse QryService(QSEnumAPIType apiType,string version);

        /// <summary>
        /// 连接
        /// </summary>
        public abstract void Connect();

        /// <summary>
        /// 断开连接
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public abstract void Send(byte[] msg);

    }
}
