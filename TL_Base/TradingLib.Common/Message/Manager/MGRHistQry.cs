using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 查询历史委托请求
    /// </summary>
    public class MGRQryOrderRequest:RequestPacket
    {
        public MGRQryOrderRequest()
        {
            _type = MessageTypes.MGRQRYORDER;
            this.TradingAccount = string.Empty;
            this.Settleday = 0;
        }
        /// <summary>
        /// 查询交易帐号
        /// </summary>
        public string TradingAccount { get; set; }
        
        /// <summary>
        /// 查询的结算日
        /// </summary>
        public int Settleday { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Settleday.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Settleday = int.Parse(rec[1]);
        }
    }


    /// <summary>
    /// 查询历史委托回报
    /// </summary>
    public class RspMGRQryOrderResponse : RspResponsePacket
    {
        public RspMGRQryOrderResponse()
        {
            _type = MessageTypes.MGRORDERRESPONSE;
            this.OrderToSend = null;
        }

        public Order OrderToSend { get; set; }

        public override string ResponseSerialize()
        {
            if (this.OrderToSend == null)
                return string.Empty;
            return OrderImpl.Serialize(this.OrderToSend);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.OrderToSend = null;
                return;
            }
            this.OrderToSend = OrderImpl.Deserialize(content);
        }
    }

    /// <summary>
    /// 查询历史成交
    /// </summary>
    public class MGRQryTradeRequest : RequestPacket
    {
        public MGRQryTradeRequest()
        {
            _type = MessageTypes.MGRQRYTRADE;
            this.TradingAccount = string.Empty;
            this.Settleday = 0;
        }
        /// <summary>
        /// 查询交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 查询的结算日
        /// </summary>
        public int Settleday { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Settleday.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Settleday = int.Parse(rec[1]);
        }

    }

    /// <summary>
    /// 查询历史成交回报
    /// </summary>
    public class RspMGRQryTradeResponse : RspResponsePacket
    {
        public RspMGRQryTradeResponse()
        {
            _type = MessageTypes.MGRTRADERESPONSE;
            this.TradeToSend = null;
        }

        public Trade TradeToSend { get; set; }

        public override string ResponseSerialize()
        {
            if (this.TradeToSend == null)
                return string.Empty;
            return TradeImpl.Serialize(this.TradeToSend);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                this.TradeToSend = null;
                return;
            }
            this.TradeToSend = TradeImpl.Deserialize(content);
        }
    }


    /// <summary>
    /// 请求查询结算持仓
    /// </summary>
    public class MGRQryPositionRequest : RequestPacket
    {
        public MGRQryPositionRequest()
        {
            _type = MessageTypes.MGRQRYPOSITION;
            this.TradingAccount = string.Empty;
            this.Settleday = 0;

        }
        /// <summary>
        /// 查询交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 查询的结算日
        /// </summary>
        public int Settleday { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Settleday.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Settleday = int.Parse(rec[1]);
        }

    }

    /// <summary>
    /// 结算持仓回报
    /// </summary>
    public class RspMGRQryPositionResponse : RspResponsePacket
    {
        public RspMGRQryPositionResponse()
        {
            _type = MessageTypes.MGRPOSITIONRESPONSE;
            this.PostionToSend = new PositionDetailImpl();
        }

        public PositionDetail PostionToSend { get; set; }

        public override string ResponseSerialize()
        {
            return PositionDetailImpl.Serialize(this.PostionToSend);
        }

        public override void ResponseDeserialize(string content)
        {
            this.PostionToSend = PositionDetailImpl.Deserialize(content);
        }
    }

    /// <summary>
    /// 出入金查询
    /// </summary>
    public class MGRQryCashRequest : RequestPacket
    {
        public MGRQryCashRequest()
        {
            _type = MessageTypes.MGRQRYCASH;
        }
        /// <summary>
        /// 查询交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 查询的结算日
        /// </summary>
        public int Settleday { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Settleday.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Settleday = int.Parse(rec[1]);
        }

    }

    /// <summary>
    /// 出入金查询回报
    /// </summary>
    public class RspMGRQryCashResponse : RspResponsePacket
    {
        public RspMGRQryCashResponse()
        {
            _type = MessageTypes.MGRCASHRESPONSE;
            this.CashTransToSend = null;
        }

        public CashTransaction CashTransToSend { get; set; }

        public override string ResponseSerialize()
        {
            if (this.CashTransToSend == null) return string.Empty;
            return CashTransactionImpl.Serialize(this.CashTransToSend);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            this.CashTransToSend = CashTransactionImpl.Deserialize(content);
        }
    }


    /// <summary>
    /// 查询结算单请求
    /// </summary>
    public class MGRQrySettleRequest : RequestPacket
    {
        public MGRQrySettleRequest()
        {
            _type = MessageTypes.MGRQRYSETTLEMENT;//查询结算单
        }
        /// <summary>
        /// 查询交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 查询的结算日
        /// </summary>
        public int Settleday { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Settleday.ToString());
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Settleday = int.Parse(rec[1]);
        }

    }

    /// <summary>
    /// 查询结算单回报
    /// </summary>
    public class RspMGRQrySettleResponse : RspResponsePacket
    { 
        public RspMGRQrySettleResponse()
        {
            _type = MessageTypes.MGRSETTLEMENTRESPONSE;
        }

        public string TradingAccount { get; set; }
        public int Tradingday { get; set; }
        //public int SettlementID { get; set; }
        public string SettlementContent { get; set; }
        //public int SequenceNo { get; set; }

        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Tradingday);
            sb.Append(d);
            sb.Append(this.SettlementContent.Replace('|', '*'));
            return sb.ToString();
        }

        public override void ResponseDeserialize(string content)
        {
            string[] rec = content.Split(',');
            this.TradingAccount = rec[0];
            this.Tradingday = int.Parse(rec[1]);
            this.SettlementContent = rec[2].Replace('*', '|');
        }
    }
}
