using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class NotifyMGRAccountInfoLiteResponse:NotifyResponsePacket
    {
        public IAccountInfoLite InfoLite { get; set; }
        public NotifyMGRAccountInfoLiteResponse()
        {
            _type = MessageTypes.MGRACCOUNTINFOLITENOTIFY;
            InfoLite = new AccountInfoLite();
        }

        public override string ContentSerialize()
        {
            return AccountInfoLite.Serialize(InfoLite);
        }

        public override void ContentDeserialize(string contentstr)
        {
            InfoLite = AccountInfoLite.Deserialize(contentstr);
        }
    }
}
