using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace FrontServer.TLServiceHost
{
    public class TLServerBase : AppServer<TLSessionBase,TLRequestInfo>
    {
        public TLServerBase()
            : base(new DefaultReceiveFilterFactory<TLReceiveFilter, TLRequestInfo>())
        {

        }
    }
}
