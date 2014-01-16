using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Contrib.FinService
{

    /// <summary>
    /// 配资服务接口
    /// 定义具体的业务逻辑
    /// 然后在FinServiceStub中进行调用
    /// 不同的FinServiceStub其实是加载了不同的IFinService
    /// FinServiceStub同时实现IAccountService接口
    /// 
    /// 收费模式
    /// 交易回合收费
    /// 按成交收费
    /// 按日收费
    /// 
    /// </summary>
    public interface IFinService
    {
        /// <summary>
        /// 对应服务计划ID
        /// </summary>
        int ServicePlanFK { get; set; }

        /// <summary>
        /// 对应服务ID
        /// </summary>
        int ServiceID { get; set; }

        /// <summary>
        /// 配资服务费用计算方式
        /// </summary>
        EnumFeeChargeType ChargeType { get; }

        /// <summary>
        /// 配资服务费用采集方式
        /// </summary>
        EnumFeeCollectType CollectType { get; }

        /// <summary>
        /// 获得配资服务的参数
        /// </summary>
        Dictionary<string, Argument> AccountArgumentMap { get; }

        /// <summary>
        /// 是否被强平
        /// </summary>
        bool ForceClose { get; }

        #region 响应交易事件
        /// <summary>
        /// 响应手续费调整事件
        /// </summary>
        /// <param name="t"></param>
        /// <param name="pr"></param>
        /// <returns></returns>
        decimal OnAdjustCommission(Trade t, IPositionRound pr);

        /// <summary>
        /// 响应成交
        /// </summary>
        /// <param name="t"></param>
        void OnTrade(Trade t);

        /// <summary>
        /// 响应成交回合
        /// </summary>
        /// <param name="round"></param>
        void OnRound(IPositionRound round);

        /// <summary>
        /// 响应结算事件
        /// 结算事件应该分为 结算后事件和结算前事件
        /// 具体这里需要结合业务逻辑进行分析
        /// </summary>
        void OnSettle();
        #endregion



        #region 交易业务逻辑部分

        /// <summary>
        /// 返回某个合约的手续费参数
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        CommissionConfig GetCommissionConfig(Symbol symbol);

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

        /// <summary>
        /// 获得该配资服务提供的配资金额 用于以资金为单位的配资模式
        /// </summary>
        /// <returns></returns>
        decimal GetFinAmountAvabile();
        #endregion


        /// <summary>
        /// 执行定时帐户检查 
        /// 当帐户情况到到一定条件时
        /// 执行强平
        /// </summary>
        void CheckAccount();


        /// <summary>
        /// 服务调整
        /// 结算时按照权益重新计算配资额度等
        /// </summary>
        void OnCashOperation(JsonWrapperCashOperation op);

        /// <summary>
        /// 响应参数变化
        /// </summary>
        void OnArgumentChanged();

        /// <summary>
        /// 响应帐户激活事件
        /// </summary>
        /// <param name="account"></param>
        void OnAccountActive(string account);
    }


    
}
