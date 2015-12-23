using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Tick
    {
        /// <summary>
        /// 行情格式类别
        /// 不同的行情格式类别有不同的序列化和反序列化方式
        /// </summary>
        EnumTickType Type { get; set; }

        /// <summary>
        /// symbol for tick
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// tick date
        /// </summary>
        int Date { get; set; }

        /// <summary>
        /// tick time in 24 format (4:59pm => 1659)
        /// </summary>
        int Time { get; set; }
               
        

        #region Trade
        /// <summary>
        /// size of last trade
        /// </summary>
        int Size { get; set; } 
        /// <summary>
        /// trade price
        /// </summary>
        decimal Trade { get; set; }
        /// <summary>
        /// trade exchange
        /// </summary>
        string Exchange { get; set; }
        #endregion

        #region Quote
        /// <summary>
        /// bid price
        /// </summary>
        decimal BidPrice { get; set; } 
        
        /// <summary>
        /// normal bid size (size/100 for equities, /1 for others)
        /// </summary>
        int BidSize { get; set; } 

        /// <summary>
        /// tick.bs*100 (only for equities)
        /// </summary>
        int StockBidSize { get; set; }

        /// <summary>
        /// bid exchange
        /// </summary>
        string BidExchange { get; set; }

        /// <summary>
        /// offer price
        /// </summary>
        decimal AskPrice { get; set; }
        /// <summary>
        /// normal ask size (size/100 for equities, /1 for others)
        /// </summary>
        int AskSize { get; set; } 
        /// <summary>
        /// tick.os*100 (only for equities)
        /// </summary>
        int StockAskSize { get; set; } 

        /// <summary>
        /// ask exchange
        /// </summary>
        string AskExchange { get; set; }

        #endregion

        /// <summary>
        /// depth of last bid/ask quote
        /// </summary>
        int Depth { get; set; }


        //bool isTrade { get; }
        //bool hasBid { get; }
        //bool hasAsk { get; }
        //bool isFullQuote { get; }
        //bool isQuote { get; }
        //bool isValid { get; }
        //bool isIndex { get; }

        bool hasVol { get; }
        bool hasOI { get; }
        bool hasOpen { get; }
        bool hasPreSettle { get; }
        bool hasHigh { get; }
        bool hasLow { get; }
        bool hasPreOI { get; }

        /// <summary>
        /// index of symbol associated with this tick.
        /// this is not guaranteed to be set
        /// </summary>
        int symidx { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        int Vol { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        decimal Open { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        decimal High { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        decimal Low { get; set; }

        /// <summary>
        /// 昨日持仓量
        /// </summary>
        int PreOpenInterest { get; set; }

        /// <summary>
        /// 持仓量
        /// </summary>
        int OpenInterest { get; set; }

        /// <summary>
        /// 昨日结算价
        /// </summary>
        decimal PreSettlement { get; set; }

        /// <summary>
        /// 结算价
        /// </summary>
        decimal Settlement { get; set; }

        /// <summary>
        /// 涨停价
        /// </summary>
        decimal UpperLimit { get; set; }

        /// <summary>
        /// 跌停价
        /// </summary>
        decimal LowerLimit { get; set; }

        /// <summary>
        /// 昨日结算
        /// </summary>
        decimal PreClose { get; set; }

        /// <summary>
        /// 行情源
        /// </summary>
        QSEnumDataFeedTypes DataFeed { get; set; }
    }

    /// <summary>
    /// 定义了行情传输过程中的内容类别
    /// 根据不同的内容可以解析出不同的行情内容
    /// 避免了每次都发送相同的内容 比如高开低收等不经常变化的变量
    /// Tick行情为了兼容和体现不同交易所的行情报价 需要进行综合处理
    /// 
    /// </summary>
    public enum EnumTickType
    { 
        /// <summary>
        /// 成交信息
        /// </summary>
        TRADE=0,
        /// <summary>
        /// 报价信息
        /// </summary>
        QUOTE=1,
        /// <summary>
        /// 深度行情信息
        /// </summary>
        LEVEL2=2,
        /// <summary>
        /// 统计信息 Open 
        /// </summary>
        SUMMARY=3,
        /// <summary>
        /// 快照数据
        /// </summary>
        SNAPSHOT=4,

    }

    public class InvalidTick : Exception { }

}
