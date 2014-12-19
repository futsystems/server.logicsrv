using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class ManagerUtils_Right
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
        /// 判断Manager是否属于Root主域
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool RightRootDomain(this Manager mgr)
        {
            if (mgr.Type == QSEnumManagerType.ROOT || mgr.BaseManager.Type == QSEnumManagerType.ROOT)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool IsRoot(this Manager mgr)
        {
            return mgr.Type == QSEnumManagerType.ROOT;
        }

        /// <summary>
        /// 是否是代理
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool IsAgent(this Manager mgr)
        {
            return mgr.Type == QSEnumManagerType.AGENT;
        }


        /// <summary>
        /// 是否可以访问某个交易帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool RightAccessAccount(this Manager mgr, IAccount account)
        {
            if (account == null) return false;
            if (!mgr.Domain.IsAccountInDomain(account)) return false;//不在同一个域
            if (mgr.RightRootDomain()) return true;
            //如果交易帐户直接属于该Manager的域 则有权限
            if (mgr.BaseMgrID.Equals(account.Mgr_fk))
            {
                return true;
            }
            //如果该管理员可以管理该帐户的所属域，则有权限
            if (mgr.RightAgentParent(account.Mgr_fk))
            {
                return true;
            }
            return false;
        }

        public static bool RightAgentParent(this Manager mgr, int mgrfk)
        {
            Manager manager = BasicTracker.ManagerTracker[mgrfk];
            return mgr.RightAgentParent(manager);
        }
        /// <summary>
        /// 判断mgrfk是否是Manager的子代理或者子子xx代理，代理A对发展的代理A1 以及代理A1发展的代理A11都具有控制权限
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="mgrkf"></param>
        /// <returns></returns>
        public static bool RightAgentParent(this Manager mgr, Manager submgr)
        {
            if (submgr == null) return false;
            if (!mgr.Domain.IsInDomain(submgr)) return false;//不在同一域

            Manager agent = submgr.BaseManager;//获得主域代理

            if (agent == null) return false;
            while (!agent.RightRootDomain())//只要不是Root域 则进行递归
            {
                if (agent.parent_fk == mgr.BaseMgrID)//如果该代理的父域和当前Manager的主域一致，则具有管理权限
                    return true;
                agent = agent.ParentManager;//递归到父域
            }
            return false;
        }


        /// <summary>
        /// 检查Manager是否有权限添加Manager
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="managertoadd"></param>
        /// <returns></returns>
        public static bool RightAddManager(this Manager mgr, ManagerSetting managertoadd)
        {
            //超级管理员有权限添加
            if (mgr.RightRootDomain() || mgr.Type == QSEnumManagerType.AGENT)
            {
                return true;
            }
            return false;
        }


        #region 权限验证 抛出异常
        public static void ValidRightRead(this Manager mgr, string accid)
        {
            IAccount account = TLCtxHelper.CmdAccount[accid];
            mgr.ValidRightRead(account);
        }

        public static void ValidRightRead(this Manager mgr, IAccount account)
        {
            if (!mgr.RightAccessAccount(account)) throw new FutsRspError("无权查看该帐户");
        }
        #endregion
    }
}
