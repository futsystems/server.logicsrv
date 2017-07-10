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

        /// <summary>
        /// 检查管理员是否有权操作账户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        public static void PermissionCheckAccount(this Manager mgr, string account)
        {
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError(string.Format("交易账户:{0}不存在", account));
            }
            if(!mgr.RightAccessAccount(acc))
            {
                throw new FutsRspError(string.Format("无权操作交易账户:{0}",account));
            }
        }
    }
}
