using System;
using System.Collections.Generic;
using System.Text;
using System.Data;


namespace TradingLib.API
{

    /// <summary>
    /// ClearCentre母本，用于提供基本的账户操作,财务信息计算,账户交易信息,总账户交易信息等
    /// </summary>
    public interface IClearCentreBase :IAccountOperation,IAccountOperationCritical, IFinanceCaculation, IAccountTradingInfo, ITotalAccountInfo, IGotTradingInfo
    {

        /// <summary>
        /// 请求获得某个symbol的Tick数据
        /// </summary>
        event GetSymbolTickDel newSymbolTickRequest;

        /// <summary>
        /// 获得某个合约的当前市场价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetAvabilePrice(string symbol);
    }

   

    
   





}
