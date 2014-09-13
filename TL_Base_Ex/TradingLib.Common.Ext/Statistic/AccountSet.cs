using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 账户组,用于在用户组的基础上进行数据统计
    /// </summary>
    public class AccountsSet : ThreadSafeList<IAccount>
    {
        //protected ClearCentreBase _clearcentre;
        //public AccountsSet(ClearCentreBase cc)
        //{
        //    _clearcentre = cc;//绑定清算中心
        //
        //}

        /// <summary>
        /// 设定账户组账户
        /// </summary>
        /// <param name="list"></param>
        public virtual void SetAccounts(IAccount[] list)
        {
            this.Clear();
            foreach (IAccount a in list)
            {
                this.Add(a);
            }
        }


        private string _format = "{0:F1}";
        protected string decDisp(decimal d)
        {
            return string.Format(_format, d);
        }

    }
}
