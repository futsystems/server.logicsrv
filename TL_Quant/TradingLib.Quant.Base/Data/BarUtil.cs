using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;



namespace TradingLib.Quant.Base
{
    public class BarUtil
    {
        public static double GetValueForBarElement(Bar b, BarDataType type)
        {
            switch (type)
            {
                case BarDataType.Open:
                    return (double)b.Open;
                case BarDataType.High:
                    return (double)b.High;
                case BarDataType.Low:
                    return (double)b.Low;
                case BarDataType.Close:
                    return (double)b.Close;
                case BarDataType.Volume:
                    return (double)b.Volume;
                default:
                    return 0.0;
            }

        }
        public static BarConstructionType GetBarConstruction(Security symbol, BarConstructionType barConstruction)
        {
            BarConstructionType type = barConstruction;
            if (type != BarConstructionType.Default)
            {
                return type;
            }
            /*
            if (symbol.AssetClass == AssetClass.Forex)
            {
                return BarConstructionType.Mid;
            }**/
            return BarConstructionType.Trades;
        }

 

 


        /// <summary>
        /// 从本地Tick数据源生成某个symbol Freq的Bar数据
        /// 这个函数禁用,Tick数据生成对应的Bar数据,我们可以放入到BarGenerater中单独进行
        /// 实际交易过程中直接由Bar数据生成Barlist来进行
        /// </summary>
        /// <param name="symfreq"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static BarList BarListFromTickList(SecurityFreq symfreq,DateTime start , DateTime end)
        {
            /*
            IDataStore bsotre = QuantGlobals.Access.GetDataSore();


            IList<Tick> klist = bsotre.GetTickStorage(symfreq.Security).Load(Util.ToTLDateTime(start), Util.ToTLDateTime(end), -1,true);
            BarListImpl barlist = new BarListImpl(symfreq.Security.Symbol, symfreq.Frequency.Interval, symfreq.Frequency.Type);

            foreach (Tick k in klist)
            {
                k.symbol = symfreq.Security.Symbol;
                barlist.newTick(k);
            }
            return barlist;**/
            return null;
        }

        public static BarList BarListFromLocalStorage(SecurityFreq symfreq, DateTime start, DateTime end)
        {
            /*
            IDataStore bsotre = QuantGlobals.Access.GetDataSore();
            List<Bar> bl = bsotre.GetBarStorage(symfreq).Load(Util.ToTLDateTime(start), Util.ToTLDateTime(end), -1, true);
            BarListImpl barlist = new BarListImpl(symfreq.Security.Symbol,symfreq.Frequency.Interval, symfreq.Frequency.Type);
            barlist.FillBars(bl);
            return barlist;**/
            return null;
        }

        /*
        public static BarListSingle BarListSingleFromLocalStorage(SecurityFreq symfreq, DateTime start, DateTime end)
        {
            IDataStore bsotre = QuantGlobals.Access.GetDataSore();
            List<Bar> bl = bsotre.GetBarStorage(symfreq).Load(start, end, -1, true);
            BarListSingle barlist = new BarListSingle(symfreq.Security, symfreq.Frequency);
            barlist.FillBars(bl);
            return barlist;
        }**/

        public static IBarData IBarDataFromLocalStorage(SecurityFreq symfreq, DateTime start, DateTime end)
        {
            /*
            BarDataV data = new BarDataV(symfreq.Security, symfreq.Frequency);
            IDataStore bsotre = QuantGlobals.Access.GetDataSore();
            List<Bar> bl = bsotre.GetBarStorage(symfreq).Load(start, end, -1, true);
            data.FillBars(bl);

            return new QListBar(data);**/
            return null;
        }

        

    }
}
