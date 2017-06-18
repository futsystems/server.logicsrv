///////////////////////////////////////////////////////////////////////////////////////
// 查询结算信息
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
    public class XQrySettleInfoRequest:RequestPacket
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 交易日
        /// </summary>
        public int Tradingday { get; set; }


        public XQrySettleInfoRequest()
        {
            _type = MessageTypes.XQRYSETTLEINFO;
        }

        public override string ContentSerialize()
        {
            return Account + "," + Tradingday.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            Account = rec[0];
            Tradingday = int.Parse(rec[1]);
        }
    }

    public class RspXQrySettleInfoResponse : RspResponsePacket
    {
        public string TradingAccount { get; set; }
        public int Tradingday { get; set; }
        public int SettlementID { get; set; }
        public string SettlementContent { get; set; }
        public int SequenceNo { get; set; }

        public RspXQrySettleInfoResponse()
        {
            _type = MessageTypes.XSETTLEINFORESPONSE;
            TradingAccount = string.Empty;
            Tradingday = 0;
            SettlementID = 0;
            SequenceNo = 0;
            SettlementContent = string.Empty;
        }
        
        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(TradingAccount);
            sb.Append(d);
            sb.Append(Tradingday.ToString());
            sb.Append(d);
            sb.Append(SettlementID.ToString());
            sb.Append(d);
            sb.Append(SequenceNo.ToString());
            sb.Append(d);
            sb.Append(SettlementContent.Replace('|', '*'));
            return sb.ToString();
        }

        public override void ResponseDeserialize(string content)
        {
            string[] rec = content.Split(',');
            TradingAccount = rec[0];
            Tradingday = int.Parse(rec[1]);
            SettlementID = int.Parse(rec[2]);
            SequenceNo = int.Parse(rec[3]);
            SettlementContent = rec[4].Replace('*', '|');
        }


    }

    /// <summary>
    /// 结算汇总查询
    /// 查询每日结算记录 对应数据单日结算记录
    /// </summary>
    public class XQrySettleSummaryRequest : RequestPacket
    {

        public int StartSettleday { get; set; }

        public int EndSettleday { get; set; }

        public XQrySettleSummaryRequest()
        {
            _type = MessageTypes.XQRYSETTLESUMMAY;
        }

        public override string ContentSerialize()
        {
            return string.Format("{0},{1}", this.StartSettleday, this.EndSettleday);
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.StartSettleday = int.Parse(rec[0]);
            this.EndSettleday = int.Parse(rec[1]);
        }
    }

    public class RspXqrySettleSummaryResponse : RspResponsePacket
    {
        public RspXqrySettleSummaryResponse()
        {
            _type = MessageTypes.XQRYSETTLESUMMAYRESPONSE;
            this.Settlement = null;
        }

        public AccountSettlement Settlement { get; set; }

        public override string ResponseSerialize()
        {
            return AccountSettlementImpl.Serialize(this.Settlement);
        }

        public override void ResponseDeserialize(string content)
        {

            this.Settlement = AccountSettlementImpl.Deserialize(content);
        }
    }
}
