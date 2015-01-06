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
        public IAccount Account { get; private set; }//记录客户端登入所对应的交易帐号
        
        public TrdClientInfo(TrdClientInfo copythis)
            : base(copythis)
        {
            Account = copythis.Account;
        }


        public TrdClientInfo()
        {
            this.Account = null;
        }

        public override void BindState(object obj)
        {
            if (obj != null && obj is IAccount)
            {
                this.Account = Account;
                this.Authorized = true;
            }
            else
            {
                this.Authorized = false;
            }
        }


        public override string SubSerialize()
        {
            StringBuilder sb = new StringBuilder();
            char d = ',';
            sb.Append(Account);
            //sb.Append(d);
            //sb.Append(UserID.ToString());
            return sb.ToString();
        }

        public override void SubDeserialize(string str)
        {
            string[] rec = str.Split(',');
            //Account = rec[0];
            //UserID = int.Parse(rec[1]);
        }

        public override string ToString()
        {
            return "Client:" + this.Location.ClientID + " FrontID:" + this.Location.FrontID + " 登入:" + (this.Authorized ? "成功" : "失败") + (this.Authorized?( " Account:"+this.Account):"");
        }

    }
}
