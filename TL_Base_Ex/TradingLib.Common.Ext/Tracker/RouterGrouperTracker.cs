using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// 路由组管理器
    /// </summary>
    public class RouterGrouperTracker
    {
        ConcurrentDictionary<int, RouterGroup> routergroupmap = new ConcurrentDictionary<int, RouterGroup>();
        ConcurrentDictionary<int, RouterConnecterMap> routerconnectormap = new ConcurrentDictionary<int, RouterConnecterMap>();

        public RouterGrouperTracker()
        {
            foreach (RouterGroup rg in ORM.MRouterGroup.SelectRouterGroup())
            {
                routergroupmap.TryAdd(rg.ID, rg);
            }

            foreach (RouterConnecterMap rm in ORM.MRouterGroup.SelectRouterConnectorMap())
            {
                routerconnectormap.TryAdd(rm.ID, rm);
            }
        }

        /// <summary>
        /// 获得某个路由组所有通道映射
        /// </summary>
        /// <param name="rgid"></param>
        /// <returns></returns>
        public IEnumerable<RouterConnecterMap> GetRouterConnectorMap(int rgid)
        {
            return routerconnectormap.Values.Where(rm => rm.routegroup_id == rgid);
        }

        
        /// <summary>
        /// 通过ID获得对应的路由组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RouterGroup this[int id]
        {
            get
            { 
                RouterGroup rg =null;
                if (routergroupmap.TryGetValue(id, out rg))
                {
                    return rg;
                }
                return null;
            }
        }

        /// <summary>
        /// 返回所有路由组
        /// </summary>
        public IEnumerable<RouterGroup> RouterGroups
        {
            get
            {
                return routergroupmap.Values;
            }
        }
    }
}
