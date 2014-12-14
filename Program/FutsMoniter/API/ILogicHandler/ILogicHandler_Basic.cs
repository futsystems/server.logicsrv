using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public partial interface ILogicHandler
    {
        #region 基础信息
        /// <summary>
        /// 获得交易所列表回报
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="islast"></param>
        void OnMGRExchangeResponse(Exchange ex, bool islast);

        /// <summary>
        /// 获得交易时间段列表回报
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="islast"></param>
        void OnMGRMarketTimeResponse(MarketTime mt, bool islast);


        /// <summary>
        /// 获得品种列表回报
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="islast"></param>
        void OnMGRSecurityResponse(SecurityFamilyImpl sec, bool islast);


        /// <summary>
        /// 获得合约列表回报
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="islast"></param>
        void OnMGRSymbolResponse(SymbolImpl sym, bool islast);


        /// <summary>
        /// 品种增加回报
        /// </summary>
        /// <param name="security"></param>
        /// <param name="islast"></param>
        void OnMGRSecurityAddResponse(SecurityFamilyImpl security, bool islast);

        /// <summary>
        /// 合约增加回报
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="islast"></param>
        void OnMGRSymbolAddResponse(SymbolImpl symbol, bool islast);


        /// <summary>
        /// 管理员列表回报
        /// </summary>
        /// <param name="manger"></param>
        /// <param name="islast"></param>
        void OnMGRMangerResponse(Manager manger, bool islast);
        #endregion
    }
}
