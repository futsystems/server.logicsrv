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
    /*
     * 如何处理好接口的统一调用
     * 
     * 
     * 
     * 发单时获得近端编号并设定
     * 
     * 获得委托回报时候 获得远端编号并设定本地的OrderSysID
     * 
     * 
     * 
     * **/
    /// <summary>
    /// 
    /// </summary>
    public class TLBrokerCTPDirect:TLBroker
    {

        #region 委托索引map用于按不同的方式定位委托
        /// <summary>
        /// 本地系统委托ID与委托的map
        /// </summary>
        ConcurrentDictionary<long, Order> platformid_order_map = new ConcurrentDictionary<long, Order>();
        /// <summary>
        /// 通过本地系统id查找对应的委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order PlatformID2Order(long id)
        {
            Order o = null;
            if (platformid_order_map.TryGetValue(id, out o))
            {
                return o;
            }
            return null;
        }

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

        /// <summary>
        /// 交易所编号 委托 map
        /// </summary>
        ConcurrentDictionary<string, Order> remoteOrderID_map = new ConcurrentDictionary<string, Order>();
        Order RemoteID2Order(string sysid)
        {
            Order o = null;
            if (remoteOrderID_map.TryGetValue(sysid, out o))
            {
                return o;
            }
            return null;
        }
        #endregion


        public override void  OnResume()
        {
            try
            {
                debug("从清算中心得到当天的委托数据并恢复到缓存中", QSEnumDebugLevel.INFO);
                IEnumerable<Order> olist = ClearCentre.GetOrdersViaBroker(this.Token);

                foreach (Order o in olist)
                {
                    //平台ID编号
                    platformid_order_map.TryAdd(o.id, o);
                    //远端编号
                    if (!string.IsNullOrEmpty(o.BrokerRemoteOrderID))
                    {
                        remoteOrderID_map.TryAdd(o.BrokerRemoteOrderID, o);
                    }
                    //近端编号
                    if (!string.IsNullOrEmpty(o.BrokerLocalOrderID))
                    {
                        localOrderID_map.TryAdd(o.BrokerLocalOrderID, o);
                    }
                }
                debug(string.Format("load {0} orders form database.", olist.Count()), QSEnumDebugLevel.INFO);
            }
            catch (Exception ex)
            {
                debug("Resotore error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        public override void SendOrder(Order o)
        {
            debug("TLBrokerXAP[" + this.Token + "]: " + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
            XOrderField order = new XOrderField();

            order.ID = o.id.ToString();
            order.Date = o.Date;
            order.Time = o.Time;
            order.Symbol = o.Symbol;
            order.Exchange = o.Exchange;
            order.Side = o.Side;
            order.TotalSize = Math.Abs(o.TotalSize);
            order.FilledSize = 0;
            order.UnfilledSize = 0;

            order.LimitPrice = (double)o.LimitPrice;
            order.StopPrice = 0;

            order.OffsetFlag = o.OffsetFlag;
            
            o.Broker = this.Token;


            //通过接口发送委托,如果成功会返回接口对应逻辑的近端委托编号 否则就是发送失败
            string localid = WrapperSendOrder(ref order);
            bool success = !string.IsNullOrEmpty(localid);
            if (success)
            {
                //1.发送委托时设定本地委托编号
                o.BrokerLocalOrderID = localid;

                //将委托复制后加入到接口维护的map中
                Order lo = new OrderImpl(o);
                //系统ID委托map
                platformid_order_map.TryAdd(o.id, lo);
                //近端ID委托map
                localOrderID_map.TryAdd(o.BrokerLocalOrderID, lo);
                debug("Send Order Success,LocalID:" + localid, QSEnumDebugLevel.INFO);

            }
            else
            {
                debug("Send Order Fail,will notify to client", QSEnumDebugLevel.WARNING);
                o.Status = QSEnumOrderStatus.Reject;
            }
        }


        public override void CancelOrder(long oid)
        {
            Order o = PlatformID2Order(oid);
            if (o != null)
            {
                XOrderActionField action = new XOrderActionField();
                action.ActionFlag = QSEnumOrderActionFlag.Delete;

                action.ID = o.id.ToString();
                action.BrokerLocalOrderID = o.BrokerLocalOrderID;
                action.BrokerRemoteOrderID = o.BrokerRemoteOrderID;
                action.Exchange = o.Exchange;
                action.Price = 0;
                action.Size = 0;
                action.Symbol = o.Symbol;


                if (WrapperSendOrderAction(ref action))
                {

                }
                else
                {
                    debug("Cancel order fail,will notify to client");
                }
            }
            else
            {
                Util.Debug("Order:" + oid.ToString() + " is not in platform_order_map in broker", QSEnumDebugLevel.WARNING);
            }
        }

        public override void ProcessOrder(ref XOrderField order)
        {
            //1.获得本地委托数据 更新相关状态后对外触发
            Order o = LocalID2Order(order.BrokerLocalOrderID);
            if (o != null)//本地记录了该委托 更新数量 状态 并对外发送
            {
                //更新委托
                o.Status = order.OrderStatus;//更新委托状态
                o.Comment = order.StatusMsg;//填充状态信息
                o.FilledSize = order.FilledSize;//成交数量
                o.Size = order.UnfilledSize * (o.Side ? 1 : -1);//更新当前数量

                //更新RemoteOrderId/OrderSysID
                if (!string.IsNullOrEmpty(order.BrokerRemoteOrderID))//如果远端编号存在 则设定远端编号 同时入map
                {
                    o.BrokerRemoteOrderID = order.BrokerRemoteOrderID;
                    //按照不同接口的实现 从RemoteOrderID中获得对应的OrderSysID
                    o.OrderSysID = order.BrokerRemoteOrderID.Split(':')[1];
                    //如果不存在该委托则加入该委托
                    if (!remoteOrderID_map.Keys.Contains(order.BrokerRemoteOrderID))
                    {
                        Util.Debug("order remoteid is set,put into map, remoteid:" + order.BrokerRemoteOrderID);
                        remoteOrderID_map.TryAdd(order.BrokerRemoteOrderID, o);
                    }

                }
                NotifyOrder(o);
            }
        }

        public override void ProcessTrade(ref XTradeField trade)
        {
            //CTP接口的成交通过远端编号与委托进行关联
            Order o = RemoteID2Order(trade.BrokerRemoteOrderID);
            //
            Util.Debug("trade info,localid:" + trade.BrokerLocalOrderID + " remoteid:" + trade.BrokerRemoteOrderID, QSEnumDebugLevel.INFO);
            if (o != null)
            {
                Util.Debug("该成交是本地委托所属成交,进行回报处理", QSEnumDebugLevel.WARNING);
                Trade fill = (Trade)(new OrderImpl(o));

                //设定价格 数量 以及日期信息
                fill.xSize = trade.Size * (trade.Side ? 1 : -1);
                fill.xPrice = (decimal)trade.Price;

                fill.xDate = trade.Date;
                fill.xTime = trade.Time;
                //远端成交编号
                fill.BrokerTradeID = trade.BrokerTradeID;
                //其余委托类的相关字段在Order处理中获得

                NotifyTrade(fill);
            }
        }

        public override void ProcessOrderError(ref XOrderError error)
        {
            Util.Debug("some error accor in order:" + error.Order.BrokerLocalOrderID, QSEnumDebugLevel.WARNING);
            Order o = LocalID2Order(error.Order.BrokerLocalOrderID);
            if (o != null)
            {
                RspInfo info = new RspInfoImpl();
                info.ErrorID = error.Error.ErrorID;
                info.ErrorMessage = error.Error.ErrorMsg;

                o.Status = QSEnumOrderStatus.Reject;
                o.Comment = info.ErrorMessage;

                NotifyOrderError(o, info);
            }
        }
    }
}
