using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 定义了财务计算接口用于计算保证金 平仓利润 等等信息
    /// </summary>
    public interface IFinanceCaculation
    {
        decimal CalOrderFundRequired(Order o, decimal mktMvalue = decimal.MaxValue);

        decimal CalFutMargin(IAccount acc);//计算期货保证金
        decimal CalFutMarginFrozen(IAccount acc);//计算期货冻结保证金
        decimal CalFutUnRealizedPL(IAccount acc);//计算期货浮动盈亏
        decimal CalFutRealizedPL(IAccount acc);//计算期货平仓盈亏
        decimal CalFutCommission(IAccount acc);//计算期货交易手续费
        decimal CalFutSettleUnRealizedPL(IAccount acc);//计算期货结算时盯市盈亏

        decimal CalOptPositionCost(IAccount acc);//计算期权持仓成本
        decimal CalOptPositionValue(IAccount acc);//计算期权市值
        decimal CalOptRealizedPL(IAccount acc);//计算期权平仓盈亏
        decimal CalOptCommission(IAccount acc);//计算期权交易手续费
        decimal CalOptMoneyFrozen(IAccount acc);//计算期权资金冻结
        decimal CalOptSettlePositionValue(IAccount acc);//计算期权结算市值


        decimal CalInnovPositionCost(IAccount acc);//计算异化合约持仓成本
        decimal CalInnovPositionValue(IAccount acc);//计算异化合约持仓市值
        decimal CalInnovCommission(IAccount acc);//计算异化合约手续费
        decimal CalInnovRealizedPL(IAccount acc);//计算异化合约的平仓盈亏
        decimal CalInnovMargin(IAccount acc);//计算异化合约所占用的保证金
        decimal CalInnovMarginFrozen(IAccount acc);//异化合约保证金
        decimal CalInnovSettlePositionValue(IAccount acc);//异化合约的结算持仓市值
    }
}
