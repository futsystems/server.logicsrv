using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户计算类公共接口
    /// </summary>
    public interface IAccCal
    {
        /// <summary>
        /// 计算某个委托所占用资金
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        decimal CalOrderFundRequired(Order o,decimal defaultvalue);

        /// <summary>
        /// 获得某个合约的可用资金
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetFundAvabile(Symbol symbol);
    }
}
