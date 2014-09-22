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


        decimal OnAdjustCommission(Trade t, IPositionRound pr);

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

        #region 交易业务逻辑部分

        /// <summary>
        /// 检查合约交易权限
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CanTradeSymbol(Symbol symbol, out string msg);

        /// <summary>
        /// 保证金检查
        /// </summary>
        /// <param name="o"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        bool CanTakeOrder(Order o, out string msg);

        /// <summary>
        /// 获得帐户某个合约的可用资金
        /// 在进行保证金检查时需要查询某个合约的可用资金
        /// 在业务逻辑覆写时 通过服务对应的结构对外暴露
        /// 然后在account主逻辑中进行调用
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        decimal GetFundAvabile(Symbol symbol);

        /// <summary>
        /// 计算通过配资服务后某个合约的可开手数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        int CanOpenSize(Symbol symbol);
        #endregion


    }
}
