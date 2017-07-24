using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.APIService
{
    public class JsonReply
    {
        public int RspCode;
        public string Message;
        public object Payload;

        

        public JsonReply(int code, string message, object obj = null)
        {
            this.RspCode = code;
            this.Message = message;
            this.Payload = obj;
        }
    }
}
