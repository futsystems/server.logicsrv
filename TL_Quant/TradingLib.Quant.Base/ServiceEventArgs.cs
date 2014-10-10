using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    public class ServiceEventArgs : EventArgs
    {


        // Methods
        public ServiceEventArgs()
        {
            this.Message = string.Empty;
        }

        // Properties
        public ServiceEventType EventType
        {
            get;
            set;
        }

        public string Message
        {
            get;
            set;
        }

        public bool ShouldSyncAccountState
        {
            get;
            set;
        }

        public bool SuppressAutoReconnect
        {
            get;
            set;
        }
    }


}
