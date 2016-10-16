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
            logger.Info(string.Format("Session:{0} Started", this.SessionID));
        }

        
        protected override void HandleException(Exception e)
        {
            logger.Error("ex:" + e.ToString());
            base.HandleException(e);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            logger.Info(string.Format("Session:{0} Closed:{1}", this.SessionID, reason));
            base.OnSessionClosed(reason);
        }

        

        
        
    }
}
