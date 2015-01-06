using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class MGRQryAccountRequest:RequestPacket
    {
        public MGRQryAccountRequest()
        {
            _type = MessageTypes.MGRQRYACCOUNTS;
        }

        public override string ContentSerialize()
        {
            return string.Empty;
        }

        public override void ContentDeserialize(string contentstr)
        {

        }


    }

    public class RspMGRQryAccountResponse : RspResponsePacket
    {
        public AccountLite oAccount { get; set; }
        public RspMGRQryAccountResponse()
        {
            _type = MessageTypes.MGRQRYACCOUNTSRESPONSE;
            oAccount = null;
        }

        public override string ResponseSerialize()
        {
            if (this.oAccount == null)
                return string.Empty;
            return AccountLite.Serialize(this.oAccount);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.oAccount = null;
                return;
            }
            this.oAccount = AccountLite.Deserialize(content);
        }

    }
}
