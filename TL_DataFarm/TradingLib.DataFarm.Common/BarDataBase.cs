using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.DataFarm.Common
{
    internal class BarDatabase : BinaryDatabase<Bar>
    {
        public BarDatabase()
            : base(BarDatabase.BarDataSize())
        {
        }

        protected override DateTime ReadCurrentDateTime(BinaryReader br)
        {
            if (br.BaseStream.Position != br.BaseStream.Length)
            {
                Bar barData = new BarImpl();
                barData.Ask = br.ReadDouble();
                barData.Bid = br.ReadDouble();
                barData.Close = br.ReadDouble();
                barData.EmptyBar = br.ReadBoolean();
                barData.High = br.ReadDouble();
                barData.Low = br.ReadDouble();
                barData.Open = br.ReadDouble();
                barData.OpenInterest = br.ReadInt64();
                double d = br.ReadDouble();
                barData.BarStartTime = DateTime.FromOADate(d);
                barData.Volume = br.ReadInt64();
                return barData.BarStartTime;
            }
            return DateTime.MinValue;
        }

        protected override Bar ReadItem(byte[] buffer, long startPos)
        {
            Bar barData = new BarImpl();
            int num = (int)startPos;
            barData.Ask = BitConverter.ToDouble(buffer, num);
            num += 8;
            barData.Bid = BitConverter.ToDouble(buffer, num);
            num += 8;
            barData.Close = BitConverter.ToDouble(buffer, num);
            num += 8;
            barData.EmptyBar = BitConverter.ToBoolean(buffer, num);
            num++;
            barData.High = BitConverter.ToDouble(buffer, num);
            num += 8;
            barData.Low = BitConverter.ToDouble(buffer, num);
            num += 8;
            barData.Open = BitConverter.ToDouble(buffer, num);
            num += 8;
            barData.OpenInterest = BitConverter.ToInt64(buffer, num);
            num += 4;
            double d = BitConverter.ToDouble(buffer, num);
            barData.BarStartTime = DateTime.FromOADate(d);
            num += 8;
            barData.Volume = BitConverter.ToInt64(buffer, num);
            num += 8;
            return barData;
        }

        protected override void WriteItem(BinaryWriter bw, Bar bar)
        {
            
            bw.Write(bar.Ask);
            bw.Write(bar.Bid);
            bw.Write(bar.Close);
            bw.Write(bar.EmptyBar);
            bw.Write(bar.High);
            bw.Write(bar.Low);
            bw.Write(bar.Open);
            bw.Write(bar.OpenInterest);
            bw.Write(bar.BarStartTime.ToOADate());
            bw.Write(bar.Volume);
        }

        protected override DateTime GetTime(Bar item)
        {
            return item.BarStartTime;
        }

        private static int BarDataSize()
        {
            int num = 0;
            num += 8;//Ask
            num += 8;//bid
            num += 8;//Close
            num++;
            num += 8;//high
            num += 8;//low
            num += 8;//open
            num += 8;//openintereset
            num += 8;//time
            return num + 8;//volume
        }
    }
}
