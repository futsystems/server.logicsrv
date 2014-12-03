using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public class LogicLiveRequest : RequestPacket
    {
        public LogicLiveRequest()
        {
            _type = MessageTypes.LOGICLIVEREQUEST;
        }

        public override string ContentSerialize()
        {
            return "";
        }

        public override void ContentDeserialize(string contentstr)
        {
            
        }
    }


    public class LogicLiveResponse : RspResponsePacket
    {
        public LogicLiveResponse()
        {
            _type = MessageTypes.LOGICLIVERESPONSE;
        }

        public override string ResponseSerialize()
        {
            return "";
        }

        public override void ResponseDeserialize(string content)
        {
            
        }
    }
}
