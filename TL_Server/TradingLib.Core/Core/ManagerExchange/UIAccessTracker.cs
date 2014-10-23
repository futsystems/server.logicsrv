using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;
using TradingLib.ORM;


namespace TradingLib.Core
{
    /// <summary>
    /// 界面访问权限管理
    /// </summary>
    public class UIAccessTracker
    {

        ConcurrentDictionary<int, UIAccess> uiaccessmap = new ConcurrentDictionary<int, UIAccess>();
        ConcurrentDictionary<int, int> manageruiidxmap = new ConcurrentDictionary<int, int>();

        static UIAccessTracker defalutinstance = null;
        static UIAccessTracker()
        {
            defalutinstance = new UIAccessTracker();
        }

        private UIAccessTracker()
        { 
            //加载访问权限对象到内存
            foreach(UIAccess  a in ORM.MUIAccess.SelectUIAccess())
            {
                uiaccessmap.TryAdd(a.ID, a);
            }

            foreach (Manager2UIACcess access in ORM.MUIAccess.SelectManager2UIAccess())
            {
                manageruiidxmap.TryAdd(access.manager_id, access.access_id);
            }
        }

        /// <summary>
        /// 获得某个管理员的UIAccess
        /// 1.如果有指定的权限则使用该权限
        /// 2.如果指定则使用主域权限
        /// 3.主域没有指定则使用上级代理的权限
        /// 4.直接到ROOT则使用默认权限
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public static UIAccess GetUIAccess(Manager manager)
        {
            //1.如果直接指定了权限则使用该权限
            if (defalutinstance.manageruiidxmap.Keys.Contains(manager.ID))
            {
                return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[manager.ID]];
            }

            //2.如果没有指定使用主域界面权限
            manager = manager.BaseManager;
            if (defalutinstance.manageruiidxmap.Keys.Contains(manager.ID))
            {
                return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[manager.ID]];
            }


            //3.如果主域没有对应的权限则查找其父代理
            Manager agent = manager.ParentManager;//获得主域代理

            if (agent == null) return null;//没有父代理返回默认权限


            while (!agent.RightRootDomain())//如果父代理不是Root域 则进行递归
            {
                if (defalutinstance.manageruiidxmap.Keys.Contains(agent.ID))
                {
                    return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[agent.ID]];
                }
                agent = agent.ParentManager;//递归到父域
            }
            return null;//递归到父代理 返回默认权限
        }

        /// <summary>
        /// 获得所有界面访问权限
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UIAccess> GetUIAccessList()
        {
            return defalutinstance.uiaccessmap.Values;
        }


    }

}
