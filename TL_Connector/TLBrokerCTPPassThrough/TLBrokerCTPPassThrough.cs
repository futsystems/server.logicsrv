﻿using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace Broker.Live
{
    public class TLBrokerCTPPassThrough : TLBroker
    {

        /// <summary>
        /// 初始化交易接口
        /// </summary>
        public override void InitBroker()
        {
            base.InitBroker();
            debug("xxxxxxxxxxxxxxxxx", QSEnumDebugLevel.ERROR);
            
        }

        /// <summary>
        /// 销毁交易接口
        /// </summary>
        public override void DestoryBroker()
        {

            //清空委托map
            localOrderID_map.Clear();
            remoteOrderID_map.Clear();
        }

        /// <summary>
        /// 恢复交易接口数据
        /// </summary>
        public override void OnResume()
        {
            
        }

        //public override bool Restore()
        //{
        //    //恢复日内数据前 清空本地委托map
        //    localOrderID_map.Clear();
        //    remoteOrderID_map.Clear();

        //    //调用底层恢复数据
        //    return this.WrapperRestore();
        //}
        /// <summary>
        /// 响应行情 驱动本地相关数据计算
        /// </summary>
        /// <param name="k"></param>
        public override void GotTick(Tick k)
        {
            
        }


        #region 委托索引map用于按不同的方式定位委托
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
        /// 通过BrokerLocalID判断 该委托是否是新的委托
        /// </summary>
        /// <param name="localid"></param>
        /// <returns></returns>
        //bool IsNewOrder(string localid)
        //{ 
        //    return localOrderID_map.Keys.cont
        //}

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


        /// <summary>
        /// 提交委托
        /// </summary>
        /// <param name="o"></param>
        public override void SendOrder(Order o)
        {
            debug("send order to broker:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);

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
            bool success = WrapperSendOrder(ref order);
            if (success)
            {
                //0.更新子委托状态为Submited状态 表明已经通过接口提交
                o.Status = QSEnumOrderStatus.Submited;
                //1.发送委托时设定本地委托编号
                o.BrokerLocalOrderID = order.BrokerLocalOrderID;

                Order lo = new OrderImpl(o);
                //近端ID委托map
                //localOrderID_map.TryAdd(o.BrokerLocalOrderID, lo);

                //交易信息维护器获得委托 //？将委托复制后加入到接口维护的map中 在发送子委托过程中 本地记录的Order就是分拆过程中产生的委托，改变这个委托将同步改变委托分拆器中的委托
                //tk.GotOrder(lo);//原来引用的是分拆器发送过来的子委托 现在修改成本地复制后的委托
                //对外触发成交侧委托数据用于记录该成交接口的交易数据
                debug("Send Order Success,LocalID:" + order.BrokerLocalOrderID, QSEnumDebugLevel.INFO);

            }
            else
            {
                o.Status = QSEnumOrderStatus.Reject;
                debug("Send Order Fail,will notify to client", QSEnumDebugLevel.WARNING);
            }

            
        }

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="oid"></param>
        public override void CancelOrder(long oid)
        {
            

        }


        /// <summary>
        /// 处理接口回报的委托
        /// 监听主帐户过程中
        /// 其它终端登入提交的委托通过BrokerLocalID进行识别
        /// </summary>
        /// <param name="order"></param>
        public override void ProcessOrder(ref XOrderField order)
        {
            debug(string.Format("Got Order LocalID:{0} RemoteID:{1} Price:{2} TotalSize:{3} OffsetFlag:{4} OrderStatus:{5}", order.BrokerLocalOrderID,order.BrokerRemoteOrderID,order.LimitPrice,order.TotalSize,order.OffsetFlag,order.OrderStatus), QSEnumDebugLevel.INFO);

            Order localorder = LocalID2Order(order.BrokerLocalOrderID);
            //如果本地委托不存在 则该委托为新委托，创建本地Order与之对应
            if (localorder == null)
            {
                string symbol = order.Symbol;
                bool side = order.Side;
                int size = Math.Abs(order.UnfilledSize)*(side?1:-1);

                localorder = new OrderImpl(symbol, side, size);
                localorder.Date = order.Date;
                localorder.Time = order.Time;
                localorder.Exchange = order.Exchange;
                localorder.FilledSize = order.FilledSize;
                localorder.TotalSize = Math.Abs(order.TotalSize) * (side ? 1 : -1);

                localorder.LimitPrice = (decimal)order.LimitPrice;
                localorder.StopPrice = (decimal)order.StopPrice;
                localorder.BrokerLocalOrderID = order.BrokerLocalOrderID;
                localorder.BrokerRemoteOrderID = order.BrokerRemoteOrderID;
                localorder.OffsetFlag = order.OffsetFlag;
                localorder.Status = order.OrderStatus;
                localorder.Comment = order.StatusMsg;
                localorder.Broker = this.Token;

                //将委托保存到map
                localOrderID_map.TryAdd(localorder.BrokerLocalOrderID, localorder);
                debug("Got New Order:" + localorder.GetOrderInfo(), QSEnumDebugLevel.ERROR);
            }
            else
            {
                //更新委托参数
                localorder.Status = order.OrderStatus;
                localorder.Comment = order.StatusMsg;
                localorder.FilledSize = order.FilledSize;
                localorder.Size = order.UnfilledSize;

                debug("Order Updated:" + localorder.GetOrderInfo(), QSEnumDebugLevel.ERROR);
            }

            if (!string.IsNullOrEmpty(order.BrokerRemoteOrderID))//如果远端编号存在 则设定远端编号 同时入map
            {
                string[] ret = order.BrokerRemoteOrderID.Split(':');
                //需要设定了OrderSysID 否则只是Exch:空格 
                if (!string.IsNullOrEmpty(ret[1]))
                {
                    localorder.BrokerRemoteOrderID = order.BrokerRemoteOrderID;
                    //按照不同接口的实现 从RemoteOrderID中获得对应的OrderSysID
                    localorder.OrderSysID = ret[1];
                    //如果不存在该委托则加入该委托
                    if (!remoteOrderID_map.Keys.Contains(order.BrokerRemoteOrderID))
                    {
                        remoteOrderID_map.TryAdd(order.BrokerRemoteOrderID,localorder);
                    }
                }
            }

            //向外回报委托
            this.NotifyOrder(localorder);

        }

        /// <summary>
        /// 处理委托错误回报
        /// </summary>
        /// <param name="error"></param>
        public override void ProcessOrderError(ref XOrderError error)
        {
            debug(string.Format("OrderError LocalID:{0} RemoteID:{1} ErrorID:{2} ErrorMsg:{3}", error.Order.BrokerLocalOrderID, error.Order.BrokerRemoteOrderID, error.Error.ErrorID, error.Error.ErrorMsg), QSEnumDebugLevel.ERROR);
            
        }

        /// <summary>
        /// 处理委托操作错误
        /// </summary>
        /// <param name="error"></param>
        public override void ProcessOrderActionError(ref XOrderActionError error)
        {
            


        }

        /// <summary>
        /// 处理成交数据
        /// </summary>
        /// <param name="trade"></param>
        public override void ProcessTrade(ref XTradeField trade)
        {
            debug(string.Format("Got Fill, LocalID:{0} RemoteID:{1} BrokerTradeID:{2} XPrice:{3} Side:{4} XSize:{5}", trade.BrokerLocalOrderID, trade.BrokerRemoteOrderID, trade.BrokerTradeID,trade.Price,trade.Side,trade.Size), QSEnumDebugLevel.INFO);

            //CTP接口的成交通过远端编号与委托进行关联
            Order o = RemoteID2Order(trade.BrokerRemoteOrderID);
            if (o != null)
            {
                Trade fill = (Trade)(new OrderImpl(o));
                //设定价格 数量 以及日期信息
                fill.xSize = trade.Size * (trade.Side ? 1 : -1);
                fill.xPrice = (decimal)trade.Price;

                fill.xDate = trade.Date;
                fill.xTime = trade.Time;
                //远端成交编号 在成交侧 需要将该字读填入TradeID 成交明细以TradeID来标识成交记录
                fill.BrokerTradeID = trade.BrokerTradeID;
                fill.TradeID = trade.BrokerTradeID;

                Util.Debug("获得成交:" + fill.GetTradeDetail(), QSEnumDebugLevel.INFO);

                this.NotifyTrade(fill);
            }
        }


    }
}