using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;
using TradingLib.XLProtocol;


namespace FrontServer.TLServiceHost
{
    /// <summary>
    /// 连接状态
    /// 记录连接相关信息
    /// </summary>
    public class ConnectionState : FrontServer.IConnectionState
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

    public class TLConnection : FrontServer.IConnection
    {
        TLSessionBase _session = null;
        ILog logger = LogManager.GetLogger("conn");

        public TLConnection(TLServiceHost host, TLSessionBase session)
        {
            _serviceHost = host;
            _session = session;

            this.SessionID = _session.SessionID;
            this.State = new ConnectionState();
            string[] remote = session.RemoteEndPoint.ToString().Split(':');
            if (remote.Length == 2)
            {
                this.State.IPAddress = remote[0];
            }
        }


        public bool IsXLProtocol { get { return false; } }

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

        int _seqQryId = 1;
        object _seqIDLock = new object();
        /// <summary>
        /// 下一个Req回报序号
        /// </summary>
        public int NextSeqQryId
        { 
            get
            {
                lock (_seqIDLock)
                {
                    int seq = _seqQryId;
                    _seqQryId++;
                    return seq;
                    
                }
            }
        }
        /// <summary>
        /// RTN 需要需要从1开始 否则CTP客户端接口无法正常解析回报数据
        /// </summary>
        int _seqRtnId = 1;
        /// <summary>
        /// 下一个RTN回报序号
        /// </summary>
        public int NextSeqRtnId
        {
            get
            {
                lock (_seqIDLock)
                {
                    int seq = _seqRtnId;
                    _seqRtnId++;
                    return seq;

                }
            }
        }

        int _seqReqId = 0;
        /// <summary>
        /// 下一个REQ回报序号
        /// </summary>
        public int NextSeqReqId
        {
            get
            {
                lock (_seqIDLock)
                {
                    int seq = _seqReqId;
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
        /// 向逻辑服务器发送数据包
        /// 客户端提交上来的请求转换成内部数据格式 向逻辑服务器发送
        /// </summary>
        /// <param name="packet"></param>
        public void ForwardToLogic(IPacket packet)
        {
            logger.Info("ForwardToLogic:" + packet.ToString());
            //
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            _session.Close();
        }

        /// <summary>
        /// 应答XLPacketData
        /// </summary>
        /// <param name="data"></param>
        public void ResponseXLPacket(XLPacketData data, uint requestID, bool isLast)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 通知XLPacketData
        /// </summary>
        /// <param name="data"></param>
        public void NotifyXLPacket(XLPacketData data)
        {
            throw new NotImplementedException();
        }
    }
}
