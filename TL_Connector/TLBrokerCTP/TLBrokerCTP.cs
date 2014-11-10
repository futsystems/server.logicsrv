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
     * 
     * 
     * 
     * 
     * **/
    /// <summary>
    /// 
    /// </summary>
    public class TLBrokerCTP:TLBroker
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

        ConcurrentDictionary<string, Order> localid_order_map = new ConcurrentDictionary<string, Order>();
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
            if (localid_order_map.TryGetValue(localid, out o))
            {
                return o;
            }
            return null;
        }

        /// <summary>
        /// 交易所编号 委托 map
        /// </summary>
        ConcurrentDictionary<string, Order> exchange_order_map = new ConcurrentDictionary<string, Order>();
        string GetExchKey(Order o)
        {
            return o.Exchange + ":" + o.OrderSysID;
        }
        string GetExchKey(ref XTradeField f)
        {
            return f.Exchange + ":" + f.OrderSysID;
        }
        string GetExchKey(ref XOrderField o)
        {
            return o.Exchange + ":" + o.OrderExchID;
        }

        Order ExchKey2Order(string sysid)
        {
            Order o = null;
            if (exchange_order_map.TryGetValue(sysid, out o))
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
                    platformid_order_map.TryAdd(o.id, o);

                    //如果有交易所编号
                    if (!string.IsNullOrEmpty(o.OrderSysID))
                    {
                        exchange_order_map.TryAdd(GetExchKey(o), o);
                    }
                    if (!string.IsNullOrEmpty(o.BrokerLocalID))
                    {
                        localid_order_map.TryAdd(o.BrokerLocalID, o);
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
            //通过接口发送委托
            string localid = WrapperSendOrder(ref order);
            bool success = !string.IsNullOrEmpty(localid);
            if (success)
            {
                //1.将委托加入到接口委托维护列表
                o.BrokerLocalID = localid;
                //将委托复制后加入到接口维护的map中
                Order lo = new OrderImpl(o);
                platformid_order_map.TryAdd(o.id, lo);
                localid_order_map.TryAdd(o.BrokerLocalID, lo);

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
                action.LocalID = o.BrokerLocalID;
                string[] rec = o.OrderSysID.Split(':');

                action.Exchange = rec[0];
                action.OrderExchID = rec[1];
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
            Order o = LocalID2Order(order.LocalID);
            if (o != null)//本地记录了该委托 更新数量 状态 并对外发送
            {
                o.Status = order.OrderStatus;//更新委托状态
                o.Comment = order.StatusMsg;//填充状态信息
                o.FilledSize = order.FilledSize;//成交数量
                o.Size = order.UnfilledSize * (o.Side ? 1 : -1);//更新当前数量
                o.Exchange = order.Exchange;
                //o.OrderExchID = order.OrderExchID;//更新交易所委托编号


                if (!string.IsNullOrEmpty(order.OrderExchID))//如果orderexchid存在 则加入对应的键值
                {
                    string exchkey = GetExchKey(ref order);//使用接口传递过来的Exchange信息来生成key
                    o.OrderSysID = exchkey;
                    Util.Debug("order exchange is not emty,try to insert into exch_order_map," + exchkey);
                    //如果不存在该委托则加入该委托
                    if (!exchange_order_map.Keys.Contains(exchkey))
                    {
                        exchange_order_map.TryAdd(exchkey, o);
                    }

                }
                NotifyOrder(o);
            }
        }

        public override void ProcessTrade(ref XTradeField trade)
        {
            string exchkey = GetExchKey(ref trade);
            Order o = ExchKey2Order(exchkey);
            //
            if (o != null)
            {
                Util.Debug("该成交是本地委托所属成交,进行回报处理", QSEnumDebugLevel.WARNING);
                Trade fill = (Trade)(new OrderImpl(o));
                fill.xSize = trade.Size * (trade.Side ? 1 : -1);
                fill.xPrice = (decimal)trade.Price;

                fill.xDate = trade.Date;
                fill.xTime = trade.Time;

                fill.Broker = this.Token;
                fill.OrderSysID = o.OrderSysID;
                fill.BrokerKey = trade.TradeID;

                NotifyTrade(fill);
            }
        }

        public override void ProcessOrderError(ref XOrderError error)
        {
            Util.Debug("some error accor in order:" + error.Order.LocalID, QSEnumDebugLevel.WARNING);
            Order o = LocalID2Order(error.Order.LocalID);
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
