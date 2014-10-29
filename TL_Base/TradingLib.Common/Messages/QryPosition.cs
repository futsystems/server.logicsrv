///////////////////////////////////////////////////////////////////////////////////////
// 查询持仓
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
    public class QryPositionRequest:RequestPacket
    {

        public QryPositionRequest()
        {
            _type = MessageTypes.QRYPOSITION;
        }
        public QryPositionRequest(string account,string symbol="")
        {
            _type = MessageTypes.QRYPOSITION;
            Account = account;
            Symbol = symbol;
        }

        /// <summary>
        /// 交易帐户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        public override bool IsValid
        {
            get
            {
                if (string.IsNullOrEmpty(Account))
                    return false;
                return true;
            }
        }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(Account);
            sb.Append(d);
            sb.Append(Symbol);
            return sb.ToString();
        }

        public override void ContentDeserialize(string reqstr)
        {
            string[] rec = reqstr.Split(',');
            this.Account = rec[0];
            this.Symbol = rec[1];
        }
    }

    public class RspQryPositionResponse : RspResponsePacket
    {
        public RspQryPositionResponse()
        {
            _type = MessageTypes.POSITIONRESPONSE;
            PositionToSend = new PositionEx();
        }

        public PositionEx PositionToSend { get; set; }

        public override string ResponseSerialize()
        {
            if (PositionToSend == null)
                return "";
            return PositionEx.Serialize(PositionToSend);
        }

        public override void ResponseDeserialize(string content)
        {
            if (string.IsNullOrEmpty(content))
                return;
            PositionToSend = PositionEx.Deserialize(content);
        }
    }


    /// <summary>
    /// 持仓明细查询
    /// </summary>
    public class QryPositionDetailRequest : RequestPacket
    {
        public QryPositionDetailRequest()
        {
            _type = MessageTypes.QRYPOSITIONDETAIL;
            this.TradingAccount = string.Empty;
            this.Symbol = string.Empty;
        }

        public string TradingAccount { get; set; }

        public string Symbol { get; set; }
        public override string ContentSerialize()
        {
            return this.TradingAccount + "," + this.Symbol;
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Symbol = rec[1];
        }
    }

    /// <summary>
    /// 持仓明细查询回报
    /// </summary>
    public class RspQryPositionDetailResponse : RspResponsePacket
    {
        public RspQryPositionDetailResponse()
        {
            _type = MessageTypes.POSITIONDETAILRESPONSE;
            this.PositionDetailToSend = new PositionDetailImpl();
        }

        public PositionDetail PositionDetailToSend { get; set; }

        public override string ResponseSerialize()
        {
            return PositionDetailImpl.Serialize(this.PositionDetailToSend);
        }

        public override void ResponseDeserialize(string content)
        {
            this.PositionDetailToSend = PositionDetailImpl.Deserialize(content);
        }
    }
}
