using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 实时行情数据
    /// </summary>
    public interface TickData
    {
        char TickType { get; set; }

        /// <summary>
        /// 交易所
        /// </summary>
        string Exchange { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// 成交日期
        /// </summary>
        int Date { get; set; }

        /// <summary>
        /// 成交时间
        /// </summary>
        int Time { get; set; }


        #region 成交数据
        /// <summary>
        /// 成交价格
        /// </summary>
        double TradePrice { get; set; }

        /// <summary>
        /// 成交数量
        /// </summary>
        int TradeSize { get; set; }

        /// <summary>
        /// 累加成交量
        /// </summary>
        int Vol { get; set; }

        /// <summary>
        /// 成交标识
        /// </summary>
        char Flag { get; set; }

        #endregion

        #region 盘口数据
        /// <summary>
        /// 卖价
        /// </summary>
        double AskPrice { get; set; }

        /// <summary>
        /// 卖量
        /// </summary>
        int AskSize { get; set; }

        /// <summary>
        /// 买价
        /// </summary>
        double BidPrice { get; set; }

        /// <summary>
        /// 买量
        /// </summary>
        int BidSize { get; set; }
        #endregion

        #region 统计数据
        double Open { get; set; }
        double High { get; set; }
        double Low { get; set; }
        double PreClose { get; set; }
        double Settlement { get; set; }
        double PreSettlement { get; set; }
        int OI { get; set; }
        int PreOI { get; set; }
        
        #endregion

    }
}
