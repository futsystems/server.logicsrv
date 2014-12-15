using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutsMoniter;

namespace TradingLib.Common
{
    public partial class BasicInfoTracker
    {

        public void GotManager(Manager manager)
        {
            Manager target = null;
            Manager notify = null;
            //如果本地已经有该Manager则进行信息更新
            if (managermap.TryGetValue(manager.ID, out target))
            {
                target.Mobile = manager.Mobile;
                target.Name = manager.Name;
                target.QQ = manager.QQ;
                target.Active = manager.Active;
                notify = target;
            }
            else//否则添加该Manager
            {
                managermap.Add(manager.ID, manager);
                notify = manager;
            }

            //将获得的柜员列表中 属于本登入mgr_fk的manager绑定到全局对象
            if (Globals.MGRID == manager.ID)
            {
                Globals.Manager = manager;
            }
            //对外触发 初始化过程中不对外出发
            if (_firstloadfinish && GotManagerEvent != null)
            {
                GotManagerEvent(notify);
            }

        }

        /// <summary>
        /// 主管理员map
        /// </summary>
        Dictionary<int, Manager> managermap = new Dictionary<int, Manager>();

        #region 管理员
        /// <summary>
        /// 管理员
        /// </summary>
        public IEnumerable<Manager> Managers
        {
            get
            {
                return managermap.Values;
            }
        }


        public Manager GetManager(int mgrid)
        {
            Manager mgr = null;
            if (managermap.TryGetValue(mgrid, out mgr))
            {
                return mgr;
            }
            return null;
        }
        #endregion

    }
}
