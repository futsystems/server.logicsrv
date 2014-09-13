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
        public event AccountIdDel AccountInactiveEvent;

        /// <summary>
        /// 交易帐号激活事件
        /// </summary>
        public event AccountIdDel AccountActiveEvent;


        /// <summary>
        /// 添加交易帐号事件
        /// </summary>
        public event AccountIdDel AccountAddEvent;


        /// <summary>
        /// 交易帐号删除事件
        /// </summary>
        public event AccountIdDel AccountDelEvent;




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
