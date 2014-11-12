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
/*
 * 关于委托状态
 * 分帐户侧通过接口SendOrder后 如果正常返回则Order处于Submited状态
 * 
 * 成交侧则表明已经通过接口提交了委托,但是接口还没有任何返回
 * 
 * 
 * 
 * 
 * 
 * 
 * */
    /// <summary>
    /// 净持仓下单模式
    /// 需要结合当前接口的持仓状态将原来的委托进行分拆 改变开平方向，从而达到净持仓的下单目的
    /// </summary>
    public class TLBrokerCTPSplit : TLBroker
    {

        /// <summary>
        /// Borker交易信息维护器
        /// </summary>
        BrokerTracker tk = null;

        public override void InitBroker()
        {
            base.InitBroker();
            tk = new BrokerTracker(this);
        }

        /// <summary>
        /// 将原始的分帐户侧委托分拆转义
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public List<Order> SplitOrder(Order o)
        {
            List<Order> olist = new List<Order>();

            bool side = o.Side;//原始委托买入 或者卖出
            bool isEntry = o.IsEntryPosition;//是否是开仓
            bool posside = o.PositionSide;//持仓操作方向 是在多头操作 还是在空头操作

            int longsize = tk.GetPosition(o.Symbol, true).UnsignedSize;
            int shortsize = tk.GetPosition(o.Symbol,false).UnsignedSize;

            IEnumerable<Order> longEntryOrders = tk.GetPendingEntryOrders(o.Symbol, true);
            IEnumerable<Order> shortEntryOrders = tk.GetPendingEntryOrders(o.Symbol, false);
            IEnumerable<Order> longExitOrders = tk.GetPendingExitOrders(o.Symbol, true);
            IEnumerable<Order> shortExitOrders = tk.GetPendingExitOrders(o.Symbol, false);


            Util.Debug("当前持仓数量 多头:" + longsize.ToString() + " 空头:" + shortsize.ToString() + " 委托操作持仓方向:" + (posside ? "多头" : "空头") + " 开平:" + (isEntry ? "开" : "平") + " 方向:" + (side ? "买入" : "卖出") + " 数量:" + o.UnsignedSize.ToString(), QSEnumDebugLevel.INFO);

            //如果当前没有持仓
            if (longsize == 0 && shortsize == 0)
            {
                Order neworder = new OrderImpl(o);
                //如果是平仓委托 将平仓标志转换成开仓
                if (!neworder.IsEntryPosition)
                {
                    neworder.OffsetFlag = QSEnumOffsetFlag.OPEN;
                }
                olist.Add(neworder);
            }

            #region 持仓多头
            //当前持有多头持仓
            if (longsize > 0)
            {
                //当前委托为 多头持仓操作
                if (posside)
                {
                    //买入开仓操作 原有委托方向不变
                    if (isEntry)
                    {
                        Order neworder = new OrderImpl(o);
                        olist.Add(neworder);
                    }
                    else//卖出平仓操作 检查数量是否足够，如果足够 则直接平仓
                    {
                        if (o.UnsignedSize <= longsize)
                        {
                            Order neworder = new OrderImpl(o);
                            olist.Add(neworder);
                        }
                        else
                        {
                            //第一条委托平掉原来的持仓
                            Order neworder1 = new OrderImpl(o);
                            neworder1.TotalSize = -1 * longsize;
                            neworder1.Size = neworder1.TotalSize;
                            neworder1.OffsetFlag = QSEnumOffsetFlag.CLOSE;

                            //第二条委托反向 卖出开仓
                            Order neworder2 = new OrderImpl(o);
                            neworder2.TotalSize = -1 * (o.UnsignedSize - neworder1.UnsignedSize);
                            neworder2.Size = neworder2.TotalSize;
                            neworder2.OffsetFlag = QSEnumOffsetFlag.OPEN;

                            //
                            olist.Add(neworder1);
                            olist.Add(neworder2);
                        }
                    }
                }
                else//如果是空头持仓操作
                {
                    if (isEntry)//卖出开仓
                    {
                        if (o.UnsignedSize <= longsize)
                        {
                            Order neworder = new OrderImpl(o);
                            neworder.OffsetFlag = QSEnumOffsetFlag.CLOSE;//变成卖出平仓
                            olist.Add(neworder);
                        }
                        else
                        {
                            //第一条委托平掉原来的持仓
                            Order neworder1 = new OrderImpl(o);
                            neworder1.TotalSize = -1 * longsize;
                            neworder1.Size = neworder1.TotalSize;
                            neworder1.OffsetFlag = QSEnumOffsetFlag.CLOSE;

                            //第二条委托反向 卖出开仓
                            Order neworder2 = new OrderImpl(o);
                            neworder2.TotalSize = -1 * (o.UnsignedSize - neworder1.UnsignedSize);
                            neworder2.Size = neworder2.TotalSize;
                            neworder2.OffsetFlag = QSEnumOffsetFlag.OPEN;

                            //
                            olist.Add(neworder1);
                            olist.Add(neworder2);
                        }
                    }
                    else //买入平仓
                    {
                        Order neworder = new OrderImpl(o);
                        neworder.OffsetFlag = QSEnumOffsetFlag.OPEN;
                        olist.Add(neworder);
                    }
                }

            }
            #endregion


            #region 持仓空头
            //当前持有空头持仓
            if(shortsize>0)
            {
                if (posside)//多头持仓操作
                {
                    if (isEntry)//买入开仓
                    {
                        if (o.UnsignedSize <= shortsize)//数量小于当前空头持仓 则直接买入平仓
                        {
                            Order neworder = new OrderImpl(o);
                            neworder.OffsetFlag = QSEnumOffsetFlag.CLOSE;//变成买入平仓
                            olist.Add(neworder);
                        }
                        else
                        {
                            //第一条委托平掉原来的持仓
                            Order neworder1 = new OrderImpl(o);
                            neworder1.TotalSize = shortsize;
                            neworder1.Size = neworder1.TotalSize;
                            neworder1.OffsetFlag = QSEnumOffsetFlag.CLOSE;

                            //第二条委托反向 买入开仓
                            Order neworder2 = new OrderImpl(o);
                            neworder2.TotalSize = (o.UnsignedSize - neworder1.UnsignedSize);
                            neworder2.Size = neworder2.TotalSize;
                            neworder2.OffsetFlag = QSEnumOffsetFlag.OPEN;

                            //
                            olist.Add(neworder1);
                            olist.Add(neworder2);
                        }

                    }
                    else//卖出平仓
                    {
                        Order neworder = new OrderImpl(o);
                        neworder.OffsetFlag = QSEnumOffsetFlag.OPEN;
                        olist.Add(neworder);
                    }
                }
                else//空头持仓操作
                {
                    if (isEntry)//卖出开仓
                    {
                        Order neworder = new OrderImpl(o);
                        neworder.OffsetFlag = QSEnumOffsetFlag.OPEN;
                        olist.Add(neworder);
                    }
                    else//买入平仓
                    {
                        if (o.UnsignedSize <= shortsize)//数量小于当前空头持仓 则直接买入平仓
                        {
                            Order neworder = new OrderImpl(o);
                            neworder.OffsetFlag = QSEnumOffsetFlag.CLOSE;//变成买入平仓
                            olist.Add(neworder);
                        }
                        else
                        {
                            //第一条委托平掉原来的持仓
                            Order neworder1 = new OrderImpl(o);
                            neworder1.TotalSize = shortsize;
                            neworder1.Size = neworder1.TotalSize;
                            neworder1.OffsetFlag = QSEnumOffsetFlag.CLOSE;

                            //第二条委托反向 买入开仓
                            Order neworder2 = new OrderImpl(o);
                            neworder2.TotalSize = (o.UnsignedSize - neworder1.UnsignedSize);
                            neworder2.Size = neworder2.TotalSize;
                            neworder2.OffsetFlag = QSEnumOffsetFlag.OPEN;

                            //
                            olist.Add(neworder1);
                            olist.Add(neworder2);
                        }
                        
                    }
                }
            }
            #endregion

            //重置子委托的相关属性
            foreach (Order order in olist)
            {
                order.Broker = this.Token;
                order.id = _sonidtk.AssignId;

                order.BrokerLocalOrderID = "";
                order.BrokerRemoteOrderID = "";
                order.OrderSeq = 0;
                order.OrderRef = "";
                //重置委托相关状态
                order.Account = this.Token;
            }

            return olist;
        }
        IdTracker _sonidtk = new IdTracker(2);

        #region 委托索引map用于按不同的方式定位委托
        ///// <summary>
        ///// 本地系统委托ID与委托的map
        ///// </summary>
        //ConcurrentDictionary<long, Order> platformid_order_map = new ConcurrentDictionary<long, Order>();
        ///// <summary>
        ///// 通过本地系统id查找对应的委托
        ///// </summary>
        ///// <param name="id"></param>
        ///// <returns></returns>
        //Order PlatformID2Order(long id)
        //{
        //    Order o = null;
        //    if (platformid_order_map.TryGetValue(id, out o))
        //    {
        //        return o;
        //    }
        //    return null;
        //}

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


        public override void OnResume()
        {
            //try
            //{
            //    debug("从清算中心得到当天的委托数据并恢复到缓存中", QSEnumDebugLevel.INFO);
            //    IEnumerable<Order> olist = ClearCentre.GetOrdersViaBroker(this.Token);

            //    foreach (Order o in olist)
            //    {
            //        //平台ID编号
            //        platformid_order_map.TryAdd(o.id, o);
            //        //远端编号
            //        if (!string.IsNullOrEmpty(o.BrokerRemoteOrderID))
            //        {
            //            remoteOrderID_map.TryAdd(o.BrokerRemoteOrderID, o);
            //        }
            //        //近端编号
            //        if (!string.IsNullOrEmpty(o.BrokerLocalOrderID))
            //        {
            //            localOrderID_map.TryAdd(o.BrokerLocalOrderID, o);
            //        }
            //    }
            //    debug(string.Format("load {0} orders form database.", olist.Count()), QSEnumDebugLevel.INFO);
            //}
            //catch (Exception ex)
            //{
            //    debug("Resotore error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
            //}
        }

        //保存父委托
        ConcurrentDictionary<long, Order> fatherOrder_Map = new ConcurrentDictionary<long, Order>();
        Order FatherID2Order(long id)
        {
            if (fatherOrder_Map.Keys.Contains(id))
            {
                return fatherOrder_Map[id];
            }
            return null;
        }

        //用于通过父委托ID找到对应的子委托
        ConcurrentDictionary<long, List<Order>> fatherSonOrder_Map = new ConcurrentDictionary<long, List<Order>>();//父子子委托映射关系
        //通过父委托ID找到对应的子委托对
        List<Order> FatherID2SonOrders(long id)
        {
            if (fatherSonOrder_Map.Keys.Contains(id))
                return fatherSonOrder_Map[id];
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

        public override void SendOrder(Order o)
        {
            Util.Debug("XAP[" + this.Token + "] Send FatherOrder:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
            //1.分拆委托
            Order fathOrder = o;
            List<Order> sonOrders = SplitOrder(fathOrder);//分拆该委托

            //将委托加入映射map
            fatherOrder_Map.TryAdd(o.id, fathOrder);//保存付委托映射关系
            fatherSonOrder_Map.TryAdd(o.id, sonOrders);//保存父委托到子委托映射关系

            //2.统一发送子委托
            foreach (Order order in sonOrders)
            {
                sonFathOrder_Map.TryAdd(order.id, fathOrder);//保存子委托到父委托映射关系
                SendSunOrder(order);
            }
            //如果对应的子委托有正常提交的,那么父委托就是处于提交状态
            if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Submited))
            {
                fathOrder.Status = QSEnumOrderStatus.Submited;
            }
            //如果子委托状态为拒绝,则父委托的状态也为拒绝
            if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Reject))
            {
                fathOrder.Status = QSEnumOrderStatus.Reject;
            }
            //如果分拆的子委托一个被拒绝， 一个被接受如何处理？

            Util.Debug("父子委托关系链条 "+o.id +"->["+string.Join(",",sonOrders.Select(so=>so.id))+"]",QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 发送子委托
        /// 委托状态要么是Submited,要么是reject
        /// </summary>
        /// <param name="o"></param>
        void SendSunOrder(Order o)
        {
            debug("XAP[" + this.Token + "] Send SonOrder: " + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
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
                //0.更新子委托状态为Submited状态 表明已经通过接口提交
                o.Status = QSEnumOrderStatus.Submited;
                //1.发送委托时设定本地委托编号
                o.BrokerLocalOrderID = localid;
                //将委托复制后加入到接口维护的map中
                Order lo = new OrderImpl(o);
                //近端ID委托map
                localOrderID_map.TryAdd(o.BrokerLocalOrderID, lo);

                //交易信息维护器获得委托
                tk.GotOrder(o);
                debug("Send Order Success,LocalID:" + localid, QSEnumDebugLevel.INFO);

            }
            else
            {
                o.Status = QSEnumOrderStatus.Reject;
                debug("Send Order Fail,will notify to client", QSEnumDebugLevel.WARNING);
                
            }
        }


        public override void CancelOrder(long oid)
        {
            Order fatherOrder = FatherID2Order(oid);

            if (fatherOrder != null)
            {
                Util.Debug("XAP[" + this.Token + "] 取消父委托:" + fatherOrder.GetOrderInfo(), QSEnumDebugLevel.INFO);
                List<Order> sonOrders = FatherID2SonOrders(fatherOrder.id);//获得子委托

                //如果子委托状态处于pending状态 则发送取消
                foreach (Order o in sonOrders)
                {
                    if (o.IsPending())
                    {
                        CancelSunOrder(o);
                    }
                }
               
            }
            else
            {
                Util.Debug("Order:" + oid.ToString() + " is not in platform_order_map in broker", QSEnumDebugLevel.WARNING);
            }
        }

        void CancelSunOrder(Order o)
        {
            Util.Debug("XAP[" + this.Token + "] 取消子委托:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
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
                        Util.Debug(string.Format("OrderRemoteID:{0},put into map",order.BrokerRemoteOrderID),QSEnumDebugLevel.INFO);
                        remoteOrderID_map.TryAdd(order.BrokerRemoteOrderID, o);
                    }

                }
                Util.Debug("更新子委托:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                tk.GotOrder(o);

                //如果子委托是submited则不用更新组合状态
                bool needupdatestatus = (o.Status != QSEnumOrderStatus.Submited);
                if (!needupdatestatus) return;//如果不需要更新委托状态 直接返回

                //更新子委托数据完毕后 通过子委托找到父委托 然后转换状态并发送
                Order fatherOrder = SonID2FatherOrder(o.id);//获得父委托
                List<Order> sonOrders = FatherID2SonOrders(fatherOrder.id);//获得子委托

                //更新父委托状态
                fatherOrder.FilledSize = sonOrders.Sum(so => so.FilledSize);//累加成交数量
                fatherOrder.Size = sonOrders.Sum(so => so.UnsignedSize) * (o.Side ? 1 : -1);//累加未成交数量
                //fatherOrder.Comment = "";//填入状态信息

                //更新委托组合状态
                //由于通过接口提交委托时的状态就是submited或reject,因此submited在这里不用考虑
                //filled + filled = filled //所有成交 父委托成交

                //partfill + filled= partfill //任意部分成交 父委托部分成交
                //partfill + partfill =partfill
                //partfill + reject = partfill 
                //partfill + cancel = cancel //取消一个子委托


                //open + open = 待成交 //
                //filled +open=>patfill //
                //open + reject = open



                QSEnumOrderStatus fstatus = fatherOrder.Status;
                //子委托全部成交 则父委托为全部成交
                if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Filled))
                    fstatus = QSEnumOrderStatus.Filled;
                //子委托任一待成交,则父委托为待成交
                else if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Opened))
                    fstatus = QSEnumOrderStatus.Opened;
                //子委托全部拒绝,则父委托为拒绝
                else if (sonOrders.All(so => so.Status == QSEnumOrderStatus.Reject))
                    fstatus = QSEnumOrderStatus.Reject;
                //子委托有任一取消,则父委托为取消
                else if(sonOrders.Any(so=>so.Status == QSEnumOrderStatus.Canceled))
                    fstatus = QSEnumOrderStatus.Canceled;

                //子委托有一个委托为部分成交
                else if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.PartFilled))
                {
                    //另一个委托为取消，则父委托为取消
                    if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Canceled))
                        fstatus = QSEnumOrderStatus.Canceled;

                    //另一个委托为拒绝,则父委托为取消
                    //if (sonOrders.Any(so => so.Status == QSEnumOrderStatus.Reject))
                    fstatus = QSEnumOrderStatus.PartFilled;
                }
                fatherOrder.Status = fstatus;
                Util.Debug("更新父委托:" + fatherOrder.GetOrderInfo(), QSEnumDebugLevel.INFO);
                NotifyOrder(fatherOrder);
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

                //子委托对应的成交
                Trade sonfill = (Trade)(new OrderImpl(o));
                //设定价格 数量 以及日期信息
                sonfill.xSize = trade.Size * (trade.Side ? 1 : -1);
                sonfill.xPrice = (decimal)trade.Price;

                sonfill.xDate = trade.Date;
                sonfill.xTime = trade.Time;
                //远端成交编号
                sonfill.BrokerTradeID = trade.BrokerTradeID;
                Util.Debug("获得子成交:" + sonfill.GetTradeDetail(), QSEnumDebugLevel.INFO);
                tk.GotFill(sonfill);

                //付委托对应的成交
                Order fatherOrder = SonID2FatherOrder(o.id);//获得父委托
                Trade fill = (Trade)(new OrderImpl(fatherOrder));

                //设定价格 数量 以及日期信息
                fill.xSize = trade.Size * (trade.Side ? 1 : -1);
                fill.xPrice = (decimal)trade.Price;

                fill.xDate = trade.Date;
                fill.xTime = trade.Time;
                //远端成交编号
                //fill.BrokerTradeID = trade.BrokerTradeID;
                //其余委托类的相关字段在Order处理中获得
                Util.Debug("获得父成交:" + fill.GetTradeDetail(), QSEnumDebugLevel.INFO);
                NotifyTrade(fill);
            }
        }

        public override void ProcessOrderError(ref XOrderError error)
        {
            Util.Debug("some error accor in order:" + error.Order.BrokerLocalOrderID, QSEnumDebugLevel.WARNING);
            Order o = LocalID2Order(error.Order.BrokerLocalOrderID);
            if (o != null)
            {
                Order fatherOrder = SonID2FatherOrder(o.id);//获得父委托
                RspInfo info = new RspInfoImpl();
                info.ErrorID = error.Error.ErrorID;
                info.ErrorMessage = error.Error.ErrorMsg;

                fatherOrder.Status = QSEnumOrderStatus.Reject;
                fatherOrder.Comment = info.ErrorMessage;

                NotifyOrderError(fatherOrder, info);
            }
        }
    }
}
