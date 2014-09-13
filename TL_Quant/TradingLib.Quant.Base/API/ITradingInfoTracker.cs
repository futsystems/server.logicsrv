using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    public interface ITradingInfoTracker: IOwnedDataSerializableAndRecreatable, IOwnedDataSerializable
    {
        event PositionEventDel SendEntryPosiitonEvent;
        event PositionEventDel SendExitPositionEvent;
        event PositionEventDel SendChangePositionEvent;

        event PositionEventDel SendHistoryPositionDataEvent;

        OrderTracker OrderManager { get; }
        PositionTracker PositionManager { get; }
        List<Trade> TradeManager { get;}

        PositionRound[] GetPositionRoundList();

        List<PositionDataPair> GetOpenPositionData();

        void GotOrder(Order o);
        void GotFill(Trade f);
        void GotCancel(long val);

        /// <summary>
        /// 记录完委托后 转发一个委托事件
        /// </summary>
        event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 记录完取消后 转发一个取消事件
        /// </summary>
        event LongDelegate GotCancelEvent;

        /// <summary>
        /// 记录完一个成交后 转发一个成交事件
        /// </summary>
        event FillDelegate GotFillEvent;

        void GotTick(Tick k);
    }
}
