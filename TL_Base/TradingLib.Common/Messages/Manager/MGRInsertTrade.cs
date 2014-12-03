using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 管理端请求插入委托
    /// </summary>
    public class MGRReqInsertTradeRequest:RequestPacket
    {
        public MGRReqInsertTradeRequest()
        {
            _type = MessageTypes.MGRINSERTTRADE;
            this.TradeToSend = new TradeImpl();
        }

        public Trade TradeToSend { get; set; }

        public override string ContentSerialize()
        {
            return TradeImpl.Serialize(this.TradeToSend);
        }

        public override void ContentDeserialize(string contentstr)
        {
            this.TradeToSend = TradeImpl.Deserialize(contentstr);
        }
    }
}
