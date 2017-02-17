using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public static class Utils_FollowItem
    {
        /// <summary>
        /// 生成某个跟单项目的所有委托信息
        /// 首先将跟单项目的所有触发的委托生成对应的信息对象
        /// 然后再通过成交查找的方式 获得该委托对应的成交项并转换成信息对象
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static FollowItemOrderInfo[] GenFollowItemOrderInfos(this FollowItem item)
        {
            FollowItemOrderInfo[] orders = item.Orders.Select(o => o.ToFollowItemOrder()).ToArray();
            foreach (var o in orders)
            {
                FollowItemTradeInfo[] trades = item.Trades.Where(f => f.id == o.OrderID).Select(f => f.ToFollowItemTrade()).ToArray();
                o.Trades = trades;
            }
            return orders;
        }

        public static FollowItemDetail GenFollowItemDetail(this FollowItem item)
        {
            //开仓跟单项目
            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                FollowItemDetail detail = new FollowItemDetail();
                detail.FollowKey = item.FollowKey;
                detail.PositionHoldSize = item.PositionHoldSize;
                detail.TotalSlip = item.TotalSlip + item.ExitFollowItems.Sum(f => f.TotalSlip);
                detail.TotalRealizedPL = item.ExitFollowItems.Sum(f => f.FollowProfit);

                //绑定明细 开仓成交数据
                detail.EntrySignalTrade = item.SignalTrade.ToFollowItemSignalTrade();
                detail.EntrySignalTrade.Orders = item.GenFollowItemOrderInfos();

                //绑定 成交触发的平仓跟单项
                detail.ExitSignalTrades = item.ExitFollowItems.Where(exit=>exit.TriggerType == QSEnumFollowItemTriggerType.SigTradeTrigger).Select(exit => {FollowItemSignalTradeInfo trade = exit.SignalTrade.ToFollowItemSignalTrade();trade.Orders = exit.GenFollowItemOrderInfos();return trade;}).ToArray();
                detail.ExitManualTrigger = item.ExitFollowItems.Where(exit => exit.TriggerType == QSEnumFollowItemTriggerType.ManualExitTrigger).Select(exit => new FollowItemManualTriggerInfo { Side = exit.FollowSide, Size = exit.FollowSize, Symbol = exit.Symbol, Price = 0, Orders = exit.GenFollowItemOrderInfos() }).ToArray();
                return detail;
            }
            else
            {
                return item.EntryFollowItem.GenFollowItemDetail();
            }
        }

        /// <summary>
        /// 生成开仓跟单项信息
        /// 用于管理端显示
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static EntryFollowItemStruct GenEntryFollowItemStruct(this FollowItem item)
        {
            if (item.EventType == QSEnumPositionEventType.ExitPosition)
            {
                throw new ArgumentException("item need EntryFollowItem");
            }

            EntryFollowItemStruct entryitem = new EntryFollowItemStruct();
            entryitem.StrategyID = item.Strategy.ID;
            entryitem.TriggerType = item.TriggerType;
            entryitem.Symbol = item.Symbol;
            entryitem.Filter_SourceSignal = item.SourceSignal;
            entryitem.Filter_PositonHold = item.PositionHoldSize;
            entryitem.Filter_Profit = item.ExitFollowItems.Sum(f => f.FollowProfit);

            entryitem.FollowKey = item.FollowKey;
            entryitem.Side = item.FollowSide;
            entryitem.PriceFormat = item.SignalTrade.oSymbol.SecurityFamily.GetPriceFormat();

            entryitem.FollowSentSize = item.FollowSentSize;
            entryitem.FollowFillSize = item.FollowFillSize;
            entryitem.FollowAvgPrice = item.FollowPrice;
            entryitem.Stage = item.Stage;
            entryitem.FollowSlip = item.TotalSlip;

            //累计滑点
            entryitem.TotalSlip = item.TotalSlip + item.ExitFollowItems.Sum(f => f.TotalSlip);
            entryitem.TotalRealizedPL = item.ExitFollowItems.Sum(f => f.FollowProfit);
            entryitem.PositionHoldSize = item.PositionHoldSize;
            entryitem.Comment = item.Comment;
            switch (item.TriggerType)
            {
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    {
                        entryitem.SignalID = item.Signal.ID;
                        entryitem.SignalToken = item.Signal.Token;
                        entryitem.OpenTradeID = item.PositionEvent.PositionEntry.TradeID;
                        entryitem.SigPrice = item.SignalTrade.xPrice;
                        entryitem.SigSize = item.SignalTrade.UnsignedSize;
                        break;
                    }
                default:
                    break;
            }
            return entryitem;
        }

        /// <summary>
        /// 生成平仓跟单项信息 用于管理端显示
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static ExitFollowItemStruct GenExitFollowItemStruct(this FollowItem item)
        {

            if (item.EventType == QSEnumPositionEventType.EntryPosition)
            {
                throw new ArgumentException("item need ExitFollowItem");
            }


            ExitFollowItemStruct exit = new ExitFollowItemStruct();
            exit.StrategyID = item.Strategy.ID;
            exit.TriggerType = item.TriggerType;
            exit.Symbol = item.Symbol;
            exit.Filter_SourceSignal = item.SourceSignal;
            exit.Filter_PositonHold = item.EntryFollowItem.PositionHoldSize;
            exit.Filter_Profit = item.EntryFollowItem.ExitFollowItems.Sum(f => f.FollowProfit);

            exit.PriceFormat = item.EntryFollowItem.SignalTrade.oSymbol.SecurityFamily.GetPriceFormat();

            exit.FollowKey = item.FollowKey;
            exit.Side = item.FollowSide;
            exit.EntryFollowKey = item.EntryFollowItem.FollowKey;
            
            

            exit.FollowSentSize = item.FollowSentSize;
            exit.FollowFillSize = item.FollowFillSize;
            exit.FollowAvgPrice = item.FollowPrice;
            exit.FollowSlip = item.TotalSlip;
            exit.FollowProfit = item.FollowProfit;//跟单平仓盈亏
            exit.Stage = item.Stage;
            exit.Comment = item.Comment;
            switch (item.TriggerType)
            {
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    {
                        exit.CloseTradeID = item.PositionEvent.PositionExit.CloseTradeID;
                        exit.SigPrice = item.SignalTrade.xPrice;
                        exit.SigSize = item.SignalTrade.UnsignedSize;
                        break;
                    }
                default:
                    break;
            }
            

            return exit;
        }

        /// <summary>
        /// 开仓跟单项与平仓跟单项目执行双向绑定
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="exit"></param>
        public static void Link(this FollowItem entry, FollowItem exit)
        {
            entry.NewExitFollowItem(exit);
            exit.NewEntryFollowItem(entry);
        }

        public static FollowItemData ToFollowItemData(this FollowItem item)
        {
            FollowItemData data = new FollowItemData();

            data.FollowKey = item.FollowKey;
            data.StrategyID = item.Strategy.ID;
            data.TriggerType = item.TriggerType;
            data.Stage = item.Stage;

            data.Exchange = item.Exchange;
            data.Symbol = item.Symbol;
            data.FollowSide = item.FollowSide;
            data.FollowSize = item.FollowSize;
            data.FollowPower = item.FollowPower;
            data.EventType = item.EventType;
            data.Comment = item.Comment;
            switch (item.TriggerType)
            {
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    {
                        data.SignalID = item.Signal.ID;
                        data.SignalTradeID = item.SignalTrade.TradeID;
                        if (item.EventType == QSEnumPositionEventType.EntryPosition)
                        {
                            data.OpenTradeID = item.PositionEvent.PositionEntry.TradeID;
                        }
                        if (item.EventType == QSEnumPositionEventType.ExitPosition)
                        {
                            data.OpenTradeID = item.PositionEvent.PositionExit.OpenTradeID;
                            data.CloseTradeID = item.PositionEvent.PositionExit.CloseTradeID;
                        }
                        break;
                    }
                default:
                    break;
            }

            return data;
        }

        public static string GetLocalKey(this FollowItem entry)
        {
            switch (entry.TriggerType)
            { 
                case QSEnumFollowItemTriggerType.SigTradeTrigger:
                    return entry.PositionEvent.PositionEntry.TradeID;
                default:
                    return null;
            }
        }

        public static FollowExecution ToFollowExecution(this FollowItem entry)
        {
            FollowExecution ex = new FollowExecution();
            ex.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            ex.StrategyID = entry.Strategy.ID;
            ex.FollowKey = entry.FollowKey;
            ex.SourceSignal = entry.SourceSignal;
            ex.SignalInfo = entry.SignalInfo;
            ex.Exchange = entry.Exchange;
            ex.Symbol = entry.Symbol;
            ex.Side = entry.FollowSide;
            ex.Size = entry.FollowSize;
            ex.OpenTime = entry.Trades.First().xTime;
            ex.OpenAvgPrice = entry.FollowPrice;
            ex.OpenSlip = entry.TotalSlip;
            ex.CloseTime = entry.ExitFollowItems.Last().Trades.Last().xTime;
            ex.CloseAvgPrice = entry.ExitFollowItems.Sum(exit => exit.FollowPrice * exit.FollowSize) / entry.ExitFollowItems.Sum(exit => exit.FollowSize);//计算统一平仓均价
            ex.CloseSlip = entry.ExitFollowItems.Sum(exit => exit.TotalSlip);
            ex.RealizedPL = entry.ExitFollowItems.Sum(f => f.FollowProfit) * entry.Trades.First().oSymbol.SecurityFamily.Multiple;
            ex.Commission = entry.Trades.Sum(t => t.GetCommission()) + entry.ExitFollowItems.Sum(exit => exit.Trades.Sum(t => t.GetCommission()));
            ex.Profit = ex.RealizedPL - ex.Commission;
            return ex;
        }
    }
}
