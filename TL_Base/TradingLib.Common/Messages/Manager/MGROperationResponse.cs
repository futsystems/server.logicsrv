using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class RspMGROperationResponse:RspResponsePacket
    {

        public RspMGROperationResponse()
        {
            _type = MessageTypes.MGROPERATIONRESPONSE;
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
