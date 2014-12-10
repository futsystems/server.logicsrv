//using System;
//using System.Collections.Generic;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Text;
//using System.Reflection;
//using TradingLib.API;
//using TradingLib.Common;
//using TradingLib.Mixins.JsonObject;
//using TradingLib.ORM;


//namespace TradingLib.Core
//{
//    /// <summary>
//    /// 界面访问权限管理
//    /// </summary>
//    public class UIAccessTracker
//    {
//        /// <summary>
//        /// 数据库ID与对应的权限设置影射
//        /// </summary>
//        ConcurrentDictionary<int, UIAccess> uiaccessmap = new ConcurrentDictionary<int, UIAccess>();
//        /// <summary>
//        /// ManagerID与对应的权限ID的影射
//        /// </summary>
//        ConcurrentDictionary<int, int> manageruiidxmap = new ConcurrentDictionary<int, int>();

//        static ConfigDB _cfgdb = null;
//        static UIAccessTracker defalutinstance = null;
//        static List<string> _excludePermissionForAgent = new List<string>();
//        static UIAccessTracker()
//        {
//            defalutinstance = new UIAccessTracker();
//            _cfgdb = new ConfigDB("UIAccessTracker");
//            if (!_cfgdb.HaveConfig("Comment"))
//            {
//                _cfgdb.UpdateConfig("ExcludePermissionForAgent", QSEnumCfgType.String, "fm_debug,nav_system,nav_basic,nav_manager,nav_manager_permissionagent,nav_finance_cashercentre,moniter_router,moniter_acctype,moniter_tab_finance,moniter_menu_delaccount,fun_info_operation,fun_tab_placeorder,moniter_tab_config_inactive,acctype_sim,acctype_dealer", "代理默认排除的权限列表");
//            }
//            foreach (string s in _cfgdb["ExcludePermissionForAgent"].AsString().Split(','))
//            {
//                _excludePermissionForAgent.Add(s);
//            }
//        }

//        private UIAccessTracker()
//        { 
//            //加载访问权限对象到内存
//            foreach(UIAccess  a in ORM.MUIAccess.SelectUIAccess())
//            {
//                uiaccessmap.TryAdd(a.id, a);
//            }

//            foreach (Manager2UIACcess access in ORM.MUIAccess.SelectManager2UIAccess())
//            {
//                manageruiidxmap.TryAdd(access.manager_id, access.access_id);
//            }
//        }

//        /// <summary>
//        /// 获得某个代理的UIAccess
//        /// </summary>
//        /// <param name="managerid"></param>
//        /// <returns></returns>
//        public static UIAccess GetAgentUIAccess(int managerid)
//        {
//            if (defalutinstance.manageruiidxmap.Keys.Contains(managerid))
//            {
//                return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[managerid]];
//            }
//            return null;
//        }

//        /// <summary>
//        /// 更新某个Manager的权限设置
//        /// </summary>
//        /// <param name="managerid"></param>
//        /// <param name="accessid"></param>
//        public static void UpdateAgentPermission(int managerid, int accessid)
//        {
//            if (!defalutinstance.uiaccessmap.Keys.Contains(accessid))
//            {
//                throw new FutsRspError("指定权限不存在");
//            }
//            if (defalutinstance.manageruiidxmap.Keys.Contains(managerid))
//            {
//                //更新
//                defalutinstance.manageruiidxmap[managerid] = accessid;
//                ORM.MUIAccess.UpdateManagerPermissionSet(managerid, accessid);
//            }
//            //新增
//            defalutinstance.manageruiidxmap[managerid] = accessid;
//            ORM.MUIAccess.InsertManagerPermissionSet(managerid, accessid);
//        }

//        /// <summary>
//        /// 获得某个管理员的UIAccess的优先顺序
//        /// 1.如果有指定的权限则使用该权限
//        /// 2.如果指定则使用主域权限
//        /// 3.主域没有指定则使用上级代理的权限
//        /// 4.直接到ROOT则使用默认权限
//        /// </summary>
//        /// <param name="manager"></param>
//        /// <returns></returns>
//        public static UIAccess GetUIAccess(Manager manager)
//        {
//            //1.如果直接指定了权限则使用该权限
//            if (defalutinstance.manageruiidxmap.Keys.Contains(manager.ID))
//            {
//                return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[manager.ID]];
//            }

//            //2.如果没有指定使用主域界面权限
//            Manager basemanager = manager.BaseManager;
//            if (defalutinstance.manageruiidxmap.Keys.Contains(basemanager.ID))
//            {
//                return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[basemanager.ID]];
//            }


//            //3.如果主域没有对应的权限则查找其父代理
//            Manager agent = basemanager.ParentManager;//获得主域代理
//            while (!agent.RightRootDomain())//如果父代理不是Root域 则进行递归
//            {
//                if (defalutinstance.manageruiidxmap.Keys.Contains(agent.ID))
//                {
//                    return defalutinstance.uiaccessmap[defalutinstance.manageruiidxmap[agent.ID]];
//                }
//                agent = agent.ParentManager;//递归到父域
//            }
//            Util.Debug(manager.ToString() + " have no permission set,use default",QSEnumDebugLevel.WARNING);

//            if (manager.RightRootDomain())//如果是Root权限 则返回默认管理员权限 所有权限打开
//            {
//                return GetDefaultRootAccess();
//            }
//            else
//            {
//                return GetDefaultAgentAccess();//其余代理 返回默认代理商的权限
//            }
           
//        }


//        static UIAccess GetDefaultRootAccess()
//        {
//            PropertyInfo[] propertyInfos = typeof(UIAccess).GetProperties();
//            UIAccess access = new UIAccess();
//            for (int i = 0; i < propertyInfos.Length; i++)
//            {
//                PropertyInfo pi = propertyInfos[i];
//                if (pi.Name.Equals("id"))
//                    continue;
//                if (pi.Name.Equals("name"))
//                    continue;
//                if (pi.Name.Equals("desp"))
//                    continue;
//                pi.SetValue(access,true,null);
                
//            }
//            return access;
//        }

//        static UIAccess GetDefaultAgentAccess()
//        {
//            PropertyInfo[] propertyInfos = typeof(UIAccess).GetProperties();
//            UIAccess access = new UIAccess();
//            for (int i = 0; i < propertyInfos.Length; i++)
//            {
//                PropertyInfo pi = propertyInfos[i];
//                if (pi.Name.Equals("id"))
//                    continue;
//                if (pi.Name.Equals("name"))
//                    continue;
//                if (pi.Name.Equals("desp"))
//                    continue;
//                if (_excludePermissionForAgent.Contains(pi.Name))
//                {
//                    pi.SetValue(access, false, null);
//                }
//                else
//                {
//                    pi.SetValue(access, true, null);
//                }
//            }
//            return access;
//        }

//        /// <summary>
//        /// 获得所有界面访问权限
//        /// </summary>
//        /// <returns></returns>
//        public static IEnumerable<UIAccess> GetUIAccessList()
//        {
//            return defalutinstance.uiaccessmap.Values;
//        }

//        /// <summary>
//        /// 更新某个权限或者新增某个权限
//        /// </summary>
//        /// <param name="access"></param>
//        public static void UpdateUIAccess(UIAccess access)
//        {
//            if (defalutinstance.uiaccessmap.Keys.Contains(access.id))
//            {
//                defalutinstance.uiaccessmap[access.id] = access;
//                ORM.MUIAccess.UpdateUIAccess(access);
//            }
//            else
//            {
//                ORM.MUIAccess.InsertUIAccess(access);
//                defalutinstance.uiaccessmap[access.id] = access;
//            }
//        }

//    }

//}
