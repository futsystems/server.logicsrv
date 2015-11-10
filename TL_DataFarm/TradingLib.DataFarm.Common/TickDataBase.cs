using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.DataFarm.Common
{
    internal class TickDatabase : BinaryDatabase<Tick>
    {
        public TickDatabase()
            : base(TickDatabase.TickDataSize())
        {
        }

        protected override DateTime ReadCurrentDateTime(BinaryReader br)
        {
            if (br.BaseStream.Position != br.BaseStream.Length)
            {
                Tick tickData = new TickImpl();

                
                double d = br.ReadDouble();
                tickData.Datetime = DateTime.FromOADate(d);
                tickData.AskPrice = (decimal)br.ReadDouble();
                tickData.AskSize = br.ReadInt32();
                tickData.BidPrice = (decimal)br.ReadDouble();
                tickData.BidSize = br.ReadInt32();
                tickData.Trade = (decimal)br.ReadDouble();
                tickData.Size = br.ReadInt32();
                tickData.Vol = br.ReadInt32();
                tickData.OpenInterest = br.ReadInt32();
                return tickData.Datetime;
            }
            return DateTime.MinValue;
        }

        protected override Tick ReadItem(byte[] buffer, long startPos)
        {
            Tick result = new TickImpl();
            int num = (int)startPos;
            double d = BitConverter.ToDouble(buffer, num);
            result.Datetime = DateTime.FromOADate(d);
            num += 8;
            result.AskPrice = (decimal)BitConverter.ToDouble(buffer, num);
            num += 8;
            result.AskSize = BitConverter.ToInt32(buffer, num);
            num += 4;
            result.BidPrice = (decimal)BitConverter.ToDouble(buffer, num);
            num += 8;
            result.BidSize = BitConverter.ToInt32(buffer, num);
            num += 4;
            result.Trade = (decimal)BitConverter.ToDouble(buffer, num);
            num += 8;
            result.Size = BitConverter.ToInt32(buffer, num);
            num += 4;
            result.Vol = BitConverter.ToInt32(buffer, num);
            num += 4;
            result.OpenInterest = BitConverter.ToInt32(buffer, num);
            num += 4;
            return result;
        }

        protected override void WriteItem(BinaryWriter bw, Tick tick)
        {
            bw.Write(tick.Datetime.ToOADate());
            bw.Write((double)tick.AskPrice);
            bw.Write(tick.AskSize);
            bw.Write((double)tick.BidPrice);
            bw.Write(tick.BidSize);
            bw.Write((double)tick.Trade);
            bw.Write(tick.Size);
            bw.Write(tick.Vol);
            bw.Write(tick.OpenInterest);



        }

        protected override DateTime GetTime(Tick item)
        {
            return item.Datetime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static int TickDataSize()
        {
            int num = 0;
            num += 8;
            num += 8;
            num += 4;
            num += 8;
            num += 4;
            num += 8;
            num += 4;
            num += 4;
            num += 4;
            return num;
        }
    }
}
