using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IAccCal
    {
        #region 帐户公共类计算


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

        /// <summary>
        /// 获得帐户所有可用资金
        /// </summary>
        /// <returns></returns>
        decimal GetFundAvabile();

        /// <summary>
        /// 获得帐户所有资金包含已经使用和未使用资金
        /// </summary>
        /// <returns></returns>
        decimal GetFundTotal();

        /// <summary>
        /// 获得所使用资金
        /// </summary>
        /// <returns></returns>
        decimal GetFundUsed();


        #endregion
    }
}
