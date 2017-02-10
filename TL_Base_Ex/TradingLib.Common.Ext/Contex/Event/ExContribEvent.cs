﻿using System;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 扩展模块的强关系所对应的事件
    /// 系统解耦过程中有一些关系是无法彻底解耦的,在这个类里面放置了底层系统对扩展模块的调用
    /// 这些事件只有在扩展模块存在时才有意义,是针对该扩展模块的一些回调
    /// </summary>
    public class ExContribEvent
    {
        /// <summary>
        /// 获得某个交易帐号的所有可用配资额度
        /// </summary>
        public event AccountFinAmmountDel GetFinAmmountAvabileEvent;

        /// <summary>
        /// 获得某个交易帐号总的配资额度 用于提现在帐户可用资金
        /// </summary>
        public event AccountFinAmmountDel GetFinAmmountTotalEvent;

        /// <summary>
        /// 针对某笔交易调整交易手续费
        /// </summary>
        public event AdjustCommissionDel AdjustCommissionEvent;



        internal decimal GetFinAmmountAvabile(string account)
        {
            if (GetFinAmmountAvabileEvent != null)
                return GetFinAmmountAvabileEvent(account);
            return 0;
        }

        internal decimal GetFinAmmountTotal(string account)
        {
            if (GetFinAmmountTotalEvent != null)
                return GetFinAmmountTotalEvent(account);
            return 0;
        }

        internal decimal AdjustCommission(Trade f, PositionRound pr)
        {
            if (AdjustCommissionEvent != null)
                return AdjustCommissionEvent(f, pr);
            return f.Commission;
        }
    }
}