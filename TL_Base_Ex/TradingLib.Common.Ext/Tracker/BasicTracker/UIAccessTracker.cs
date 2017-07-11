using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using Common.Logging;

namespace TradingLib.Common
{
    public class UIAccessTracker
    {
        ILog logger = LogManager.GetLogger("AccessTracker");
        /// <summary>
        /// 数据库ID与对应的权限设置影射
        /// </summary>
        ConcurrentDictionary<int, Permission> permissionMap = new ConcurrentDictionary<int, Permission>();

        ConcurrentDictionary<string, string> permissionTitleMap = new ConcurrentDictionary<string, string>();

        ConfigDB _cfgdb = null;
        List<string> _excludePermissionForAgent = new List<string>();

        public UIAccessTracker()
        {
            _cfgdb = new ConfigDB("UIAccessTracker");
            if (!_cfgdb.HaveConfig("ExcludePermissionForAgent"))
            {
                _cfgdb.UpdateConfig("ExcludePermissionForAgent", QSEnumCfgType.String, "r_account_del,r_cashop,r_commission,r_margin,r_exstrategy", "代理默认排除的权限列表");
            }
            foreach (string s in _cfgdb["ExcludePermissionForAgent"].AsString().Split(','))
            {
                _excludePermissionForAgent.Add(s);
            }

            //加载访问权限对象到内存
            foreach (Permission a in ORM.MPermission.SelectUIAccess())
            {
                permissionMap.TryAdd(a.id, a);
            }


            List<PropertyInfo> list = new List<PropertyInfo>();
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            foreach (PropertyInfo pi in propertyInfos)
            {
                PermissionFieldAttr attr = (PermissionFieldAttr)Attribute.GetCustomAttribute(pi, typeof(PermissionFieldAttr));
                if (attr != null)
                {
                    permissionTitleMap.TryAdd(pi.Name, attr.Title);
                }
            }
        }

        /// <summary>
        /// 获取权限Title
        /// 权限对象通过PermissionFieldAttr进行标注
        /// </summary>
        /// <param name="filed"></param>
        /// <returns></returns>
        public string GetPermissionTitle(string filed)
        {
            string target = string.Empty;
            if (permissionTitleMap.TryGetValue(filed, out target))
            {
                return target;
            }
            return "权限不存在";
        }


        public Permission this[int id]
        {
            get
            {
                Permission access = null;
                if (permissionMap.TryGetValue(id, out access))
                {
                    return access;
                }
                return null;
            }
        }

        /// <summary>
        /// 获得某个管理员的UIAccess的优先顺序
        /// 1.如果有指定的权限则使用该权限
        /// 2.如果指定则使用主域权限
        /// 3.主域没有指定则使用上级代理的权限
        /// 4.直接到ROOT则使用默认权限
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        internal Permission GetPermission(Manager manager)
        {
            //1.Root返回全部权限
            if (manager.IsRoot())
            {
                return GetDefaultRootPermission();
            }

            Permission target = null;
            //2.如果直接指定了权限则使用该权限
            if (permissionMap.TryGetValue(manager.Permission_ID,out target))
            {
                return target;
            }

            
            //3.员工返回默认员工权限(无任何权限只能观察)
            if (manager.IsStaff())
            {
                return GetDefaultStaffPermission();
            }

            /*
            //2.如果没有指定使用主域界面权限
            //Manager basemanager = manager.BaseManager;
            //if (manageruiidxmap.Keys.Contains(basemanager.ID))
            //{
            //    access = uiaccessmap[manageruiidxmap[basemanager.ID]];
            //    if (access != null)
            //        return access;
            //}

            **/

            
            //4.如果代理没有指定权限则查找其父代理 继承父代理的权限
            Manager agent = manager.ParentManager;
            while (!agent.IsInRoot())//如果父代理不是Root域 则进行递归
            {
                if (permissionMap.TryGetValue(manager.Permission_ID, out target))
                {
                    return target;
                }
                agent = agent.ParentManager;//递归到父域
            }
            logger.Warn(manager.ToString() + " have no permission set,use default");
            return GetDefaultAgentPermission();//其余代理 返回默认代理商的权限
        }


        #region 默认权限
        /// <summary>
        /// 获得默认Root权限
        /// 所有权限都打开
        /// </summary>
        /// <returns></returns>
        Permission GetDefaultRootPermission()
        {
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            Permission access = new Permission();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo pi = propertyInfos[i];
                if (pi.Name.Equals("id"))
                    continue;
                if (pi.Name.Equals("name"))
                    continue;
                if (pi.Name.Equals("domain_id"))
                    continue;
                if (pi.Name.Equals("desp"))
                    continue;
                if (pi.Name.Equals("manager_id"))
                    continue;
                pi.SetValue(access, true, null);

            }
            return access;
        }

        /// <summary>
        /// Staff默认没有任何权限
        /// </summary>
        /// <returns></returns>
        Permission GetDefaultStaffPermission()
        {
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            Permission access = new Permission();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo pi = propertyInfos[i];
                if (pi.Name.Equals("id"))
                    continue;
                if (pi.Name.Equals("name"))
                    continue;
                if (pi.Name.Equals("domain_id"))
                    continue;
                if (pi.Name.Equals("desp"))
                    continue;
                if (pi.Name.Equals("manager_id"))
                    continue;
                pi.SetValue(access, false, null);

            }
            return access;
        }



        /// <summary>
        /// 获得默认权限列表
        /// </summary>
        /// <returns></returns>
        Permission GetDefaultAgentPermission()
        {
            PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
            Permission access = new Permission();
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                PropertyInfo pi = propertyInfos[i];
                if (pi.Name.Equals("id"))
                    continue;
                if (pi.Name.Equals("name"))
                    continue;
                if (pi.Name.Equals("domain_id"))
                    continue;
                if (pi.Name.Equals("desp"))
                    continue;
                if (pi.Name.Equals("manager_id"))
                    continue;
                if (_excludePermissionForAgent.Contains(pi.Name))
                {
                    pi.SetValue(access, false, null);
                }
                else
                {
                    pi.SetValue(access, true, null);
                }
            }
            return access;
        }
        #endregion



        /// <summary>
        /// 更新某个Manager的权限设置
        /// </summary>
        /// <param name="managerid"></param>
        /// <param name="accessid"></param>
        public  void UpdateManagerPermission(Manager mgr, int permission_id)
        {
            mgr.Permission_ID = permission_id;
            mgr.Permission = GetPermission(mgr);

            ORM.MManager.UpdateManagerPermission(mgr.ID, permission_id);
        }

        /// <summary>
        /// 获得所有界面访问权限
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Permission> Permissions
        {
            get
            {
                return permissionMap.Values;
            }
        }

        /// <summary>
        /// 删除权限模板
        /// </summary>
        /// <param name="access"></param>
        public void DeletePermissionTemplate(int template_id)
        {
            Permission target = null;
            if (permissionMap.TryGetValue(template_id, out target))
            {
                permissionMap.TryRemove(template_id, out target);
                ORM.MPermission.DeletePermissionTemplate(template_id);

                if (target != null)
                {
                    var items = BasicTracker.ManagerTracker.Managers.Where(mgr => mgr.Permission_ID == template_id).ToArray();
                    foreach (var item in items)
                    {
                        this.UpdateManagerPermission(item, 0);
                    }
                }
            }
        
        }


        /// <summary>
        /// 更新权限对象
        /// </summary>
        /// <param name="access"></param>
        public void UpdatePermission(Permission access)
        {
            Permission target = null;
            if (permissionMap.TryGetValue(access.id, out target))
            {
                //遍历所遇属性并传递值
                PropertyInfo[] propertyInfos = typeof(Permission).GetProperties();
                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    PropertyInfo pi = propertyInfos[i];
                    if (pi.Name.Equals("id"))
                        continue;
                    object val = pi.GetValue(access,null);
                    pi.SetValue(target, val, null);

                }
                ORM.MPermission.UpdatePermissionTemplate(target);
            }
            else
            {
                target = access;
                ORM.MPermission.InsertPermissionTemplate(target);
                permissionMap[target.id] = target;
            }
        }

    }
}
