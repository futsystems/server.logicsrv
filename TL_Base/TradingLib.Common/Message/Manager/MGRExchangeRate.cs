//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using TradingLib.API;

//namespace TradingLib.Common
//{
//    /// <summary>
//    /// 查询汇率信息
//    /// </summary>
//    public class MGRQryExchangeRateRequuest : RequestPacket
//    {
//        public MGRQryExchangeRateRequuest()
//        {
//            _type = MessageTypes.MGRQRYEXCHANGERATE;
//        }

//        public override string ContentSerialize()
//        {
//            return string.Empty;
//        }

//        public override void ContentDeserialize(string contentstr)
//        {

//        }
//    }

//    /// <summary>
//    /// 查询汇率信息回报
//    /// </summary>
//    public class RspMGRQryExchangeRateResponse : RspResponsePacket
//    {
//        public RspMGRQryExchangeRateResponse()
//        {
//            _type = MessageTypes.MGRQRYEXCHANGERATERESPONSE;
//            ExchangeRate = null;
//        }

//        public ExchangeRate ExchangeRate { get; set; }
//        public override string ResponseSerialize()
//        {
//            if (this.ExchangeRate == null) return string.Empty;
//            return TradingLib.Common.ExchangeRate.Serialize(this.ExchangeRate);
//        }

//        public override void ResponseDeserialize(string content)
//        {
//            if (string.IsNullOrEmpty(content))
//            {
//                this.ExchangeRate = null;
//                return;
//            }
//            this.ExchangeRate = TradingLib.Common.ExchangeRate.Deserialize(content);
//        }
//    }

//}
