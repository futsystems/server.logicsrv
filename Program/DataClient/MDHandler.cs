using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.MDClient;

namespace DataClient
{
    public class MDHandler : MDHandlerBase
    {
        public event Action<Tick> TickEvent;
        public event Action<Bar, RspInfo, int, bool> BarRspEvent;
        public event Action<List<BarImpl>, RspInfo, int, bool> BarsRspEvent;
        public override void OnRtnTick(Tick k)
        {
            if (TickEvent != null)
            {
                TickEvent(k);
            }
        }

        public override void OnRspQryBar(Bar bar, RspInfo rsp, int requestID, bool isLast)
        {
            if (BarRspEvent != null)
            {
                BarRspEvent(bar, rsp, requestID, isLast);
            }
        }

        public override void OnRspQryBarBin(List<BarImpl> bars, RspInfo rsp, int requestID, bool isLast)
        {
            if (BarsRspEvent != null)
            {
                BarsRspEvent(bars, rsp, requestID, isLast);
            }
        }
    }
}
