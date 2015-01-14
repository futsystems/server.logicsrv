using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 交易信息类的扩展方法
    /// </summary>
    public static class AccountUtils_TradingInfoEx
    {
        public static AccountBase GetBase(this IAccount account)
        {
            return account as AccountBase;
        }

        

        /// <summary>
        /// 是否有任何持仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetAnyPosition(this IAccount account)
        {
            return GetPositionsHold(account).Count() > 0;
        }


        /// <summary>
        /// 获得当前所有持仓 持仓数量不为0的持仓对象
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<Position> GetPositionsHold(this IAccount account)
        {
            return account.Positions.Where(pos => !pos.isFlat);//当前持仓不为Flat
        }

        /// <summary>
        /// 返回所有持仓手数之和
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static int GetTotalPositionSize(this IAccount account)
        {
            return account.GetPositionsHold().Sum(pos => pos.UnsignedSize);
        }

        /// <summary>
        /// 获得某个合约上的所有待成交委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static IEnumerable<Order> GetPendingOrders(this IAccount account, string symbol)
        {
            return account.Orders.Where(o => o.IsPending() && o.Symbol.Equals(symbol));
        }

        /// <summary>
        /// 获得所有待成交委托
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IEnumerable<Order> GetPendingOrders(this IAccount account)
        {
            return account.Orders.Where(o => o.IsPending());
        }


        /// <summary>
        /// 待处理开仓委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public static IEnumerable<Order> GetPendingEntryOrders(this IAccount account, string symbol, bool positionside)
        {
            return account.Orders.Where(o => (o.Symbol.Equals(symbol)) && o.IsPending() && (o.PositionSide == positionside) && (o.IsEntryPosition));
        }

        /// <summary>
        /// 待处理开仓委托数量
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public static int GetPendingEntrySize(this IAccount account, string symbol, bool positionside)
        {
            return GetPendingEntryOrders(account, symbol, positionside).Sum(o => o.UnsignedSize);
        }

        /// <summary>
        /// 待处理平仓委托
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public static IEnumerable<Order> GetPendingExitOrders(this IAccount account, string symbol, bool positionside)
        {
            return account.Orders.Where(o => (o.Symbol.Equals(symbol)) && o.IsPending() && (o.PositionSide == positionside) && (!o.IsEntryPosition));
        }

        /// <summary>
        /// 待处理平仓委托数量
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <param name="positionside"></param>
        /// <returns></returns>
        public static int GetPendingExitSize(this IAccount account, string symbol, bool positionside)
        {
            return GetPendingExitOrders(account,symbol,positionside).Sum(o => o.UnsignedSize);
        }

        /// <summary>
        /// 获得某个合约的所有持仓数量包含多和空
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static int GetPositionSize(this IAccount account, string symbol)
        {
            return account.GetLongPositionSize(symbol) + account.GetShortPositionSize(symbol);
        }

        /// <summary>
        /// 获得某个合约的多头持仓数量
        /// </summary>
        /// <param name="accoount"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static int GetLongPositionSize(this IAccount accoount, string symbol)
        {
            return accoount.GetPosition(symbol, true).UnsignedSize;
        }
        /// <summary>
        /// 获得某个合约的空头持仓数量
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static int GetShortPositionSize(this IAccount account, string symbol)
        {
            return account.GetPosition(symbol, false).UnsignedSize; 
        }
        /// <summary>
        /// 判断是否有多方头寸
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetHaveLongPosition(this IAccount account)
        {
            return account.PositionsLong.Where(pos => !pos.isFlat).Count() > 0;
        }

        public static bool GetHaveLongPosition(this IAccount account, string symbol)
        {
            return account.PositionsLong.Where(pos => !pos.isFlat && pos.Symbol.Equals(symbol)).Count()>0;
        }

        /// <summary>
        /// 判断是否有空方头寸
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetHaveShortPosition(this IAccount account)
        {
            return account.PositionsShort.Where(pos => !pos.isFlat).Count() > 0;
        }

        public static bool GetHaveShortPosition(this IAccount account, string symbol)
        {
            return account.PositionsShort.Where(pos => !pos.isFlat && pos.Symbol.Equals(symbol)).Count() > 0;
        }
        
    }
}