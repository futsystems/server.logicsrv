using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    /// <summary>
    /// 用于植入TLClientNet 响应客户端的回报
    /// </summary>
    public interface ILogicHandler
    {

        #region  交易数据回报与处理
        /// <summary>
        /// 行情数据回报
        /// </summary>
        /// <param name="k"></param>
        void OnTick(Tick k);
        /// <summary>
        /// 获得服务端委托回报
        /// </summary>
        /// <param name="o"></param>
        void OnOrder(Order o);

        /// <summary>
        /// 获得服务端昨日持仓回报
        /// </summary>
        /// <param name="pos"></param>
        void OnHoldPosition(Position pos);

        /// <summary>
        /// 获得服务端成交回报
        /// </summary>
        /// <param name="f"></param>
        void OnTrade(Trade f);


        /// <summary>
        /// 持仓更新回报
        /// </summary>
        /// <param name="pos"></param>
        void OnPositionUpdate(Position pos);

        #endregion
        /// <summary>
        /// 响应客户端交易帐户回报
        /// </summary>
        /// <param name="account"></param>
        void OnAccountLite(IAccountLite account, bool islast);

        /// <summary>
        /// 响应服务端交易帐户实时资金变动信息
        /// </summary>
        /// <param name="account"></param>
        void OnAccountInfoLite(IAccountInfoLite account);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        void OnMGRResumeResponse(RspMGRResumeAccountResponse response);

        /// <summary>
        /// 交易客户端 登入 退出状态更新
        /// </summary>
        /// <param name="notify"></param>
        void OnMGRSessionUpdate(NotifyMGRSessionUpdateNotify notify);

        /// <summary>
        /// 管理端查询交易帐户信息回报
        /// </summary>
        /// <param name="accountinfo"></param>
        void OnAccountInfo(IAccountInfo accountinfo);

        /// <summary>
        /// 交易帐户变动
        /// </summary>
        /// <param name="account"></param>
        void OnAccountChagne(IAccountLite account);


        /// <summary>
        /// 响应服务端的查询通道列表回报
        /// </summary>
        /// <param name="response"></param>
        void OnMGRConnectorResponse(ConnectorInfo c, bool islast);



        #region 基础信息
        /// <summary>
        /// 获得交易所列表回报
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="islast"></param>
        void OnMGRExchangeResponse(Exchange ex, bool islast);

        /// <summary>
        /// 获得交易时间段列表回报
        /// </summary>
        /// <param name="mt"></param>
        /// <param name="islast"></param>
        void OnMGRMarketTimeResponse(MarketTime mt, bool islast);


        /// <summary>
        /// 获得品种列表回报
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="islast"></param>
        void OnMGRSecurityResponse(SecurityFamilyImpl sec, bool islast);


        /// <summary>
        /// 获得合约列表回报
        /// </summary>
        /// <param name="sym"></param>
        /// <param name="islast"></param>
        void OnMGRSymbolResponse(SymbolImpl sym, bool islast);


        /// <summary>
        /// 品种增加回报
        /// </summary>
        /// <param name="security"></param>
        /// <param name="islast"></param>
        void OnMGRSecurityAddResponse(SecurityFamilyImpl security, bool islast);

        /// <summary>
        /// 合约增加回报
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="islast"></param>
        void OnMGRSymbolAddResponse(SymbolImpl symbol, bool islast);
        #endregion

        #region 风控规则类
        /// <summary>
        /// 风控规则种类回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRuleClassResponse(RuleClassItem item, bool islast);

        /// <summary>
        /// 帐户风控规则项目回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRuleItemResponse(RuleItem item, bool islast);

        /// <summary>
        /// 风控项更新回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRuleItemUpdate(RuleItem item, bool islast);
        /// <summary>
        /// 删除风控项回报
        /// </summary>
        /// <param name="item"></param>
        /// <param name="islast"></param>
        void OnMGRRulteItemDelete(RuleItem item, bool islast);
        #endregion


        #region 系统状态与通知
        /// <summary>
        /// 查询系统状态回报
        /// </summary>
        /// <param name="status"></param>
        /// <param name="islast"></param>
        void OnMGRSytstemStatus(SystemStatus status, bool islast);
        #endregion


        #region 历史记录查询
        void OnMGROrderResponse(Order o, bool islast);
        void OnMGRTradeResponse(Trade f, bool islast);
        void OnMGRPositionResponse(SettlePosition pos, bool islast);
        void OnMGRCashTransactionResponse(CashTransaction c, bool islast);
        void OnMGRSettlementResponse(RspMGRQrySettleResponse response);
        #endregion

    }
}
