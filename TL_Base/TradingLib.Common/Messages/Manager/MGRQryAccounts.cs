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
            return "MGRQryAccounts";
        }

        public override void ContentDeserialize(string contentstr)
        {

        }


    }

    public class RspMGRQryAccountResponse : RspResponsePacket
    {
        public IAccountLite oAccount { get; set; }
        public RspMGRQryAccountResponse()
        {
            _type = MessageTypes.MGRQRYACCOUNTSRESPONSE;
            oAccount = new AccountLite();
        }

        public override string ResponseSerialize()
        {
            return AccountLite.Serialize(this.oAccount);
        }

        public override void ResponseDeserialize(string content)
        {
            this.oAccount = AccountLite.Deserialize(content);
        }

    }
}
