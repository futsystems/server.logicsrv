using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public class DBManagerTracker
    {
        ConcurrentDictionary<string, Manager> managermap = new ConcurrentDictionary<string, Manager>();
        ConcurrentDictionary<int, Manager> mgridmap = new ConcurrentDictionary<int, Manager>();

        public DBManagerTracker()
        {
            IList<Manager> mlist = ORM.MManager.SelectManager();

            foreach (Manager m in mlist)
            {
                managermap[m.Login] = m;
                mgridmap[m.ID] = m;

                m.Domain = BasicTracker.DomainTracker[m.domain_id];
            }
            foreach (Manager m in mlist)
            {
                m.BaseManager = this[m.mgr_fk];
                m.ParentManager = this[m.parent_fk];
            }

            Manager sroot = new Manager();
            sroot.AccLimit = 1000;
            sroot.Active = true;
            sroot.AgentLimit = 100;
            sroot.domain_id = 1;
            sroot.ID = -1;
            sroot.Login = "sroot";
            sroot.mgr_fk = 1;
            sroot.parent_fk = 1;
            sroot.domain_id = 1;
            sroot.Type = QSEnumManagerType.ROOT;


            managermap[sroot.Login] = sroot;
            mgridmap[sroot.ID] = sroot;

            sroot.Domain = BasicTracker.DomainTracker[sroot.domain_id];
            sroot.BaseManager = this[sroot.mgr_fk];
            sroot.ParentManager = this[sroot.parent_fk];



            //绑定代理财务账户
            foreach (var mgr in mlist)
            {
                //管理员与代理账户需要绑定代理账户？？
                //if (mgr.Type != QSEnumManagerType.AGENT && mgr.Type != QSEnumManagerType.ROOT) continue;
                //ROOT AGENT需要绑定结算账户
                if (mgr.Type != QSEnumManagerType.STAFF)
                {
                    AgentImpl agent = BasicTracker.AgentTracker[mgr.Login];
                    if (agent == null)
                    {
                        //如果没有结算账户则为该管理域创建结算账户
                        AgentSetting setting = new AgentSetting();
                        setting.Account = mgr.Login;
                        setting.AgentType = EnumAgentType.SelfOperated;
                        setting.Currency = GlobalConfig.BaseCurrency;
                        BasicTracker.AgentTracker.UpdateAgent(setting);

                        agent = BasicTracker.AgentTracker[setting.ID];

                    }
                    agent.BindManager(mgr);
                }
                

                //绑定管理员权限
                mgr.Permission = BasicTracker.UIAccessTracker.GetPermission(mgr);
            }
        }


        /// <summary>
        /// 获得Root全局ID
        /// </summary>
        /// <returns></returns>
        public int GetRootFK()
        {
            foreach (Manager m in mgridmap.Values)
            {
                if (m.Type == QSEnumManagerType.ROOT)
                    return m.ID;
            }
            return 0;
        }

        /// <summary>
        /// 删除某个管理员
        /// </summary>
        /// <param name="mgr"></param>
        public void DeleteManager(ManagerSetting mgr)
        { 
            /*
            Manager target = null;
            if (mgridmap.TryGetValue(mgr.ID, out target))
            {
                ORM.MManager.DeleteManager(mgr.ID);
                mgridmap.TryRemove(mgr.ID, out target);
                if (target != null)
                {
                    managermap.TryRemove(mgr.Login, out target);
                }
            }**/
            Manager target = null;
            if (mgridmap.TryGetValue(mgr.ID, out target))
            {
                mgr.Deleted = true;
                mgr.DeletedSettleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                ORM.MManager.MarkManagerDeleted(mgr.ID);
            }
        }
        public void UpdateManager(ManagerSetting mgr)
        {
            Manager target = null;
            //添加
            if (!mgridmap.TryGetValue(mgr.ID, out target))
            {
                target = new Manager();
                target.AccLimit = mgr.AccLimit;
                target.Login = mgr.Login;
                target.Type = mgr.Type;
                target.User_Id = mgr.User_Id;
                target.Active = mgr.Active;
                target.AgentLimit = mgr.AgentLimit;

                target.mgr_fk = mgr.mgr_fk;
                target.domain_id = mgr.domain_id;
                target.parent_fk = mgr.parent_fk;

                ORM.MManager.InsertManager(target);
                mgr.ID = target.ID;

                //添加到数据结构
                managermap[target.Login] = target;
                mgridmap[target.ID] = target;

                //绑定BaseManger和ParentManager
                target.BaseManager = this[target.mgr_fk];
                target.ParentManager = this[target.parent_fk];
                //绑定域
                target.Domain = BasicTracker.DomainTracker[target.domain_id];
            }
            else//更新
            {
                target.AccLimit = mgr.AccLimit;
                target.AgentLimit = mgr.AgentLimit;
                ORM.MManager.UpdateManager(target);
            }
        }

        /// <summary>
        /// 获得所有基础主域
        /// 管理员的基础域ID和他的ID相同 则该Manager为主域Manager
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Manager> GetBaseManagers()
        {
            return managermap.Values.Where(m => m.mgr_fk == m.ID);
        }

        /// <summary>
        /// 所有管理员
        /// </summary>
        public IEnumerable<Manager> Managers
        {
            get
            {
                return managermap.Values;
            }
        }
        ///// <summary>
        ///// 查询某个管理员可以查询的管理员列表
        ///// </summary>
        ///// <param name="mgr"></param>
        ///// <returns></returns>
        //public IEnumerable<Manager> GetManagers(Manager mgr)
        //{
        //    if (mgr.Type == QSEnumManagerType.SUPERROOT)
        //    {
        //        return managermap.Values;
        //    }
        //    else if (mgr.Type == QSEnumManagerType.ROOT)
        //    {
        //        return managermap.Values.Where(m=>m.domain_id == mgr.domain_id);
        //    }
        //    else
        //    { 
        //        //如果是代理 返回所有属于该代理的所有柜员
        //        if(mgr.Type == QSEnumManagerType.AGENT)
        //        {
        //            //如果Manager的mgr_fk等于该代理的ID则返回 或则该Manger的下属一级子代理 如果是员工登入则只显示员工帐户
        //            return managermap.Values.Where(m => m.mgr_fk.Equals(mgr.ID)||m.parent_fk.Equals(mgr.ID));
        //        }
        //        else
        //        {
        //            return new Manager[]{mgr};
        //        }
                
        //    }
        //    //return new List<Manager>();
        //}

        /// <summary>
        /// 更新管理员密码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pass"></param>
        public void UpdatePassword(int id, string pass)
        {
            Manager m = this[id];
            if (m != null)
            {
                ORM.MManager.UpdateManagerPass(id, pass);
            }
        }

        /// <summary>
        /// 更新管理员类型
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        public void UpdateManagerType(int id, QSEnumManagerType type)
        { 
             Manager m = this[id];
             if (m != null)
             {
                 ORM.MManager.UpdateManagerType(id, type);
             }
            
        }

        /// <summary>
        /// 通过数据库ID返回对应的Manager
        /// </summary>
        /// <param name="mgrid"></param>
        /// <returns></returns>
        public Manager this[int mgrid]
        {
            get
            {
                Manager m = null;
                if (mgridmap.TryGetValue(mgrid, out m))
                {
                    return m;
                }
                return null;
            }
        }

        /// <summary>
        /// 通过login返回对应的Manager
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public Manager this[string login]
        {
            get
            { 
                Manager m = null;
                if (managermap.TryGetValue(login, out m))
                {
                    return m;
                }
                return null;
            }
        }
    }
}
