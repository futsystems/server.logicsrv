using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class SymbolShortCutKey
    {

        string _symcode = "";
        string _symbol = "";
        string _keycode = "";

        public string SymbolShortCode { get { return _symcode; } set { _symcode = value; } }
        public string Symbol { get { return _symbol; } set { _symbol = value; } }
        public string KeyCode { get { return _keycode; } set { _keycode = value; } }

        public SymbolShortCutKey(string symcode, string symbol, string keycode)
        {
            _symcode = symcode;
            _symbol = symbol;
            _keycode = keycode;
        }
    }
}
