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
        }

        public string TradingAccount { get; set; }

        public string Token { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.Token);

            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.Token = rec[1];
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
