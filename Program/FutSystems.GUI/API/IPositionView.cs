using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace FutSystems.GUI
{
    public interface IPositionView
    {
        event DebugDelegate SendDebugEvent;//日志对外输出时间
        event OrderDelegate SendOrderEvent;//发送委托
        event LongDelegate SendCancelEvent;//取消委托
        //event FindSymbolDel FindSecurityEvent;//获得对应的合约信息
        event PositionOffsetArgsDel UpdatePostionOffsetEvent;//对外触发更新止盈止损事件

        LSPositionTracker PositionTracker { get; set; }
        OrderTracker OrderTracker { get; set; }
        void GotTick(Tick k);
        void GotFill(Trade t);
        void GotOrder(Order o);
        void GotCancel(long oid);
    }
}
