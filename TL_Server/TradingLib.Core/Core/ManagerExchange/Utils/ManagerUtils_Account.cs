using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Core
{
    public  static partial class MangerUtils
    {
        /// <summary>
        /// 查看某个代理的可见帐户
        /// ROOT权限的用户可以查看所有帐户
        /// 如果是代理则只能看到代理商的直接客户
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetVisibleAccount(this Manager mgr)
        {
            if (mgr.RightRootDomain())
            {
                //获得系统所有交易帐号
                return TLCtxHelper.CmdAccount.Accounts;
            }
            else
            {
                return TLCtxHelper.CmdAccount.Accounts.Where(acc => mgr.RightAccessAccount(acc));
            }
            
        }
    }
}
