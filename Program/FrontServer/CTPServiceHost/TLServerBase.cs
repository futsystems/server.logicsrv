using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace CTPService
{
    public class CTPServerBase : AppServer<CTPSessionBase, CTPRequestInfo>
    {
        public CTPServerBase()
            : base(new DefaultReceiveFilterFactory<CTPReceiveFilter, CTPRequestInfo>())
        {

        }
    }
}
