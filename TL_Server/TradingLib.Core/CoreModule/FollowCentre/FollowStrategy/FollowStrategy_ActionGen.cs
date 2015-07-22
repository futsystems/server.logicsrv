using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Core
{
    public partial class FollowStrategy
    {
        /// <summary>
        /// 生成某个跟单项的跟单操作
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        FollowAction GenAction(TradeFollowItem item)
        {
            
            switch (item.Stage)
            {
                    //新建跟单项创建委托
                case QSEnumFollowStage.ItemCreated:
                    {
                        Order o = GenOrder(item);
                        if (o != null)
                        {
                            FollowAction action = new FollowAction(item);
                            action.ActionType = QSEnumFollowActionType.PlaceOrder;
                            action.TargetOrders.Add(o);
                            logger.Info("StrategyEngine generate acton for item:" + item.ToString());
                            return action;
                        }
                        return null;
                    }
                //委托已发送等待回报
                case QSEnumFollowStage.FollowOrderSent:
                case QSEnumFollowStage.FollowOrderOpened:
                case QSEnumFollowStage.FollowOrderPartFilled:
                    {
                        //挂单延迟判定
                        //logger.Info(string.Format("Item:{0} SendTime:{1} OpenTime:{2} FillTime{3}", item.ToString(), item.TimeOrderSend, item.TimeOrderOpen, item.TimeOrderFill));
                        if (item.EventType == QSEnumPositionEventType.EntryPosition)
                        {
                            switch (this.Config.EntryPendingThresholdType)
                            {
                                    //按挂单时间决定挂单失效阀值
                                case QSEnumPendingThresholdType.ByTime:
                                    {
                                        //发送时间超过设定阀值 如果选择挂单则需要判断是否获得OrderOpen回报 否则该值为0
                                        if (Util.ToTLTime() - item.TimeOrderSend > this.Config.EntryPendingThresholdValue)
                                        {
                                            logger.Info(string.Format("Item:{0} OpenTime:{1} Now:{2} diff > {3} will cancel order", item.ToString(), item.TimeOrderOpen, Util.ToTLTime(), this.Config.EntryPendingThresholdValue));
                                            
                                            FollowAction action = new FollowAction(item);
                                            action.ActionType = QSEnumFollowActionType.CancelOrder;
                                            action.TargetOrders.Add(item.WorkingOrder);
                                            logger.Info("StrategyEngine generate acton for item:" + item.ToString());
                                            return action;
                                        }
                                        return null;
                                    }
                                case QSEnumPendingThresholdType.ByTicks:
                                    {
                                        Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(item.SignalTrade.Symbol);
                                        if(k !=null && item.WorkingOrder.LimitPrice>0 && k.Trade >0)
                                        {
                                            //当前成交价格与报单价diff
                                            if (Math.Abs(k.Trade - item.WorkingOrder.LimitPrice) > this.Config.EntryPendingThresholdValue * item.SignalTrade.oSymbol.SecurityFamily.PriceTick)
                                            {
                                                logger.Info(string.Format("Item:{0} current trade price:{1} order limitprice:{2} diff will cancel order", item.ToString(), k.Trade, item.WorkingOrder.LimitPrice));

                                                FollowAction action = new FollowAction(item);
                                                action.ActionType = QSEnumFollowActionType.CancelOrder;
                                                action.TargetOrders.Add(item.WorkingOrder);
                                                logger.Info("StrategyEngine generate acton for item:" + item.ToString());
                                                return action;
                                            }
                                        }
                                        return null;
                                    }
                                default:
                                    break;
                                
                            }
                        }
                        return null;
                    }

                //委托全部成交则跟单项完成,生成关闭操作
                case QSEnumFollowStage.FollowOrderFilled:
                    {
                        FollowAction action = new FollowAction(item);
                        action.ActionType = QSEnumFollowActionType.CloseItem;
                        logger.Info("StrategyEngine generate acton for item:" + item.ToString());
                        logger.Info(string.Format("Item:{0} SendTime:{1} OpenTime:{2} FillTime{3}", item.ToString(), item.TimeOrderSend, item.TimeOrderOpen, item.TimeOrderFill));
                        return action;
                    }

                //如果发送的委托已经撤单 那么判定是否需要追单
                case QSEnumFollowStage.FollowOrderCanceled:
                    {
                        //开仓跟单项
                        if (item.EventType == QSEnumPositionEventType.EntryPosition)
                        {
                            if (this.Config.EntryPendingOperationType == QSEnumPendingOperationType.ByMarket)
                            { 
                                
                            }
                            //挂单延迟未成交 取消则委托取消后 返回关闭操作
                            if (this.Config.EntryPendingOperationType == QSEnumPendingOperationType.Cancel)
                            {
                                FollowAction action = new FollowAction(item);
                                action.ActionType = QSEnumFollowActionType.CloseItem;
                                return action;
                            }
                        }
                        //平仓跟单项
                        if (item.EventType == QSEnumPositionEventType.ExitPosition)
                        { 
                            
                        }
                        return null;
                    }
                default:
                    return null;
                    
            }
        }

        Order GenOrder(TradeFollowItem item)
        {
            //开仓
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                //判定下单方向
                bool side = true;
                if (this.Config.FollowDirection == QSEnumFollowDirection.Positive)
                {
                    side = item.SignalTrade.Side;
                }
                else
                {
                    side = !item.SignalTrade.Side;
                }

                //判定下单数量
                int size = Math.Abs(item.SignalTrade.xSize) * this.Config.FollowPower;

                //合约
                Symbol symbol = item.SignalTrade.oSymbol;


                Order o = new OrderImpl(symbol.Symbol, side, size);

                //开平标识
                o.OffsetFlag = QSEnumOffsetFlag.OPEN;

                Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(item.SignalTrade.Symbol);

                //价格
                decimal price = 0;
                switch (this.Config.EntryPriceType)
                {
                    case QSEnumFollowPriceType.MarketPrice://市价
                        price = 0;
                        break;
                    case QSEnumFollowPriceType.OpponentPrice://对手价
                        price = side ? k.AskPrice : k.BidPrice;
                        break;
                    case QSEnumFollowPriceType.HangingPrice:
                        price = side ? (item.SignalTrade.xPrice - this.Config.EntryOffsetTicks * symbol.SecurityFamily.PriceTick) : (item.SignalTrade.xPrice + this.Config.EntryOffsetTicks * symbol.SecurityFamily.PriceTick);
                        break;
                    case QSEnumFollowPriceType.SignalPrice:
                        price = item.SignalTrade.xPrice;
                        break;
                    default:
                        price = 0;
                        break;
                }
                o.LimitPrice = price;
                return o;
            }

            //平仓
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                //平仓需要通过对应的平仓持仓明细来查找开仓成交，并检查对应的开仓成交是否有对应的持仓
                //避免错误平仓
                //比如开仓成交A 没有触发操作，触发后没有成交被撤销，出发后部分成交等
                PositionCloseDetail close = item.PositionEvent.PositionExit;
                if (close == null)
                {
                    logger.Warn("ExitPoitionEvent CloseDetail is null");
                    return null;
                }
                
                //通过开仓成交编号 获得对应的跟单项目
                logger.Info(string.Format("EntryItem:{0} SentSize:{1} FillSize:{2} ExitFillSize:{3}", item.EntryFollowItem.ToString(), item.EntryFollowItem.FollowSentSize, item.EntryFollowItem.FollowFillSize, item.EntryFollowItem.ExitFollowFillSize));

                //判定下单方向 跟单策略不能修改跟单方向 否则后续平仓跟单无法正确处理方向问题
                bool side = true;
                if (this.Config.FollowDirection == QSEnumFollowDirection.Positive)
                {
                    side = item.SignalTrade.Side;
                }
                else
                {
                    side = !item.SignalTrade.Side;
                }

                //判定下单数量 数量需要结合开仓跟单项的敞口持仓来判定
                //理论数量
                int size = Math.Abs(item.SignalTrade.xSize) * this.Config.FollowPower;
                int possize = item.EntryFollowItem.PositionHoldSize;

                size = size < possize ? size : possize;//平仓数量不能超过对应持仓数量

                //合约
                Symbol symbol = item.SignalTrade.oSymbol;


                Order o = new OrderImpl(symbol.Symbol, side, size);

                //开平标识
                o.OffsetFlag = QSEnumOffsetFlag.CLOSE;

                Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(item.SignalTrade.Symbol);
                //价格
                decimal price = 0;
                switch (this.Config.ExitPriceType)
                {
                    case QSEnumFollowPriceType.MarketPrice:
                        price = 0;
                        break;
                    case QSEnumFollowPriceType.OpponentPrice:
                        price = side ? k.AskPrice : k.BidPrice;
                        break;
                    case QSEnumFollowPriceType.HangingPrice:
                        price = side ? (item.SignalTrade.xPrice - this.Config.EntryOffsetTicks * symbol.SecurityFamily.PriceTick) : (item.SignalTrade.xPrice + this.Config.EntryOffsetTicks * symbol.SecurityFamily.PriceTick);
                        break;
                    case QSEnumFollowPriceType.SignalPrice:
                        price = item.SignalTrade.xPrice;
                        break;
                    default:
                        price = 0;
                        break;
                }
                o.LimitPrice = price;
                return o;
            }

            return null;
        }
    }
}
