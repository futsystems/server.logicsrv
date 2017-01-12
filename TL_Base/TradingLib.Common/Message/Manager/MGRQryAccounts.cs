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
        public AccountItem AccountItem { get; set; }
        public RspMGRQryAccountResponse()
        {
            _type = MessageTypes.MGRQRYACCOUNTSRESPONSE;
            AccountItem = null;
        }

        public override string ResponseSerialize()
        {
            if (this.AccountItem == null)
                return string.Empty;
            return AccountItem.Serialize(this.AccountItem);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.AccountItem = null;
                return;
            }
            this.AccountItem = AccountItem.Deserialize(content);
        }

    }
}
