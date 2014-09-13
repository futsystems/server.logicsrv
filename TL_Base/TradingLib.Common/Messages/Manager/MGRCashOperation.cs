using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public class MGRCashOperationRequest:RequestPacket
    {

        public MGRCashOperationRequest()
        {
            _type = MessageTypes.MGRCASHOPERATION;
        }

        public string Account { get; set; }
        public decimal Amount { get; set; }
        public string TransRef { get; set; }
        public string Comment { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Account);
            sb.Append(d);
            sb.Append(this.Amount.ToString());
            sb.Append(d);
            sb.Append(this.TransRef.ToString());
            sb.Append(d);
            sb.Append(this.Comment);

            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Account = rec[0];
            this.Amount = decimal.Parse(rec[1]);
            this.TransRef = rec[2];
            this.Comment = rec[3];
        }


    }
}
