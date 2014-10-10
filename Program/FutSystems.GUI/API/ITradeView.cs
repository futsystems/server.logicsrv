using System;
using TradingLib.API;


namespace FutSystems.GUI
{
    public interface ITradeView
    {
        /// <summary>
        /// TradeView获得对应的security用于获得品种的相关信息
        /// </summary>
        event FindSymbolDel FindSecurityEvent;
        /// <summary>
        /// 获得成交回报
        /// </summary>
        /// <param name="t"></param>
        void GotFill(Trade t);

       

    }
}
