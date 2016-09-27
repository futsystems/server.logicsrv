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


        decimal AskPrice2 { get; set; }
        decimal BidPrice2 { get; set; }
        int AskSize2 { get; set; }
        int BidSize2 { get; set; }

        decimal AskPrice3 { get; set; }
        decimal BidPrice3 { get; set; }
        int AskSize3 { get; set; }
        int BidSize3 { get; set; }

        decimal AskPrice4 { get; set; }
        decimal BidPrice4 { get; set; }
        int AskSize4 { get; set; }
        int BidSize4 { get; set; }

        decimal AskPrice5 { get; set; }
        decimal BidPrice5 { get; set; }
        int AskSize5 { get; set; }
        int BidSize5 { get; set; }

        #endregion

        /// <summary>
        /// depth of last bid/ask quote
        /// </summary>
        int Depth { get; set; }

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
        /// �ɽ���
        /// </summary>
        int Vol { get; set; }

        /// <summary>
        /// ���̼�
        /// </summary>
        decimal Open { get; set; }

        /// <summary>
        /// ��߼�
        /// </summary>
        decimal High { get; set; }

        /// <summary>
        /// ��ͼ�
        /// </summary>
        decimal Low { get; set; }

        /// <summary>
        /// ���ճֲ���
        /// </summary>
        int PreOpenInterest { get; set; }

        /// <summary>
        /// �ֲ���
        /// </summary>
        int OpenInterest { get; set; }

        /// <summary>
        /// ���ս����
        /// </summary>
        decimal PreSettlement { get; set; }

        /// <summary>
        /// �����
        /// </summary>
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


        /// <summary>
        /// �������
        /// </summary>
        string UpdateType { get; set; }

        /// <summary>
        /// ������������ر�
        /// </summary>
        bool MarketOpen { get; set; }

        /// <summary>
        /// ���ڶ�ʱ���±��� ����ʵʱ����ϵͳ���˹����̿�����
        /// </summary>
        bool QuoteUpdate { get; set; }
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

        /// <summary>
        /// ʱ�� ���ڸ��µ�ǰ����ʱ��
        /// </summary>
        TIME = 5,

        /// <summary>
        /// ��Ʊ��������
        /// </summary>
        STKSNAPSHOT = 6,

    }

    public class InvalidTick : Exception { }

}
