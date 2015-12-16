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
        public event Action<IAccount> AccountInactiveEvent;

        /// <summary>
        /// 交易帐号激活事件
        /// </summary>
        public event Action<IAccount> AccountActiveEvent;


        /// <summary>
        /// 添加交易帐号事件
        /// </summary>
        public event Action<IAccount> AccountAddEvent;


        /// <summary>
        /// 交易帐号删除事件
        /// </summary>
        public event Action<IAccount> AccountDelEvent;


        /// <summary>
        /// 交易帐号设置发送变动事件
        /// </summary>
        public event Action<IAccount> AccountChangeEvent;


        /// <summary>
        /// 交易帐户警告开启事件
        /// </summary>
        public event Action<string,string> AccountWarnOnEvent;

        /// <summary>
        /// 交易帐户警告关闭事件
        /// </summary>
        public event Action<string, string> AccountWarnOffEvent;


        /// <summary>
        /// 交易帐户出入金事件
        /// </summary>
        public event Action<string, QSEnumCashOperation, decimal> AccountCashOperationEvent;

        /// <summary>
        /// 交易帐户交易通知事件
        /// </summary>
        public event Action<string, string> AccountTradingNoticeEvent;



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
        internal void FireAccountCashOperationEvent(string account, QSEnumCashOperation type, decimal amount)
        {
            if (AccountCashOperationEvent != null)
                AccountCashOperationEvent(account, type, amount);
        }



        internal void FireAccountWarnOnEvent(string account,string message)
        { 
            if(AccountWarnOnEvent != null)
                AccountWarnOnEvent(account,message);
        }


        internal void FireAccountWarnOffEvent(string account,string message)
        {
            if (AccountWarnOffEvent != null)
                AccountWarnOffEvent(account, message);
        }



        internal void FireAccountInactiveEvent(IAccount account)
        {
            if(AccountInactiveEvent != null && account!= null)
                AccountInactiveEvent(account);
        }

        internal void FireAccountActiveEvent(IAccount account)
        {
            if (AccountActiveEvent != null && account != null)
                AccountActiveEvent(account);
        }

        internal void FireAccountAddEvent(IAccount account)
        {
            if (AccountAddEvent != null && account != null)
                AccountAddEvent(account);
        }

        internal void FireAccountDelEvent(IAccount account)
        {
            if (AccountDelEvent != null && account != null)
                AccountDelEvent(account);
        }

        internal void FireAccountChangeEent(IAccount account)
        {
            if (AccountChangeEvent != null && account != null)
                AccountChangeEvent(account);
        }

    }
}
