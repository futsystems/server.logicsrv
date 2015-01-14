using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class NotifyMGRAccountChangeUpdateResponse:NotifyResponsePacket
    {

        public AccountLite oAccount { get; set; }

        public NotifyMGRAccountChangeUpdateResponse()
        {
            _type = MessageTypes.MGRACCOUNTCHANGEUPDATE;
            oAccount = new AccountLite();
        }

        public override string ContentSerialize()
        {
            return AccountLite.Serialize(this.oAccount);
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.oAccount = AccountLite.Deserialize(contentstr);
        }


    }
}
