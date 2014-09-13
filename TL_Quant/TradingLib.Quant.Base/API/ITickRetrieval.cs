using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    public interface ITickRetrieval
    {
        // Methods
        IQService GetService();

        bool IsWatching();
        bool SubscribeSymbols(Basket b);
        bool Start();
        bool Stop();

        // Properties
        bool RealTimeDataAvailable { get; }

        /// <summary>
        /// 接口有数据到达时进行的回调
        /// </summary>
        event TickDelegate GotTickEvent;
    }

 

}
