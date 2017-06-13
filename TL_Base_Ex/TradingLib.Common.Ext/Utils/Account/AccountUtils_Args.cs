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
            return account.Domain.Cfg_MaxMarginSide;
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
            return account.Domain.Cfg_GrossPosition;

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

        #region 模拟成交参数
        /// <summary>
        /// 限价单是否按挂单价格成交
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetSimExecuteStickLimitPrice(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SimExecuteStickLimitPrice;
            return AccountImpl.SimExecuteStickLimitPrice;
        }

        /// <summary>
        /// 是否一次成交所有
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetSimExecuteFillAll(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SimExecuteFillAll;
            return AccountImpl.SimExecuteFillAll;
        }

        /// <summary>
        /// 最小成交数量
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static int GetSimExecuteMinSize(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SimExecuteMinSize;
            return AccountImpl.SimExecuteMinSize;
        }

        /// <summary>
        /// 检查Tick时间
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetSimExecuteTimeCheck(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SimExecuteTimeCheck;
            return AccountImpl.SimExecuteTimeCheck;
        }

        /// <summary>
        /// 使用盘口成交
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetSimExecuteUseBidAsk(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SimExecuteUseAskBid;
            return AccountImpl.SimExecuteUseAskBid;
        }

        /// <summary>
        /// 是否启用中金所交易策略
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool GetSimExecuteCFFEXStrategy(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.SimExecuteCFFEXStrategy;
            return AccountImpl.SimExecuteCFFEXStrategy;
        }
        #endregion

        #region 出入金部分

        public static decimal GetDepositCommission(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.DepositCommission;
            return AccountImpl.DepositCommission;
        }

        public static decimal GetWithdrawCommission(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.WithdrawCommission;
            return AccountImpl.WithdrawCommission;
        }

        public static decimal GetLeverageRatio(this IAccount account)
        {
            ExStrategy s = account.GetExStrategy();
            if (s != null)
                return s.LeverageRatio;
            return AccountImpl.LeverageRatio;
        }

        #endregion
    }
}