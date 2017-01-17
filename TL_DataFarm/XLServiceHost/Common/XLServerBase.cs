using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace XLServiceHost
{
    public class XLServerBase : AppServer<XLSessionBase,XLRequestInfo>
    {
        public XLServerBase()
            : base(new DefaultReceiveFilterFactory<XLReceiveFilter, XLRequestInfo>())
        {

        }
    }
}
