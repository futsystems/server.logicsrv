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
    /*
     * 关于路由选择
     * 开仓委托有自主选择委托的功能
     * 平仓委托则需要按照对应持仓的Broker来决定需要发送到那个成交接口
     * 那如何知道当前所平持仓呢？如果当前持仓横跨了几个成交接口，那如何处理，是否需要在BrokerRouter侧再进行一次委托拆分？这样应该是太过于复杂了
     * 平仓时，我们获得当前所平持仓明细的Broker，从而就知道该委托应该发送到哪个Broker去平仓
     * 应为建立持仓明细时，开仓成交中有对应的Broker数据，表明该成就是从哪个Broker获得的成交
     * 
     * 关于委托分拆，我们需要建立更为抽象可用的分拆机制
     * 父委托-子委托，然后父委托进去，子委出来，子委托获得回报后，对应有父委托的回报
     * 
     * 
     * **/
    /*
     * 关于委托状态
     * 分帐户侧通过接口SendOrder后 如果正常返回则Order处于Submited状态
     * 
     * 成交侧则表明已经通过接口提交了委托,但是接口还没有任何返回
     * */
    /// <summary>
    /// 净持仓下单模式
    /// 需要结合当前接口的持仓状态将原来的委托进行分拆 改变开平方向，从而达到净持仓的下单目的
    /// 状态一
    /// 持有多头2手，挂单卖开2手，系统判断后 转换成  卖平2手，此时系统可挂平仓单量为0
    /// 此时买开没有问题，
    /// 
    /// 在实盘成交侧 帐户侧委托通过分拆发送或直接发送的模式向接口发单，在发单过程成分帐户侧的交易记录由清算中心记录
    /// 成交侧的委托也需要记录到数据库,在接口加载时从数据库加载成交侧的交易数据
    /// 
    /// </summary>
    public class TLBrokerESunny : TLXBroker, IBroker
    {


        #region 成交接口的交易数据
       


        public override int GetPositionAdjustment(Order o)
        {

            //return base.GetPositionAdjustment(o);
            PositionMetric metric = GetPositionMetric(o.Symbol);
            PositionAdjustmentResult adjust = PositionMetricHelper.GenPositionAdjustmentResult(metric, o);
            int increment = 0;
            if (o.oSymbol.SecurityFamily.Code.Equals("IF"))
            {
                increment = PositionMetricHelper.GenPositionIncrement(metric, adjust, true);
            }
            else
            {
                increment = PositionMetricHelper.GenPositionIncrement(metric, adjust, true);
            }

            return increment;
        }

        public override PositionMetric GetPositionMetric(string symbol)
        {
            PositionMetricImpl mertic = new PositionMetricImpl(symbol);

            mertic.LongHoldSize = this.BrokerTracker.GetPosition(symbol, true).UnsignedSize;
            mertic.ShortHoldSize = this.BrokerTracker.GetPosition(symbol, false).UnsignedSize;

            IEnumerable<Order> longEntryOrders = this.BrokerTracker.GetPendingEntryOrders(symbol, true);
            IEnumerable<Order> shortEntryOrders = this.BrokerTracker.GetPendingEntryOrders(symbol, false);
            IEnumerable<Order> longExitOrders = this.BrokerTracker.GetPendingExitOrders(symbol, true);
            IEnumerable<Order> shortExitOrders = this.BrokerTracker.GetPendingExitOrders(symbol, false);
            mertic.LongPendingEntrySize = longEntryOrders.Sum(po => po.UnsignedSize);
            mertic.LongPendingExitSize = longExitOrders.Sum(po => po.UnsignedSize);
            mertic.ShortPendingEntrySize = shortEntryOrders.Sum(po => po.UnsignedSize);
            mertic.ShortPendingExitSize = shortExitOrders.Sum(po => po.UnsignedSize);
            mertic.Token = this.Token;
            return mertic;
        }

        /// <summary>
        /// 获得所有持仓统计数据
        /// </summary>
        public override IEnumerable<PositionMetric> PositionMetrics
        {
            get
            {
                List<PositionMetric> pmlist = new List<PositionMetric>();
                foreach (string sym in WorkingSymbols)
                {
                    pmlist.Add(GetPositionMetric(sym));
                }
                return pmlist;
            }
        }
        #endregion


        /// <summary>
        /// 初始化接口 停止后再次启动不会调用该函数
        /// </summary>
        public override void InitBroker()
        {
            base.InitBroker();
            orderIdtk = new IdTracker(IdTracker.ConnectorOwnerIDStart + _cfg.ID);//用数据库ID作为委托编号生成器预留10个id用于系统其他地方使用

            

        }

        /// <summary>
        /// 记录从Broker交易信息维护器产生的平仓明细
        /// </summary>
        /// <param name="obj"></param>
        void tk_NewPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            this.LogBrokerPositionClose(obj);
        }

        IdTracker orderIdtk = null;

      


        /// <summary>
        /// 停止时候要重置接口状态
        /// </summary>
        public override void OnDisposed()
        {
            logger.Info("Clear Broker Memory");

            //清空委托map
            localOrderID_map.Clear();
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
                logger.Info("Resume TradingInfo Fron ClearCentre....");
                IEnumerable<Order> orderlist = this.SelectBrokerOrders();
                IEnumerable<Trade> tradelist = this.SelectBrokerTrades();
                IEnumerable<PositionDetail> positiondetaillist = this.SelectBrokerPositionDetails();

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
                            logger.Info("Duplicate BrokerLocalOrderID,Order:" + o.GetOrderInfo());
                        }
                    }
                    this.BrokerTracker.GotOrder(o);

                    if (!string.IsNullOrEmpty(o.BrokerRemoteOrderID))
                    {
                        //添加远程OrderMap
                        remoteOrderID_map.TryAdd(o.BrokerRemoteOrderID, o);
                    }

                }
                logger.Info(string.Format("Resumed {0} Orders", orderlist.Count()));

                //恢复日内成交
                foreach (Trade t in tradelist)
                {
                    this.BrokerTracker.GotFill(t);
                }
                logger.Info(string.Format("Resumed {0} Trades", tradelist.Count()));



                //恢复委托父子关系对 然后恢复到委托分拆器
                List<FatherSonOrderPair> lChildOrderList = GetOrderPairs(orderlist);
                foreach (FatherSonOrderPair item in lChildOrderList)
                {
                    fatherOrder_Map.TryAdd(item.FatherOrder.id, item.FatherOrder);
                    fatherSonOrder_Map.TryAdd(item.FatherOrder.id, item.SonOrders.FirstOrDefault());
                    foreach (TradingLib.API.Order o in item.SonOrders)
                    {
                        sonFathOrder_Map.TryAdd(o.id, item.FatherOrder);
                    }
                }

                //数据恢复完毕后再绑定平仓明细事件
                this.BrokerTracker.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(tk_NewPositionCloseDetailEvent);

            }
            catch (Exception ex)
            {
                logger.Error("OnInit Error:" + ex.ToString());
            }
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
                        father = this.SentOrder(o.FatherID, QSEnumOrderBreedType.ACCT);
                    }
                    if (bt == QSEnumOrderBreedType.ROUTER)
                    {
                        father = this.SentOrder(o.FatherID, QSEnumOrderBreedType.ROUTER);
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

        ConcurrentDictionary<string, Order> remoteOrderID_map = new ConcurrentDictionary<string, Order>();
        /// <summary>
        /// 通过成交对端localid查找委托
        /// 本端向成交端提交委托时需要按一定的方式储存一个委托本地编号,用于远端定位
        /// 具体来讲就是通过该编号可以按一定方法告知成交对端进行撤单
        /// </summary>
        /// <param name="localid"></param>
        /// <returns></returns>
        Order RemoteID2Order(string localid)
        {
            Order o = null;
            if (remoteOrderID_map.TryGetValue(localid, out o))
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
        ConcurrentDictionary<long, Order> fatherSonOrder_Map = new ConcurrentDictionary<long, Order>();//父子子委托映射关系
        //通过父委托ID找到对应的子委托对
        Order FatherID2SonOrder(long id)
        {
            if (fatherSonOrder_Map.Keys.Contains(id))
                return fatherSonOrder_Map[id];
            return null;
        }
        #endregion

        object _sendorder = new object();
        /// <summary>
        /// 发送委托过程为同步发送
        /// </summary>
        /// <param name="o"></param>
        public override void SendOrder(Order o)
        {
            lock (_sendorder)
            {
                //复制接口接受到的委托 并设置相关字段 lo相当于是原始委托o的一个子委托
                Order lo = new OrderImpl(o);
                //接口发出的委托相当于是接口接受委托的一个 一对一关系的子委托
                lo.FatherID = o.id;
                lo.FatherBreed = o.Breed;

                lo.Breed = QSEnumOrderBreedType.BROKER;
                lo.Broker = this.Token;

                lo.id = orderIdtk.AssignId;
                lo.BrokerLocalOrderID = "";
                lo.BrokerRemoteOrderID = "";
                lo.OrderSeq = 0;
                lo.OrderRef = "";
                lo.FrontIDi = 0;
                lo.SessionIDi = 0;
                lo.Account = this.Token;

                //港交所市价修正
                if (o.oSymbol.SecurityFamily.Exchange.EXCode == "HKEX")
                {
                    //市价单修正价格 市价单 在盘口基础上 让100个tick
                    if (o.LimitPrice == 0)
                    {
                        Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(o.Exchange,o.Symbol);
                        if (k != null)
                        {
                            o.LimitPrice = o.Side ? (k.AskPrice + 5 * o.oSymbol.SecurityFamily.PriceTick) : (k.BidPrice - 5 * o.oSymbol.SecurityFamily.PriceTick);
                        }
                    }
                }
                logger.Info("XAPI[" + this.Token + "] Send Order: " + o.GetOrderInfo(true));
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



                //通过接口发送委托,如果成功会返回接口对应逻辑的近端委托编号 否则就是发送失败
                bool success = WrapperSendOrder(ref order);
                if (success)
                {
                    //0.更新子委托状态为Submited状态 表明已经通过接口提交
                    o.Status = QSEnumOrderStatus.Submited;
                    //1.发送委托时设定本地委托编号
                    o.BrokerLocalOrderID = order.BrokerLocalOrderID;

                    lo.Status = QSEnumOrderStatus.Submited;
                    lo.BrokerLocalOrderID = order.BrokerLocalOrderID;

                    //记录委托map
                    //近端ID委托map 用于记录递增的OrderId与委托映射关系
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
                    logger.Info("Send Order Fail,will notify to client");
                }

                //发送子委托时 记录到数据库
                this.LogBrokerOrder(lo);
            }
        }

        public override void CancelOrder(long oid)
        {

            logger.Info("XAPI[" + this.Token + "] Cancel Order:" + oid.ToString());

            Order sonorder = FatherID2SonOrder(oid);
            if (sonorder != null)
            { 
                XOrderActionField action = new XOrderActionField();
                action.ActionFlag = QSEnumOrderActionFlag.Delete;
                action.BrokerLocalOrderID = sonorder.BrokerLocalOrderID;
                action.BrokerRemoteOrderID = sonorder.BrokerRemoteOrderID;
                action.ID = sonorder.id.ToString();
                /*
                 * C接口撤单操作 通过BrokerRemoteOrderID进行撤单 此处需要判定BrokerRemoteOrderID是否为空 需要置零否则未收到委托回报 发送撤单会导致C接口异常
                    TEsOrderDeleteReqField req;
                    memset(&req,0,sizeof(req));
                    req.OrderId = str2int(pAction->BrokerRemoteOrderID);
                    int ret = Tap_Api->OrderDelete(req,++m_lRequestID);
                 */
                if (string.IsNullOrEmpty(action.BrokerRemoteOrderID))
                {
                    action.BrokerRemoteOrderID = "0";
                }
                bool ret = WrapperSendOrderAction(ref action);
                logger.Info(string.Format("Send OrderAction:{0}", ret ? "Success" : "Fail"));
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



        public override void ProcessOrder(ref XOrderField order)
        {
            //1.获得本地委托数据 更新相关状态后对外触发
            Order lo = LocalID2Order(order.BrokerLocalOrderID);
            if (lo != null)//本地记录了该委托 更新数量 状态 并对外发送
            {
                //更新委托
                lo.Status = order.OrderStatus;//更新委托状态
                lo.Comment = order.StatusMsg;//填充状态信息
                lo.FilledSize = order.FilledSize;//成交数量
                lo.Size = order.UnfilledSize * (lo.Side ? 1 : -1);//更新当前数量

                //如果BrokerRemoteOrderID为空 则记录BrokerRemoteID
                if (string.IsNullOrEmpty(lo.BrokerRemoteOrderID) && !string.IsNullOrEmpty(order.BrokerRemoteOrderID))
                {
                    lo.BrokerRemoteOrderID = order.BrokerRemoteOrderID;
                    lo.OrderSysID = order.BrokerRemoteOrderID;

                    //添加远程OrderMap
                    remoteOrderID_map.TryAdd(lo.BrokerRemoteOrderID, lo);
                }
                //更新并记录该委托
                this.BrokerTracker.GotOrder(lo);
                this.LogBrokerOrderUpdate(lo);

                //更新对应的父委托
                Order fatherOrder = FatherID2Order(lo.FatherID);
                fatherOrder.Size = lo.Size;
                fatherOrder.FilledSize = lo.FilledSize;
                fatherOrder.Status = lo.Status;

                if (string.IsNullOrEmpty(fatherOrder.BrokerRemoteOrderID) && !string.IsNullOrEmpty(lo.BrokerRemoteOrderID))
                {
                    fatherOrder.BrokerRemoteOrderID = lo.BrokerRemoteOrderID;
                    fatherOrder.OrderSysID = lo.BrokerRemoteOrderID;
                }
                this.NotifyOrder(fatherOrder);
            }
            else
            {
                logger.Info(string.Format("Order brokerlocalid:{0} not found locally", order.BrokerLocalOrderID));
            }
        }

        /// <summary>
        /// 父委托与子委托一一对应,父委为分账户侧 子委托为接口侧
        /// </summary>
        /// <param name="trade"></param>
        public override void ProcessTrade(ref XTradeField trade)
        {
            logger.Info(string.Format("Process Trade {3} {0} X {1} {2} RemoteOrderID:{4} BrokerTradeID:{5}", trade.Size, trade.Price, trade.Symbol, trade.Side ? "Buy" : "Sell", trade.BrokerRemoteOrderID, trade.BrokerTradeID));
            Order o = RemoteID2Order(trade.BrokerRemoteOrderID);
            if (o != null)
            {
                //Util.Debug("该成交是本地委托所属成交,进行回报处理", QSEnumDebugLevel.WARNING);
                //子委托对应的成交
                Trade fill = (Trade)(new OrderImpl(o));
                //设定价格 数量 以及日期信息
                fill.xSize = trade.Size * (fill.Side ? 1 : -1);
                fill.xPrice = (decimal)trade.Price;
                
                fill.xDate = trade.Date;
                fill.xTime = trade.Time;
                //远端成交编号 在成交侧 需要将该字读填入TradeID 成交明细以TradeID来标识成交记录
                fill.BrokerTradeID = trade.BrokerTradeID;
                //fill.TradeID = trade.BrokerTradeID;

                logger.Info("获得子成交:" + fill.GetTradeDetail());
                this.BrokerTracker.GotFill(fill);
                //记录接口侧成交数据
                this.LogBrokerTrade(fill);

                //找对应的父委托生成父成交
                Order fatherOrder = FatherID2Order(o.FatherID);
                Trade fatherfill = (Trade)(new OrderImpl(fatherOrder));
                fatherfill.xSize = fill.xSize;
                fatherfill.xPrice = fill.xPrice;
                fatherfill.xDate = fill.xDate;
                fatherfill.xTime = fill.xTime;

                this.NotifyTrade(fatherfill);
            }
        }

        public override void ProcessOrderError(ref XOrderField pOrder, ref XErrorField pError)
        {
            logger.Info(string.Format("OrderError LocalID:{0} RemoteID:{1} ErrorID:{2} ErrorMsg:{3}", pOrder.BrokerLocalOrderID, pOrder.BrokerRemoteOrderID, pError.ErrorID, pError.ErrorMsg));
            Order lo = LocalID2Order(pOrder.BrokerLocalOrderID);
            if (lo != null)
            {
                if(lo.Status != QSEnumOrderStatus.Reject)//如果委托已经处于拒绝状态 则不用处理 接口可能会产生多次错误回报
                {
                    Order fatherOrder = SonID2FatherOrder(lo.id);
                    if (fatherOrder == null) return;

                    lo.Status = QSEnumOrderStatus.Reject;
                    lo.Comment = pError.ErrorMsg;
                    this.BrokerTracker.GotOrder(lo); //Broker交易信息管理器
                    this.LogBrokerOrderUpdate(lo);//委托跟新 更新到数据库

                    RspInfo info = new RspInfoImpl();
                    info.ErrorID = pError.ErrorID;
                    info.ErrorMessage = pError.ErrorMsg;
                    fatherOrder.Status = QSEnumOrderStatus.Reject;
                    fatherOrder.Comment = pError.ErrorMsg;
                    NotifyOrderError(fatherOrder, info);

                }

            }
        }

        public override void ProcessOrderActionError(ref XOrderActionField pOrderAction, ref XErrorField pError)
        {
            logger.Info(string.Format("OrderActionError LocalID:{0} RemoteID:{1} ErrorID:{2} ErrorMsg:{3}", pOrderAction.BrokerLocalOrderID, pOrderAction.BrokerRemoteOrderID, pError, pError.ErrorMsg));
            Order o = LocalID2Order(pOrderAction.BrokerLocalOrderID);
            //if (o != null)
            //{
            //    //生成对应的子委托OrderAction 关键是获得对应的子委托本地ID
            //    OrderAction action = new OrderActionImpl();
            //    action.Account = o.Account;
            //    action.ActionFlag = error.OrderAction.ActionFlag;
            //    action.Exchagne = error.OrderAction.Exchange;
            //    action.Symbol = error.OrderAction.Symbol;
            //    action.OrderID = o.id;//*

            //    //调用分解器处理子委托操作错误
            //    _splittracker.GotSonOrderActionError(action, XErrorField2RspInfo(ref error.Error));

            //    //委托已经被撤销 不能再撤 有些代码需要判断后同步本地委托状态
            //    if (error.Error.ErrorID == 26)
            //    {
            //        o.Status = QSEnumOrderStatus.Canceled;
            //        o.Comment = error.Error.ErrorMsg;
            //        Util.Debug("更新子委托:" + o.GetOrderInfo(true), QSEnumDebugLevel.INFO);

            //        tk.GotOrder(o); //Broker交易信息管理器

            //        this.LogBrokerOrderUpdate(o);//委托跟新 更新到数据库

            //        _splittracker.GotSonOrder(o);//委托分拆器获得子委托,用于对外更新父委托 这里采用委托更新还是委托操作错误更新，再研究
            //    }
            //    else
            //    {

            //    }
            //}
        }


        public override void SettleExchange(Exchange exchange, int settleday)
        {
            List<PositionDetail> positiondetail_settle = new List<PositionDetail>();
            foreach (var pos in this.GetPositions(exchange).Where(p => !p.isFlat))
            {
                //设定持仓结算价格
                var target = BasicTracker.SettlementPriceTracker[settleday, pos.Symbol];
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
    }
}
