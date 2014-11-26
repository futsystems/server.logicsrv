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
     * 
     * 
     * **/
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
    /// 状态一
    /// 持有多头2手，挂单卖开2手，系统判断后 转换成  卖平2手，此时系统可挂平仓单量为0
    /// 此时买开没有问题，
    /// 
    /// 在实盘成交侧 帐户侧委托通过分拆发送或直接发送的模式向接口发单，在发单过程成分帐户侧的交易记录由清算中心记录
    /// 成交侧的委托也需要记录到数据库,在接口加载时从数据库加载成交侧的交易数据
    /// 
    /// </summary>
    public class TLBrokerCTPSplit : TLBroker
    {


        #region 成交接口的交易数据
        /// <summary>
        /// 获得成交接口所有委托
        /// </summary>
        public override IEnumerable<Order> Orders { get { return tk.Orders; } }

        /// <summary>
        /// 获得成交接口所有成交
        /// </summary>
        public override IEnumerable<Trade> Trades { get { return tk.Trades; } }

        /// <summary>
        /// 获得成交接口所有持仓
        /// </summary>
        public override IEnumerable<Position> Positions { get { return tk.Positions; } }

        #endregion


        /// <summary>
        /// Borker交易信息维护器
        /// </summary>
        BrokerTracker tk = null;

        /// <summary>
        /// 初始化接口 停止后再次启动不会调用该函数
        /// </summary>
        public override void InitBroker()
        {
            base.InitBroker();
            tk = new BrokerTracker(this);
            _sonidtk = new IdTracker(IdTracker.ConnectorOwnerIDStart + _cfg.ID);//用数据库ID作为委托编号生成器预留10个id用于系统其他地方使用

            #region 初始化委托拆分维护器 绑定事件
            _splittracker = new OrderSplitTracker(this.Token);
            //委托分拆器发送子委托通过接口发送
            _splittracker.SendSonOrderEvent += new OrderDelegate(SendSonOrder);
            //委托分拆器发送取消委托通过接口取消
            _splittracker.CancelSonOrderEvent += new OrderDelegate(CancelSonOrder);

            //委托分拆器请求分拆委托,调用本地实现的分拆逻辑SplitOrder
            _splittracker.SplitOrdereEvent += new Func<Order, List<Order>>(SplitOrder);

            //委托分拆器更新父委托 本地对外通知父委托更新
            _splittracker.GotFatherOrderEvent += new OrderDelegate(NotifyOrder);
            //_splittracker.GotFatherCancelEvent += 
            //委托分拆器更新成交 本地对外通知成交更新
            _splittracker.GotFatherFillEvent += new FillDelegate(NotifyTrade);
            //委托分拆器更新错误 本地对外通知错误更新
            _splittracker.GotFatherOrderErrorEvent += new OrderErrorDelegate(NotifyOrderError);
            #endregion

        }

        /// <summary>
        /// 记录从Broker交易信息维护器产生的平仓明细
        /// </summary>
        /// <param name="obj"></param>
        void tk_NewPositionCloseDetailEvent(PositionCloseDetail obj)
        {
            this.LogBrokerPositionClose(obj);
        }

        IdTracker _sonidtk = null;
        OrderSplitTracker _splittracker= null;

        #region 委托拆分逻辑
        /// <summary>
        /// 直接发送委托
        /// 不分拆委托进行保证金降低
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        List<Order> DirectOrder(Order o)
        {
            List<Order> olist = new List<Order>();
            Order neworder = new OrderImpl(o);
            olist.Add(neworder);

            ResetOrder(o,ref olist);

            return olist;
        }
        /// <summary>
        /// 将原始的分帐户侧委托分拆转义
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        List<Order> SplitOrder(Order o)
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
            int longPendingEntrySize = longEntryOrders.Sum(po => po.UnsignedSize);
            int longPendingExitSize = longExitOrders.Sum(po => po.UnsignedSize);
            int shortPendingEntrySize = shortEntryOrders.Sum(po => po.UnsignedSize);
            int shortPendingExitSize = shortExitOrders.Sum(po => po.UnsignedSize);

            Util.Debug("当前持仓数量 多头:" + longsize.ToString() + " 空头:" + shortsize.ToString() +"待买开:"+longPendingEntrySize.ToString() +" 待卖平:"+longPendingExitSize.ToString()+" 待卖开:"+shortPendingEntrySize.ToString() +" 待买平"+shortPendingExitSize.ToString()+ " 委托操作持仓方向:" + (posside ? "多头" : "空头") + " 开平:" + (isEntry ? "开" : "平") + " 方向:" + (side ? "买入" : "卖出") + " 数量:" + o.UnsignedSize.ToString(), QSEnumDebugLevel.INFO);

            //如果当前没有持仓 则直接开仓
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
            /*
             * 持有2手多单,卖开2手转成卖平2手
             * 
             * 卖开/卖平，就不为能再转成卖平，需要转换成卖开
             * 
             * 此时再下买开，则需要跳转到空的方向去平，而不是直接判断多头方向，
             * 系统需要往保证金减少的方向进行
             * 并且委托只能在一个方向进行拆单
             * 
             * 规则2
             * 多头2手
             * 空头5手
             * 
             * 此时空头操作，买平2手，如果按照原来的逻辑，则进入多头检查，结果就是变成买开2手，这个逻辑要调整
             * 要进入保证金多的方向进行检查
             * 
             * 如果进入空头部分检查，直接买平，如果空头有挂单，无可平持仓，则买开
             * 
             * */
            int longcanexit = longsize - longPendingExitSize;//当前持有的多头持仓数量 - 处于挂单状态的卖平数量 = 当前可以挂卖平数量
            int shortcanexit = shortsize - shortPendingExitSize;//当前持有的空头持仓数量 - 处于挂单状态的买平数量 = 当前可以挂买平数量
           
            //如果同时持有多头和空头 则需要判断进入哪个持仓方向进行拆单
            if (longsize > 0 && shortsize > 0)
            {
                //买入操作 希望进入空头判定 转换成买平操作
                if (side)
                {
                    goto SHORTSIDE;
                }
                else//卖出操作 进入多头判定 转换成卖平操作
                { 
                    goto LONGSIDE;
                }
            }
            


            #region 持仓多头
            //当前持有多头持仓 可以挂买开 卖平
        LONGSIDE:
            if (longsize > 0 && olist.Count==0)
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
                    else//卖出平仓操作 
                    {
                        if (longcanexit > 0)//如果有可平持仓
                        {
                            if (o.UnsignedSize <= longcanexit) //检查数量是否足够，如果足够 则直接平仓
                            {
                                Order neworder = new OrderImpl(o);
                                olist.Add(neworder);
                            }
                            else //否则拆单
                            {
                                //第一条委托平掉原来的持仓
                                Order neworder1 = new OrderImpl(o);
                                neworder1.TotalSize = -1 * longcanexit;
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
                        else//已经被挂单占用 没有可平手数，则全部转换成开仓
                        {
                            Order neworder = new OrderImpl(o);
                            neworder.OffsetFlag = QSEnumOffsetFlag.OPEN;
                            olist.Add(neworder);
                        }
                    }
                }
                else//如果是空头持仓操作
                {
                    if (isEntry)//卖出开仓
                    {
                        if (longcanexit > 0)
                        {
                            if (o.UnsignedSize <= longcanexit)
                            {
                                Order neworder = new OrderImpl(o);
                                neworder.OffsetFlag = QSEnumOffsetFlag.CLOSE;//变成卖出平仓
                                olist.Add(neworder);
                            }
                            else
                            {
                                //第一条委托平掉原来的持仓
                                Order neworder1 = new OrderImpl(o);
                                neworder1.TotalSize = -1 * longcanexit;
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
                        else//卖出开仓,直接开仓
                        {
                            Order neworder = new OrderImpl(o);
                            olist.Add(neworder);
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
        SHORTSIDE:
            //当前持有空头持仓
            if (shortsize > 0 && olist.Count == 0)
            {
                if (posside)//多头持仓操作
                {
                    if (isEntry)//买入开仓
                    {
                        if (shortcanexit > 0)
                        {
                            if (o.UnsignedSize <= shortcanexit)//数量小于当前空头持仓 则直接买入平仓
                            {
                                Order neworder = new OrderImpl(o);
                                neworder.OffsetFlag = QSEnumOffsetFlag.CLOSE;//变成买入平仓
                                olist.Add(neworder);
                            }
                            else
                            {
                                //第一条委托平掉原来的持仓
                                Order neworder1 = new OrderImpl(o);
                                neworder1.TotalSize = shortcanexit;
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
                        else//没有空头持仓可以平 则直接开多仓
                        {
                            Order neworder = new OrderImpl(o);
                            olist.Add(neworder);
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
                        if (shortcanexit > 0)
                        {
                            if (o.UnsignedSize <= shortcanexit)//数量小于当前空头持仓 则直接买入平仓
                            {
                                Order neworder = new OrderImpl(o);
                                neworder.OffsetFlag = QSEnumOffsetFlag.CLOSE;//变成买入平仓
                                olist.Add(neworder);
                            }
                            else
                            {
                                //第一条委托平掉原来的持仓
                                Order neworder1 = new OrderImpl(o);
                                neworder1.TotalSize = shortcanexit;
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
                        else//空头没有持仓可以平，则直接转换成开仓
                        {
                            Order neworder = new OrderImpl(o);
                            neworder.OffsetFlag = QSEnumOffsetFlag.OPEN;
                            olist.Add(neworder);
                        }
                        
                    }
                }
            }
            #endregion

            ResetOrder(o,ref olist);

            return olist;
        }

        /// <summary>
        /// 重置子委托相关字段
        /// </summary>
        /// <param name="olist"></param>
        void ResetOrder(Order fatherorder,ref List<Order> olist)
        {
            //重置子委托的相关属性
            foreach (Order order in olist)
            {
                //设定分解后委托父ID
                order.FatherID = fatherorder.id;
                //设定当前分解源
                order.Breed = QSEnumOrderBreedType.BROKER;

                order.Broker = this.Token;
                order.id = _sonidtk.AssignId;

                order.BrokerLocalOrderID = "";
                order.BrokerRemoteOrderID = "";
                order.OrderSeq = 0;
                order.OrderRef = "";
                order.FrontIDi = 0;
                order.SessionIDi = 0;
                if (order.OffsetFlag == QSEnumOffsetFlag.CLOSE)
                {
                    order.OffsetFlag = QSEnumOffsetFlag.CLOSETODAY;
                }
                
                //重置委托相关状态
                order.Account = this.Token;
            }

           
        }
        #endregion


        /// <summary>
        /// 停止时候要重置接口状态
        /// </summary>
        public override void  DestoryBroker()
        {
            Util.Debug("DestoryBroker ...............");
            //清空接口交易状态维护器
            tk.Clear();
            tk = null;

            //清空委托拆分器状态
            _splittracker.Clear();
            _splittracker = null;

            //清空委托map
            localOrderID_map.Clear();
            remoteOrderID_map.Clear();

        }
        /// <summary>
        /// 恢复日内交易状态
        /// 从数据库加载昨日持仓数据 和当日交易数据并填充到 成交接口维护器中 用于恢复日内交易状态
        /// </summary>
        public override void OnResume()
        {
            try
            {
                debug("Resume trading info from clearcentre....", QSEnumDebugLevel.INFO);
                IEnumerable<Order> orderlist = ClearCentre.SelectBrokerOrders(this.Token);
                IEnumerable<Trade> tradelist = ClearCentre.SelectBrokerTrades(this.Token);
                IEnumerable<PositionDetail> positiondetaillist = ClearCentre.SelectBrokerPositionDetails(this.Token);

                //恢复隔夜持仓数据
                foreach (PositionDetail pd in positiondetaillist)
                {
                    tk.GotPosition(pd);
                }
                debug(string.Format("Resumed {0} Positions", positiondetaillist.Count()), QSEnumDebugLevel.INFO);
                //恢复日内委托
                foreach (Order o in orderlist)
                {
                    if(!string.IsNullOrEmpty(o.BrokerLocalOrderID))//BrokerLocalOrderID不为空
                    {
                        if (!localOrderID_map.Keys.Contains(o.BrokerLocalOrderID))
                        {
                            localOrderID_map.TryAdd(o.BrokerLocalOrderID, o);
                        }
                        else
                        {
                            debug("Duplicate BrokerLocalOrderID,Order:" + o.GetOrderInfo(), QSEnumDebugLevel.WARNING);
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
                            debug("Duplicate BrokerRemoteOrderID,Order:" + o.GetOrderInfo(), QSEnumDebugLevel.WARNING);
                        }
                    }
                    tk.GotOrder(o);

                }
                debug(string.Format("Resumed {0} Orders", orderlist.Count()),QSEnumDebugLevel.INFO);
                //恢复日内成交
                foreach (Trade t in tradelist)
                {
                    tk.GotFill(t);
                }
                debug(string.Format("Resumed {0} Trades", tradelist.Count()),QSEnumDebugLevel.INFO);
                List<FatherSonOrderPair> pairs = GetOrderPairs(orderlist);
                foreach(FatherSonOrderPair pair in pairs)
                {
                    _splittracker.ResumeOrder(pair);
                }

                //数据恢复完毕后再绑定平仓明细事件
                tk.NewPositionCloseDetailEvent += new Action<PositionCloseDetail>(tk_NewPositionCloseDetailEvent);

            }
            catch (Exception ex)
            { 
                
            }
        }

        List<FatherSonOrderPair> GetOrderPairs(IEnumerable<Order> sonOrders)
        {
            Dictionary<long, FatherSonOrderPair> pairmap = new Dictionary<long, FatherSonOrderPair>();
            foreach (Order o in sonOrders)
            {
                Order father = ClearCentre.SentOrder(o.FatherID);
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
        #endregion

        public override void SendOrder(Order o)
        {
            _splittracker.SendFatherOrder(o);
        }

        public override void CancelOrder(long oid)
        {
            _splittracker.CancelFatherOrder(oid);
        }

        public override void GotTick(Tick k)
        {
            base.GotTick(k);
            if (this.IsLive)
            {
                //行情驱动brokertracker用于更新成交侧持仓
                tk.GotTick(k);
            }
        }
        /// <summary>
        /// 发送子委托
        /// 委托状态要么是Submited,要么是reject
        /// 如果底层发单异常,则返回的localid为空 以该字段是否为空来判断底层是否发单异常
        /// </summary>
        /// <param name="o"></param>
        void SendSonOrder(Order o)
        {
            debug("XAP[" + this.Token + "] Send SonOrder: " + o.GetOrderInfo(true), QSEnumDebugLevel.INFO);
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
                //对外触发成交侧委托数据用于记录该成交接口的交易数据
                debug("Send Order Success,LocalID:" + localid, QSEnumDebugLevel.INFO);

            }
            else
            {
                o.Status = QSEnumOrderStatus.Reject;
                debug("Send Order Fail,will notify to client", QSEnumDebugLevel.WARNING);
            }

            //对外输出分解的子委托,用于记录到数据库
            this.LogBrokerOrder(o);
        }

        void CancelSonOrder(Order o)
        {
            Util.Debug("XAP[" + this.Token + "] 取消子委托:" + o.GetOrderInfo(true), QSEnumDebugLevel.INFO);
            XOrderActionField action = new XOrderActionField();
            action.ActionFlag = QSEnumOrderActionFlag.Delete;

            action.ID = o.id.ToString();
            action.BrokerLocalOrderID = o.BrokerLocalOrderID;
            action.BrokerRemoteOrderID = o.BrokerRemoteOrderID;
            action.Exchange = o.BrokerRemoteOrderID.Split(':')[0];//从RemoeOrderID获得交易所信息
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
                    string[] ret = order.BrokerRemoteOrderID.Split(':');
                    //需要设定了OrderSysID 否则只是Exch:空格 
                    if (!string.IsNullOrEmpty(ret[1]))
                    {
                        o.BrokerRemoteOrderID = order.BrokerRemoteOrderID;
                        //按照不同接口的实现 从RemoteOrderID中获得对应的OrderSysID
                        o.OrderSysID = ret[1];
                        //如果不存在该委托则加入该委托
                        if (!remoteOrderID_map.Keys.Contains(order.BrokerRemoteOrderID))
                        {
                            remoteOrderID_map.TryAdd(order.BrokerRemoteOrderID, o);
                        }
                    }
                   

                }
                Util.Debug("更新子委托:" + o.GetOrderInfo(true), QSEnumDebugLevel.INFO);
                tk.GotOrder(o);
                this.LogBrokerOrderUpdate(o);


                //如果子委托是submited则不用更新组合状态
                bool needupdatestatus = (o.Status != QSEnumOrderStatus.Submited);
                if (!needupdatestatus) return;//如果不需要更新委托状态 直接返回

                _splittracker.GotSonOrder(o);
            }
        }

        public override void ProcessTrade(ref XTradeField trade)
        {
            //CTP接口的成交通过远端编号与委托进行关联
            Order o = RemoteID2Order(trade.BrokerRemoteOrderID);
            //
            //Util.Debug("trade info,localid:" + trade.BrokerLocalOrderID + " remoteid:" + trade.BrokerRemoteOrderID, QSEnumDebugLevel.INFO);
            if (o != null)
            {
                //Util.Debug("该成交是本地委托所属成交,进行回报处理", QSEnumDebugLevel.WARNING);
                //子委托对应的成交
                Trade sonfill = (Trade)(new OrderImpl(o));
                //设定价格 数量 以及日期信息
                sonfill.xSize = trade.Size * (trade.Side ? 1 : -1);
                sonfill.xPrice = (decimal)trade.Price;

                sonfill.xDate = trade.Date;
                sonfill.xTime = trade.Time;
                //远端成交编号 在成交侧 需要将该字读填入TradeID 成交明细以TradeID来标识成交记录
                sonfill.BrokerTradeID = trade.BrokerTradeID;
                sonfill.TradeID = trade.BrokerTradeID;

                Util.Debug("获得子成交:" + sonfill.GetTradeDetail(), QSEnumDebugLevel.INFO);
                tk.GotFill(sonfill);
                this.LogBrokerTrade(sonfill);

                _splittracker.GotSonFill(sonfill);
            }
        }

        public override void ProcessOrderError(ref XOrderError error)
        {
            Util.Debug("some error accor in order:" + error.Order.BrokerLocalOrderID, QSEnumDebugLevel.WARNING);
            Order o = LocalID2Order(error.Order.BrokerLocalOrderID);
            if (o != null)
            {
                //
                o.Status = QSEnumOrderStatus.Reject;
                o.Comment = error.Error.ErrorMsg;
                Util.Debug("更新子委托:" + o.GetOrderInfo(true), QSEnumDebugLevel.INFO);
                tk.GotOrder(o);
                this.LogBrokerOrderUpdate(o);//更新日志

                RspInfo info = new RspInfoImpl();
                info.ErrorID = error.Error.ErrorID;
                info.ErrorMessage = error.Error.ErrorMsg;

                _splittracker.GotSonOrderError(o, info);

            }
        }

        public override void ProcessOrderActionError(ref XOrderActionError error)
        {
            Util.Debug("some error happend in order action",QSEnumDebugLevel.WARNING);
            Util.Debug("remoteid:" + error.OrderAction.BrokerRemoteOrderID + " local: " + error.OrderAction.BrokerLocalOrderID + " errorid:" + error.Error.ErrorID.ToString() + " message:" + error.Error.ErrorMsg);
            Order o = LocalID2Order(error.OrderAction.BrokerLocalOrderID);
            if (o != null)
            {
                //委托已经被撤销 不能再撤
                if (error.Error.ErrorID == 26)
                {
                    o.Status = QSEnumOrderStatus.Canceled;
                    o.Comment = error.Error.ErrorMsg;
                    Util.Debug("更新子委托:" + o.GetOrderInfo(true), QSEnumDebugLevel.INFO);

                    tk.GotOrder(o); //Broker交易信息管理器

                    this.LogBrokerOrderUpdate(o);//委托跟新 更新到数据库

                    _splittracker.GotSonOrder(o);//委托分拆器获得子委托,用于对外更新父委托 这里采用委托更新还是委托操作错误更新，再研究
                }
            }
        }
    }
}
