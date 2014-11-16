using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IAccConfig
    {
        /// <summary>
        /// 获得某个合约的手续费设置
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        CommissionConfig GetCommissionConfig(Symbol symbol);
    }
}
