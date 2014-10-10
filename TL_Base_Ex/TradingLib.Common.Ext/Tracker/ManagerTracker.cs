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
            }
            foreach (Manager m in mlist)
            { 
                mgridmap[m.ID] = m;
            }
            foreach (Manager m in mlist)
            {
                m.BaseManager = this[m.mgr_fk];
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

        public void UpdateManager(Manager mgr)
        {
            //添加
            if (mgr.ID == 0)
            {
                ORM.MManager.InsertManager(mgr);
                managermap[mgr.Login] = mgr;
                mgridmap[mgr.ID] = mgr;
                mgr.BaseManager = this[mgr.mgr_fk];
            }
            else//更新
            {
                Manager target = null;
                if (mgridmap.TryGetValue(mgr.ID, out target))
                {
                    target.Type = mgr.Type;
                    target.Name = mgr.Name;
                    target.Mobile = mgr.Mobile;
                    target.QQ = mgr.QQ;
                    target.AccLimit = mgr.AccLimit;
                    ORM.MManager.UpdateManager(target);
                }
            }
        }


        /// <summary>
        /// 查询某个管理员可以查询的管理员列表
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <returns></returns>
        public IEnumerable<Manager> GetManagers(Manager mgr)
        {
            if (mgr.Type == QSEnumManagerType.ROOT)
            {
                return managermap.Values;
            }
            else
            { 
                //如果是代理 返回所有属于该代理的所有柜员
                if(mgr.Type == QSEnumManagerType.AGENT)
                {
                    //如果Manager的mgr_fk等于该代理的ID则返回
                    return managermap.Values.Where(m => m.mgr_fk.Equals(mgr.ID));
                }
                else
                {
                    return new Manager[]{mgr};
                }
                
            }
            //return new List<Manager>();
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
    }
}
