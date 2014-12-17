//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;


//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 查询通道
//    /// </summary>
//    public class MGRQryConnectorRequest:RequestPacket
//    {
//        public MGRQryConnectorRequest()
//        {
//            _type = MessageTypes.MGRQRYCONNECTOR;
//        }
//    }


   

//    /// <summary>
//    /// 查询通道回报
//    /// </summary>
//    public class RspMGRQryConnectorResponse : RspResponsePacket
//    {
//        public RspMGRQryConnectorResponse()
//        {
//            _type = MessageTypes.MGRCONNECTORRESPONSE;
//            Connector = new ConnectorInfo();
//        }



//        public ConnectorInfo Connector { get; set; } 
//        public override string ResponseSerialize()
//        {
//            return Connector.Serialize();
//        }

//        public override void ResponseDeserialize(string content)
//        {
//            Connector.Deserialize(content);
//        }

//    }

//    /// <summary>
//    /// 请求启动交易通道
//    /// </summary>
//    public class MGRReqStartBrokerRequest : RequestPacket
//    {
//        public MGRReqStartBrokerRequest()
//        {
//            _type = MessageTypes.MGRSTARTBROKER;
//            this.FullName = string.Empty;
//        }

//        public string FullName { get; set; }

//        public override string ContentSerialize()
//        {
//            return this.FullName;
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            this.FullName = contentstr;
//        }
//    }

//    /// <summary>
//    /// 请求停止交易通道
//    /// </summary>
//    public class MGRReqStopBrokerRequest : RequestPacket
//    {
//        public MGRReqStopBrokerRequest()
//        {
//            _type = MessageTypes.MGRSTOPBROKER;
//            this.FullName = string.Empty;
//        }

//        public string FullName { get; set; }

//        public override string ContentSerialize()
//        {
//            return this.FullName;
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            this.FullName = contentstr;
//        }
//    }

//    /// <summary>
//    /// 请求启动行情通道
//    /// </summary>
//    public class MGRReqStartDataFeedRequest : RequestPacket
//    {
//        public MGRReqStartDataFeedRequest()
//        {
//            _type = MessageTypes.MGRSTARTDATAFEED;
//            this.FullName = string.Empty;
//        }

//        public string FullName { get; set; }

//        public override string ContentSerialize()
//        {
//            return this.FullName;
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            this.FullName = contentstr;
//        }
//    }

//    /// <summary>
//    /// 请求停止行情通道
//    /// </summary>
//    public class MGRReqStopDataFeedRequest : RequestPacket
//    {
//        public MGRReqStopDataFeedRequest()
//        {
//            _type = MessageTypes.MGRSTOPDATAFEED;
//            this.FullName = string.Empty;
//        }

//        public string FullName { get; set; }

//        public override string ContentSerialize()
//        {
//            return this.FullName;
//        }

//        public override void ContentDeserialize(string contentstr)
//        {
//            this.FullName = contentstr;
//        }
//    }
//}
