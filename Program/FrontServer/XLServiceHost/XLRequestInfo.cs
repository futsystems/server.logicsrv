using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.XLProtocol;

namespace FrontServer.XLServiceHost
{
    public class XLRequestInfo : RequestInfo<XLPacketData>
    {
        public XLRequestInfo(string key,XLDataHeader header,  XLPacketData data)
            : base(key, data)
        {
            DataHeader = header;
        }

        public XLDataHeader DataHeader;

    }
}
