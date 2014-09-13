using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 实现其他相关功能接口
    /// </summary>
    public partial class ClearCentre
    {


        #region *【IBrokerTradingInfo】从内存中获得某个成交接口的所有委托
        /// <summary>
        /// 获得某个交易通道当天的委托信息,用于交易通道恢复交易数据
        /// </summary>
        /// <param name="broker"></param>
        /// <returns></returns>
        public IList<Order> getOrders(IBroker broker)
        {
            IList<Order> olist = LoadOrderFromMysql();
            IList<Order> brokreorder = new List<Order>();

            foreach (Order o in olist)
            {
                if (o.Broker == broker.GetType().FullName)
                    brokreorder.Add(o);
            }
            return brokreorder;
        }

        #endregion

            

            



    }
}
