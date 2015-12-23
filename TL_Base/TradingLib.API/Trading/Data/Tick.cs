using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface Tick
    {
        /// <summary>
        /// �����ʽ���
        /// ��ͬ�������ʽ����в�ͬ�����л��ͷ����л���ʽ
        /// </summary>
        EnumTickType Type { get; }

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
        bool isFullQuote { get; }
        bool isQuote { get; }
        bool isValid { get; }
        bool isIndex { get; }

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

        int Vol { get; set; }
        decimal Open { get; set; }
        decimal High { get; set; }
        decimal Low { get; set; }
        int PreOpenInterest { get; set; }
        int OpenInterest { get; set; }
        decimal PreSettlement { get; set; }
        decimal Settlement { get; set; }
        /// <summary>
        /// ��ͣ��
        /// </summary>
        decimal UpperLimit { get; set; }

        /// <summary>
        /// ��ͣ��
        /// </summary>
        decimal LowerLimit { get; set; }

        /// <summary>
        /// ���ս���
        /// </summary>
        decimal PreClose { get; set; }

        /// <summary>
        /// ����Դ
        /// </summary>
        QSEnumDataFeedTypes DataFeed { get; set; }
    }

    /// <summary>
    /// ���������鴫������е��������
    /// ���ݲ�ͬ�����ݿ��Խ�������ͬ����������
    /// ������ÿ�ζ�������ͬ������ ����߿����յȲ������仯�ı���
    /// Tick����Ϊ�˼��ݺ����ֲ�ͬ�����������鱨�� ��Ҫ�����ۺϴ���
    /// 
    /// </summary>
    public enum EnumTickType
    { 
        /// <summary>
        /// �ɽ���Ϣ
        /// </summary>
        TRADE=0,
        /// <summary>
        /// ������Ϣ
        /// </summary>
        QUOTE=1,
        /// <summary>
        /// ���������Ϣ
        /// </summary>
        LEVEL2=2,
        /// <summary>
        /// ͳ����Ϣ Open 
        /// </summary>
        SUMMARY=3,
        /// <summary>
        /// ��������
        /// </summary>
        SNAPSHOT=4,

    }

    public class InvalidTick : Exception { }

}
