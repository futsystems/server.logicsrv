using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public static partial class MangerUtils
    {
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
        /// 返回某个交易帐户的主域FK
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public static int GetBaseMGR(this Manager mgr)
        {
            return mgr.mgr_fk;
        }

        
        /// <summary>
        /// 是否可以访问某个交易帐户
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool RightAccessAccount(this Manager mgr, IAccount account)
        {
            if (mgr.RightRootDomain()) return true;
            //如果交易帐户直接属于该Manager的域 则有权限
            if (mgr.GetBaseMGR().Equals(account.Mgr_fk))
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

        /// <summary>
        /// 判断mgrfk是否是Manager的子代理或者子子xx代理，代理A对发展的代理A1 以及代理A1发展的代理A11都具有控制权限
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="mgrkf"></param>
        /// <returns></returns>
        public static bool RightAgentParent(this Manager mgr, int mgrfk)
        {
            Manager manager = BasicTracker.ManagerTracker[mgrfk];
            Manager agent = manager.BaseManager;//获得主域代理

            if (agent == null) return false;
            while (!agent.RightRootDomain())//只要不是Root域 则进行递归
            {
                if (agent.parent_fk == mgr.GetBaseMGR())//如果该代理的父域和当前Manager的主域一致，则具有管理权限
                    return true;
                agent = agent.ParentManager;//递归到父域
            }
            return false;
        }

    }
}
