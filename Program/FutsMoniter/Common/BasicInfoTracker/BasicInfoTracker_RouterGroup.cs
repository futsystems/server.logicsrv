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
        public IEnumerable<RouterGroupSetting> RouterGroups
        {
            get
            {
                return rgmap.Values;
            }
        }

        public RouterGroupSetting GetRouterGroup(int rgid)
        {
            if (rgmap.Keys.Contains(rgid))
                return rgmap[rgid];
            return null;
        }
        Dictionary<int, RouterGroupSetting> rgmap = new Dictionary<int, RouterGroupSetting>();

        public void GotRouterGroup(RouterGroupSetting rg)
        {
            //Globals.Debug("got routergroup setting .....");
            if (rg == null) return;
            RouterGroupSetting target = null;
            
            if (rgmap.TryGetValue(rg.ID, out target))
            {
                //已经存在RouterGroup
                target.Description = rg.Description;
                target.Name = rg.Name;
                target.Strategy = rg.Strategy;
            }
            else
            {
                rgmap.Add(rg.ID, rg);
            }

            //对外触发获得路由组事件
            if (_firstloadfinish && GotRouterGroupEvent != null)
            {
                GotRouterGroupEvent(target!=null?target:rg);
            }
        }
    }
}
