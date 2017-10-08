using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using Common.Logging;

namespace TradingLib.Common
{

    public class DBManagerTracker
    {
        ILog logger = LogManager.GetLogger("DBManagerTracker");
        ConcurrentDictionary<string, Manager> managermap = new ConcurrentDictionary<string, Manager>();
        ConcurrentDictionary<int, Manager> mgridmap = new ConcurrentDictionary<int, Manager>();

        ConcurrentDictionary<string, ManagerProfile> profilemap = new ConcurrentDictionary<string, ManagerProfile>();

        public DBManagerTracker()
        {



            IList<Manager> mlist = ORM.MManager.SelectManager();
            foreach (Manager m in mlist)
            {
                if (m.Deleted && TLCtxHelper.ModuleSettleCentre.Tradingday > m.DeletedSettleday)
                {
                    //逻辑删除 不处理删除账户数据
                    if (GlobalConfig.LogicDelete)
                    {

                    }
                    else //物理删除
                    {
                        //删除数据库
                        ORM.MManager.DeleteManager(m);
                    }
                    continue;
                }

                m.Domain = BasicTracker.DomainTracker[m.domain_id];
                managermap[m.Login] = m;
                mgridmap[m.ID] = m;
            }
            List<Manager> errorList = new List<Manager>();
            foreach (Manager m in managermap.Values)
            {
                m.BaseManager = this[m.mgr_fk];
                m.ParentManager = this[m.parent_fk];

                if (m.BaseManager == null||m.ParentManager == null)//删除不完备导致 有代理相关数据为空
                {
                    errorList.Add(m);
                }
            }
            logger.Warn("Error Manager:" + string.Join(",", errorList.Select(m => m.Login).ToArray()));
            foreach (var m in errorList)
            { 
                Manager target;
                managermap.TryRemove(m.Login, out target);
                mgridmap.TryRemove(m.ID, out target);
            }


            //ManagerProfile
            foreach (var p in ORM.MManagerProfile.SelectManagerProfile())
            {
                if (this[p.Account] == null) //如果对应的Manager不存在 则不加载该Profile
                    continue;
                profilemap[p.Account] = p;
            }

             //绑定Manager相关对象
            foreach (var mgr in managermap.Values)
            {
                //绑定管理员Profile
                mgr.Profile = this.GetProfile(mgr.Login);
            }

            //绑定Manager相关对象
            foreach (var mgr in managermap.Values)
            {
                //管理员与代理账户需要绑定结算账户
                if (mgr.Type != QSEnumManagerType.STAFF)
                {
                    AgentImpl agent = BasicTracker.AgentTracker[mgr.Login];
                    if (agent == null)
                    {
                        //如果没有结算账户则为该管理域创建结算账户
                        AgentSetting setting = new AgentSetting();
                        setting.Account = mgr.Login;
                        setting.AgentType = mgr.Type == QSEnumManagerType.ROOT ? EnumAgentType.SelfOperated : EnumAgentType.Normal;
                        setting.Currency = GlobalConfig.BaseCurrency;
                        BasicTracker.AgentTracker.UpdateAgent(setting);//绑定Agent时 会引用ManagerTracker形成循环
                        agent = BasicTracker.AgentTracker[setting.ID];
                    }
                    agent.BindManager(mgr);
                }

                //绑定管理员权限
                mgr.Permission = BasicTracker.UIAccessTracker.GetPermission(mgr);

                ////绑定管理员Profile
                //mgr.Profile = this.GetProfile(mgr.Login);
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
        public void DeleteManager(ManagerSetting mgr,bool direct)
        {
            if (direct)
            {
                Manager target = null;
                if (mgridmap.TryGetValue(mgr.ID, out target))
                {
                    mgr.Deleted = true;
                    ORM.MManager.DeleteManager(mgr);
                    mgridmap.TryRemove(mgr.ID, out target);
                    if (target != null)
                    {
                        managermap.TryRemove(mgr.Login, out target);
                    }
                }
            }
            else
            {
                Manager target = null;
                if (mgridmap.TryGetValue(mgr.ID, out target))
                {
                    mgr.Deleted = true;
                    mgr.DeletedSettleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
                    ORM.MManager.MarkManagerDeleted(mgr.ID);
                }
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

                //更新Profile
                this.UpdateManagerProfile(mgr.Profile);

                //添加到数据结构
                managermap[target.Login] = target;
                mgridmap[target.ID] = target;

                //绑定BaseManger和ParentManager
                target.BaseManager = this[target.mgr_fk];
                target.ParentManager = this[target.parent_fk];
                //绑定域
                target.Domain = BasicTracker.DomainTracker[target.domain_id];

                //绑定Profile
                target.Profile = this.GetProfile(target.Login);
            }
            else//更新
            {
                target.AccLimit = mgr.AccLimit;
                target.AgentLimit = mgr.AgentLimit;
                ORM.MManager.UpdateManager(target);

                //更新Profile
                this.UpdateManagerProfile(mgr.Profile);
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


        #region ManagerProfile
        /// <summary>
        /// 获得某个交易帐户的Profile
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ManagerProfile GetProfile(string account)
        {
            if (string.IsNullOrEmpty(account)) return null;
            ManagerProfile target = null;
            if (this.profilemap.TryGetValue(account, out target))
            {
                return target;
            }
            else
            {
                //如果不存在对应交易账户的profile信息 这里生成对应的默认profile加入 这样通过交易账号获得profile对象 均不可能为空，生成结算单时 避免了profile为空时的异常
                ManagerProfile profile = new ManagerProfile();
                profile.Account = account;
                UpdateManagerProfile(profile);
                return profilemap[account];
            }
        }


        /// <summary>
        /// 更新某个交易帐户的个人信息
        /// </summary>
        /// <param name="profile"></param>
        void UpdateManagerProfile(ManagerProfile profile)
        {
            ManagerProfile target = null;

            //已经存在
            if (this.profilemap.TryGetValue(profile.Account, out target))
            {
                ORM.MManagerProfile.UpdateManagerProfile(profile);
                target.Bank_ID = profile.Bank_ID;
                target.BankAC = profile.BankAC;
                target.Branch = profile.Branch;
                target.Email = profile.Email;
                target.IDCard = profile.IDCard;
                target.Mobile = profile.Mobile;
                target.Name = profile.Name;
                target.QQ = profile.QQ;
                target.Memo = profile.Memo;

            }
            else
            {
                ORM.MManagerProfile.InsertManagerProfile(profile);
                profilemap.TryAdd(profile.Account, profile);
            }
        }
        #endregion

    }
}
