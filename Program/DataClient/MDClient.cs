using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace DataClient
{
    /// <summary>
    /// 行情客户端
    /// 用于封装TLClient通讯
    /// 正对行情部分进行封装
    /// </summary>
    public class MDClient
    {

        ILog logger = LogManager.GetLogger("MDClient");

        TLClient<TLSocket_TCP> _tlclient;

        public MDClient()
        {
            _tlclient = new TLClient<TLSocket_TCP>("127.0.0.1", 5060, "MDClient");

            _tlclient.OnConnectEvent += new ConnectDel(_tlclient_OnConnectEvent);
            _tlclient.OnDisconnectEvent += new DisconnectDel(_tlclient_OnDisconnectEvent);
            _tlclient.OnPacketEvent += new Action<IPacket>(_tlclient_OnPacketEvent);
        }

        void _tlclient_OnPacketEvent(IPacket obj)
        {
            //throw new NotImplementedException();
        }

        void _tlclient_OnDisconnectEvent()
        {
            //throw new NotImplementedException();
        }

        void _tlclient_OnConnectEvent()
        {
            //throw new NotImplementedException();
        }

    }
}
