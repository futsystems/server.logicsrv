using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    //DataFeed类型需要与TradeLibFast进行同步(与messagetypes一样进行同步)
    /// <summary>
    /// FastTickSrv中请求启动某个数据通道或者向某个数据通道请求某组合约需要指定对应的通道类型/名称
    /// 与本地exchagne_index -> DataFeed 通过交易所编然后通过 合约的交易所来获得对应的数据通道接口进行操作不同
    /// 在定义SecurityFamily时我们可以预先制定该合约所对应的数据通道/或者按照一定的规则来获得对应的数据通道
    /// </summary>
    public enum QSEnumDataFeedTypes
    {
        DEFAULT,//默认
        CTP,//国内CTP期货 DataFeed
        CTPOPT,//国内CTP期权 DataFeed
        IB,//外盘IBDataFeed
    };
}
