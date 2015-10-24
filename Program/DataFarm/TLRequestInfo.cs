using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using TradingLib.API;
using TradingLib.Common;

namespace DataFarm
{
    public class TLRequestInfo : RequestInfo<string>
    {
        public TLRequestInfo(string key,string content,Message message)
            : base(key, content)
        {
            this.Message = message;
        }

        public Message Message { get; private set; }
    }
}
