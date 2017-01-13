using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class NotifyMGRAccountStatistic:NotifyResponsePacket
    {
        public AccountStatistic Statistic { get; set; }
        public NotifyMGRAccountStatistic()
        {
            _type = MessageTypes.MGRACCOUNTINFOLITENOTIFY;
            Statistic = null;
        }

        public override string ContentSerialize()
        {
            if (this.Statistic == null) return string.Empty;
            return AccountStatistic.Serialize(this.Statistic);
        }

        public override void ContentDeserialize(string contentstr)
        {
            if (string.IsNullOrEmpty(contentstr))
            {
                this.Statistic = null;
                return;
            }
            this.Statistic = AccountStatistic.Deserialize(contentstr);
        }
    }
}
