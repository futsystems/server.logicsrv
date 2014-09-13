using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Core
{
    /// <summary>
    /// 缓存委托类信息
    /// </summary>
    public struct OrderMessage
    {
        public Order Order;
        public string Message;
        public OrderMessage(Order o, string msg)
        {
            Order = o;
            Message = msg;
        }
    }
    /// <summary>
    /// 缓存系统类信息
    /// </summary>
    public struct SysMessage
    {
        public string Message;
        public string ClientID;
        public SysMessage(string msg, string clientid)
        {
            Message = msg;
            ClientID = clientid;
        }
    }
   

}
