﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public enum QSEnumSettleCentreStatus
    {
        /// <summary>
        /// 未初始化
        /// </summary>
        UNKNOWN,//未知

        /// <summary>
        /// 结算信息不完整 需要在对应日期上加载历史记录并做结算
        /// </summary>
        HISTSETTLE,//历史结算

        /// <summary>
        /// 当前是交易日 可以进行正常的交易与结算
        /// </summary>
        TRADINGDAY,//交易日

        /// <summary>
        /// 当前非交易日
        /// </summary>
        NOTRADINGDAY,//非交易日
    }


    public interface ISettleCentre
    {
        QSEnumSettleCentreStatus SettleCentreStatus { get; }

        /// <summary>
        /// 结算模式，结算模式决定了清算中心数据加载方式
        /// </summary>
        QSEnumSettleMode SettleMode { get; }
        /// <summary>
        /// 上次结算日
        /// </summary>
        int LastSettleday { get; }

        /// <summary>
        /// 当前交易日
        /// </summary>
        int Tradingday { get; }

        /// <summary>
        /// 获得当前结算时间
        /// </summary>
        int SettleTime { get; }

        /// <summary>
        /// 下一个结算时间
        /// </summary>
        long NextSettleTime { get; }

        /// <summary>
        /// 当前是否是交易日
        /// </summary>
        bool IsTradingday { get; }

        /// <summary>
        /// 结算中心是否处于正常状态
        /// </summary>
        bool IsNormal { get; }


        /// <summary>
        /// 结算中心是否处于结算状态
        /// 正常工作状态下在交易完成后的一段时间内进行结算工作用于保存交易记录，生成结算记录和更新帐户结算权益
        /// </summary>
        bool IsInSettle { get; }


        /// <summary>
        /// 获取结算中心重置时间
        /// </summary>
        int ResetTime { get; }


        /// <summary>
        /// 获得某个交易日 某个合约的计算价格
        /// </summary>
        /// <param name="settleday"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        SettlementPrice GetSettlementPrice(int settleday, string symbol);

        /// <summary>
        /// 获得某个合约的最后一个行情快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick GetLastTickSnapshot(string symbol);
    }
}