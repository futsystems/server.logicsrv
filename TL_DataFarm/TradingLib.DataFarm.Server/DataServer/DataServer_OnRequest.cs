using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.DataFarm.Common;


namespace TradingLib.DataFarm.Common
{

    /// <summary>
    /// 执行客户端提交上来的请求
    /// </summary>
    public partial class DataServer
    {
        //更新客户端心跳时间戳
        void UpdateHeartBeat(IConnection conn)
        {
            conn.LastHeartBeat = DateTime.Now;
        }

        void OnSessionClosedEvent(IServiceHost arg1, IConnection arg2)
        {

        }

        void OnSessionCreatedEvent(IServiceHost arg1, IConnection arg2)
        {
            
            if (!connectionMap.Keys.Contains(arg2.SessionID))
            {
                connectionMap.TryAdd(arg2.SessionID, arg2);
                logger.Info(string.Format("Connection:{0} created", arg2.SessionID));
            }
            else
            {
                logger.Warn(string.Format("Connection:{0} already exit", arg2.SessionID));
            }
        }


        IPacket OnServiceEvent(IServiceHost arg1, IPacket arg2)
        {
            if (arg2.Type == MessageTypes.SERVICEREQUEST)
            {
                QryServiceRequest request = arg2 as QryServiceRequest;
                RspQryServiceResponse response = ResponseTemplate<RspQryServiceResponse>.SrvSendRspResponse(request);

                //执行逻辑判断 是否提供服务 比如连接数大于多少/cpu资源大于多少 就拒绝服务
                response.OnService = true;
                return response;
            }
            return null;
        }



        void OnRequestEvent(IServiceHost host, IConnection conn, IPacket packet)
        {
            logger.Info(string.Format("ServiceHost:{0} Connection:{1} Request:{2}", host.Name, conn.SessionID, packet.ToString()));
            
            //更新客户端连接心跳

            switch (packet.Type)
            { 
                    //响应客户端版本查询
                case MessageTypes.VERSIONREQUEST:
                    SrvOnVersionRequest(host, conn, packet as VersionRequest);
                    break;
                    //查询功能列表
                case MessageTypes.FEATUREREQUEST:
                    SrvOnFeatureRequest(host, conn, packet as FeatureRequest);
                    break;
                    //响应客户端心跳
                case MessageTypes.HEARTBEAT:
                    UpdateHeartBeat(conn);
                    break;
                    //响应客户端心跳查询
                case MessageTypes.HEARTBEATREQUEST:
                    SrvOnHeartbeatRequest(host, conn, packet as HeartBeatRequest);
                    break;

                case MessageTypes.BARREQUEST:
                    SrvOnBarRequest(host, conn, packet as QryBarRequest);
                    break;
                default:
                    logger.Warn(string.Format("Message Type:{0} not handled",packet.Type));
                    break;
            }
        }

        /// <summary>
        /// 服务端版本查询
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void SrvOnVersionRequest(IServiceHost host, IConnection conn, VersionRequest request)
        {
            
            VersionResponse response = ResponseTemplate<VersionResponse>.SrvSendRspResponse(request);
            TLVersion v = new TLVersion();
            v.ProductType = QSEnumProductType.CounterSystem;
            v.Platfrom = PlatformID.Unix;
            v.Major=1;
            v.Minor =0;
            v.Fix=0;
            v.DeployID = "demo";
            response.Version = v;

            conn.Send(response);

        }

        void SrvOnFeatureRequest(IServiceHost host, IConnection conn, FeatureRequest request)
        {
            FeatureResponse response = ResponseTemplate<FeatureResponse>.SrvSendRspResponse(request);
            response.Add(MessageTypes.XQRYMARKETTIME);

            conn.Send(response);
        }

        void SrvOnHeartbeatRequest(IServiceHost host, IConnection conn, HeartBeatRequest request)
        {
            HeartBeatResponse response = ResponseTemplate<HeartBeatResponse>.SrvSendRspResponse(request);
            conn.Send(response);
        }


        void SrvOnBarRequest(IServiceHost host, IConnection conn, QryBarRequest request)
        {
            logger.Info("Got Qry Bar Request:" + request.ToString());
            //查询Bar数据
            IEnumerable<Bar> bars = localdb.QryBar(request.Symbol, request.IntervalType, request.Interval, request.Start, request.End, (int)request.MaxCount, request.FromEnd);

            logger.Info("got cnt:" + bars.Count());
        }

        
    }
}
