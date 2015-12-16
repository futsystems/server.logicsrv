///////////////////////////////////////////////////////////////////////////////////////
// 查询委托
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

    /// <summary>
    /// 查询委托
    /// </summary>
    public class XQryOrderRequest : RequestPacket
    {

        public XQryOrderRequest()
        {
            _type = MessageTypes.XQRYORDER;
        }

        public override string ContentSerialize()
        {
            return "";
        }

        public override void ContentDeserialize(string reqstr)
        {
           
        }
    }

    public class RspXQryOrderResponse : RspResponsePacket
    {
        public RspXQryOrderResponse()
        {
            _type = MessageTypes.XORDERRESPONSE;
        }

        Order _order = null;
        public Order Order { get { return _order; } set { _order = value; } }

        public override string ResponseSerialize()
        {
            if (Order == null)
                return "";
            return OrderImpl.Serialize(Order);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            Order = OrderImpl.Deserialize(content);
        }
    }

    public class XQryTradeRequest : RequestPacket
    {
        public XQryTradeRequest()
        {
            _type = MessageTypes.XQRYTRADE;
        }



        public override string ContentSerialize()
        {
            return "";
        }

        public override void ContentDeserialize(string reqstr)
        {
     
        }
    }


    public class RspXQryTradeResponse : RspResponsePacket
    {
        public RspXQryTradeResponse()
        { 
            _type = MessageTypes.XTRADERESPONSE;
        }
        Trade _trade = null;

        public Trade Trade { get { return _trade; } set { _trade = value; } }
        public override string ResponseSerialize()
        {
            if (this.Trade == null)
                return "";

            return TradeImpl.Serialize(this.Trade);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            this.Trade = TradeImpl.Deserialize(content);
        }
    }

    public class XQryYDPositionRequest : RequestPacket
    {

        public XQryYDPositionRequest()
        {
            _type = MessageTypes.XQRYYDPOSITION;
        }


        public override string ContentSerialize()
        {
            return "";
        }

        public override void ContentDeserialize(string reqstr)
        {

        }
    }

    public class RspXQryYDPositionResponse : RspResponsePacket
    {
        public RspXQryYDPositionResponse()
        {
            _type = MessageTypes.XYDPOSITIONRESPONSE;
            this.YDPosition = null;
        }

        public PositionDetail YDPosition { get; set; }

        public override string ResponseSerialize()
        {
            if (YDPosition == null)
                return "";
            return PositionDetailImpl.Serialize(this.YDPosition);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            YDPosition = PositionDetailImpl.Deserialize(content);
        }
    }

}
