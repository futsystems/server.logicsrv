using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServerBase
    {
        /// <summary>
        /// ServiceHost插件目录
        /// </summary>
        private readonly string _ServiceHostFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ServiceHost");
        private readonly List<IServiceHost> _serviceHosts = new List<IServiceHost>();

        bool _serviceHostLoaded = false;
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

                        //connection service must support IDataServerServiceHost interface
                        if (type.GetInterface("TradingLib.DataFarm.API.IServiceHost") != null)
                        {
                            object o = Activator.CreateInstance(type);

                            if (o is DataFarm.API.IServiceHost)
                            {
                                _serviceHosts.Add(o as DataFarm.API.IServiceHost);
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
        /// 启动ServiceHost
        /// </summary>
        protected void StartServiceHosts()
        {
            if (!_serviceHostLoaded)
            {
                LoadServiceHosts();
            }

            logger.Info("Start ServiceHosts....");
            foreach (var h in _serviceHosts)
            {
                StartServiceHost(h);
            }
        }

        /// <summary>
        /// 启动某个ServiceHost
        /// 绑定ServiceHost
        /// </summary>
        /// <param name="host"></param>
        void StartServiceHost(IServiceHost host)
        {
            host.SessionCreatedEvent += new Action<IServiceHost, IConnection>(OnSessionCreatedEvent);
            host.SessionClosedEvent += new Action<IServiceHost, IConnection>(OnSessionClosedEvent);
            host.RequestEvent += new Action<IServiceHost, IConnection, IPacket>(OnRequestEvent);    //(OnRequestEvent);
            host.ServiceEvent += new Func<IServiceHost, IPacket, IPacket>(OnServiceEvent);
            host.Start();
        }


        protected virtual void OnSessionClosedEvent(IServiceHost arg1, IConnection arg2)
        {
            logger.Info(string.Format("ServiceHost:{0} Connection:{1} Closed",arg1.Name,arg2.SessionID));
        }

        protected virtual void OnSessionCreatedEvent(IServiceHost arg1, IConnection arg2)
        {
            logger.Info(string.Format("ServiceHost:{0} Connection:{1} Closed", arg1.Name, arg2.SessionID));
            OnSessionCreated(arg1, arg2);
        }


        protected virtual IPacket OnServiceEvent(IServiceHost arg1, IPacket arg2)
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


        /// <summary>
        /// 处理请求
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="packet"></param>
        protected void ProcessRequest(IServiceHost host, IConnection conn, IPacket packet)
        {
            OnRequestEvent(host, conn, packet);
        }

        void OnRequestEvent(IServiceHost host, IConnection conn, IPacket packet)
        {
            //logger.Info(string.Format("ServiceHost:{0} Connection:{1} Request:{2}", host.Name, conn.SessionID, packet.ToString()));

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
                //查询历史Bar数据
                case MessageTypes.BARREQUEST:
                    SrvOnBarRequest(host, conn, packet as QryBarRequest);
                    break;
                default:
                    logger.Warn(string.Format("Message Type:{0} not handled", packet.Type));
                    break;
            }
        }

        


    }
}
