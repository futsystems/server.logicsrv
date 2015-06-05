using System;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 交易帐号类事件,交易帐号增加,交易帐号冻结
    /// </summary>
    public class AccountEvent
    {
        /// <summary>
        /// 交易帐号冻结事件
        /// </summary>
        public event AccoundIDDel AccountInactiveEvent;

        /// <summary>
        /// 交易帐号激活事件
        /// </summary>
        public event AccoundIDDel AccountActiveEvent;


        /// <summary>
        /// 添加交易帐号事件
        /// </summary>
        public event AccoundIDDel AccountAddEvent;


        /// <summary>
        /// 交易帐号删除事件
        /// </summary>
        public event AccoundIDDel AccountDelEvent;


        /// <summary>
        /// 交易帐户出入金事件
        /// </summary>
        public event Action<string, QSEnumCashOperation, decimal> AccountCashOperationEvent;

        /// <summary>
        /// 交易帐户交易通知事件
        /// </summary>
        public event Action<string,string> AccountTradingNoticeEvent;



        /// <summary>
        /// 交易帐户路由切换11:00
        /// </summary>
        public event Action<string,QSEnumOrderTransferType> AccountRouterSwitchEvent;

        internal void FireAccountRouterSwitchEvent(string account, QSEnumOrderTransferType type)
        {
            if (AccountRouterSwitchEvent != null)
                AccountRouterSwitchEvent(account, type);
        }


        internal void FireAccountTradingNoticeEvent(string account, string content)
        {
            if (AccountTradingNoticeEvent != null)
                AccountTradingNoticeEvent(account, content);
        }
        /// <summary>
        /// 触发某个交易帐户的出入金事件
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        internal void FireAccountCashOperationEvent(string account,QSEnumCashOperation type,decimal amount)
        {
            if (AccountCashOperationEvent != null)
                AccountCashOperationEvent(account, type, amount);
        }

        internal void FireAccountInactiveEvent(string account)
        {
            if(AccountInactiveEvent != null)
                AccountInactiveEvent(account);
        }

        internal void FireAccountActiveEvent(string account)
        {
            if (AccountActiveEvent != null)
                AccountActiveEvent(account);
        }

        internal void FireAccountAddEvent(string account)
        {
            if (AccountAddEvent != null)
                AccountAddEvent(account);
        }

        internal void FireAccountDelEvent(string account)
        {
            if (AccountDelEvent != null)
                AccountDelEvent(account);
        }



    }
}
