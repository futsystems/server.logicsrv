using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    internal class JsonWrapperSessionUpdate
    {
        string _account = "";
        bool _online = false;
        ClientInfoBase _info = null;
        public JsonWrapperSessionUpdate(string account, bool online, ClientInfoBase info)
        {
            _account = account;
            _online = online;
            _info = info;
        
        }

        public string Account { get { return _account; } }

        public bool IsOnline { get { return _online; } }

        public string Location { get { return _info.IPAddress+"@"+(string.IsNullOrEmpty(_info.Location.FrontID)?"":_info.Location.FrontID); } }
    }
}
