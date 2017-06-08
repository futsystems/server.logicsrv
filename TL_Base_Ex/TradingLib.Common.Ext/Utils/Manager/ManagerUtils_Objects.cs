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
        /// 获得某个管理员下的帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetAccounts(this Manager mgr)
        {
            //返回某个管理员下的帐户
            return mgr.Domain.GetAccounts().Where(ac => ac.Mgr_fk == mgr.BaseMgrID);
        }

        /// <summary>
        /// 获得某个代理的直客
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetDirectAccounts(this Manager mgr)
        {
            return mgr.Domain.GetAccounts().Where(ac => ac.Mgr_fk == mgr.BaseMgrID);
        }

        /// <summary>
        /// 返回某个管理员的直接下级代理
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<Manager> GetDirectAgents(this Manager mgr)
        {
            List<Manager> mgrlist = new List<Manager>();
            foreach (var item in mgr.Domain.GetManagers())
            {
                if (item.parent_fk == mgr.mgr_fk && item.Type == QSEnumManagerType.AGENT)
                {
                    mgrlist.Add(item);
                }
            }
            return mgrlist;
            //return mgr.Domain.GetManagers().Where(mgr2 => mgr.parent_fk == mgr.mgr_fk && mgr2.Type == QSEnumManagerType.AGENT);
        }

        /// <summary>
        /// 查看某个代理的可见帐户
        /// ROOT权限的用户可以查看所有帐户
        /// 如果是代理则只能看到代理商的直接客户
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<IAccount> GetVisibleAccount(this Manager mgr)
        {
            if (mgr.IsInRoot())
            {
                //获得系统所有交易帐号
                return mgr.Domain.GetAccounts();
            }
            else
            {
                //获得某个域下所有交易帐户 如果管理员有权查看该帐户则返回
                return mgr.Domain.GetAccounts().Where(acc => mgr.RightAccessAccount(acc));
            }

        }

        /// <summary>
        /// 查看所有可见柜员帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static IEnumerable<Manager> GetVisibleManager(this Manager mgr)
        {
            if (mgr.IsInRoot())
            {
                if (mgr.Login.Equals("sroot"))
                {
                    return mgr.Domain.GetManagers();
                }
                else
                {
                    return mgr.Domain.GetManagers().Where(m => !m.Login.Equals("sroot"));
                }
            }
            else
            {
                return mgr.Domain.GetManagers().Where(mgr2 => mgr.RightAccessManager(mgr2));
            }
        }


        /// <summary>
        /// 获得某个管理员的权限
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static UIAccess GetAccess(this Manager mgr)
        {
            return BasicTracker.UIAccessTracker.GetUIAccess(mgr);
        }

    }
}
