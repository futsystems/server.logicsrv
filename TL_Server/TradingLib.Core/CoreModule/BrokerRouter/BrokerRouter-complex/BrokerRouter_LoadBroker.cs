using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouter
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
            broker.GetSymbolTickEvent += new GetSymbolTickDel(Broker_GetSymbolTickEvent);
            //数据路由中Tick事件驱动交易通道中由Tick部分
            //DataFeedRouter.GotTickEvent += new TickDelegate(broker.GotTick);
            this.GotTickEvent += new TickDelegate(broker.GotTick);

            //将清算中心绑定到交易通道
            broker.ClearCentre = new ClearCentreAdapterToBroker();

            if (broker is TLBrokerBase)
            {
                TLBrokerBase brokerbase = broker as TLBrokerBase;
                brokerbase.NewBrokerOrderEvent += new OrderDelegate(LogBrokerOrderEvent);
                brokerbase.NewBrokerOrderUpdateEvent += new OrderDelegate(LogBrokerOrderUpdateEvent);
                brokerbase.NewBrokerFillEvent += new FillDelegate(LogBrokerFillEvent);
                brokerbase.NewBrokerPositionCloseDetailEvent += new Action<PositionCloseDetail>(LogBrokerPositionCloseDetailEvent);
            }
        }


        #region Broker向本地回报操作
        /// <summary>
        /// 交易接口查询某个symbol的当前最新Tick快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick Broker_GetSymbolTickEvent(string symbol)
        {
            try
            {
                return TLCtxHelper.ModuleDataRouter.GetTickSnapshot(symbol);
            }
            catch (Exception ex)
            {
                logger.Info(PROGRAME + ":get symbol tick snapshot error:" + ex.ToString());
                return null;
            }
        }




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
                if (order.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    logger.Info("Reply ErrorOrder To Spliter:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage);
                    LogRouterOrderUpdate(order);//更新路由侧委托
                    _splittracker.GotSonOrderError(order, error);
                    return;
                }
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
        void Broker_GotFill(Trade fill)
        {
            if (fill != null && fill.isValid)
            {
                if (fill.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    logger.Info("Reply Fill To Spliter:" + fill.GetTradeInfo());
                    _splittracker.GotSonFill(fill);
                    return;
                }
                logger.Info("Reply Fill To MessageExch:" + fill.GetTradeInfo());

                //设置成交滑点
                Trade t = new TradeImpl(fill);
                IAccount account = TLCtxHelper.ModuleAccountManager[t.Account];
                if (account != null)
                {
                    ExStrategy strategy = account.GetExStrategy();
                    //交易参数模板存在 则调整成交价格
                    if (strategy != null)
                    {
                        //如果需要检查限价单 则获得对应的委托
                        if (strategy.LimitCheck)
                        {
                            Order o = TLCtxHelper.ModuleClearCentre.SentOrder(t.id);
                            //委托为空 直接发送成交
                            if (o == null)
                            {
                                goto SENDDIRECT;
                            }
                            //限价单 直接发送成交
                            if (o.isLimit)
                            {
                                goto SENDDIRECT;
                            }
                        }

                        int val = _slipRandom.Next(0, 100);
                        //获得的随机数在设定的范围内 则执行价格修正
                        if (val <= strategy.Probability)
                        {
                            if (t.IsEntryPosition)
                            {
                                t.xPrice = t.xPrice + (t.Side ? 1 : -1) * strategy.EntrySlip * t.oSymbol.SecurityFamily.PriceTick;
                            }
                            else
                            {
                                t.xPrice = t.xPrice + (t.Side ? 1 : -1) * strategy.ExitSlip * t.oSymbol.SecurityFamily.PriceTick;
                            }
                        }
                    }
                }
            SENDDIRECT:
                _fillcache.Write(t);
            }
            else
            {
                string msg = string.Empty;
                if (fill==null)
                {
                    msg = " Fill is none";
                }
                else
                {
                    msg = string.Format("size:{0} price:{1}", fill.xSize, fill.xPrice);
                }
                logger.Error("Got Invalid Fill," + msg);
            }
        }

        /// <summary>
        /// 委托正确回报时回报msgexch
        /// 这里回报需要判断是否需同通过委托拆分器处理,如果是子委托则通过委托拆分器处理
        /// </summary>

        void Broker_GotOrder(Order o)
        {
            if (o != null && o.isValid)
            {
                //这里需要判断,该委托回报是拆分过的子委托还是分帐户侧的委托 如果是拆分过的委托则需要回报给拆分器
                if (o.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    logger.Info("Reply Order To Spliter:" + o.GetOrderInfo());
                    LogRouterOrderUpdate(o);//更新路由侧委托
                    _splittracker.GotSonOrder(o);
                    return;
                }
                Order no = new OrderImpl(o);
                logger.Info("Reply Order To MessageExch:" + no.GetOrderInfo());
                _ordercache.Write(no);
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

        #endregion 

       



       

    }
}
