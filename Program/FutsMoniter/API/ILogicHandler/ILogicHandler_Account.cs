﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.API
{
    public partial interface ILogicHandler
    {
        IEnumerable<IAccountLite> Accounts { get; }

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

    }
}
