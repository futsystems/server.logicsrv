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
            this.Symbol = string.Empty;
            this.Start = 0;
            this.End = 0;
        }
        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public int End { get; set; }


        public override string ContentSerialize()
        {
            char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Symbol);
            sb.Append(d);
            sb.Append(this.Start);
            sb.Append(d);
            sb.Append(this.End);

            return sb.ToString();
        }

        public override void ContentDeserialize(string reqstr)
        {
            string[] rec = reqstr.Split(',');
            this.Symbol = rec[0];
            this.Start = int.Parse(rec[1]);
            this.End = int.Parse(rec[2]);
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
            this.Symbol = string.Empty;
            this.Start = 0;
            this.End = 0;
        }


        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public int Start { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public int End { get; set; }


        public override string ContentSerialize()
        {
            char d = ',';
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Symbol);
            sb.Append(d);
            sb.Append(this.Start);
            sb.Append(d);
            sb.Append(this.End);

            return sb.ToString();
        }

        public override void ContentDeserialize(string reqstr)
        {
            string[] rec = reqstr.Split(',');
            this.Symbol = rec[0];
            this.Start = int.Parse(rec[1]);
            this.End = int.Parse(rec[2]);
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
