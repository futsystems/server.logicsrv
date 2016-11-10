﻿using System;
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
        /// 默认不执行单向大边
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetParamSideMargin(this IAccount account)
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
        public static bool GetParamCreditSeparate(this IAccount account)
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
        public static bool GetParamPositionLock(this IAccount account,SecurityFamily sec)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.PositionLock;

            if (sec.Currency == CurrencyType.RMB)
                return true;
            return false;

        }

        /// <summary>
        /// 获得交易账户的保证金计算方式
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static QSEnumMarginPrice GetParamMarginPriceType(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.MarginPrice;
            return QSEnumMarginPrice.OpenPrice;
        }

        /// <summary>
        /// 返回交易账户浮盈是否可开仓设置
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetParamIncludePositionProfit(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.IncludePositionProfit;
            return true;
        }

        /// <summary>
        /// 返回交易平仓盈亏是否可开仓
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetParamIncludeCloseProfit(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.IncludeCloseProfit;
            return true;
        }

        /// <summary>
        /// 返回交易帐户 浮动盈亏计算 算法
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static QSEnumAlgorithm GetParamAlgorithm(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.Algorithm;
            return QSEnumAlgorithm.AG_All;
        }

    }
}