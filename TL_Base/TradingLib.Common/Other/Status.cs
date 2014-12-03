using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 行情路由状态
    /// </summary>
    public class DataFeedRouterStatus
    {
        /// <summary>
        /// 是否处于行情时间段内
        /// </summary>
        public bool IsTickSpan { get; set; }

        /// <summary>
        /// 是否处于MassAlert中
        /// </summary>
        public bool MassAlert { get; set; }

        /// <summary>
        /// 默认行情通道是否处于工作状态
        /// </summary>
        public bool IsDefaultDataFeedLive { get; set; }
    }

    
    /// <summary>
    /// 交易消息交换状态
    /// </summary>
    public class ExchSrvStatus
    {
        /// <summary>
        /// 连接前置机数量
        /// </summary>
        public int FrontCnt { get; set; }

        /// <summary>
        /// 连接终端数量
        /// </summary>
        public int EndPointCnt { get; set; }

        /// <summary>
        /// 累计委托
        /// </summary>
        public int OrdersCnt { get; set; }

        /// <summary>
        /// 累计成交
        /// </summary>
        public int TradesCnt { get; set; }


    
    }
}
