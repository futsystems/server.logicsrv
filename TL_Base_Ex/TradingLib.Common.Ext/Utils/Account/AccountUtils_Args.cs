using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 将交易账户交易参数全部放入模板中进行设置
    /// 这里以扩展函数的形式提供参数暴露
    /// </summary>
    public static class AccountUtils_Args
    {
        /// <summary>
        /// 获得交易账户单向大边保证金设置
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetArgsSideMargin(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SideMargin;
            return false;
        }

        /// <summary>
        /// 交易账户财务信息中是否将信用额度分开显示
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetArgsCreditSeparate(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.CreditSeparate;
            return true;
        }

        /// <summary>
        /// 是否支持锁仓操作
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetArgsPositionLock(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.PositionLock;
            return true;

        }

        /// <summary>
        /// 获得交易账户的保证金计算方式
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static QSEnumMarginStrategy GetArgsMarginStrategy(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.Margin;
            return QSEnumMarginStrategy.LastPrice;
        }

        /// <summary>
        /// 返回交易账户浮盈是否可开仓设置
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static QSEnumAvabileFundStrategy GetArgsAvabileFundStrategy(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.AvabileFund;
            return QSEnumAvabileFundStrategy.UnPLInclude;
        }
 
    }
}