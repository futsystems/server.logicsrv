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

        /// <summary>
        /// 对外发送委托
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            logger.Info("BrokerRouter try to send order out side:" + o.GetOrderInfo());
            //通过交易帐户获得对应的通道
            IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(o.Account);
            if (broker == null)
            {
                logger.Warn("交易通道不存在");
            }

            //通过交易通道直接发送委托
            broker.SendOrder(o);

        }



        /// <summary>
        /// 向broker取消一个order
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long val)
        {

            logger.Info("BrokerRouter try to cancel order to brokersize:");
            Order o = TLCtxHelper.ModuleClearCentre.SentOrder(val);
            if (o != null)
            {
                IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(o.Account);
                if (broker == null)
                {
                    logger.Warn("交易通道不存在");
                    return;
                }
                if (!broker.IsLive)
                {
                    logger.Warn("交易通道未连接");
                    return;
                }

                //通过交易通道直接发送委托
                broker.CancelOrder(val);
            }
            else
            {
                logger.Warn(string.Format("委托:{0} 不存在", val));
            }
            
        }

    }
}
