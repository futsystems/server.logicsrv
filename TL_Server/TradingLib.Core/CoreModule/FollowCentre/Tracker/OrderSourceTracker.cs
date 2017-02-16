using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{

    /// <summary>
    /// 用于维护委托出发源与委托的映射关系
    /// OrderID-FollowItem的对应关系 用于通过委托编号反向查找到对应的跟单项目 
    /// </summary>
    public class OrderSourceTracker
    {

        ConcurrentDictionary<long, FollowItem> orderSourceMap = new ConcurrentDictionary<long, FollowItem>();

        /// <summary>
        /// 获得某个委托的触发源
        /// 这里直接定位跟单项 跟单操作不保持状态 只是整个跟单引擎运行的中间过程数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FollowItem this[long id]
        {
            get
            {
                FollowItem followitem = null;
                if (orderSourceMap.TryGetValue(id, out followitem))
                {
                    return followitem;
                }
                return null;
            }
        }

        /// <summary>
        /// 记录一条委托ID与触发源关系
        /// </summary>
        /// <param name="item"></param>
        /// <param name="o"></param>
        public void NewOrder(FollowItem item, Order o)
        {
            orderSourceMap.TryAdd(o.id, item);
            //委托编号和跟单项的对应关系 需要记录到数据库
        }
    }
}
