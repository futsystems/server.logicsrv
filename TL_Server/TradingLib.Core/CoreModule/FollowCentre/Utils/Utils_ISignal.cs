using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class Utils_ISignal
    {
        public static string GetInfo(this ISignal signal)
        {
            return string.Format("ID:{0} Token:{1}", signal.ID, signal.Token);
        }

    }

    public static class Utils_IPositionEvent
    {
        public static string GetInfo(this PositionEvent pe)
        { 
            if(pe.EventType == QSEnumPositionEventType.EntryPosition)
            {
                return string.Format("{0} OpenTradeID:{1}",pe.EventType,pe.PositionEntry.TradeID);
            }
            else
            {
                return string.Format("{0} OpenTradeID:{1} CloseTradeID:{2}", pe.EventType, pe.PositionExit.OpenTradeID, pe.PositionExit.CloseTradeID);
            }
        }
    }
}
