using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using FutSystems.GUI;

namespace FutsMoniter
{
    public delegate void SymbolImplDel(SymbolImpl sym,bool islast);

    public class Utils
    {
        
        public static ArrayList GetTradeableCBList(bool any=true)
        {
            ArrayList list = new ArrayList();
            if (any)
            {
                ValueObject<int> vo = new ValueObject<int>();
                vo.Name = "<Any>";
                vo.Value = 0;
                list.Add(vo);
            }

            ValueObject<int> vo1 = new ValueObject<int>();
            vo1.Name = "可交易";
            vo1.Value = 1;
            list.Add(vo1);

            ValueObject<int> vo2 = new ValueObject<int>();
            vo2.Name = "不可交易";
            vo2.Value = -1;
            list.Add(vo2);
            return list;
        }

        public static string GenSymbol(SecurityFamilyImpl sec, int month)
        {
            switch (sec.Type)
            { 
                case SecurityType.FUT:
                    return GenFutSymbol(sec, month);
                default:
                    return sec.Code;
            }
        }

        static string GenFutSymbol(SecurityFamilyImpl sec, int month)
        {
            if (sec.Exchange.EXCode.Equals("CZCE"))
            {
                return sec.Code + month.ToString().Substring(3);
            }
            else
            {
                return sec.Code + month.ToString().Substring(2);
            }
        }
    }
}
