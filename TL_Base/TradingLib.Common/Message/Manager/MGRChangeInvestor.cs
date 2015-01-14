using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    /// <summary>
    /// 请求修改投资者信息
    /// </summary>
    public class MGRReqChangeInvestorRequest:RequestPacket
    {
        public MGRReqChangeInvestorRequest()
        {
            _type = MessageTypes.MGRCHANGEINVESTOR;
            this.TradingAccount = string.Empty;
            this.Name = string.Empty;
            this.Broker = string.Empty;
            this.BankFK = 0;
            this.BankAC = string.Empty;
        }

        public string TradingAccount { get; set; }

        public string Name { get; set; }

        public string Broker { get; set; }

        public int BankFK { get; set; }


        public string BankAC { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Name);
            sb.Append(d);
            sb.Append(this.Broker);
            sb.Append(d);
            sb.Append(this.BankFK);
            sb.Append(d);
            sb.Append(this.BankAC);

            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Name = rec[1];
            this.Broker = rec[2];
            this.BankFK = int.Parse(rec[3]);
            this.BankAC = rec[4];
        }
    }

    /// <summary>
    /// 修改投资者信息回报
    /// </summary>
    public class RspMGRReqChangeInvestorResponse : RspResponsePacket
    {
        public RspMGRReqChangeInvestorResponse()
        {
            _type = MessageTypes.MGRCHANGEINVESTORRESPONSE;

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
