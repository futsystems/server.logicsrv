using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    /// <summary>
    /// 修改密码请求
    /// </summary>
    public class MGRChangeAccountPassRequest:RequestPacket
    {
        public MGRChangeAccountPassRequest()
        {
            _type = MessageTypes.MGRCHANGEACCOUNTPASS;
            this.TradingAccount = string.Empty;
            this.NewPassword = string.Empty;
        }

        /// <summary>
        /// 交易帐号
        /// </summary>
        public string TradingAccount { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        public string NewPassword { get; set; }

        public override string ContentSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(this.TradingAccount);
            sb.Append(d);
            sb.Append(this.NewPassword);
            return sb.ToString();
        }

        public override void ContentDeserialize(string contentstr)
        {
            string[] rec = contentstr.Split(',');
            this.TradingAccount = rec[0];
            this.NewPassword = rec[1];
        }
    }


    /// <summary>
    /// 修改密码回报
    /// </summary>
    public class RspMGRChangeAccountPassResponse : RspResponsePacket
    {
        public RspMGRChangeAccountPassResponse()
        {
            _type = MessageTypes.MGRCHANGEACCOUNTPASSRESPONSE;
            this.NewPassword = string.Empty;
        }

        public string NewPassword { get; set; }

        public override string ResponseSerialize()
        {
            return this.NewPassword;
        }

        public override void ResponseDeserialize(string content)
        {
            this.NewPassword = content;
        }


        
        
    }
}
