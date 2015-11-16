using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServerBase
    {

        //更新客户端心跳时间戳
        void SrvUpdateHeartBeat(IConnection conn)
        {
            conn.LastHeartBeat = DateTime.Now;
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
            v.Major = 1;
            v.Minor = 0;
            v.Fix = 0;
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
            IHistDataStore store = this.GetHistDataSotre();
            if (store == null)
            {
                logger.Warn("HistDataSotre is null, can not provider QryBar service");
            }

            Bar[] bars = store.QryBar(request.Symbol, request.IntervalType, request.Interval, request.Start, request.End, (int)request.MaxCount, request.FromEnd).ToArray();

            logger.Info("got bar cnt:" + bars.Count());

            for (int i = 0; i < bars.Length; i++)
            {
                RspQryBarResponse response = ResponseTemplate<RspQryBarResponse>.SrvSendRspResponse(request);
                response.Bar = bars[i];
                conn.SendResponse(response, i == bars.Length - 1);
            }
        }


        void SrvOnRegisterSymbolTick(IServiceHost host, IConnection conn, RegisterSymbolTickRequest request)
        {
            logger.Info(string.Format("Conn:{0} try to register symbol:{1}", conn.SessionID, request.Symbols));
            OnRegisterSymbol(conn, request);
            
        }

        void SrvOnUnregisterSymbolTick(IServiceHost host, IConnection conn, UnregisterSymbolTickRequest request)
        {
            logger.Info(string.Format("Conn:{0} try to unregister symbol:{1}", conn.SessionID, request.Symbols));
            OnUngisterSymbol(conn, request);
        }
    }
}
