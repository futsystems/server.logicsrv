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
        ConcurrentDictionary<int, RouterGroupImpl> routergroupmap = new ConcurrentDictionary<int, RouterGroupImpl>();
        ConcurrentDictionary<int, RouterItemImpl> routeritemmap = new ConcurrentDictionary<int, RouterItemImpl>();

        public RouterGrouperTracker()
        {
            foreach (RouterGroupImpl rg in ORM.MRouterGroup.SelectRouterGroup())
            {
                routergroupmap.TryAdd(rg.ID, rg);
            }

            foreach (RouterItemImpl item in ORM.MRouterGroup.SelectRouterItem())
            {
                routeritemmap.TryAdd(item.ID, item);

                RouterGroupImpl group = null;
                routergroupmap.TryGetValue(item.routegroup_id,out group);
                if(group != null)
                {
                    item.RouteGroup = group;//绑定路由条目的RouterGroup
                    group.AppendRouterItem(item);//实现双向绑定
                }
                //绑定实盘帐户对象
                item.Vendor = BasicTracker.VendorTracker[item.vendor_id];//绑定路由条目的Vendor
                
            }
        }

        /// <summary>
        /// 获得某个RouterGroup
        /// </summary>
        /// <param name="rgid"></param>
        /// <returns></returns>
        //public RouterGroup GetRouterGroup(int rgid)
        //{
        //    if (routergroupmap.Keys.Contains(rgid))
        //        return routergroupmap[rgid];
        //    return null;
        //}

        

        /// <summary>
        /// 获得某个路由组所有通道映射
        /// </summary>
        /// <param name="rgid"></param>
        /// <returns></returns>
        //public IEnumerable<RouterItem> GetRouterItem(int rgid)
        //{
        //    return routeritemmap.Values.Where(rm => rm.routegroup_id == rgid);
        //}

        
        /// <summary>
        /// 通过ID获得对应的路由组
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RouterGroup this[int id]
        {
            get
            { 
                RouterGroupImpl rg =null;
                if (routergroupmap.TryGetValue(id,out rg))
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

        /// <summary>
        /// 返回所有路由项目
        /// </summary>
        public IEnumerable<RouterItemImpl> RouterItems
        {
            get
            {
                return routeritemmap.Values;
            }
        }
    }
}
