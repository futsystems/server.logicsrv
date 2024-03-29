﻿using System;
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
        public EnumFrontType FrontType { get { return EnumFrontType.WebSocket; } }
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


        void OnConnectionCreated(IConnection conn)
        {
            if (ConnectionCreatedEvent != null)
            {
                ConnectionCreatedEvent(this, conn);
            }
        }

        void OnConnectionClosed(IConnection conn)
        {
            if (ConnectionClosedEvent != null)
            {
                ConnectionClosedEvent(this, conn);
            }
        }

        /// <summary>
        /// 调用服务逻辑处理XL数据包
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="data"></param>
        /// <param name="requestId"></param>
        void OnXLRequestEvent(IConnection conn, TradingLib.XLProtocol.XLPacketData data, int requestId)
        {
            if (XLRequestEvent != null && data != null)
            {
                XLRequestEvent(this, conn, data, requestId);
            }
        }

        void OnRequestEvent(IConnection conn, IPacket packet)
        {
            if (RequestEvent != null && packet != null)
            {
                RequestEvent(this, conn, packet);
            }
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

    }
}
