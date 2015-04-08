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
        /// 交易帐号设置发送变动事件
        /// </summary>
        public event AccoundIDDel AccountChangeEvent;


        /// <summary>
        /// 交易帐户警告开启事件
        /// </summary>
        public event Action<string,string> AccountWarnOnEvent;

        internal void FireAccountWarnOnEvent(string account,string message)
        { 
            if(AccountWarnOnEvent != null)
                AccountWarnOnEvent(account,message);
        }
        /// <summary>
        /// 交易帐户警告关闭事件
        /// </summary>
        public event Action<string,string> AccountWarnOffEvent;

        internal void FireAccountWarnOffEvent(string account,string message)
        {
            if (AccountWarnOffEvent != null)
                AccountWarnOffEvent(account, message);
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

        internal void FireAccountChangeEent(string account)
        {
            if (AccountChangeEvent != null)
                AccountChangeEvent(account);
        }

    }
}
