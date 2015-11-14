using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TCPServiceHost
{
    public partial class TCPServiceHost:IServiceHost
    {
        /// <summary>
        /// 客户端连接建立
        /// </summary>
        public event Action<IServiceHost, IConnection> SessionCreatedEvent;

        /// <summary>
        /// 客户端连接断开事件
        /// </summary>
        public event Action<IServiceHost, IConnection> SessionClosedEvent;

        /// <summary>
        /// 客户端请求事件
        /// </summary>
        public event Action<IServiceHost,IConnection,IPacket> RequestEvent;

        /// <summary>
        /// 客户端服务查询事件
        /// </summary>
        public event Func<IServiceHost, IPacket, IPacket> ServiceEvent;


        void OnSessionCreated(IConnection conn)
        {
            if (SessionCreatedEvent != null)
            {
                SessionCreatedEvent(this, conn);
            }
        }

        void OnSessionClosed(IConnection conn)
        {
            if (SessionClosedEvent != null)
            {
                SessionClosedEvent(this, conn);
            }
        }


        void OnRequestEvent(IPacket packet)
        {

        }

        /// <summary>
        /// 查询服务是否可用
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private RspQryServiceResponse QryService(QryServiceRequest request)
        {
            RspQryServiceResponse response = null;
            if (ServiceEvent != null)
            {
                IPacket packet = ServiceEvent(this, request);

                if (packet != null && packet.Type == MessageTypes.SERVICERESPONSE)
                {
                    response = packet as RspQryServiceResponse;
                    response.APIType = QSEnumAPIType.MD_ZMQ;
                    response.APIVersion = "1.0.0";
                }
            }
            if (response == null)
            {
                response = ResponseTemplate<RspQryServiceResponse>.SrvSendRspResponse(request);
                response.APIType = QSEnumAPIType.ERROR;
                response.APIVersion = "";
            }
            return response;
        }


        void Send(TLSessionBase session, byte[] data)
        {
            
            session.Send("A");
        }
    }
}
