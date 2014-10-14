using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.RechargeOnLine
{
    public class ReceiveableAccount:Drop
    {
        /// <summary>
        /// 收款银行数据库ID
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        public string Bank_AC { get; set; }

        /// <summary>
        /// 分行地址
        /// </summary>
        public string Branch { get; set; }
    }

    public static class JsonWrapperReceivableAccountUtils
    {
        public static ReceiveableAccount ToDrop(this JsonWrapperReceivableAccount ac)
        {
            ReceiveableAccount account = new ReceiveableAccount
            {
                BankName = ac.BankName,
                Name = ac.Name,
                Bank_AC = ac.Bank_AC,
                Branch = ac.Branch,
                ID = ac.ID,
            };
            return account;
        }
    }
}


