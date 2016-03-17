using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace Broker.SIM
{
    public class BOOrderBook
    {



        ConcurrentDictionary<long, BinaryOptionOrder> ordermap = new ConcurrentDictionary<long, BinaryOptionOrder>();


        /// <summary>
        /// 二元期权委托关闭事件
        /// </summary>
        public event Action<BinaryOptionOrder> OrderExitEvent;


        /// <summary>
        /// 返回所有处于持权状态的二元期权委托
        /// </summary>
        IEnumerable<BinaryOptionOrder> Orders
        {
            get { return ordermap.Values.Where(o=>o.IsEntry()); }
        }

        

        /// <summary>
        /// 清空数据
        /// </summary>
        public void Clear()
        {
            ordermap.Clear();
        }

        /// <summary>
        /// 获得委托
        /// </summary>
        /// <param name="o"></param>
        public void GotOrder(BinaryOptionOrder o)
        {
            ordermap.TryAdd(o.ID, o);
        }

        /* 二元期权通过两种事件进行关闭
         * 1.行情触发,行情有时间，通过事件判定是否需要关闭该委托
         * 2.时间触发，定时任务进行检查委托，如果超过关闭时间则执行关闭
         * 
         * */
        object _obj = new object();
        /// <summary>
        /// 响应行情
        /// </summary>
        /// <param name="k"></param>
        public void GotTick(Tick k)
        {
            lock (_obj)
            {
                List<long> remove = new List<long>();
                foreach (var order in this.Orders)
                {
                    order.GotTick(k);
                    if (order.IsExit())
                    {
                        remove.Add(order.ID);
                    }
                }
                RemoveOrder(remove);
            }
        }

        /// <summary>
        /// 响应定时时间
        /// </summary>
        /// <param name="time"></param>
        public void GotTime(long time)
        {
            lock (_obj)
            {
                List<long> remove = new List<long>();
                foreach (var order in this.Orders)
                {
                    order.GotTime(time);
                    if (order.IsExit())
                    {
                        remove.Add(order.ID);
                    }
                }
                RemoveOrder(remove);
            }
        }

        /// <summary>
        /// 从ordermap中删除委托列表
        /// </summary>
        /// <param name="remove"></param>
        void RemoveOrder(List<long> remove)
        {
            BinaryOptionOrder tmp = null;
            foreach (var id in remove)
            {
                if (ordermap.TryRemove(id, out tmp))
                {
                    if (OrderExitEvent != null)
                        OrderExitEvent(new BinaryOptionOrderImpl(tmp));
                }
            }
        }
    }
}
