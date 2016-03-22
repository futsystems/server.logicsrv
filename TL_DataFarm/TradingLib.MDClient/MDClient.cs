﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace TradingLib.MDClient
{
    /// <summary>
    /// 行情客户端接口
    /// 行情客户端接口维护2个Socket连接到服务端1个Socket用于实时行情数据,1个行情用于历史行情数据
    /// </summary>
    public partial class MDClient:IBasicInfo
    {

        ILog logger = LogManager.GetLogger("MDClient");

        TLClient<TLSocket_TCP> realClient = null;
        TLClient<TLSocket_TCP> histClient = null;
        MDHandlerBase _handler = null;

        int requestid = 0;
        object _reqidobj = new object();
        protected int NextRequestID
        {
            get
            {
                lock (_reqidobj)
                {
                    return ++requestid;
                }
            }
        }


        /// <summary>
        /// 单台服务器同时提供行情与地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="realport"></param>
        /// <param name="histport"></param>
        public MDClient(string address, int realport, int histport)
            :this(new string[]{address},realport,new string[]{address},histport)
        { 
            
        }

        /// <summary>
        /// 提供一组实时行情地址与历史行情地址
        /// </summary>
        /// <param name="realservers"></param>
        /// <param name="realport"></param>
        /// <param name="histservers"></param>
        /// <param name="histport"></param>
        public MDClient(string[] realservers,int realport,string[] histservers,int histport)
        {

            realClient = new TLClient<TLSocket_TCP>(realservers, realport, "MDClient-Real");
            histClient = new TLClient<TLSocket_TCP>(histservers, histport, "MDClient-Hist");

            //绑定事件
            WireEvent();
        }


        /// <summary>
        /// 注册行情回调函数
        /// </summary>
        /// <param name="notify"></param>
        public void RegisterHandler(MDHandlerBase handler)
        {
            _handler = handler;
        }

        public void Start()
        {
            logger.Info("Start MDClient");
            histClient.Start();
        }

        /// <summary>
        /// 绑定事件
        /// </summary>
        void WireEvent()
        {
            realClient.OnConnectEvent += new ConnectDel(realClient_OnConnectEvent);
            realClient.OnDisconnectEvent += new DisconnectDel(realClient_OnDisconnectEvent);
            realClient.OnPacketEvent += new Action<IPacket>(realClient_OnPacketEvent);

            histClient.OnConnectEvent += new ConnectDel(histClient_OnConnectEvent);
            histClient.OnDisconnectEvent += new DisconnectDel(histClient_OnDisconnectEvent);
            histClient.OnPacketEvent += new Action<IPacket>(histClient_OnPacketEvent);
        }

        /// <summary>
        /// 解绑事件
        /// </summary>
        void UnwireEvent()
        {
            realClient.OnConnectEvent -= new ConnectDel(realClient_OnConnectEvent);
            realClient.OnDisconnectEvent -= new DisconnectDel(realClient_OnDisconnectEvent);
            realClient.OnPacketEvent -= new Action<IPacket>(realClient_OnPacketEvent);

            histClient.OnConnectEvent -= new ConnectDel(histClient_OnConnectEvent);
            histClient.OnDisconnectEvent -= new DisconnectDel(histClient_OnDisconnectEvent);
            histClient.OnPacketEvent -= new Action<IPacket>(histClient_OnPacketEvent);
        }


        void histClient_OnPacketEvent(IPacket obj)
        {
            logger.Debug(string.Format("Hist Packet Type:{0} Content:{1}", obj.Type, obj.Content));
            switch (obj.Type)
            {
                case MessageTypes.TICKNOTIFY:
                    {
                        TickNotify response = obj as TickNotify;
                        if (_handler != null)
                        {
                            _handler.OnRtnTick(response.Tick);
                        }
                        return;
                    }
                case MessageTypes.XMARKETTIMERESPONSE:
                    {
                        RspXQryMarketTimeResponse response = obj as RspXQryMarketTimeResponse;
                        OnXQryMarketTimeResponse(response);
                        return;
                    }
                case MessageTypes.XEXCHANGERESPNSE:
                    {
                        RspXQryExchangeResponse response = obj as RspXQryExchangeResponse;
                        OnXQryExchangeResponse(response);
                        return;
                    }
                case MessageTypes.XSECURITYRESPONSE:
                    {
                        RspXQrySecurityResponse response = obj as RspXQrySecurityResponse;
                        OnXQrySecurityResponse(response);
                        return;
                    }
                case MessageTypes.XSYMBOLRESPONSE:
                    {
                        RspXQrySymbolResponse response = obj as RspXQrySymbolResponse;
                        OnXQrySymbolResponse(response);
                        return;
                    }
                case MessageTypes.BARRESPONSE:
                    {
                        RspQryBarResponse response = obj as RspQryBarResponse;
                        if (_handler != null)
                        {
                            _handler.OnRspQryBar(response.Bar, response.RspInfo, response.RequestID, response.IsLast);
                        }
                        return;
                    }
                case MessageTypes.BIN_BARRESPONSE:
                    {
                        RspQryBarResponseBin response = obj as RspQryBarResponseBin;
                        if (_handler != null)
                        {
                            _handler.OnRspQryBarBin(response.Bars, null, 0, true);
                        }
                        return;
                    }
                default:
                    logger.Warn(string.Format("Message Type:{0} not supported", obj.Type));
                    return;
            }
        }

        void histClient_OnDisconnectEvent()
        {
            logger.Info(string.Format("Hist Socket Disconnected"));
        }

        void histClient_OnConnectEvent()
        {
            logger.Info(string.Format("Hist Socket Connected Server:{0} Port:{1}", histClient.CurrentServer.Address, histClient.CurrentServer.Port));
            //执行登入

            //执行查询
            QryMarketTime();
        }


        void realClient_OnPacketEvent(IPacket obj)
        {
            logger.Info(string.Format("Real Packet Type:{0} Content:{1}", obj.Type, obj.Content));
        }

        void realClient_OnDisconnectEvent()
        {
            logger.Info(string.Format("Real Socket Disconnected"));
        }

        void realClient_OnConnectEvent()
        {
            logger.Info(string.Format("Real Socket Connected Server:{0} Port:{1}", histClient.CurrentServer.Address, histClient.CurrentServer.Port));
       
        }
    }
}
