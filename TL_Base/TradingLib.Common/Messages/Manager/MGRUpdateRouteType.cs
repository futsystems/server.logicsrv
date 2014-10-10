using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class MGRUpdateRouteTypeRequest:RequestPacket
    {
        public MGRUpdateRouteTypeRequest()
        {
            _type = MessageTypes.MGRUPDATEACCOUNTROUTETRANSFERTYPE;
        }

        public string Account { get; set; }

        public QSEnumOrderTransferType RouteType { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Account);
            sb.Append(d);
            sb.Append(this.RouteType.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.Account = rec[0];
            this.RouteType = (QSEnumOrderTransferType)Enum.Parse(typeof(QSEnumOrderTransferType), rec[1]);
        }
    }

}
