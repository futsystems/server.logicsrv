using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouterPassThrough
    {
        /// <summary>
        /// 将某个Broker加载到系统
        /// </summary>
        /// <param name="broker"></param>
        public void LoadBroker(IBroker broker)
        {
            //将Broker的事件触发绑定到本地函数回调
            broker.GotOrderEvent += new OrderDelegate(Broker_GotOrder);
            broker.GotCancelEvent += new LongDelegate(Broker_GotCancel);
            broker.GotFillEvent += new FillDelegate(Broker_GotFill);

            broker.GotOrderErrorEvent += new OrderErrorDelegate(Broker_GotOrderError);
            broker.GotOrderActionErrorEvent += new OrderActionErrorDelegate(Broker_GotOrderActionErrorEvent);

            //获得某个symbol的tick数据
            //broker.GetSymbolTickEvent += new GetSymbolTickDel(Broker_GetSymbolTickEvent);
            //数据路由中Tick事件驱动交易通道中由Tick部分
            //DataFeedRouter.GotTickEvent += new TickDelegate(broker.GotTick);
            //this.GotTickEvent += new TickDelegate(broker.GotTick);

           
            //if (broker is TLBrokerBase)
            //{
            //    TLBrokerBase brokerbase = broker as TLBrokerBase;
            //    brokerbase.NewBrokerOrderEvent += new OrderDelegate(LogBrokerOrderEvent);
            //    brokerbase.NewBrokerOrderUpdateEvent += new OrderDelegate(LogBrokerOrderUpdateEvent);
            //    brokerbase.NewBrokerFillEvent += new FillDelegate(LogBrokerFillEvent);
            //    brokerbase.NewBrokerPositionCloseDetailEvent += new Action<PositionCloseDetail>(LogBrokerPositionCloseDetailEvent);
            //}
            //if (broker is TLBrokerBase)
            //{
            //    TLBrokerBase brokerbase = broker as TLBrokerBase;
            //    //隔夜持仓明细回报
            //    brokerbase.GotHistPositionDetail += new Action<PositionDetail>(Broker_GotHistPositionDetail);
                
            //    //交易数据恢复开始 恢复结束
            //    brokerbase.ExDataSyncEnd += new IConnecterParamDel(Broker_ExDataSyncEnd);
            //    brokerbase.ExDataSyncStart += new IConnecterParamDel(Broker_ExDataSyncStart);

            //    //获得合约数据
            //    brokerbase.GotSymbolEvent += new Action<XSymbol, bool>(brokerbase_GotSymbolEvent);

            //    brokerbase.GotRspInfoEvent += (rspinfo) => { Broker_GotRspInfoEvent(broker, rspinfo); };

            //    brokerbase.GotTransferEvent += new Action<TLBroker, XTransferField, bool>(Broker_GotTransferEvent);
            //    brokerbase.GotAccountInfoEvent += new Action<TLBroker,XAccountInfo, bool>(Broker_GotAccountInfoEvent);
            //}
        }

        //void Broker_GotAccountInfoEvent(TLBroker broker,XAccountInfo arg1, bool arg2)
        //{
        //    BrokerAccountInfo info = new BrokerAccountInfo();
        //    info.CashIn = (decimal)arg1.Deposit;
        //    info.CashOut = (decimal)arg1.WithDraw;
        //    info.CloseProfit = (decimal)arg1.ClosePorifit;
        //    info.Commission = (decimal)arg1.Commission;
        //    info.LastEquity = (decimal)arg1.LastEquity;
        //    info.PositionProfit = (decimal)arg1.PositoinProfit;

        //    BrokerAccountInfoEventArgs arg = new BrokerAccountInfoEventArgs();
        //    arg.AccountInfo = info;
        //    arg.BrokerToken = broker.Token;


        //    TLCtxHelper.EventSystem.FireBrokerAccountInfoEvent(null,arg);
        //}

        //void Broker_GotTransferEvent(TLBroker arg1, XTransferField arg2, bool arg3)
        //{
        //    logger.Info(string.Format("Broker:{0} {1}:{2} {3}",arg1.Token,Util.GetEnumDescription(arg2.TransType),arg2.Amount,(arg2.ErrorID==0?"成功":"失败")));
        //    BrokerTransferEventArgs arg = new BrokerTransferEventArgs();
        //    arg.BrokerToken = arg1.Token;
        //    arg.Amount = (decimal)arg2.Amount;
        //    arg.ErrorID = arg2.ErrorID;
        //    arg.ErrorMsg = arg2.ErrorMsg;
        //    arg.TransType = arg2.TransType;
        //    //通过系统中继触发接口出入金事件
        //    TLCtxHelper.EventSystem.FireBrokerTransferEvent(null, arg);
        //}

        //void brokerbase_GotSymbolEvent(XSymbol arg1, bool arg2)
        //{
        //    logger.Info(string.Format("Symbol:{0} Margin:{1} EntryCommission:{2} ExitCommission:{3} ExitTodayCommission:{4}", arg1.Symbol, arg1.Margin, arg1.EntryCommission, arg1.ExitCommission, arg1.ExitTodayCommission));
        //}

        /// <summary>
        /// 交易接口交易数据同步开始
        /// </summary>
        /// <param name="tocken"></param>
        //void Broker_ExDataSyncStart(string token)
        //{
        //    logger.Info(string.Format("Broker:{0} SyncExData Start", token));
        //    IAccount account = BasicTracker.ConnectorMapTracker.GetAccountForBroker(token);
        //    if (account == null)
        //    {
        //        logger.Info(string.Format("Broker:{0} is not binded with any account", token));
        //        return;
        //    }
        //    //取消交易帐户实时监控
        //    TLCtxHelper.ModuleRiskCentre.DetachAccountCheck(account.ID);
        //}

        /// <summary>
        /// 交易接口交易数据同步完成
        /// </summary>
        /// <param name="tocken"></param>
        //void Broker_ExDataSyncEnd(string token)
        //{
        //    logger.Info(string.Format("Broker:{0} SyncExData End", token));
        //    IAccount account = BasicTracker.ConnectorMapTracker.GetAccountForBroker(token);
        //    if (account == null)
        //    {
        //        logger.Info(string.Format("Broker:{0} is not binded with any account", token));
        //        return;
        //    }
        //    //将交易帐户加入实时监控列表
        //    TLCtxHelper.ModuleRiskCentre.AttachAccountCheck(account.ID);
        //}

        //void Broker_GotRspInfoEvent(IBroker broker, RspInfo obj)
        //{
        //    logger.Info(string.Format("Message from broker:{0} ErrorID:{1} ErrorMessage:{2}", "2", obj.ErrorID, obj.ErrorMessage));
        //    //找到该broker对应的Account然后将对应的消息推送到管理端
        //    IAccount account = BasicTracker.ConnectorMapTracker.GetAccountForBroker(broker.Token);
        //    if (account == null)
        //    {
        //        logger.Info(string.Format("Broker:{0} is not binded with any account",broker.Token));
        //    }
        //    ConnectorConfig cfg = BasicTracker.ConnectorConfigTracker.GetBrokerConfig(broker.Token);

        //    var predicate = account.GetNotifyPredicate();
        //    ManagerNotify notify = new ManagerNotify();
        //    notify.NotifyType = "主帐户交易通道";
        //    notify.ErrorID = obj.ErrorID;
        //    notify.ErrorMessage = string.Format("主帐户{0}:{1}",cfg != null ? (string.Format("{0}-{1}", cfg.Name, cfg.usrinfo_userid)) : "",obj.ErrorMessage);

        //    ManagerNotifyEventArgs arg = new ManagerNotifyEventArgs(predicate, notify);
        //    TLCtxHelper.EventSystem.FireManagerNotifyEvent(this, arg);
        //}



        //void Broker_GotHistPositionDetail(PositionDetail pos)
        //{
        //    //获得该委托的通道对象
        //    IBroker broker = TLCtxHelper.ServiceRouterManager.FindBroker(pos.Broker);

        //    //如果通道对象不存在则直接返回
        //    if (broker == null)
        //    {
        //        logger.Warn(string.Format("Broker:{0} is not registed", pos.Broker));
        //        return;
        //    }

        //    //通过交易通道Broker Token获得绑定的交易帐户
        //    IAccount account = BasicTracker.ConnectorMapTracker.GetAccountForBroker(pos.Broker);
        //    if (account == null)
        //    {
        //        logger.Warn(string.Format("Broker:{0} is not binded with any account", pos.Broker));
        //        return;
        //    }
        //    pos.Account = account.ID;
        //    //设定合约
        //    pos.oSymbol = account.Domain.GetSymbol(pos.Symbol);

        //    TLCtxHelper.ModuleClearCentre.GotPosition(pos);

        //}


        /// <summary>
        /// 交易接口查询某个symbol的当前最新Tick快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        //Tick Broker_GetSymbolTickEvent(string symbol)
        //{
        //    try
        //    {
        //        return TLCtxHelper.ModuleDataRouter.GetTickSnapshot(symbol);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error(PROGRAME + ":get symbol tick snapshot error:" + ex.ToString());
        //        return null;
        //    }
        //}




        /// <summary>
        /// 发送内部产生的委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errortitle"></param>
        void GotOrderErrorNotify(Order o, string errortitle)
        {
            RspInfo info = RspInfoEx.Fill(errortitle);
            o.Comment = info.ErrorMessage;
            Broker_GotOrderError(o, info);
        }


        /// <summary>
        /// 当交易通道有Order错误信息时,进行处理
        /// </summary>
        void Broker_GotOrderError(Order order, RspInfo error)
        {
            if (order != null && order.isValid)
            {

                logger.Info("Reply ErrorOrder To MessageExch:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage);
                _errorordernotifycache.Write(new OrderErrorPack(order, error));
            }
            else
            {
                logger.Error("Got Invalid OrderError");
            }
        }

        /// <summary>
        /// 获得委托操作错误
        /// </summary>
        /// <param name="action"></param>
        /// <param name="error"></param>
        void Broker_GotOrderActionErrorEvent(OrderAction action, RspInfo error)
        {
            if (action != null)
            {

            }
            else
            {
                logger.Error("Got Invalid OrderActionError");
            }
        }



        /// <summary>
        /// 当有成交时候回报msgexch
        /// </summary>
        void Broker_GotFill(Trade trade)
        {
            if (trade != null && trade.isValid)
            {
                //CTP接口的成交通过远端编号与委托进行关联 如果对应的本地委托不存在则该成交数据也会丢弃
                Order o = RemoteID2Order(trade.BrokerRemoteOrderID);
                if (o != null)
                {
                    Trade fill = (Trade)(new OrderImpl(o));
                    //设定价格 数量 以及日期信息
                    fill.xSize = trade.xSize;
                    fill.xPrice = trade.xPrice;

                    fill.xDate = trade.xDate;
                    fill.xTime = trade.xTime;
                    //远端成交编号 在成交侧 需要将该字读填入TradeID 成交明细以TradeID来标识成交记录
                    fill.BrokerTradeID = trade.BrokerTradeID;
                    fill.TradeID = trade.BrokerTradeID;

                    logger.Debug("************** Order SysID:" + o.OrderSysID + " OffSet:" + o.OffsetFlag.ToString() + " TradeID:" + fill.BrokerTradeID + " OffsetFlag:" + fill.OffsetFlag);
                    logger.Info("获得成交:" + fill.GetTradeDetail());

                    logger.Info("Reply Fill To MessageExch:" + fill.GetTradeInfo());
                    _fillcache.Write(new TradeImpl(fill));
                }

                
            }
            else
            {
                logger.Error("Got Invalid Fill");
            }
        }

        /// <summary>
        /// 委托正确回报时回报msgexch
        /// 这里回报需要判断是否需同通过委托拆分器处理,如果是子委托则通过委托拆分器处理
        /// </summary>

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


        void Broker_GotOrder(Order o)
        {
            if (o != null && o.isValid)
            {
                Order localorder = LocalID2Order(o.BrokerLocalOrderID);

                //获得该委托的通道对象
                IBroker broker = TLCtxHelper.ServiceRouterManager.FindBroker(o.Broker);

                //如果通道对象不存在则直接返回
                if (broker == null)
                {
                    logger.Warn(string.Format("Broker:{0} is not registed", o.Broker));
                    return;
                }

                //查看与该通道绑定的交易帐户



                //如果本地委托不存在 则该委托为新委托
                //补充委托信息
                if (localorder == null)
                {
                    localorder = new OrderImpl(o);
                    //通过交易通道Broker Token获得绑定的交易帐户
                    IAccount account = BasicTracker.ConnectorMapTracker.GetAccountForBroker(o.Broker);
                    if (account == null)
                    {
                        logger.Warn(string.Format("Broker:{0} is not binded with any account", o.Broker));
                        return;
                    }

                    localorder.Account = account.ID;
                    localorder.oSymbol = account.Domain.GetSymbol(localorder.Exchange,localorder.Symbol);
                    //设定委托编号
                    TLCtxHelper.ModuleExCore.AssignOrderID(ref localorder);
                    //将路由中心处理获得的id传递给接口侧id
                    //o.id = localorder.id;


                    //将委托保存到map
                    localOrderID_map.TryAdd(localorder.BrokerLocalOrderID, localorder);
                }
                else
                {
                    //更新状态
                    localorder.Status = o.Status;
                    localorder.Comment = o.Comment;
                    localorder.FilledSize = o.FilledSize;
                    localorder.Size = o.Size;
                    
                }

                if (!string.IsNullOrEmpty(o.BrokerRemoteOrderID))//如果远端编号存在 则设定远端编号 同时入map
                {
                    string[] ret = o.BrokerRemoteOrderID.Split(':');
                    //需要设定了OrderSysID 否则只是Exch:空格 
                    if (!string.IsNullOrEmpty(ret[1]))
                    {
                        localorder.BrokerRemoteOrderID = o.BrokerRemoteOrderID;
                        //按照不同接口的实现 从RemoteOrderID中获得对应的OrderSysID
                        localorder.OrderSysID = ret[1];
                        //如果不存在该委托则加入该委托
                        if (!remoteOrderID_map.Keys.Contains(o.BrokerRemoteOrderID))
                        {
                            remoteOrderID_map.TryAdd(o.BrokerRemoteOrderID, localorder);
                        }
                    }
                }
                
                logger.Info("Reply Order To MessageExch:" + localorder.GetOrderInfo());
                _ordercache.Write(localorder);
            }
            else
            {
                logger.Error("Got Invalid Order");
            }
        }

        /// <summary>
        /// 撤单正确回报时回报msgexch
        /// </summary>

        void Broker_GotCancel(long oid)
        {
            logger.Info("Reply Cancel To MessageExch:" + oid.ToString());
            _cancelcache.Write(oid);
        }
    }
}
