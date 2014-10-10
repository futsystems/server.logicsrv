using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using TradingLib.API;
using TradingLib.Common;



namespace FutSystems.GUI
{

    public delegate SymbolImpl FindSymbolDel(string symbol);


    public class UIGlobals
    {

        public static event FindSymbolDel FindSymbolEvent;
        public static SymbolImpl FindSymbol(string symbol)
        {
            if (FindSymbolEvent != null)
                FindSymbolEvent(symbol);
            return null;
        }
        public static int HeaderHeight = 26;
        public static int RowHeight = 24;

        public static System.Drawing.Color LongSideColor = System.Drawing.Color.Crimson;
        public static System.Drawing.Color ShortSideColor = System.Drawing.Color.LimeGreen;
        public static System.Drawing.Color DefaultColor = System.Drawing.Color.Black;

        public static System.Drawing.Font BoldFont = new Font("微软雅黑", 9, FontStyle.Bold);
        public static System.Drawing.Font DefaultFont = new Font("微软雅黑", 9, FontStyle.Regular);
        public static System.Windows.Forms.Form MainForm=null;

        
    }
}
