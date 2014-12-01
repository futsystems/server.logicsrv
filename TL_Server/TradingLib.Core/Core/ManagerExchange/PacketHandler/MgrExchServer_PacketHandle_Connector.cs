using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {

        #region Connector
        void SrvOnMGRQryConnector(MGRQryConnectorRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求查询通道列表:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            List<RspMGRQryConnectorResponse> responselist = new List<RspMGRQryConnectorResponse>();

            foreach (IBroker b in manager.Domain.GetBrokers())
            {
                RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
                response.Connector = new ConnectorInfo(b);
                responselist.Add(response);
            }

            foreach (IDataFeed d in TLCtxHelper.Ctx.RouterManager.DataFeeds)
            {
                RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
                response.Connector = new ConnectorInfo(d);
                responselist.Add(response);
            }

            int totalnum = responselist.Count;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    CacheRspResponse(responselist[i], i == totalnum - 1);
                }
            }
            else
            {

            }

        }

        void SrvOnMGRStartBroker(MGRReqStartBrokerRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求启动成交通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IBroker b = TLCtxHelper.Ctx.RouterManager.FindBroker(request.FullName);
            if (b != null && !b.IsLive)
            {
                string msg = string.Empty;
                bool success = b.Start(out msg);
                if (success)
                {
                    session.OperationSuccess(string.Format("交易通道[{0}]已启动", b.Token));
                }
                else
                {
                    session.OperationError(new FutsRspError(msg));
                }
            }

            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(b);
            CachePacket(response);
        }

        void SrvOnMGRStopBroker(MGRReqStopBrokerRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求停止成交通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IBroker b = TLCtxHelper.Ctx.RouterManager.FindBroker(request.FullName);
            if (b != null && b.IsLive)
            {
                //string msg = string.Empty;
                //bool success = b.Start(out msg);
                //if (success)
                //{
                //    session.OperationSuccess(string.Format("交易通道[{0}]已启动", b.Token));
                //}
                //else
                //{
                //    session.OperationError(new FutsRspError(msg));
                //}

                b.Stop();
                session.OperationSuccess(string.Format("交易通道[{0}]已停止", b.Token));
            }

            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(b);
            CachePacket(response);
        }
        void SrvOnMGRStartDataFeed(MGRReqStartDataFeedRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求启动行情通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);

            IDataFeed d = TLCtxHelper.Ctx.RouterManager.FindDataFeed(request.FullName);

            if (d != null && !d.IsLive)
            {
                d.Start();
                session.OperationSuccess(string.Format("行情通道[{0}]已启动", d.Token));
            }

            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(d);
            CachePacket(response);
        }
        void SrvOnMGRStopDataFeed(MGRReqStopDataFeedRequest request, ISession session, Manager manager)
        {
            debug(string.Format("管理员:{0} 请求停止行情通道:{1}", session.MGRLoginName, request.ToString()), QSEnumDebugLevel.INFO);
            IDataFeed d = TLCtxHelper.Ctx.RouterManager.FindDataFeed(request.FullName);
            if (d != null && d.IsLive)
            {
                d.Stop();
                session.OperationSuccess(string.Format("行情通道[{0}]已停止", d.Token));
            }
            RspMGRQryConnectorResponse response = ResponseTemplate<RspMGRQryConnectorResponse>.SrvSendRspResponse(request);
            response.Connector = new ConnectorInfo(d);
            CachePacket(response);
        }
        #endregion

    }
}
