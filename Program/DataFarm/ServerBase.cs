using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;

namespace DataFarm
{
    public class ServerBase : AppServer<SessionBase,BinaryRequestInfo>
    {
        public ServerBase()
            //: base(new CommandLineReceiveFilterFactory(Encoding.Default, new CustomRequestInfoParser()))
            :base(new DefaultReceiveFilterFactory<ReceiveFilter,BinaryRequestInfo>())
        {

        }
    }
}
