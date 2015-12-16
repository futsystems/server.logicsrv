using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;

namespace Broker.Live
{
    public partial class TLBrokerIB
    {
        ConcurrentDictionary<string, Order> localOrderID_map = new ConcurrentDictionary<string, Order>();
        /// <summary>
        /// 通过成交对端localid查找委托
        /// 本端向成交端提交委托时需要按一定的方式储存一个委托本地编号,用于远端定位
        /// 具体来讲就是通过该编号可以按一定方法告知成交对端进行撤单
        /// </summary>
        /// <param name="localid"></param>
        /// <returns></returns>
        Order LocalID2Order(string localid)
        {
            Order o = null;
            if (localOrderID_map.TryGetValue(localid, out o))
            {
                return o;
            }
            return null;
        }


        ConcurrentDictionary<long, Order> fatherOrder_Map = new ConcurrentDictionary<long, Order>();
        /// <summary>
        /// 父委托map 记录了原始委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order FatherID2Order(long id)
        {
            if (fatherOrder_Map.Keys.Contains(id))
            {
                return fatherOrder_Map[id];
            }
            return null;
        }

        //用于通过子委托ID找到对应的父委托
        ConcurrentDictionary<long, Order> sonFathOrder_Map = new ConcurrentDictionary<long, Order>();
        /// <summary>
        /// 通过子委托ID找到对应的父委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order SonID2FatherOrder(long id)
        {
            if (sonFathOrder_Map.Keys.Contains(id))
                return sonFathOrder_Map[id];
            return null;
        }

        //用于通过父委托ID找到对应的子委托
        ConcurrentDictionary<long, Order> fatherSonOrder_Map = new ConcurrentDictionary<long,Order>();//父子子委托映射关系
        //通过父委托ID找到对应的子委托对
        Order FatherID2SonOrder(long id)
        {
            if (fatherSonOrder_Map.Keys.Contains(id))
                return fatherSonOrder_Map[id];
            return null;
        }

    }
}
