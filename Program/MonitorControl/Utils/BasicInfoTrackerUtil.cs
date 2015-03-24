using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.MoniterControl
{
    public static class BasicInfoTrackerUtil
    {

        /// <summary>
        /// 返回manger选择项
        /// 用于创建用户
        /// </summary>
        /// <returns></returns>
        public static ArrayList GetBaseManagerCombList(this IBasicInfoTracker info, bool all = false, bool includeself = true)
        {
            ArrayList list = new ArrayList();

            if (all)
            {
                list.Add(new ValueObject<int> { Name = "<Any>", Value = 0 });
            }
            //从柜员列表中获得超级管理员域或代理域
            foreach (ManagerSetting m in info.Managers.Where(g => (g.Type == QSEnumManagerType.ROOT || g.Type == QSEnumManagerType.AGENT)))
            {
                if (!includeself && m.mgr_fk == MoniterHelper.CTX.Manager.mgr_fk)
                {
                    continue;
                }
                ValueObject<int> vo1 = new ValueObject<int>();
                vo1.Name = m.Name + " - " + m.mgr_fk;
                vo1.Value = m.mgr_fk;
                list.Add(vo1);

            }
            return list;
        }
    }
}
