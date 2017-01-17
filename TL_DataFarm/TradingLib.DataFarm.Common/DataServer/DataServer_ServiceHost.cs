using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;

using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServer
    {
        /// <summary>
        /// ServiceHost插件目录
        /// </summary>
        private readonly string _ServiceHostFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceHost");

        /// <summary>
        /// IServiceHost插件 用于实现不同协议的行情服务
        /// </summary>
        private readonly List<IServiceHost> _serviceHosts = new List<IServiceHost>();

        /// <summary>
        /// 扩展命令Map
        /// </summary>
        Dictionary<string, DataCommand> cmdmap = new Dictionary<string, DataCommand>();


        bool _serviceHostLoaded = false;

        /// <summary>
        /// 启动ServiceHost
        /// </summary>
        void StartServiceHosts()
        {
            logger.Info("[Start ServiceHost]");
            if (!_serviceHostLoaded)
            {
                LoadServiceHosts();
            }
            foreach (var h in _serviceHosts)
            {
                StartServiceHost(h);
            }

            //加载所有命令
            ParseCommand();
        }


        /// <summary>
        /// 加载ServiceHost
        /// </summary>
        void LoadServiceHosts()
        {
            string[] aDLLs = null;

            try
            {
                aDLLs = Directory.GetFiles(_ServiceHostFolder, "*.dll");
            }
            catch (Exception ex)
            {
                logger.Error("Load ServiceHost Error:" + ex.ToString());
            }
            if (aDLLs.Length == 0)
                return;

            foreach (string item in aDLLs)
            {
                Assembly aDLL = Assembly.UnsafeLoadFrom(item);
                Type[] types = aDLL.GetTypes();

                foreach (Type type in types)
                {
                    try
                    {
                        if (type.GetInterface("TradingLib.DataFarm.API.IServiceHost") != null)
                        {
                            object o = Activator.CreateInstance(type);

                            if (o is IServiceHost)
                            {
                                _serviceHosts.Add(o as IServiceHost);
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }
            _serviceHostLoaded = true; 
        }

        /// <summary>
        /// 启动某个ServiceHost
        /// 绑定ServiceHost
        /// </summary>
        /// <param name="host"></param>
        void StartServiceHost(IServiceHost host)
        {
            host.ConnectionCreatedEvent += new Action<IServiceHost, IConnection>(OnConnectionCreatedEvent);
            host.ConnectionClosedEvent += new Action<IServiceHost, IConnection>(OnConnectionClosedEvent);
            host.RequestEvent += new Action<IServiceHost, IConnection, IPacket>(OnRequestEvent);
            host.ServiceEvent += new Func<IServiceHost, IPacket, IPacket>(OnServiceEvent);
            host.XLRequestEvent += new Action<IServiceHost, IConnection, object, int>(OnXLRequestEvent);
            host.Start();
        }

        

        void ParseCommand()
        {
            foreach (var info in this.FindCommand())
            {
                string key = "{0}-{1}".Put(ConstantData.MODULE_NAME.ToUpper(), info.Attr.CmdStr.ToUpper());
                cmdmap.Add(key, new DataCommand(this, info));
            }
        }

        void OnConnectionClosedEvent(IServiceHost arg1, IConnection arg2)
        {
            logger.Info(string.Format("ServiceHost:{0} Connection:{1} Closed",arg1.Name,arg2.SessionID));
            OnConnectionClosed(arg1, arg2);
        }

        void OnConnectionCreatedEvent(IServiceHost arg1, IConnection arg2)
        {
            logger.Info(string.Format("ServiceHost:{0} Connection:{1} Created", arg1.Name, arg2.SessionID));
            OnConnectionCreated(arg1, arg2);
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

        void OnXLRequestEvent(IServiceHost host, IConnection conn, object xldata, int requestId)
        {
            var reqPkt = xldata as XLProtocol.XLPacketData;
            if (reqPkt == null)
            {
                logger.Error("XLRequest Data null");
                return;
            }
            //更新客户端连接心跳
            SrvUpdateHeartBeat(conn);
            switch (reqPkt.MessageType)
            {
                case XLProtocol.XLMessageType.T_REQ_LOGIN:
                    {
                        var data = reqPkt.FieldList[0];
                        if (data is XLReqLoginField)
                        {
                            XLReqLoginField request = (XLReqLoginField)data;

                            XLRspLoginField field = new XLRspLoginField();
                            field.TradingDay = 1;
                            field.UserID = request.UserID;
                            field.Name = "";

                            ErrorField rsp = new ErrorField();

                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                            pkt.AddField(rsp);
                            pkt.AddField(field);

                            byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, true);

                            conn.Send(ret);
                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", reqPkt.MessageType));
                        }
                        break;
                    }
                    break;
                case XLProtocol.XLMessageType.T_REQ_UPDATEPASS:
                    { 
                    
                    }
                    break;
                case XLProtocol.XLMessageType.T_QRY_SYMBOL:
                    { 
                    
                    }
                    break;
                default:
                    logger.Warn(string.Format("XLMessage Type:{0} not handled", reqPkt.MessageType));
                    break;
            }
        }

        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="packet"></param>
        void OnRequestEvent(IServiceHost host, IConnection conn, IPacket packet)
        {
            //更新客户端连接心跳
            SrvUpdateHeartBeat(conn);
            switch (packet.Type)
            {
                //响应客户端登入请求
                case MessageTypes.LOGINREQUEST:
                    SrvOnLoginRequest(host, conn, packet as LoginRequest);
                    break;
                //响应客户端版本查询
                case MessageTypes.VERSIONREQUEST:
                    SrvOnVersionRequest(host, conn, packet as VersionRequest);
                    break;
                //查询功能列表
                case MessageTypes.FEATUREREQUEST:
                    SrvOnFeatureRequest(host, conn, packet as FeatureRequest);
                    break;
                ////响应客户端心跳
                case MessageTypes.HEARTBEAT:
                    SrvUpdateHeartBeat(conn);
                    break;
                //响应客户端心跳查询
                case MessageTypes.HEARTBEATREQUEST:
                    SrvOnHeartbeatRequest(host, conn, packet as HeartBeatRequest);
                    break;
                //查询交易时间段
                case MessageTypes.XQRYMARKETTIME:
                    SrvOnQryMarketTimeRequest(host, conn, packet as XQryMarketTimeRequest);
                    break;
                //查询交易所
                case MessageTypes.XQRYEXCHANGE:
                    SrvOnQryExchangeRequest(host, conn, packet as XQryExchangeRequuest);
                    break;
                //查询品种数据
                case MessageTypes.XQRYSECURITY:
                    SrvOnQrySecurityRequest(host, conn, packet as XQrySecurityRequest);
                    break;
                //查询合约数据
                case MessageTypes.XQRYSYMBOL:
                    SrvOnQrySymbolRequest(host, conn, packet as XQrySymbolRequest);
                    break;
                //注册合约行情数据
                case MessageTypes.REGISTERSYMTICK:
                    SrvOnRegisterSymbolTick(host, conn, packet as RegisterSymbolTickRequest);
                    break;
                //注销合约行情数据
                case MessageTypes.UNREGISTERSYMTICK:
                    SrvOnUnregisterSymbolTick(host, conn, packet as UnregisterSymbolTickRequest);
                    break;
                //查询行情快照
                case MessageTypes.XQRYTICKSNAPSHOT:
                    SrvOnXQryTickSnapshot(host, conn, packet as XQryTickSnapShotRequest);
                    break;
                //查询历史Bar数据
                case MessageTypes.BARREQUEST:
                    SrvOnBarRequest(host, conn, packet as QryBarRequest);
                    break;
                //查询成交数据
                case MessageTypes.XQRYTRADSPLIT:
                    SrvOnQryTradeSplitRequest(host, conn, packet as XQryTradeSplitRequest);
                    break;
                //查询价格成交量
                case MessageTypes.XQRYPRICEVOL:
                    SrvOnQryPriceVolRequest(host, conn, packet as XQryPriceVolRequest);
                    break;
                //查询分时数据
                case MessageTypes.XQRYMINUTEDATA:
                    SrvOnQryMinuteDataRequest(host, conn, packet as XQryMinuteDataRequest);
                    break;
                case MessageTypes.MD_DEMOTICK:
                    SrvOnMDDemoTick(host, conn, packet as MDDemoTickRequest);
                    break;



                #region 管理操作
                ////更新合约
                //case MessageTypes.MGRUPDATESYMBOL:
                //    SrvOnMGRUpdateSymbol(host, conn, packet as MGRUpdateSymbolRequest);
                //    break;
                ////更新品种
                //case MessageTypes.MGRUPDATESECURITY:
                //    SrvOnMGRUpdateSecurity(host, conn, packet as MGRUpdateSecurityRequest);
                //    break;
                ////更新交易所
                //case MessageTypes.MGRUPDATEEXCHANGE:
                //    SrvOnMGRUpdateExchange(host, conn, packet as MGRUpdateExchangeRequest);
                //    break;
                ////更新交易小节
                //case MessageTypes.MGRUPDATEMARKETTIME:
                //    SrvOnMGRUpdateMarketTime(host, conn, packet as MGRUpdateMarketTimeRequest);
                //    break;
                //扩展命令
                case MessageTypes.MGR_REQ_CONTRIB:
                    SrvOnMGRContribRequest(host, conn, packet as MGRContribRequest);
                    break;
                //上传历史数据
                case MessageTypes.MGR_MD_UPLOADBARDATA:
                    SrvOnMGRUploadBarData(host, conn, packet as UploadBarDataRequest);
                    break;
                #endregion
                default:
                    logger.Warn(string.Format("Message Type:{0} not handled", packet.Type));
                    break;
            }
        }
    }
}
