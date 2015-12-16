using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.Json;

namespace TradingLib.Common
{

    /// <summary>
    /// XXXXSetting 数据库数据储存对象
    /// 在实际业务逻辑中的业务对象 继承该对象 并丰富数据与操作用于实现业务逻辑
    /// </summary>
    public class RouterGroupSetting
    {
        /// <summary>
        /// 全局ID
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 路由组名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 策略名 决定了该成交路由组工作运行策略
        /// </summary>
        public QSEnumRouterStrategy Strategy { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int domain_id { get; set; }

        
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        
    }

    /// <summary>
    /// 路由组
    /// </summary>
    public class RouterGroupImpl : RouterGroupSetting, RouterGroup
    {
        ConcurrentDictionary<int, RouterItem> routeritemmap = new ConcurrentDictionary<int, RouterItem>();

        /// <summary>
        /// 域
        /// </summary>
        public Domain Domain { get; internal set; }



        /// <summary>
        /// 返回所有路由项目
        /// </summary>
        public IEnumerable<RouterItem> RouterItems
        {
            get
            {
                return routeritemmap.Values;
            }
        }

        #region 获得平仓Broker
        /// <summary>
        /// 返回所有Vendor
        /// </summary>
        /// <returns></returns>
        //IEnumerable<Vendor> GetVendors()
        //{
        //    return routeritemmap.Values.Where(r => r.Vendor != null).Select(r => r.Vendor);
        //}

        /// <summary>
        /// 获得IBroker成交路由
        /// 路由选择主体逻辑
        /// 1.开仓时由策略选择 按路由有限顺序或随机选择成交路由
        /// 2.平仓时,按所平持仓所在通道进行选择,这里涉及到拆单的问题，比如第一次开仓在A帐户,第二次开仓在B帐户,平仓时一起平仓，则委托需要拆成2个 一个从A下单,另一个从B下单
        /// 
        /// </summary>
        /// <returns></returns>
        public IBroker GetBroker(string token)
        {
            //查找路由项目的主帐户标识
            return routeritemmap.Values.Where(item => item.GetBrokerToken().Equals(token)).Select(item=>item.Broker).FirstOrDefault();
            //Vendor vendor = GetVendors().Where(v => v.GetBrokerToken().Equals(token)).FirstOrDefault();
            //if (vendor != null)
            //    return vendor.Broker;
            //else
            //    return null;
        }

        /// <summary>
        /// 返回默认的开仓通道，根据策略给出当前可用的开仓通道
        /// </summary>
        /// <returns></returns>
        public IBroker GetBroker(Order o, decimal margintouse)
        {
            if (this.Strategy == QSEnumRouterStrategy.Priority)
            {
                return PriorityBroker(o, margintouse);
            }
            else if (this.Strategy == QSEnumRouterStrategy.Stochastic)
            {
                return StochasticBroker(o, margintouse);
            }
            else
            {
                return StochasticBroker(o, margintouse);
            }

        }


        #endregion


        #region 获得开仓Broker
        Random rd = new Random(Util.ToTLTime());

        /// <summary>
        /// 返回所有可用开仓的路由项目
        /// </summary>
        /// <returns></returns>
        IEnumerable<RouterItem> GetRouterItemsForOpen()
        {
            return routeritemmap.Values.Where(r => r.Active).Where(r => r.Broker != null);
        }

        /// <summary>
        /// 按优先级别排序获得可开仓路由项目
        /// </summary>
        /// <returns></returns>
        IEnumerable<RouterItem> GetRouterItemsForOpenSorted()
        {
            return routeritemmap.Values.Where(r => r.Active).Where(r => r.Broker != null).OrderBy(r => r.priority);
        }

        IBroker StochasticBroker(Order o, decimal margintouse)
        {
            //目前实现优随机可用选择
            IBroker[] brokers = GetRouterItemsForOpen().Where(v => v.IsBrokerAvabile()).Where(v => v.AcceptEntryOrder(o, margintouse)).Select(v => v.Broker).ToArray();
            if (brokers.Length < 1)
            {
                return null;
            }
            int idx = rd.Next(0, brokers.Length);
            IBroker broker = brokers[idx];
            Util.Info(string.Format("Stochastic Strategy Select Broker[{0}]", broker.Token));
            return broker;
        }

        IBroker PriorityBroker(Order o, decimal margintouse)
        {
            IEnumerable<RouterItem> routerItemList = GetRouterItemsForOpenSorted();
            IBroker[] brokers = routerItemList.Where(v => v.IsBrokerAvabile()).Where(v => v.AcceptEntryOrder(o, margintouse)).Select(v => v.Broker).ToArray();
            if (brokers.Length < 1)
            {
                return null;
            }
            IBroker broker = brokers[0];
            Util.Info(string.Format("Priority Strategy Select Broker[{0}]", broker.Token));
            return broker;
        }


        
        #endregion



        //public void Start()
        //{
        //    foreach (IBroker b in GetBrokers())
        //    {
        //        if (b != null && !b.IsLive)
        //        {
        //            b.Start();
        //        }
        //    }
        //}

        /// <summary>
        /// 获得所有成交通道 
        /// 不保证成交通道已经启动或逻辑上可用
        /// </summary>
        /// <returns></returns>
        //public IEnumerable<IBroker> GetBrokers()
        //{
        //    return GetVendors().Where(v => v.Broker != null).Select(v => v.Broker);
        //}

        ///// <summary>
        ///// 获得所有可用的Broker用于开仓
        ///// 底层通道需启动，并且实盘通道满足设定的资金条件
        ///// </summary>
        ///// <returns></returns>
        //public IEnumerable<IBroker> GetAvabileBrokers()
        //{
        //    return GetVendors().Where(v => v.IsAvabile()).Select(v => v.Broker);
        //}

        /// <summary>
        /// 将Broker添加到路由组
        /// </summary>
        /// <param name="broker"></param>
        public void AppendRouterItem(RouterItem item)
        {
            if (routeritemmap.Keys.Contains(item.ID))
            {
                Util.Debug(string.Format("RouteItem[{0}] existed,can not append again",item.ID));
                return;
            }
            //将路由条目添加到路由组
            routeritemmap.TryAdd(item.ID, item);
            //if (brokermap.Keys.Contains(broker.Token))
            //{
            //    Util.Debug(string.Format("Broker[{0}] existed,can not append again", broker.Token));
            //    return;
            //}
            //添加到map
            //brokermap.TryAdd(broker.Token, broker);
            //prioritymap.TryAdd(broker.Token, priority);
        }

        /// <summary>
        /// 将Broker从路由组删除
        /// </summary>
        /// <param name="broker"></param>
        public void RemoveRouterItem(RouterItem item)
        {
            if (!routeritemmap.Keys.Contains(item.ID))
            {
                Util.Debug(string.Format("RouteItem[{0}] do not exist,can not remove", item.ID));
                return;
            }
            RouterItem rmitem = null;
            routeritemmap.TryRemove(item.ID, out rmitem);

            //if (!brokermap.Keys.Contains(broker.Token))
            //{
            //    Util.Debug(string.Format("Broker[{0}] do not exist,can not remove", broker.Token));
            //    return;
            //}
            //IBroker rb = null;
            //brokermap.TryRemove(broker.Token,out rb);
            //int ri = 0;
            //prioritymap.TryRemove(broker.Token, out ri);
        }

       

    }
}
