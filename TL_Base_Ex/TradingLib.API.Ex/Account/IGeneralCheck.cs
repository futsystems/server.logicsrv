using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户核心常规检查
    /// 合约交易权限检查
    /// 委托保证金检查
    /// 可开数量等
    /// </summary>
    public interface IGeneralCheck
    {
        /// <summary>
        /// 检查某个帐户是否可以接受某个委托 
        /// 用于保证金资金检查
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        bool CanFundTakeOrder(Order order,out string msg);

        /// <summary>
        /// 检查某个帐号是否能够交易某个合约
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        bool CanTakeSymbol(Symbol symbol,out string msg);

        /// <summary>
        /// 返回帐户可某个合约的手数
        /// 逻辑中包含一些特殊的保证金处理
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        int CanOpenSize(Symbol symbol);
    }
}
