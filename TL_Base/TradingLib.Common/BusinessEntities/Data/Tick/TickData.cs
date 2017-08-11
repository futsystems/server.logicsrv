using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    
    public class TickDataImpl:TickData
    {
        /// <summary>
        /// 成交数据
        /// </summary>
        public const char TICKTYPE_TRADE = 'x';

        /// <summary>
        /// 报价数据
        /// </summary>
        public const char TICKTYPE_QUOTE = 'q';

        /// <summary>
        /// 统计数据
        /// </summary>
        public const char TICKTYPE_STATISTIC = 'a';


        /// <summary>
        /// 快照数据下发
        /// </summary>
        public const char TICKTYPE_SNAPSHOT = 's';

        public char TickType { get; set; }

        /// <summary>
        /// 交易所
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 合约
        /// </summary>
        public string Symbol { get; set; }

        /// <summary>
        /// 成交日期
        /// </summary>
        public int Date { get; set; }

        /// <summary>
        /// 成交时间
        /// </summary>
        public int Time { get; set; }


        #region 成交数据
        /// <summary>
        /// 成交价格
        /// </summary>
        public double TradePrice { get; set; }

        /// <summary>
        /// 成交数量
        /// </summary>
        public int TradeSize { get; set; }

        /// <summary>
        /// 累加成交量
        /// </summary>
        public int Vol { get; set; }


        public char Flag { get; set; }
        #endregion

        #region 盘口数据
        /// <summary>
        /// 卖价
        /// </summary>
        public double AskPrice { get; set; }

        /// <summary>
        /// 卖量
        /// </summary>
        public int AskSize { get; set; }

        /// <summary>
        /// 买价
        /// </summary>
        public double BidPrice { get; set; }

        /// <summary>
        /// 买量
        /// </summary>
        public int BidSize { get; set; }
        #endregion


        #region 统计数据
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double PreClose { get; set; }
        public double PreSettlement { get; set; }
        public int OI { get; set; }
        public int PreOI { get; set; }
        public double Settlement { get; set; }
        #endregion

        public static void Write(BinaryWriter writer, TickData k)
        {
            writer.Write(k.TickType);
            writer.Write(k.Exchange);
            writer.Write(k.Symbol);
            writer.Write(k.Date);
            writer.Write(k.Time);
            switch (k.TickType)
            {
                case TickDataImpl.TICKTYPE_TRADE:
                    {
                        writer.Write(k.TradePrice);
                        writer.Write(k.TradeSize);
                        writer.Write(k.Vol);
                        writer.Write(k.Flag);
                        break;
                    }
                case TickDataImpl.TICKTYPE_QUOTE:
                    {
                        writer.Write(k.AskPrice);
                        writer.Write(k.AskSize);
                        writer.Write(k.BidPrice);
                        writer.Write(k.BidSize);
                        break;
                    }
                case TickDataImpl.TICKTYPE_STATISTIC:
                    {
                        
                        writer.Write(k.Open);
                        writer.Write(k.High);
                        writer.Write(k.Low);
                        writer.Write(k.Vol);
                        writer.Write(k.Settlement);
                        writer.Write(k.PreSettlement);
                        writer.Write(k.OI);
                        writer.Write(k.PreOI);
                        writer.Write(k.PreClose);
                        break;
                    }
                case TickDataImpl.TICKTYPE_SNAPSHOT:
                    {
                        writer.Write(k.TradePrice);
                        writer.Write(k.TradeSize);

                        writer.Write(k.AskPrice);
                        writer.Write(k.AskSize);
                        writer.Write(k.BidPrice);
                        writer.Write(k.BidSize);

                        writer.Write(k.Open);
                        writer.Write(k.High);
                        writer.Write(k.Low);
                        writer.Write(k.Vol);
                        writer.Write(k.Settlement);
                        writer.Write(k.PreSettlement);
                        writer.Write(k.OI);
                        writer.Write(k.PreOI);
                        writer.Write(k.PreClose);
                        break;
                    }
                default:
                    break;
            }
        }

        public static TickDataImpl Read(BinaryReader reader)
        {
            TickDataImpl k = new TickDataImpl();
            k.TickType = reader.ReadChar();
            k.Exchange = reader.ReadString();
            k.Symbol = reader.ReadString();
            k.Date = reader.ReadInt32();
            k.Time = reader.ReadInt32();

            switch (k.TickType)
            {
                case TickDataImpl.TICKTYPE_TRADE:
                    {
                        k.TradePrice = reader.ReadDouble();
                        k.TradeSize = reader.ReadInt32();
                        k.Vol = reader.ReadInt32();
                        k.Flag = reader.ReadChar();
                        break;
                    }
                case TickDataImpl.TICKTYPE_QUOTE:
                    {
                        k.AskPrice = reader.ReadDouble();
                        k.AskSize = reader.ReadInt32();
                        k.BidPrice = reader.ReadDouble();
                        k.BidSize = reader.ReadInt32();
                        break;
                    }
                case TickDataImpl.TICKTYPE_STATISTIC:
                    {
                        k.Open = reader.ReadDouble();
                        k.High = reader.ReadDouble();
                        k.Low = reader.ReadDouble();
                        k.Vol = reader.ReadInt32();
                        k.Settlement = reader.ReadDouble();
                        k.PreSettlement = reader.ReadDouble();
                        k.OI = reader.ReadInt32();
                        k.PreOI = reader.ReadInt32();
                        k.PreClose = reader.ReadDouble();
                        break;
                    }
                case TickDataImpl.TICKTYPE_SNAPSHOT:
                    {
                        k.TradePrice = reader.ReadDouble();
                        k.TradeSize = reader.ReadInt32();

                        k.AskPrice = reader.ReadDouble();
                        k.AskSize = reader.ReadInt32();
                        k.BidPrice = reader.ReadDouble();
                        k.BidSize = reader.ReadInt32();

                        k.Open = reader.ReadDouble();
                        k.High = reader.ReadDouble();
                        k.Low = reader.ReadDouble();
                        k.Vol = reader.ReadInt32();
                        k.Settlement = reader.ReadDouble();
                        k.PreSettlement = reader.ReadDouble();
                        k.OI = reader.ReadInt32();
                        k.PreOI = reader.ReadInt32();
                        k.PreClose = reader.ReadDouble();
                        break;
                    }
                default:
                    break;
            }

            return k;
        }
    }
}
