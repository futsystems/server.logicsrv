using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class MGRUpdateIntradayRequest : RequestPacket
    {
        public MGRUpdateIntradayRequest()
        {
            _type = MessageTypes.MGRUPDATEACCOUNTINTRADAY;

            Account = string.Empty;
            Intraday = false;
        }

        public string Account { get; set; }
        public bool Intraday { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Account);
            sb.Append(d);
            sb.Append(Intraday.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Account = rec[0];
            this.Intraday = bool.Parse(rec[1]);
        }
    }
}
