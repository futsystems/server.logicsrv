using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Contrib.FinService
{

    /// <summary>
    /// 收费模式
    /// 交易回合收费
    /// 按成交收费
    /// 按日收费
    /// 
    /// </summary>
    public interface IFinService
    {
        /// <summary>
        /// 配资服务费用计算方式
        /// </summary>
        EnumFeeChargeType ChargeType { get; }

        /// <summary>
        /// 配资服务费用采集方式
        /// </summary>
        EnumFeeCollectType CollectType { get; }


        void OnTrade(Trade t);

        void OnRound(IPositionRound round);

        void OnSettle();
        /// <summary>
        /// FinService初始化
        /// 每个服务需要初始化参数
        /// 包括帐户参数和代理参数,从而实现分润
        /// 比如
        /// 帐户设置1万/15元
        /// 代理设置1万/10元
        /// 从而可以计算获得代理收费和帐户收费
        /// </summary>
        /// <param name="accountarg"></param>
        /// <param name="agentarg"></param>
        void InitArgument(Dictionary<string, Argument> accountarg, Dictionary<string, Argument> agentarg);


        /// <summary>
        /// 按金额计算收费
        /// </summary>
        /// <param name="account"></param>
        /// <param name="finammount"></param>
       // void Cal(IAccount account, decimal finammount);

    }
}
