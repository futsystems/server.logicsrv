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
        /// 将Broker加载到路由系统
        /// </summary>
        /// <param name="broker"></param>
        public void LoadBroker(IBroker broker)
        {
            //绑定Broker事件到本地函数回调
            broker.GotOrderEvent += new OrderDelegate(Broker_GotOrder);
            broker.GotCancelEvent += new LongDelegate(Broker_GotCancel);
            broker.GotFillEvent += new FillDelegate(Broker_GotFill);

            broker.GotOrderErrorEvent += new OrderErrorDelegate(Broker_GotOrderError);
            broker.GotOrderActionErrorEvent += new OrderActionErrorDelegate(Broker_GotOrderActionError);
            
            if (broker is TLBrokerBase)
            {
                TLBrokerBase brokerbase = broker as TLBrokerBase;
                brokerbase.NewBrokerOrderEvent += new OrderDelegate(LogBrokerOrderEvent);
                brokerbase.NewBrokerOrderUpdateEvent += new OrderDelegate(LogBrokerOrderUpdateEvent);
                brokerbase.NewBrokerFillEvent += new FillDelegate(LogBrokerFillEvent);
                brokerbase.NewBrokerPositionCloseDetailEvent += new Action<PositionCloseDetail>(LogBrokerPositionCloseDetailEvent);
            }
        }

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
                Order no = new OrderImpl(order);
                logger.Info("Reply OrderError To MessageExch:" + no.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage);
                _errorordernotifycache.Write(new OrderErrorPack(no, error));
            }
            else
            {
                logger.Error("Got Invalid OrderError");
            }
        }

        void Broker_GotOrderActionError(OrderAction action, RspInfo error)
        {
            if (action != null)
            {
                Order order = TLCtxHelper.ModuleClearCentre.SentOrder(action.OrderID);
                if (order == null)
                {
                    order = TLCtxHelper.ModuleBrokerRouter.SentRouterOrder(action.OrderID);
                }
                if (order == null)
                {
                    logger.Warn(string.Format("Order:{0} do not exist", action.OrderID));
                }
                if (order.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    logger.Info("Reply OrderActionError To Spliter:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage);
                    //LogRouterOrderUpdate(order);//更新路由侧委托
                    //_splittracker.GotSonOrderError(order, error);
                    _splittracker.GotSonOrderActionError(action, error);
                    return;
                }
                logger.Info("Reply OrderActionError To MessageExch:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage);
                _actionerrorcache.Write(new OrderActionErrorPack(action, error));
            }
            else
            {
                logger.Error("Got Invalid OrderActionError");
            }
        }

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

        void Broker_GotOrder(Order o)
        {
            if (o != null && o.isValid)
            {
                //由路由中心拆分过的委托需要通过分拆器维护,委托回报发送给分拆器
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

        void Broker_GotCancel(long oid)
        {
            logger.Info("Reply Cancel To MessageExch:" + oid.ToString());
            _cancelcache.Write(oid);
        }


    }
}
