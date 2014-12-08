using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 帐户财务信息
    /// </summary>
    public interface IAccountInfo
    {
        string Account { get; set; }
        decimal LastEquity { get; set; }//昨日权益
        decimal NowEquity { get; set; }//当前动态权益

        decimal RealizedPL { get; set; }//平仓盈亏
        decimal UnRealizedPL { get; set; }//浮动盈亏
        decimal Commission { get; set; }//手续费
        decimal Profit { get; set; }//净利
        decimal CashIn { get; set; }//入金
        decimal CashOut { get; set; }//出金
        decimal MoneyUsed { get; set; } //总资金占用
        decimal TotalLiquidation { get; set; }//帐户总净值
        decimal AvabileFunds { get; set; }//帐户总可用资金


        QSEnumAccountCategory Category { get; set; }//账户类别
        QSEnumOrderTransferType OrderRouteType { get; set; }//路由类别
        bool Execute { get; set; }//冻结
        bool IntraDay { get; set; }//日内

        /// <summary>
        /// 保证金占用
        /// </summary>
        decimal Margin { get; set; }

        /// <summary>
        /// 保证金冻结
        /// </summary>
        decimal MarginFrozen { get; set; }

        /// <summary>
        /// 信用额度
        /// </summary>
        decimal Credit { get; set; }

        #region 多品种交易 账户财务数据
        decimal FutMarginUsed { get; set; }//期货占用保证金
        decimal FutMarginFrozen { get; set; }//期货冻结保证金
        decimal FutRealizedPL { get; set; }//期货平仓盈亏
        decimal FutUnRealizedPL { get; set; }//期货浮动盈亏
        decimal FutCommission { get; set; }//期货交易手续费
        decimal FutCash { get; set; }//期货交易现金
        decimal FutLiquidation { get; set; }//期货总净值
        decimal FutMoneyUsed { get; set; }//期货资金占用
        decimal FutAvabileFunds { get; set; }


        decimal OptPositionCost { get; set; }//期权持仓成本
        decimal OptPositionValue { get; set; }//期权持仓市值
        decimal OptRealizedPL { get; set; }//期权平仓盈亏
        decimal OptCommission { get; set; }//期权交易手续费
        decimal OptMoneyFrozen { get; set; }//期权资金冻结
        decimal OptCash { get; set; }//期权交易现金
        decimal OptMarketValue { get; set; }//期权总市值
        decimal OptLiquidation { get; set; }//期权总净值
        decimal OptMoneyUsed { get; set; }//期权资金占用
        decimal OptAvabileFunds { get; set; }

        decimal InnovPositionCost { get; set; }//异化合约持仓成本
        decimal InnovPositionValue { get; set; }//异化合约持仓市值
        decimal InnovCommission { get; set; }//异化合约手续费
        decimal InnovRealizedPL { get; set; }//异化合约平仓盈亏
        decimal InnovMargin { get; set; }//异化合约保证金
        decimal InnovMarginFrozen { get; set; }//异化合约冻结


        decimal InnovCash { get; set; }//异化合约现金流
        decimal InnovMarketValue { get; set; }//异化合约市值
        decimal InnovLiquidation { get; set; }//异化合约净值
        decimal InnovMoneyUsed { get; set; }//异化合约资金占用
        decimal InnovAvabileFunds { get; set; }//异化合约可用资金
        #endregion


    }
}
