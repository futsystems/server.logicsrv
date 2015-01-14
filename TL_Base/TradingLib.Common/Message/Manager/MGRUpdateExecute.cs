using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class MGRUpdateExecuteRequest:RequestPacket
    {
        public MGRUpdateExecuteRequest()
        {
            _type = MessageTypes.MGRUPDATEACCOUNTEXECUTE;
            this.Account = string.Empty;
            this.Execute = false;
        }

        public string Account { get; set; }

        public bool Execute { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Account);
            sb.Append(d);
            sb.Append(this.Execute.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Account  = rec[0];
            this.Execute = bool.Parse(rec[1]);
        }
    }
}
