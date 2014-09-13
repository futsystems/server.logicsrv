using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public struct SessionInfo
    {
        public string Account;
        public bool LoggedIn;
        public string IPAddress;

        public SessionInfo(string accid, bool online,string address )
        {
            Account = accid;
            LoggedIn = online;
            IPAddress = address;
        }

    }
}
