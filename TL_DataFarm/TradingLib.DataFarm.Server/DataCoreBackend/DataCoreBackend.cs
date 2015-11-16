using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Logging;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;

namespace TradingLib.DataFarm.Common
{
    /// <summary>
    /// 用于维护到DataCore的ZMQ连接,用于DataFront启动时候加载基础数据
    /// 在响应客户端查询时从DataCore获得历史数据
    /// 
    /// </summary>
    public class DataCoreBackend
    {
        TLClient<TLSocket_ZMQ> zmqclient;
        string _address;
        int _port;
        ILog logger;
        public DataCoreBackend(string address, int port)
        {
            _address = address;
            _port = port;
            logger =LogManager.GetLogger("DataCoreConnector");

            InitConnector();
        }


        void InitConnector()
        {
            zmqclient = new TLClient<TLSocket_ZMQ>(_address, _port, "ZMQConnector");
            zmqclient.OnConnectEvent += new ConnectDel(zmqclient_OnConnectEvent);
            zmqclient.OnDisconnectEvent += new DisconnectDel(zmqclient_OnDisconnectEvent);
            zmqclient.OnPacketEvent += new Action<IPacket>(zmqclient_OnPacketEvent);
        }


        public void Start()
        {
            logger.Info("start datacore backend service");
            zmqclient.Start();
            
        }

        public void Stop()
        {
            zmqclient.Stop();
        }

        bool _inited = false;
        /// <summary>
        /// 是否初始化基础数据完成
        /// </summary>
        public bool IsInited { get { return _inited; } }
        /// <summary>
        /// 初始化完成事件
        /// </summary>
        public event Action InitedEvent;

        int _requestId = 0;
        private int NextRequestID
        {
            get
            {
                _requestId++;
                return _requestId;
            }
        }

        /// <summary>
        /// 处理客户查询Bar数据
        /// </summary>
        /// <param name="request"></param>
        public void QryBar(QryBarRequest request)
        {
            QryBarRequest brequest = RequestTemplate<QryBarRequest>.CliSendRequest(NextRequestID);
            brequest.FromEnd = request.FromEnd;
            brequest.Symbol = request.Symbol;
            brequest.MaxCount = request.MaxCount;
            brequest.Interval = request.Interval;
            brequest.Start = request.Start;
            brequest.End = request.End;

            zmqclient.TLSend(brequest);
            
        }

        void zmqclient_OnPacketEvent(IPacket obj)
        {
            logger.Info("zmqclient got message type:" + obj.Type.ToString() + " content:" + obj.Content);
            switch (obj.Type)
            { 
                case MessageTypes.XMARKETTIMERESPONSE:
                    SrvOnXQryMarketTimeResponse(obj as RspXQryMarketTimeResponse);
                    break;
                case MessageTypes.XEXCHANGERESPNSE:
                    SrvOnXQryExchangeResponse(obj as RspXQryExchangeResponse);
                    break;
                case MessageTypes.XSECURITYRESPONSE:
                    SrvOnXQrySecurityResponse(obj as RspXQrySecurityResponse);
                    break;
                case MessageTypes.XSYMBOLRESPONSE:
                    SrvOnXQrySymbolResponse(obj as RspXQrySymbolResponse);
                    break;
                case MessageTypes.BARRESPONSE:
                    SrvOnBarResponseResponse(obj as RspQryBarResponse);
                    break;
                default:
                    logger.Warn(string.Format("message type:{0} is not handled", obj.Type));
                    break;
            }
        }

        

        void zmqclient_OnDisconnectEvent()
        {
            logger.Info("DataCoreSrv disconnected");
        }

        void zmqclient_OnConnectEvent()
        {
            logger.Info("DataCoreSrv connected");
            //连接成功后执行基础数据加载
            this.QryMarketTime();
        }


        void QryMarketTime()
        {
            XQryMarketTimeRequest request = RequestTemplate<XQryMarketTimeRequest>.CliSendRequest(NextRequestID);
            zmqclient.TLSend(request);
        }

        void QryExchange()
        {
            XQryExchangeRequuest request = RequestTemplate<XQryExchangeRequuest>.CliSendRequest(NextRequestID);
            zmqclient.TLSend(request);
        }

        void QrySecurity()
        {

            XQrySecurityRequest request = RequestTemplate<XQrySecurityRequest>.CliSendRequest(NextRequestID);
            zmqclient.TLSend(request);
        }
        void QrySymbol()
        {
            XQrySymbolRequest request = RequestTemplate<XQrySymbolRequest>.CliSendRequest(NextRequestID);
            zmqclient.TLSend(request);
        }


        void SrvOnXQryMarketTimeResponse(RspXQryMarketTimeResponse response)
        {


            if (response.IsLast && !_inited)
            {
                this.QryExchange();
            }
        
        }

        void SrvOnXQryExchangeResponse(RspXQryExchangeResponse response)
        {
            if (response.IsLast && !_inited)
            {
                this.QrySecurity();
            }
        
        }

        void SrvOnXQrySecurityResponse(RspXQrySecurityResponse response)
        {
            if (response.IsLast && !_inited)
            {
                this.QrySymbol();
            }
        }

        void SrvOnXQrySymbolResponse(RspXQrySymbolResponse response)
        {

            if (response.IsLast && !_inited)
            {
                _inited = true;
                try
                {
                    if (InitedEvent != null)
                        InitedEvent();
                }
                catch (Exception ex)
                {
                    logger.Error("Fire InitEvent Error:" + ex.ToString());
                }
            }
        }



        void SrvOnBarResponseResponse(RspQryBarResponse response)
        { 
            
        }

    }
}
