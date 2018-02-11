﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;
using Common.Logging;

namespace FrontServer.XLServiceHost
{
    /// <summary>
    /// 连接状态
    /// 记录连接相关信息
    /// </summary>
    public class ConnectionState:FrontServer.IConnectionState
    {
        public ConnectionState()
        {
            this.LastHeartBeat = DateTime.Now;
            this.CTPVersion = string.Empty;
            this.MACAddress = string.Empty;
            this.IPAddress = string.Empty;
            this.Authorized = false;
            this.LoginID = string.Empty;
            this.BrokerID = string.Empty;

        }
        /// <summary>
        /// 最近心跳时间
        /// </summary>
        public DateTime LastHeartBeat { get; set; }

        /// <summary>
        /// CTP
        /// </summary>
        public string CTPVersion { get; set; }

        /// <summary>
        /// MAC 地址
        /// </summary>
        public string MACAddress { get; set; }

        /// <summary>
        /// 网络地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 是否已认证
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// 登入ID
        /// </summary>
        public string LoginID { get; set; }

        /// <summary>
        /// BrokerID
        /// </summary>
        public string BrokerID { get; set; }

        /// <summary>
        /// 客户端信息
        /// </summary>
        public string ProductInfo { get; set; }


        /// <summary>
        /// 前置编号
        /// </summary>
        public int FrontID { get; set; }

        /// <summary>
        /// 会话编号
        /// </summary>
        public int SessionID { get; set; }
    }

    public class XLConnection : FrontServer.IConnection
    {
        XLSessionBase _session = null;
        public XLConnection(XLServiceHost host, XLSessionBase session)
        {
            _serviceHost = host;
            _session = session;

            this.SessionID = _session.SessionID;
            this.State = new ConnectionState();
            this.State.IPAddress = session.RemoteEndPoint.ToString();
        }

        public bool Connected { get { return _session.Connected; } }

        /// <summary>
        /// 回话编号
        /// </summary>
        public string SessionID { get; private set; }

        /// <summary>
        /// 返回用户ID
        /// </summary>
        public string UserID { get { return this.State.LoginID; } }


        FrontServer.IServiceHost _serviceHost = null;
        /// <summary>
        /// Connection所在的ServiceHost对象
        /// </summary>
        public FrontServer.IServiceHost ServiceHost { get { return _serviceHost; } }

        uint _seqQryId = 1;
        object _seqIDLock = new object();
        /// <summary>
        /// 下一个Req回报序号
        /// </summary>
        public uint NextSeqQryId
        {
            get
            {
                lock (_seqIDLock)
                {
                    uint seq = _seqQryId;
                    _seqQryId++;
                    return seq;

                }
            }
        }
        /// <summary>
        /// RTN 需要需要从1开始 否则CTP客户端接口无法正常解析回报数据
        /// </summary>
        uint _seqRtnId = 1;
        /// <summary>
        /// 下一个RTN回报序号
        /// </summary>
        public uint NextSeqRtnId
        {
            get
            {
                lock (_seqIDLock)
                {
                    uint seq = _seqRtnId;
                    _seqRtnId++;
                    return seq;

                }
            }
        }

        uint _seqReqId = 0;
        /// <summary>
        /// 下一个REQ回报序号
        /// </summary>
        public uint NextSeqReqId
        {
            get
            {
                lock (_seqIDLock)
                {
                    uint seq = _seqReqId;
                    _seqReqId++;
                    return seq;

                }
            }
        }

        public ConnectionState State { get; private set; }
        public FrontServer.IConnectionState IState { get { return this.State; } }
        /// <summary>
        /// 更新心跳状态
        /// </summary>
        public void UpdateHeartBeat()
        {
            this.State.LastHeartBeat = DateTime.Now;
        }

        public void Send(byte[] data)
        {
            _session.Send(data, 0, data.Length);
        }

        public void Send(byte[] data, int len)
        {
            _session.Send(data, 0, len);
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            _session.Close();
        }
    }
}