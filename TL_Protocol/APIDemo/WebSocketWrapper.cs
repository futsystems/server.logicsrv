using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WebSocket4Net;
using Common.Logging;

namespace APIClient
{
    public class WebSocketWrapper : WebSocket
    {
        ILog logger = LogManager.GetLogger("WebSocketWrapper");

        public WebSocketWrapper(string address)
            : base(address)
        { 
        
        }

        public new void Send(string msg)
        {
            logger.Info("Request:" + msg);
            base.Send(msg);
        }
    }
}
