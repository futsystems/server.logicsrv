using System;
using TradingLib.API;
using TradingLib.Common;



namespace FutSystems.GUI
{
    //public delegate int QryCanOpenPositionLocalDel(string symbol);
    public interface IOrderSender
    {
        event OrderDelegate SendOrderEvent;//发送Order
        event LongDelegate SendCancelEvent;//发送取消
        event DebugDelegate SendDebugEvent;
        event PositionOffsetArgsDel UpdatePostionOffsetEvent;//对外触发止盈止损更新事件
        event QryCanOpenPosition QryCanOpenPositionEvent;
        event QryCanOpenPositionLocalDel QryCanOpenPositionLocalEvent;



        void OnBasketChange();
        void GotTick(Tick k);
        void GotOrder(Order o);
        void GotTrade(Trade fill);
        void GotCancel(long id);

        //void UpdateMaxOpenSize();
        void UpdateMaxOpenSize(int psize);//更新最多可开手数

        Symbol CurrentSecurity { get; }


        SymbolBasket DefaultBasket { get; set; }
        PositionTracker PositionTracker { get; set; }
        OrderTracker OrderTracker { get; set; }

        string Account { get; set; }

        void SetSecurity(Symbol sec);

        

    }
}
