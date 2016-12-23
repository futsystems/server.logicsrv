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
        /// <summary>
        /// 客户端绑定的业务对象
        /// </summary>
        public IAccount Account { get; private set; }
        
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
                this.Account = obj as IAccount;
                this.Authorized = true;
            }
            else
            {
                this.Authorized = false;
            }
        }



        public override string ToString()
        {
            return "Client:" + this.Location.ClientID + " FrontID:" + this.Location.FrontID + " 登入:" + (this.Authorized ? "成功" : "失败") + (this.Authorized?( " Account:"+this.Account):"");
        }

    }
}
