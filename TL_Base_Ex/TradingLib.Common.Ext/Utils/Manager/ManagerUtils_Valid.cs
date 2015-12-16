using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static partial class ManagerUtils
    {
        /// <summary>
        /// 验证是否有权限操作某帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        public static void ValidRightReadAccount(this Manager mgr, string accid)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[accid];
            mgr.ValidRightReadAccount(account);
        }

        /// <summary>
        /// 验证是否有权限操作某帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        public static void ValidRightReadAccount(this Manager mgr, IAccount account)
        {
            if (!mgr.RightAccessAccount(account)) throw new FutsRspError("无权查看该帐户");
        }



        /// <summary>
        /// 验证是否有添加管理员的权限
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="managertoadd"></param>
        /// <returns></returns>
        public static void ValidRightAddManager(this Manager mgr, ManagerSetting managertoadd)
        {
            //是否有权限添加管理员
            if (mgr.IsRoot() || mgr.IsAgent())
            {
                return;
            }
            throw new FutsRspError("无权添加柜员帐户");
        }
    }
}
