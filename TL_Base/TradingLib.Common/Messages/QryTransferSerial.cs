using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 查询出入金
    /// </summary>
    public class QryTransferSerialRequest:RequestPacket
    {
        public QryTransferSerialRequest()
        {
            _type = MessageTypes.QRYTRANSFERSERIAL;
            this.TradingAccount = string.Empty;
            this.BankID = string.Empty;
        }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 对应银行的流水号
        /// </summary>
        public string BankID { get; set; }

        public override string ContentSerialize()
        {
            return this.TradingAccount + "," + this.BankID;
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');

            this.TradingAccount = rec[0];
            this.BankID = rec[1];
        }
    }

    /// <summary>
    /// 查询出入金回报
    /// </summary>
    public class RspQryTransferSerialResponse : RspResponsePacket
    {
        public RspQryTransferSerialResponse()
        {
            _type = MessageTypes.TRANSFERSERIALRESPONSE;
        }

        /// <summary>
        /// 时间
        /// </summary>
        public int Date { get; set; }

        /// <summary>
        /// 日期
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 交易对端银行帐号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 交易流水备注
        /// </summary>
        public string TransRef { get; set; }

        public override string ResponseSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.Date.ToString());
            sb.Append(d);
            sb.Append(this.Time.ToString());
            sb.Append(d);
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Amount.ToString());
            sb.Append(d);
            sb.Append(this.BankAccount);
            sb.Append(d);
            sb.Append(this.TransRef);

            return sb.ToString();
        }

        public override void ResponseDeserialize(string content)
        {
            string[] rec = content.Split(',');
            this.Date = int.Parse(rec[0]);
            this.Time = int.Parse(rec[1]);
            this.TradingAccount = rec[2];
            this.Amount = decimal.Parse(rec[3]);
            this.BankAccount = rec[4];
            this.TransRef = rec[5];
        }
    }
}
