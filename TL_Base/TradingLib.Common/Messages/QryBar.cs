///////////////////////////////////////////////////////////////////////////////////////
// 用于查询历史行情
// 
//
///////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class QryBarRequest:RequestPacket
    {
        public QryBarRequest()
        {
            _type = MessageTypes.QRYBAR;
        }

        public override string ContentSerialize()
        {
            return base.ContentSerialize();
        }

        public override void ContentDeserialize(string contentstr)
        {
            base.ContentDeserialize(contentstr);
        }


    }

    public class RspBarResponse : RspResponsePacket
    {
        public RspBarResponse()
        {
            _type = MessageTypes.BARRESPONSE;
        }

        public override string ResponseSerialize()
        {
            return base.ResponseSerialize();
        }

        public override void ResponseDeserialize(string content)
        {
            base.ResponseDeserialize(content);
        }


    }
}
