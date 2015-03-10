using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    /// <summary>
    /// 发送委托和取消委托部分
    /// </summary>
    public partial class BrokerRouterPassThrough
    {


        public Order SentRouterOrder(long oid)
        {
            throw new NotImplementedException();
        }


        public void SendOrder(Order o)
        {
           
        }



        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long val)
        {
           

        }

    }
}
