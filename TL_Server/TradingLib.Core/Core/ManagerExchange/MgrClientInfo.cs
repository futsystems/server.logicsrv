using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public class MgrClientInfo:ClientInfoBase
    {

        public MgrClientInfo()
            :base()
        {
            this.ManagerID = string.Empty;
        }
        public string ManagerID { get; set; }
    }
}
