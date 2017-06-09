using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{

    public interface ISettleCentre
    {
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
        /// 结算时间
        /// </summary>
        int SettleTime { get; }

        /// <summary>
        /// 重置时间
        /// </summary>
        int ResetTime { get; }

        /// <summary>
        /// 下一个结算时间
        /// </summary>
        long NextSettleTime { get; }

        /// <summary>
        /// 投资者账户结算成功
        /// 结算中心结算投资者账户时 将每个投资者账户结算记录放入缓存
        /// 代理账户进行结算时 自营代理结算需要用到投资者账户结算记录
        /// </summary>
        /// <param name="settlement"></param>
        void InvestAccountSettled(AccountSettlement settlement);

        /// <summary>
        /// 获得某个交易账户结算记录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        AccountSettlement GetInvestSettlement(string account);
    }
}
