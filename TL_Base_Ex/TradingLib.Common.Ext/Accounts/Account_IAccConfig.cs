using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        /// <summary>
        /// 获得某个交易帐户某个合约的手续费设置
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public virtual CommissionConfig GetCommissionConfig(Symbol symbol)
        {
           
            CommissionConfig cfg = symbol.GetCommissionConfig();
            cfg.Account = this.ID;
            return cfg;
            
        }
    }
}
