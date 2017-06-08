using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static partial class ManagerUtils
    {
        #region 基础权限判断
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
        /// 判断Manager是否在Root管理域
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool IsInRoot(this Manager mgr)
        {
            if (mgr.Type == QSEnumManagerType.ROOT || mgr.BaseManager.Type == QSEnumManagerType.ROOT)
            {
                return true;
            }
            return false;
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
        /// 判断管理员是否在Agent管理域
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static bool IsInAgent(this Manager mgr)
        {
            if (mgr.Type == QSEnumManagerType.AGENT || mgr.BaseManager.Type == QSEnumManagerType.AGENT)
            {
                return true;
            }
            return false;
        }


        
        /// <summary>
        /// 是否是某个mgrfk管理员的父管理员(包含多级父管理员)
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="mgrfk"></param>
        /// <returns></returns>
        public static bool IsParentOf(this Manager mgr, int mgrfk)
        {
            Manager manager = BasicTracker.ManagerTracker[mgrfk];
            return mgr.IsParentOf(manager);
        }


        /// <summary>
        /// 判断mgrfk是否是Manager的子代理或者子子xx代理，代理A对发展的代理A1 以及代理A1发展的代理A11都具有控制权限
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="mgrkf"></param>
        /// <returns></returns>
        public static bool IsParentOf(this Manager mgr, Manager submgr)
        {
            if (submgr == null) return false;
            if (!mgr.Domain.IsInDomain(submgr)) return false;//不在同一域

            Manager agent = submgr.BaseManager;//获得主域代理

            if (agent == null) return false;
            while (!agent.IsInRoot())//只要不是Root域 则进行递归 递归到Root则不用再进行判断，父子关系只到Root结束
            {
                if (agent.parent_fk == mgr.BaseMgrID)//如果该代理的父域和当前Manager的主域一致，则具有管理权限
                    return true;
                agent = agent.ParentManager;//递归到上级父域代理
            }
            return false;
        }
        #endregion




        public static bool RightAccessManager(this Manager mgr, string agent)
        {
            Manager mgr2 = BasicTracker.ManagerTracker[agent];
            return mgr.RightAccessManager(mgr2);
        }
        /// <summary>
        /// 判断mgr是否有权操作mgr2
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="mgr2"></param>
        /// <returns></returns>
        public static bool RightAccessManager(this Manager mgr, Manager mgr2)
        {
            if (mgr2 == null) return false;//mgr2为空则 mgr无权操作 
            if (!mgr.Domain.IsInDomain(mgr2)) return false;//mgr2与mgr不在同一个域 则无权操作

            if (mgr.IsInRoot()) return true; //如果mgr是管理员则有权操作 域内所有管理员

            //如果当前柜员是代理域成员
            if (mgr.IsInAgent())
            {
                //当前管理域ID与目标管理员管理域ID一致
                if (mgr.BaseMgrID.Equals(mgr2.BaseMgrID))
                    return true;

                //如果mgr是mgr2的父代理 则有权操作
                if (mgr.IsParentOf(mgr2))
                {
                    return true;
                }
            }
            return false;
        }



        /// <summary>
        /// 是否可以访问某个交易帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool RightAccessAccount(this Manager mgr, IAccount account)
        {
            if (account == null) return false;//帐户为空则返回false
            if (!mgr.Domain.IsInDomain(account)) return false;//帐户不再管理员的柜台分区 返回false

            if (mgr.IsInRoot()) return true;

            //如果交易帐户直接属于该Manager的域 则有权限
            if (mgr.BaseMgrID.Equals(account.Mgr_fk))
            {
                return true;
            }
            //如果该管理员是交易帐号所属管理员的父代理则有权查看
            if (mgr.IsParentOf(account.Mgr_fk))
            {
                return true;
            }
            return false;
        }



        
    }
}
