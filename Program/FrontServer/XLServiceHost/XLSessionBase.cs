﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;
using Common.Logging;

namespace FrontServer.XLServiceHost
{
    public class XLSessionBase : AppSession<XLSessionBase, XLRequestInfo>
    {
        ILog logger = LogManager.GetLogger("XLSession");
        protected override void OnSessionStarted()
        {
            logger.Debug(string.Format("Session:{0} Started", this.SessionID));
            base.OnSessionStarted();
        }

        
        protected override void HandleException(Exception e)
        {
            logger.Error("ex:" + e.ToString());
            base.HandleException(e);
        }

        protected override void OnSessionClosed(CloseReason reason)
        {
            logger.Debug(string.Format("Session:{0} Closed:{1}", this.SessionID, reason));
            base.OnSessionClosed(reason);
        }

        

        
        
    }
}