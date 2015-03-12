using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using FutsMoniter;

namespace TradingLib.Common
{
    /// <summary>
    /// 消息处理中继,实现ILogicHandler,用于处理底层回报上来的消息
    /// 界面层订阅这里的事件 实现数据展示
    /// </summary>
    public partial class Ctx
    {
        /// <summary>
        /// 响应帐户选择事件
        /// </summary>
        public event Action<AccountLite> GotAccountSelectedEvent;

        /// <summary>
        /// 交易帐户同步事件
        /// </summary>
        public event Action<AccountLite> GotAccountSyncEvent;

        /// <summary>
        /// 交易帐号
        /// </summary>
        public event Action<AccountLite> GotAccountEvent;

        /// <summary>
        /// 财务信息
        /// </summary>
        //public event Action<IAccountInfo> GotFinanceInfoEvent;

        /// <summary>
        /// 动态财务信息
        /// </summary>
        public event Action<AccountInfoLite> GotFinanceInfoLiteEvent;

        /// <summary>
        /// 交易帐户变化
        /// </summary>
        public event Action<AccountLite> GotAccountChangedEvent;

        /// <summary>
        /// 交易帐户登入信息事件
        /// </summary>
        public event Action<NotifyMGRSessionUpdateNotify> GotSessionUpdateEvent;

        /// <summary>
        /// 获得恢复交易帐号事件
        /// </summary>
        public event Action<RspMGRResumeAccountResponse> GotResumeResponseEvent;

    }
}
