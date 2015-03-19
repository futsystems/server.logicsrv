using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public partial class TLClientNet:IMGRClient
    {

        #region Event
        /// <summary>
        /// 日志输出回调
        /// </summary>
        public event DebugDelegate OnDebugEvent;
        /// <summary>
        /// 行情连接建立回调
        /// </summary>
        public event VoidDelegate OnDataConnectEvent;
        /// <summary>
        /// 行情连接断开回调
        /// </summary>
        public event VoidDelegate OnDataDisconnectEvent;
        /// <summary>
        /// 交易连接建立回调
        /// </summary>
        public event VoidDelegate OnConnectEvent;
        /// <summary>
        /// 交易连接断开回调
        /// </summary>
        public event VoidDelegate OnDisconnectEvent;
        /// <summary>
        /// 登入回调
        /// </summary>
        public event RspMGRLoginResponseDel OnLoginEvent;


        /// <summary>
        /// 行情回调
        /// </summary>
        public event TickDelegate OnTickEvent;
        /// <summary>
        /// 委托回报回调
        /// </summary>
        //public event OrderDelegate OnOrderEvent;
        /// <summary>
        /// 昨日持仓回调
        /// </summary>
        //public event PositionDelegate OnOldPositionEvent;
        /// <summary>
        /// 持仓更新回调
        /// </summary>
        //public event PositionDelegate OnPositionUpdateEvent;
        /// <summary>
        /// 成交回调
        /// </summary>
        //public event FillDelegate OnTradeEvent;


        #endregion

        TLClient_MQ connecton = null;


        public TLVersion ServerVersion { get { return connecton == null ? null : connecton.ServerVersion; } }
        string[] _servers = new string[] { };
        int _port = 5570;


        string _account = "";
        public TLClientNet(string[] servers, int port, bool verb)
        {
            _servers = servers;
            _port = port;
            _noverb = !verb;

        }
        public void Start()
        {
            debug("TLClientNet Starting......", QSEnumDebugLevel.INFO);
            connecton = new TLClient_MQ(_servers, _port, "demo", _noverb);
            connecton.ProviderType = QSEnumProviderType.Both;
            BindConnectionEvent();

            connecton.Start();


        
        }

        public void Stop()
        {
            debug("TLClientNet Stopping......");
            if (connecton != null && connecton.IsConnected)
            {
                connecton.Stop();
            }
            connecton = null;
            
        }

        void BindConnectionEvent()
        {
            connecton.OnDebugEvent += new DebugDelegate(connecton_OnDebugEvent);
            connecton.OnConnectEvent += new ConnectDel(connecton_OnConnectEvent);
            connecton.OnDisconnectEvent += new DisconnectDel(connecton_OnDisconnectEvent);
            connecton.OnDataPubConnectEvent += new DataPubConnectDel(connecton_OnDataPubConnectEvent);
            connecton.OnDataPubDisconnectEvent += new DataPubDisconnectDel(connecton_OnDataPubDisconnectEvent);

            connecton.OnTick += new TickDelegate(connecton_OnTick);
            connecton.OnPacketEvent += new IPacketDelegate(connecton_OnPacketEvent);


        }

        
        int requestid = 0;

        void SendPacket(IPacket packet)
        {
            //权限或者登入状态检查
            if (connecton != null && connecton.IsConnected)
            {
                connecton.TLSend(packet);
            }
        }

        void connecton_OnDataPubDisconnectEvent()
        {
            if (OnDataDisconnectEvent != null)
                OnDataDisconnectEvent();
        }

        void connecton_OnDataPubConnectEvent()
        {
            if (OnDataConnectEvent != null)
                OnDataConnectEvent();
        }

        void connecton_OnDisconnectEvent()
        {
            if (OnDisconnectEvent != null)
                OnDisconnectEvent();
        }

        void connecton_OnConnectEvent()
        {
            if (OnConnectEvent != null)
                OnConnectEvent();
        }

        void connecton_OnDebugEvent(string msg)
        {
            if (OnDebugEvent != null)
                OnDebugEvent(msg);
        }

        void connecton_OnTick(Tick t)
        {
            this.handler.OnTick(t);
        }







        ILogicHandler handler = null;
        public void BindLogicHandler(ILogicHandler h)
        {
            handler = h;
        }

        #region 功能函数

        bool _debugEnable = true;
        public bool DebugEnable { get { return _debugEnable; } set { _debugEnable = value; } }
        QSEnumDebugLevel _debuglevel = QSEnumDebugLevel.DEBUG;
        public QSEnumDebugLevel DebugLevel { get { return _debuglevel; } set { _debuglevel = value; } }

        /// <summary>
        /// 判断日志级别 然后再进行输出
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="level"></param>
        protected void debug(string msg, QSEnumDebugLevel level = QSEnumDebugLevel.DEBUG)
        {
            if ((int)level <= (int)_debuglevel && _debugEnable)
                msgdebug("[" + level.ToString() + "] " + msg);
        }
        void msgdebug(string msg)
        {
            if (OnDebugEvent != null)
                OnDebugEvent(msg);

        }
        bool _noverb = true;
        /// <summary>
        /// enable/disable extended debugging
        /// </summary>
        public bool VerboseDebugging { get { return !_noverb; } set { _noverb = !value; } }

        void v(string msg)
        {
            if (_noverb) return;
            msgdebug(msg);
        }


        #endregion

    }
}
