using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public partial interface ILogicHandler
    {
        IEnumerable<AccountLite> Accounts { get; }

        /// <summary>
        /// 响应客户选择事件
        /// </summary>
        /// <param name="account"></param>
        void OnAccountSelected(AccountLite account);

        /// <summary>
        /// 触发交易帐户 同步交易数据事件
        /// </summary>
        /// <param name="account"></param>
        void FireAccountSyncEvent(AccountLite account);
        /// <summary>
        /// 响应客户端交易帐户回报
        /// </summary>
        /// <param name="account"></param>
        void OnAccountLite(AccountLite account, bool islast);

        /// <summary>
        /// 响应服务端交易帐户实时资金变动信息
        /// </summary>
        /// <param name="account"></param>
        void OnAccountInfoLite(AccountInfoLite account);

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
        void OnAccountInfo(AccountInfo accountinfo);

        /// <summary>
        /// 交易帐户变动
        /// </summary>
        /// <param name="account"></param>
        void OnAccountChagne(AccountLite account);

    }
}
