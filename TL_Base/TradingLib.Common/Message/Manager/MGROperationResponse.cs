using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class RspMGRResponse:RspResponsePacket
    {
        public RspMGRResponse()
        {
            _type = MessageTypes.MGR_RSP;
        }

        public override void ResponseDeserialize(string content)
        {
            base.ResponseDeserialize(content);
        }

        public override string ResponseSerialize()
        {
            return base.ResponseSerialize();
        }
    }
}
