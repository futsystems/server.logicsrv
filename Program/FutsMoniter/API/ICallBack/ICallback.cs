using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.API
{
    public interface ICallbackCentre
    {
        #region 帐户类事件
        /// <summary>
        /// 交易帐号
        /// </summary>
        event Action<IAccountLite> GotAccountEvent;
        /// <summary>
        /// 财务信息
        /// </summary>
        //event Action<IAccountInfo> GotFinanceInfoEvent;
        /// <summary>
        /// 动态财务信息
        /// </summary>
        event Action<IAccountInfoLite> GotFinanceInfoLiteEvent;

        /// <summary>
        /// 交易帐户变化
        /// </summary>
        event Action<IAccountLite> GotAccountChangedEvent;

        /// <summary>
        /// 登入状态变化
        /// </summary>
        event Action<NotifyMGRSessionUpdateNotify> GotSessionUpdateEvent;

        /// <summary>
        /// 恢复日内交易数据事件
        /// </summary>
        event Action<RspMGRResumeAccountResponse> GotResumeResponseEvent;
        
        #endregion



        #region 交易事件
        /// <summary>
        /// 行情事件
        /// </summary>
        event TickDelegate GotTickEvent;

        /// <summary>
        /// 委托事件
        /// </summary>
        event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 成交事件
        /// </summary>
        event FillDelegate GotFillEvent;

        #endregion



        /// <summary>
        /// 操作回报
        /// </summary>
        event Action<RspInfo> GotRspInfoEvent;


        #region 风控规则
        event Action<RuleItem, bool> GotRuleItemEvent;

        event Action<RuleItem> GotRuleItemUpdateEvemt;

        event Action<RuleItem> GotRuleItemDeleteEvent;

        #endregion


        #region 客户交易历史查询

        event Action<Order, bool> GotHistOrderEvent;
        event Action<Trade, bool> GotHistTradeEvent;
        event Action<RspMGRQrySettleResponse> GotSettlementEvent;

        #endregion

        #region Contrib 扩展指令
        /// <summary>
        /// 注册回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        void RegisterCallback(string module, string cmd, JsonReplyDel del);


        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="module"></param>
        /// <param name="cmd"></param>
        /// <param name="del"></param>
        void UnRegisterCallback(string module, string cmd, JsonReplyDel del);
        #endregion


    }
}
