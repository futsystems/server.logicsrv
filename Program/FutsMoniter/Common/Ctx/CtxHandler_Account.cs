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

        bool _basicinfodone = false;
        public bool BasicInfoDone { get { return _basicinfodone; } set { _basicinfodone = value; } }

        Dictionary<string, IAccountLite> accountmap = new Dictionary<string, IAccountLite>();

        public IEnumerable<IAccountLite> Accounts { get { return accountmap.Values; } }

        /// <summary>
        /// 事件中继响应帐户选择事件
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountSelected(IAccountLite account)
        {
            if (GotAccountSelectedEvent != null)
                GotAccountSelectedEvent(account);
        }
        /// <summary>
        /// 响应客户端交易帐户回报
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountLite(IAccountLite account, bool islast)
        {
            if (account != null)
            {
                accountmap[account.Account] = account;
                if (GotAccountEvent != null)
                    GotAccountEvent(account);
            }
            if (islast)
            {
                _basicinfodone = true;
                if (GotBasicInfoDoneEvent != null)
                    GotBasicInfoDoneEvent();
            }
        }

        /// <summary>
        /// 响应服务端交易帐户实时资金变动信息
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountInfoLite(IAccountInfoLite account)
        {
            if (GotFinanceInfoLiteEvent != null)
                GotFinanceInfoLiteEvent(account);
        }

        /// <summary>
        /// 管理端查询交易帐户信息回报
        /// </summary>
        /// <param name="accountinfo"></param>
        public void OnAccountInfo(IAccountInfo accountinfo)
        {
            //if (GotFinanceInfoEvent != null)
            //    GotFinanceInfoEvent(accountinfo);
        }

        /// <summary>
        /// 交易帐户变动
        /// </summary>
        /// <param name="account"></param>
        public void OnAccountChagne(IAccountLite account)
        {
            if (GotAccountChangedEvent != null)
                GotAccountChangedEvent(account);
        }


       
        /// <summary>
        /// 恢复交易帐户交易数据
        /// </summary>
        /// <param name="response"></param>
        public void OnMGRResumeResponse(RspMGRResumeAccountResponse response) 
        {
            if (GotResumeResponseEvent != null)
                GotResumeResponseEvent(response);
        }

        
        /// <summary>
        /// 交易客户端 登入 退出状态更新
        /// </summary>
        /// <param name="notify"></param>
        public void OnMGRSessionUpdate(NotifyMGRSessionUpdateNotify notify)
        {
            if (GotSessionUpdateEvent != null)
                GotSessionUpdateEvent(notify);
        }

        

    }
}
