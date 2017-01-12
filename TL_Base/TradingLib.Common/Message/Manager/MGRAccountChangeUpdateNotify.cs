using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class NotifyMGRAccountChangeUpdateResponse:NotifyResponsePacket
    {

        public AccountItem AccountItem { get; set; }

        public NotifyMGRAccountChangeUpdateResponse()
        {
            _type = MessageTypes.MGRACCOUNTCHANGEUPDATE;
            AccountItem = null;
        }

        public override string ContentSerialize()
        {
            if (this.AccountItem == null) 
                return string.Empty;
            return AccountItem.Serialize(this.AccountItem);
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.AccountItem = null;
                return;
            }
            this.AccountItem = AccountItem.Deserialize(contentstr);
        }


    }
}
