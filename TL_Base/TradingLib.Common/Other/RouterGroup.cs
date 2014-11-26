using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    /// <summary>
    /// 路由与路由组映射关系
    /// </summary>
    public class RouterConnecterMap
    {
        /// <summary>
        /// 全局序号
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// 优先级
        /// </summary>
        public int priority { get; set; }

        /// <summary>
        /// 路由组ID
        /// </summary>
        public int routegroup_id { get; set; }

        /// <summary>
        /// 通道ID
        /// </summary>
        public int connector_id { get; set; }

        /// <summary>
        /// 接受委托规则
        /// </summary>
        public string rule { get; set; }
    }


    public class RouterGroupImpl : RouterGroup
    {

        ConcurrentDictionary<string, IBroker> brokermap = new ConcurrentDictionary<string, IBroker>();
        ConcurrentDictionary<string, int> prioritymap = new ConcurrentDictionary<string, int>();
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
            IBroker broker = null;
            if (brokermap.TryGetValue(token, out broker))
            {
                return broker;
            }
            return null;
        }

        Random rd = new Random(Util.ToTLTime());
        /// <summary>
        /// 返回默认的开仓通道，根据策略给出当前可用的开仓通道
        /// </summary>
        /// <returns></returns>
        public IBroker GetBroker(Order o)
        { 
            //目前实现优随机可用选择
            IBroker[] brokers = GetAvabileBrokers();
            if (brokers.Length < 1)
            {
                return null;
            }
            int idx = rd.Next(0, brokers.Length);
            
            return brokers[idx];
        }

        public void Start()
        {
            foreach (IBroker b in brokermap.Values)
            {
                if (b != null && !b.IsLive)
                {
                    b.Start();
                }
            }
        }
        public IBroker[] GetAvabileBrokers()
        {
            return brokermap.Values.Where(b => b.IsLive && b.Avabile).ToArray();
        }
        /// <summary>
        /// 将Broker添加到路由组
        /// </summary>
        /// <param name="broker"></param>
        public void AppendBroker(IBroker broker, int priority)
        {
            if (brokermap.Keys.Contains(broker.Token))
            {
                Util.Debug(string.Format("Broker[{0}] existed,can not append again", broker.Token));
                return;
            }
            //添加到map
            brokermap.TryAdd(broker.Token, broker);
            prioritymap.TryAdd(broker.Token, priority);
        }

        /// <summary>
        /// 将Broker从路由组删除
        /// </summary>
        /// <param name="broker"></param>
        public void RemoveBroker(IBroker broker)
        {
            if (!brokermap.Keys.Contains(broker.Token))
            {
                Util.Debug(string.Format("Broker[{0}] do not exist,can not remove", broker.Token));
                return;
            }
            IBroker rb = null;
            brokermap.TryRemove(broker.Token,out rb);
            int ri = 0;
            prioritymap.TryRemove(broker.Token, out ri);
        }

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
        public string Strategy { get; set; }

        /// <summary>
        /// 域ID
        /// </summary>
        public int Domain_ID { get; set; }

        

    }
}
