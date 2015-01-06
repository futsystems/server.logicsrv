
using System;
using System.Collections.Generic;
using System.Text;


namespace TradingLib.API
{

    /// <summary>
    /// 获得交易信息并进行处理的接口
    /// 比如成交接口获得委托回报,成交回报,取消回报等
    /// </summary>
    public interface IGotTradingInfo : GotTickIndicator, GotCancelIndicator, GotOrderIndicator, GotFillIndicator, GotPositionIndicator
    {

    }
}