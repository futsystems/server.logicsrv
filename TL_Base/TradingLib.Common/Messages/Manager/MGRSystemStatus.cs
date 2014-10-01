using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class MGRQrySystemStatusRequest:RequestPacket
    {
        public MGRQrySystemStatusRequest()
        {
            _type = MessageTypes.MGRQRYSYSTEMSTATUS;
        }
    }

    public class RspMGRQrySystemStatusResponse : RspResponsePacket
    {
        public RspMGRQrySystemStatusResponse()
        {
            _type = MessageTypes.MGRSYSTEMSTATUSRESPONSE;
            this.Status = new SystemStatus();
        }

        public SystemStatus Status { get; set; }

        public override string ResponseSerialize()
        {
            return Status.Serialize();
        }

        public override void ResponseDeserialize(string content)
        {
            Status.Deserialize(content);
        }
    }
}
