using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils_FinCal
    {

        #region 财务计算

        #region 对象过滤 返回对象不toarray避免的内存引用copy所有的计算只需要进行一次foreach循环
        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Order> FilterOrders(this IAccount account, SecurityType type)
        {
            return account.Orders.Where(o => o.SecurityType == type);
        }

        public static IEnumerable<Order> FilterPendingOrders(this IAccount account, SecurityType type)
        {
            return account.Orders.Where(o => o.SecurityType == type && o.IsPending());
        }

        public static IEnumerable<Trade> FilterTrades(this IAccount account, SecurityType type)
        {
            return account.Trades.Where(f => f.SecurityType == type);
        }

        public static IEnumerable<Position> FilterPositions(this IAccount account, SecurityType type)
        {
            return account.Positions.Where(p => p.oSymbol.SecurityType == type);
        }
        #endregion


        #region 计算期货财务数据
        public static decimal CalFutMargin(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcPositionMargin());
        }

        public static decimal CalFutMarginFrozen(this IAccount account)
        {
            return FilterPendingOrders(account, SecurityType.FUT).Where(o => o.IsEntryPosition).Sum(e => account.CalOrderFundRequired(e, 0));
        }

        public static decimal CalFutUnRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcUnRealizedPL());
        }

        public static decimal CalFutSettleUnRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcSettleUnRealizedPL());
        }

        public static decimal CalFutRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.FUT).Sum(pos => pos.CalcRealizedPL());
        }

        public static decimal CalFutCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.FUT).Sum(fill => fill.GetCommission());
        }

        public static decimal CalFutCash(this IAccount account)
        {
            return CalFutRealizedPL(account) + CalFutUnRealizedPL(account) - CalFutCommission(account);
        }

        public static decimal CalFutLiquidation(this IAccount account)
        {
            return CalFutCash(account);
        }

        public static decimal CalFutMoneyUsed(this IAccount account)
        {
            return CalFutMargin(account) + CalFutMarginFrozen(account);
        }


        #endregion

        #region 期权计算
        /// <summary>
        /// 期权持仓成本
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalOptPositionCost(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcPositionCost());
        }
        /// <summary>
        /// 期权持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptPositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcPositionValue());
        }
        /// <summary>
        /// 期权结算市值
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalOptSettlePositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcSettlePositionValue());
        }
        /// <summary>
        /// 期权平仓盈亏
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal CalOptRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.OPT).Sum(pos => pos.CalcRealizedPL());
        }
        /// <summary>
        /// 期权交易手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.OPT).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 期权资金占用
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalOptMoneyFrozen(this IAccount account)
        {
            try
            {
                return FilterPendingOrders(account, SecurityType.OPT).Sum(e => account.CalOrderFundRequired(e, 0));
            }
            catch (Exception ex)
            {
                return decimal.MaxValue;
            }
        }


        public static decimal CalOptCash(this IAccount account)
        {
            return CalOptRealizedPL(account) - CalOptCommission(account) - CalOptPositionCost(account);
        }

        public static decimal CalOptLiquidation(this IAccount account)
        {
            return CalOptPositionValue(account) + CalOptCash(account);
        }

        public static decimal CalOptMoneyUsed(this IAccount account)
        {
            return CalOptMoneyFrozen(account) + CalOptPositionCost(account);
        }
  
        #endregion

        #region 异化合约财务计算
        /// <summary>
        /// 异化合约持仓成本
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovPositionCost(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcPositionCost());
        }

        /// <summary>
        /// 异化合约持仓市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovPositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcPositionValue());
        }

        /// <summary>
        /// 异化合约结算市值
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovSettlePositionValue(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcSettlePositionValue());
        }

        /// <summary>
        /// 异化合约手续费
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovCommission(this IAccount account)
        {
            return FilterTrades(account, SecurityType.INNOV).Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 异化合约平仓盈亏
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovRealizedPL(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcRealizedPL());
        }

        /// <summary>
        /// 异化合约保证金
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovMargin(this IAccount account)
        {
            return FilterPositions(account, SecurityType.INNOV).Sum(pos => pos.CalcPositionMargin());
        }

        /// <summary>
        /// 异化合约保证金冻结
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static decimal CalInnovMarginFrozen(this IAccount account)
        {
            return FilterPendingOrders(account, SecurityType.INNOV).Sum(e => account.CalOrderFundRequired(e, 0));

        }

        public static decimal CalInnovCash(this IAccount account)
        {
            return CalInnovRealizedPL(account) - CalInnovCommission(account) - CalInnovPositionCost(account);
        }

        public static decimal CalInnovLiquidation(this IAccount account)
        {
            return CalInnovPositionValue(account) + CalInnovCash(account);
        }

        public static decimal CalInnovMoneyUsed(this IAccount account)
        {
            return CalInnovMargin(account) + CalInnovMarginFrozen(account);
        }
        #endregion


        #endregion



        public static string GetCustName(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.Name))
            {
                return Util.GetEnumDescription(account.Category) + "[" + account.ID + "]";
            }
            return account.Name;
        }

        public static string GetCustBroker(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.Broker))
            {
                return GlobalConfig.DefaultBroker;
            }
            return account.Broker;
        }

        public static string GetCustBankID(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.BankID))
            {
                return GlobalConfig.DefaultBankID;
            }

            return account.BankID;
        }

        public static string GetCustBankAC(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.BankAC))
            {
                return GlobalConfig.DefaultBankAC;
            }
            return account.BankAC;
        }
    }
}
