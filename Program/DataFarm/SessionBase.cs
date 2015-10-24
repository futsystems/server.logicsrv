using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;

namespace DataFarm
{
    public class SessionBase : AppSession<SessionBase, BinaryRequestInfo>
    {
        protected override void OnSessionStarted()
        {
            Send("Welcome");
        }

        protected override void HandleException(Exception e)
        {

        }
    }
}
