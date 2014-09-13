using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 记录交易客户端通信息
    /// </summary>
    public class TrdClientInfo : ClientInfoBase
    {
        public int UserID { get; set; }//记录交易帐号所对应的UserID用于查询用户信息
        public string Account { get; set; }//记录客户端登入所对应的交易帐号

        public TrdClientInfo(TrdClientInfo copythis)
            : base(copythis)
        {
            Account = copythis.Account;
            UserID = copythis.UserID;


        }
        public TrdClientInfo(string frontid, string clientid)
            : base(frontid, clientid)
        {
            UserID = 0;
            Account = string.Empty;
        }

        public TrdClientInfo()
        {
            UserID = 0;
            Account = string.Empty;
        }

        public override string SubSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(Account);
            sb.Append(d);
            sb.Append(UserID.ToString());
            return sb.ToString();
        }

        public override void SubDeserialize(string str)
        {
            string[] rec = str.Split(',');
            Account = rec[0];
            UserID = int.Parse(rec[1]);
        }

        public override string ToString()
        {
            return "Client:" + this.Location.ClientID + " FrontID:" + this.Location.FrontID + " 登入:" + (this.Authorized ? "成功" : "失败") + (this.Authorized?( " Account:"+this.Account):"");
        }

    }
}
