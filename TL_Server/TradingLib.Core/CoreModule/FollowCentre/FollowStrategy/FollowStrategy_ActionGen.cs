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
                        return null;
                    }
                //委托全部成交则跟单项完成,生成关闭操作
                case QSEnumFollowStage.FollowOrderFilled:
                    {
                        FollowAction action = new FollowAction(item);
                        action.ActionType = QSEnumFollowActionType.CloseItem;
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
                    case QSEnumFollowPriceType.MarketPrice:
                        price = 0;
                        break;
                    case QSEnumFollowPriceType.OpponentPrice:
                        price = side ? k.AskPrice : k.BidPrice;
                        break;
                    case QSEnumFollowPriceType.HangingPrice:
                        price = side ? k.BidPrice : k.AskPrice;
                        break;
                    case QSEnumFollowPriceType.SignalPrice:
                        price = side ? (item.SignalTrade.xPrice - this.Config.EntryOffsetTicks * symbol.SecurityFamily.PriceTick) : (item.SignalTrade.xPrice + this.Config.EntryOffsetTicks * symbol.SecurityFamily.PriceTick);
                        break;
                    default:
                        price = 0;
                        break;
                }

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

                }

                //获得与该平仓成交对应的开仓成交，查询对应的项目

            }

            return null;
        }
    }
}
