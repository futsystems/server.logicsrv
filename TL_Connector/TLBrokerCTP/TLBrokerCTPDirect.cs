using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;
using TradingLib.Mixins;

namespace Broker.Live
{
    /* CTPDirect
     * 1.直接将分账户侧的委托发送到CTP 同时处理CTP对应的返回
     * 2.接口只负责发单不负责检查持仓是否正确即BrokerTracker所维护的数据不用于委托检查
     * 
     * 
     * 
     * */
    /// <summary>
    /// 
    /// </summary>
    public class TLBrokerCTPDirect : TLXBroker
    {


        #region 成交接口的交易数据
        //public override int GetPositionAdjustment(Order o)
        //{

        //    //return base.GetPositionAdjustment(o);
        //    PositionMetric metric = GetPositionMetric(o.Symbol);
        //    PositionAdjustmentResult adjust = PositionMetricHelper.GenPositionAdjustmentResult(metric, o);
        //    int increment = 0;
        //    if (o.oSymbol.SecurityFamily.Code.Equals("IF"))
        //    {
        //        increment = PositionMetricHelper.GenPositionIncrement(metric, adjust, true);
        //    }
        //    else
        //    {
        //        increment = PositionMetricHelper.GenPositionIncrement(metric, adjust, true);
        //    }

        //    return increment;
        //}

        //public override PositionMetric GetPositionMetric(string symbol)
        //{
        //    PositionMetricImpl mertic = new PositionMetricImpl(symbol);

        //    mertic.LongHoldSize = _BrokerTracker.GetPosition(symbol, true).UnsignedSize;
        //    mertic.ShortHoldSize = _BrokerTracker.GetPosition(symbol, false).UnsignedSize;

        //    IEnumerable<Order> longEntryOrders = _BrokerTracker.GetPendingEntryOrders(symbol, true);
        //    IEnumerable<Order> shortEntryOrders = _BrokerTracker.GetPendingEntryOrders(symbol, false);
        //    IEnumerable<Order> longExitOrders = _BrokerTracker.GetPendingExitOrders(symbol, true);
        //    IEnumerable<Order> shortExitOrders = _BrokerTracker.GetPendingExitOrders(symbol, false);
        //    mertic.LongPendingEntrySize = longEntryOrders.Sum(order => order.UnsignedSize);
        //    mertic.LongPendingExitSize = longExitOrders.Sum(order => order.UnsignedSize);
        //    mertic.ShortPendingEntrySize = shortEntryOrders.Sum(order => order.UnsignedSize);
        //    mertic.ShortPendingExitSize = shortExitOrders.Sum(order => order.UnsignedSize);
        //    mertic.Token = this.Token;
        //    return mertic;
        //}

        ///// <summary>
        ///// 获得所有持仓统计数据
        ///// </summary>
        //public override IEnumerable<PositionMetric> PositionMetrics
        //{
        //    get
        //    {
        //        List<PositionMetric> pmlist = new List<PositionMetric>();
        //        foreach (string sym in WorkingSymbols)
        //        {
        //            pmlist.Add(GetPositionMetric(sym));
        //        }
        //        return pmlist;
        //    }
        //}
        #endregion


        /// <summary>
        /// Borker交易信息维护器
        /// </summary>
        IdTracker _ChildIDTracker = null;

        /// <summary>
        /// 初始化接口 停止后再次启动不会调用该函数
        /// </summary>
        public override void InitBroker()
        {
            base.InitBroker();
            _ChildIDTracker = new IdTracker(IdTracker.ConnectorOwnerIDStart + _cfg.ID);//用数据库ID作为委托编号生成器预留10个id用于系统其他地方使用    
        }

        /// <summary>
        /// 停止时候要重置接口状态
        /// </summary>
        public override void OnDisposed()
        {
            logger.Info("Reset Broker DataStruct");
            //清空委托map
            localOrderID_map.Clear();
            remoteOrderID_map.Clear();
            fatherOrder_Map.Clear();
            sonFathOrder_Map.Clear();
            fatherSonOrder_Map.Clear();

        }
        /// <summary>
        /// 恢复日内交易状态
        /// 从数据库加载昨日持仓数据 和当日交易数据并填充到 成交接口维护器中 用于恢复日内交易状态
        /// </summary>
        public override void OnInit()
        {
            try
            {
                logger.Info("Load Trading Info From ClearCentre");
                this.BrokerTracker.Clear();//清空交易数据维护器
                IEnumerable<Order> orderlist = ClearCentre.SelectBrokerOrders(this.Token);
                IEnumerable<Trade> tradelist = ClearCentre.SelectBrokerTrades(this.Token);
                IEnumerable<PositionDetail> positiondetaillist = ClearCentre.SelectBrokerPositionDetails(this.Token);

                //恢复隔夜持仓数据
                foreach (PositionDetail pd in positiondetaillist)
                {
                    this.BrokerTracker.GotPosition(pd);
                }
                logger.Info(string.Format("Resumed {0} Positions", positiondetaillist.Count()));
                //恢复日内委托
                foreach (Order o in orderlist)
                {
                    if (!string.IsNullOrEmpty(o.BrokerLocalOrderID))//BrokerLocalOrderID不为空
                    {
                        if (!localOrderID_map.Keys.Contains(o.BrokerLocalOrderID))
                        {
                            localOrderID_map.TryAdd(o.BrokerLocalOrderID, o);
                        }
                        else
                        {
                            logger.Warn("Duplicate BrokerLocalOrderID,Order:" + o.GetOrderInfo());
                        }
                    }
                    if (!string.IsNullOrEmpty(o.BrokerRemoteOrderID))//BrokerRemoteOrderID不为空
                    {
                        if (!remoteOrderID_map.Keys.Contains(o.BrokerRemoteOrderID))
                        {
                            remoteOrderID_map.TryAdd(o.BrokerRemoteOrderID, o);
                        }
                        else
                        {
                            logger.Warn("Duplicate BrokerRemoteOrderID,Order:" + o.GetOrderInfo());
                        }
                    }
                    this.BrokerTracker.GotOrder(o);

                }
                logger.Info(string.Format("Resumed {0} Orders", orderlist.Count()));
                //恢复日内成交
                foreach (Trade t in tradelist)
                {
                    this.BrokerTracker.GotFill(t);
                }
                logger.Info(string.Format("Resumed {0} Trades", tradelist.Count()));

                List<FatherSonOrderPair> pairs = GetOrderPairs(orderlist);
                foreach (FatherSonOrderPair pair in pairs)
                {
                    fatherOrder_Map.TryAdd(pair.FatherOrder.id, pair.FatherOrder);
                    fatherSonOrder_Map.TryAdd(pair.FatherOrder.id, pair.SonOrders.FirstOrDefault());
                    foreach (var o in pair.SonOrders)
                    {
                        sonFathOrder_Map.TryAdd(o.id, pair.FatherOrder);
                    }
                }

                //数据恢复完毕后再绑定平仓明细事件
                this.BrokerTracker.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(tk_NewPositionCloseDetailEvent);

            }
            catch (Exception ex)
            {
                logger.Error("Resue Data Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 记录从Broker交易信息维护器产生的平仓明细
        /// </summary>
        /// <param name="obj"></param>
        void tk_NewPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            this.LogBrokerPositionClose(obj);
        }


        List<FatherSonOrderPair> GetOrderPairs(IEnumerable<Order> sonOrders)
        {
            Dictionary<long, FatherSonOrderPair> pairmap = new Dictionary<long, FatherSonOrderPair>();
            foreach (Order o in sonOrders)
            {
                Order father = null;
                if (o.FatherBreed != null)
                {
                    QSEnumOrderBreedType bt = (QSEnumOrderBreedType)o.FatherBreed;
                    if (bt == QSEnumOrderBreedType.ACCT)//如果直接分帐户侧分解 从清算中查找该委托
                    {
                        father = ClearCentre.SentOrder(o.FatherID, QSEnumOrderBreedType.ACCT);
                    }
                    if (bt == QSEnumOrderBreedType.ROUTER)
                    {
                        father = ClearCentre.SentOrder(o.FatherID, QSEnumOrderBreedType.ROUTER);
                    }
                }
                //如果存在父委托
                if (father != null)
                {
                    //如果不存在该父委托 则增加
                    if (!pairmap.Keys.Contains(father.id))
                    {
                        pairmap[father.id] = new FatherSonOrderPair(father);
                    }
                    //将子委托加入到列表
                    pairmap[father.id].SonOrders.Add(o);
                }
            }
            return pairmap.Values.ToList();
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

        ConcurrentDictionary<long, Order> fatherOrder_Map = new ConcurrentDictionary<long, Order>();
        /// <summary>
        /// 通过接口外侧ID找到原始委托
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
        /// 通过接口侧委托ID找到接口外侧委托
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
        ConcurrentDictionary<long, Order> fatherSonOrder_Map = new ConcurrentDictionary<long, Order>();//父子子委托映射关系
        /// <summary>
        /// 通过接口外侧委托ID找到接口内侧委托
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Order FatherID2SonOrder(long id)
        {
            if (fatherSonOrder_Map.Keys.Contains(id))
                return fatherSonOrder_Map[id];
            return null;
        }
        #endregion


        #region IBroker交易接口
        public override void SendOrder(Order o)
        {
            //发送委托时 底层CTP接口有一个递增操作 该操作不是线程安全的，如果多个线程同时调用该函数则会出现orderref相同的情况从而出现下单错乱的问题。
            lock (this)
            {
                //复制接口接受到的委托 并设置相关字段 lo相当于是原始委托o的一个子委托
                Order lo = new OrderImpl(o);
                //接口侧委托与接收到的委托为一对一的关系
                lo.FatherID = o.id;
                lo.FatherBreed = o.Breed;
                //设定当前分解源
                lo.Breed = QSEnumOrderBreedType.BROKER;

                lo.Broker = this.Token;
                lo.id = _ChildIDTracker.AssignId;

                lo.BrokerLocalOrderID = "";
                lo.BrokerRemoteOrderID = "";
                lo.OrderSeq = 0;
                lo.OrderRef = "";
                lo.FrontIDi = 0;
                lo.SessionIDi = 0;
                lo.Account = this.Token;

                //通过接口发送委托
                logger.Info("XAPI[" + this.Token + "] Send Order: " + lo.GetOrderInfo(true));
                XOrderField order = new XOrderField();

                order.ID = lo.id.ToString();
                order.Date = lo.Date;
                order.Time = lo.Time;
                order.Symbol = lo.Symbol;
                order.Exchange = lo.Exchange;
                order.Side = lo.Side;
                order.TotalSize = Math.Abs(lo.TotalSize);
                order.FilledSize = 0;
                order.UnfilledSize = 0;

                order.LimitPrice = (double)lo.LimitPrice;
                order.StopPrice = 0;

                order.OffsetFlag = lo.OffsetFlag;

                //o.Broker = this.Token;


                //通过接口发送委托,如果成功会返回接口对应逻辑的近端委托编号 否则就是发送失败
                bool success = WrapperSendOrder(ref order);
                if (success)
                {
                    //更新接口侧外侧委托状态和近端编号
                    lo.Status = QSEnumOrderStatus.Submited;
                    lo.BrokerLocalOrderID = order.BrokerLocalOrderID;

                    //更新接口外侧委托状态和近端编号 状态为Submited状态 表明已经通过接口提交
                    o.Status = QSEnumOrderStatus.Submited;
                    o.BrokerLocalOrderID = order.BrokerLocalOrderID;

                    //近端ID委托map
                    localOrderID_map.TryAdd(lo.BrokerLocalOrderID, lo);
                    //记录父委托和子委托
                    sonFathOrder_Map.TryAdd(lo.id, o);
                    fatherOrder_Map.TryAdd(o.id, o);
                    fatherSonOrder_Map.TryAdd(o.id, lo);

                    //交易信息维护器获得委托 //？将委托复制后加入到接口维护的map中 在发送子委托过程中 本地记录的Order就是分拆过程中产生的委托，改变这个委托将同步改变委托分拆器中的委托
                    this.BrokerTracker.GotOrder(lo);//原来引用的是分拆器发送过来的子委托 现在修改成本地复制后的委托
                    //对外触发成交侧委托数据用于记录该成交接口的交易数据
                    logger.Info(string.Format("Send Order Success ID:{0} LocalID:{1}", order.ID, order.BrokerLocalOrderID));

                }
                else
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    logger.Warn("Send Order Fail,will notify to client");
                }

                //发送子委托时 记录到数据库
                this.LogBrokerOrder(lo);
            }
        }

        public override void CancelOrder(long oid)
        {
            lock (this)
            {
                logger.Info("XAPI[" + this.Token + "] Cancel Order:" + oid.ToString());
                Order lo = FatherID2SonOrder(oid);
                if (lo != null)
                {
                    XOrderActionField action = new XOrderActionField();
                    action.ActionFlag = QSEnumOrderActionFlag.Delete;
                    action.ID = lo.id.ToString();
                    action.BrokerLocalOrderID = lo.BrokerLocalOrderID;
                    action.BrokerRemoteOrderID = lo.BrokerRemoteOrderID;
                    action.Exchange = lo.BrokerRemoteOrderID.Split(':')[0];//从RemoeOrderID获得交易所信息
                    action.Price = 0;
                    action.Size = 0;
                    action.Symbol = lo.Symbol;

                    bool ret = WrapperSendOrderAction(ref action);
                    logger.Info(string.Format("Send OrderAction:{0}", ret ? "Success" : "Fail"));
                }
                else
                {
                    logger.Warn(string.Format("ParentOrder:{0} have no son order", oid));
                }
            }
        }

        public override void GotTick(Tick k)
        {
            base.GotTick(k);
            if (this.IsLive)
            {
                //行情驱动brokertracker用于更新成交侧持仓
                this.BrokerTracker.GotTick(k);
            }
        }
        #endregion


        #region 处理接口侧数据回报
        public override void ProcessOrder(ref XOrderField order)
        {
            logger.Info(string.Format("ProcessOrder LocalID:{0} RemoteID:{1} ID:{2}", order.BrokerLocalOrderID, order.BrokerRemoteOrderID, order.ID));
            //1.获得本地委托数据 更新相关状态后对外触发
            Order lo = LocalID2Order(order.BrokerLocalOrderID);
            if (lo != null)//本地记录了该委托 更新数量 状态 并对外发送
            {
                //更新委托
                lo.Status = order.OrderStatus;//更新委托状态
                lo.Comment = order.StatusMsg;//填充状态信息
                lo.FilledSize = order.FilledSize;//成交数量
                lo.Size = order.UnfilledSize * (lo.Side ? 1 : -1);//更新当前数量

                //更新RemoteOrderId/OrderSysID
                if (!string.IsNullOrEmpty(order.BrokerRemoteOrderID))//如果远端编号存在 则设定远端编号 同时入map
                {
                    string[] ret = order.BrokerRemoteOrderID.Split(':');
                    //需要设定了OrderSysID 否则只是Exch:空格 
                    if (!string.IsNullOrEmpty(ret[1]))
                    {
                        lo.BrokerRemoteOrderID = order.BrokerRemoteOrderID;
                        //按照不同接口的实现 从RemoteOrderID中获得对应的OrderSysID
                        lo.OrderSysID = ret[1];
                        //如果不存在该委托则加入该委托
                        if (!remoteOrderID_map.Keys.Contains(order.BrokerRemoteOrderID))
                        {
                            remoteOrderID_map.TryAdd(order.BrokerRemoteOrderID, lo);
                        }
                    }
                }
                logger.Info("Update Local Order:" + lo.GetOrderInfo(true));
                //更新接口侧委托
                this.BrokerTracker.GotOrder(lo);
                this.LogBrokerOrderUpdate(lo);

                if (lo.Status == QSEnumOrderStatus.Submited) return;//如果子委托为Submited状态 则不用更新父委托直接返回

                //更新对应的接口外侧委托
                Order fatherOrder = FatherID2Order(lo.FatherID);
                fatherOrder.Size = lo.Size;
                fatherOrder.FilledSize = lo.FilledSize;
                fatherOrder.Status = lo.Status;
                fatherOrder.Comment = lo.Comment;

                if (string.IsNullOrEmpty(fatherOrder.BrokerRemoteOrderID) && !string.IsNullOrEmpty(lo.BrokerRemoteOrderID))
                {
                    fatherOrder.BrokerRemoteOrderID = lo.BrokerRemoteOrderID;
                    fatherOrder.OrderSysID = lo.OrderSysID;
                }

                this.NotifyOrder(fatherOrder);
            }
            else
            {
                logger.Warn(string.Format("Son Order LocalID:{0} is not handled by Broker:{1}", order.BrokerLocalOrderID, this.Token));
            }
        }

        public override void ProcessTrade(ref XTradeField trade)
        {
            logger.Info(string.Format("Process Trade {3} {0} X {1} {2} RemoteOrderID:{4} BrokerTradeID:{5}", trade.Size, trade.Price, trade.Symbol, trade.Side ? "Buy" : "Sell", trade.BrokerRemoteOrderID, trade.BrokerTradeID));
            //CTP接口的成交通过远端编号与委托进行关联
            Order lo = RemoteID2Order(trade.BrokerRemoteOrderID);
            if (lo != null)
            {
                //Util.Debug("该成交是本地委托所属成交,进行回报处理", QSEnumDebugLevel.WARNING);
                //子委托对应的成交
                Trade fill = (Trade)(new OrderImpl(lo));
                //设定价格 数量 以及日期信息
                fill.xSize = trade.Size * (trade.Side ? 1 : -1);
                fill.xPrice = (decimal)trade.Price;

                fill.xDate = trade.Date;
                fill.xTime = trade.Time;
                //远端成交编号 在成交侧 需要将该字读填入TradeID 成交明细以TradeID来标识成交记录
                fill.BrokerTradeID = trade.BrokerTradeID;
                fill.TradeID = trade.BrokerTradeID;

                this.BrokerTracker.GotFill(fill);
                this.LogBrokerTrade(fill);

                //找对应的父委托生成父成交
                Order fatherOrder = FatherID2Order(lo.FatherID);
                Trade fatherfill = (Trade)(new OrderImpl(fatherOrder));
                fatherfill.xSize = fill.xSize;
                fatherfill.xPrice = fill.xPrice;
                fatherfill.xDate = fill.xDate;
                fatherfill.xTime = fill.xTime;

                this.NotifyTrade(fatherfill);
            }
            else
            {
                logger.Warn(string.Format("Son Order RemoteID:{0} is not handled by Broker:{1}", trade.BrokerRemoteOrderID, this.Token));
            }
        }

        public override void ProcessOrderError(ref XOrderField pOrder, ref XErrorField pError)
        {
            logger.Info(string.Format("OrderError LocalID:{0} RemoteID:{1} ErrorID:{2} ErrorMsg:{3}", pOrder.BrokerLocalOrderID, pOrder.BrokerRemoteOrderID, pError.ErrorID, pError.ErrorMsg));
            Order lo = LocalID2Order(pOrder.BrokerLocalOrderID);
            if (lo != null)
            {
                if (lo.Status != QSEnumOrderStatus.Reject)//如果委托已经处于拒绝状态 则不用处理 接口可能会产生多次错误回报
                {
                    lo.Status = QSEnumOrderStatus.Reject;
                    lo.Comment = pError.ErrorMsg;
                    logger.Info("Update Local Order:" + lo.GetOrderInfo(true));
                    this.BrokerTracker.GotOrder(lo);
                    //更新接口侧委托
                    this.LogBrokerOrderUpdate(lo);//更新日志

                    Order fatherOrder = SonID2FatherOrder(lo.id);
                    if (fatherOrder == null) return;

                    RspInfo info = new RspInfoImpl();
                    info.ErrorID = pError.ErrorID;
                    info.ErrorMessage = pError.ErrorMsg;
                    fatherOrder.Status = QSEnumOrderStatus.Reject;
                    fatherOrder.Comment = pError.ErrorMsg;
                    NotifyOrderError(fatherOrder, info);


                    //_splittracker.GotSonOrderError(o, info);
                    //平仓量超过持仓量 只能修复 在主帐户对应方向执行买入操作 形成对应的持仓 这样就可以让分帐户侧成功平仓
                    //if (error.Error.ErrorID == 30)
                    //{
                    //    //平仓缺失智能补单 这个功能可以在接口设置中进行参数化设置。同理 在撤单过程中，如果有委托已经撤除 也需要智能的进行本地同步
                    //    XOrderField norder = new XOrderField();

                    //    norder.ID = o.id.ToString();
                    //    norder.Date = Util.ToTLDate();
                    //    norder.Time = Util.ToTLTime();
                    //    norder.Symbol = o.Symbol;
                    //    norder.Exchange = o.Exchange;
                    //    norder.Side = !o.Side;
                    //    norder.TotalSize = Math.Abs(o.TotalSize);
                    //    norder.FilledSize = 0;
                    //    norder.UnfilledSize = 0;

                    //    norder.LimitPrice = 0;
                    //    norder.StopPrice = 0;

                    //    norder.OffsetFlag = QSEnumOffsetFlag.OPEN;//开仓

                    //    bool success = WrapperSendOrder(ref norder);
                    //    Log4NetHelper.Warn(string.Format("平仓量超过持仓量,主帐户侧持仓缺失,下单进行补仓 市价{0} {1} 手 {2} {3}", norder.Side ? "买入" : "卖出", norder.TotalSize, norder.Symbol, success ? "成功" : "失败"));
                    //}
                    //资金不足
                    if (pError.ErrorID == 31)
                    {

                    }
                }

            }
            else
            {
                logger.Warn(string.Format("Son Order LocalID:{0} is not handled by Broker:{1}", pOrder.BrokerLocalOrderID, this.Token));
            }
        }

        public override void ProcessOrderActionError(ref XOrderActionField pOrderAction, ref XErrorField pError)
        {
            logger.Info(string.Format("OrderActionError LocalID:{0} RemoteID:{1} ErrorID:{2} ErrorMsg:{3}", pOrderAction.BrokerLocalOrderID, pOrderAction.BrokerRemoteOrderID, pError.ErrorID, pError.ErrorMsg));
            Order lo = LocalID2Order(pOrderAction.BrokerLocalOrderID);
            if (lo != null)
            {
                Order fatherOrder = SonID2FatherOrder(lo.id);
                //如果没有对应的父委托则直接返回
                if (fatherOrder == null)
                {
                    logger.Warn(string.Format("Sone Order ID:{0} do not have father order", lo.id));
                    return;
                }
                //A 可处理错误
                //委托已经被撤销 不能再撤 有些代码需要判断后同步本地委托状态
                if (pError.ErrorID == 26)
                {
                    lo.Status = QSEnumOrderStatus.Canceled;
                    lo.Comment = pError.ErrorMsg;
                    logger.Info("Update Local Order:" + lo.GetOrderInfo(true));
                    this.BrokerTracker.GotOrder(lo); //Broker交易信息管理器
                    this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

                    fatherOrder.Status = QSEnumOrderStatus.Reject;
                    fatherOrder.Comment = pError.ErrorMsg;
                    NotifyOrder(fatherOrder);
                    return;
                }
                else
                {

                }

                //B 回报错误到父委托
                //生成父委托对应的OrderAction Error并Notify
                OrderAction action = new OrderActionImpl();
                action.Account = fatherOrder.Account;
                action.ActionFlag = pOrderAction.ActionFlag;
                action.Exchagne = pOrderAction.Exchange;
                action.Symbol = pOrderAction.Symbol;
                action.OrderID = fatherOrder.id;//*
                action.FrontID = fatherOrder.FrontIDi;
                action.SessionID = fatherOrder.SessionIDi;
                action.OrderRef = fatherOrder.OrderRef;
                action.OrderExchID = fatherOrder.OrderSysID;

                //action.

                RspInfo info = XErrorField2RspInfo(ref pError);
                NotifyOrderOrderActionError(action, info);
            }
            else
            {
                logger.Warn(string.Format("Son Order LocalID:{0} is not handled by Broker:{1}", pOrderAction.BrokerLocalOrderID, this.Token));
            }
        }

        /// <summary>
        /// XErrorField转换成RspInfo
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        RspInfo XErrorField2RspInfo(ref XErrorField error)
        {
            RspInfo info = new RspInfoImpl();
            info.ErrorID = error.ErrorID;
            info.ErrorMessage = error.ErrorMsg;
            return info;
        }
        #endregion

        public override void SettleExchange(IExchange exchange, int settleday)
        {
            List<PositionDetail> positiondetail_settle = new List<PositionDetail>();
            foreach (var pos in this.GetPositions(exchange).Where(p => !p.isFlat))
            {
                //设定持仓结算价格
                SettlementPrice target = TLCtxHelper.ModuleSettleCentre.GetSettlementPrice(settleday, pos.Symbol);
                if (target != null && target.Settlement > 0)
                {
                    pos.SettlementPrice = target.Settlement;
                }

                //如果没有正常获得结算价格 持仓结算价按对应的最新价进行结算
                if (pos.SettlementPrice == null || pos.SettlementPrice < 0)
                {
                    pos.SettlementPrice = pos.LastPrice;
                }

                //遍历该未平仓持仓对象下的所有持仓明细
                foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                {
                    //保存结算持仓明细时要将结算日更新为当前
                    pd.Settleday = settleday;
                    //保存持仓明细到数据库
                    TLCtxHelper.ModuleDataRepository.NewPositionDetail(pd);
                    positiondetail_settle.Add(pd);
                }
            }

            ///4.标注已结算数据 委托 成交 持仓
            foreach (var o in this.GetOrders(exchange, settleday))
            {
                o.Settled = true;
                TLCtxHelper.ModuleDataRepository.MarkOrderSettled(o);
            }
            foreach (var f in this.GetTrades(exchange, settleday))
            {
                f.Settled = true;
                TLCtxHelper.ModuleDataRepository.MarkTradeSettled(f);
            }
            foreach (var pos in this.GetPositions(exchange))
            {
                pos.Settled = true;
                //如果持仓有隔夜持仓 将对应的隔夜持仓标注成已结算否则会对隔夜持仓重复加载
                foreach (var pd in pos.PositionDetailYdRef)
                {
                    TLCtxHelper.ModuleDataRepository.MarkPositionDetailSettled(pd);
                }
            }
            //将已经结算的持仓从内存数据对象中屏蔽 持仓数据是一个状态数据,因此我们这里将上个周期的持仓对象进行屏蔽
            this.BrokerTracker.DropSettled();
            ///5.加载持仓明晰和交易所结算记录
            foreach (var pd in positiondetail_settle)
            {
                this.BrokerTracker.GotPosition(pd);
            }
        }

    }
}
