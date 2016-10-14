using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;
using Common.Logging;

namespace TCPServiceHost
{
    public class TLSessionBase : AppSession<TLSessionBase, TLRequestInfo>
    {
        ILog logger = LogManager.GetLogger("TLSession");
        protected override void OnSessionStarted()
        {
            logger.Info("Session Started");
        }

        
        protected override void HandleException(Exception e)
        {

        }

        
        
    }
}
