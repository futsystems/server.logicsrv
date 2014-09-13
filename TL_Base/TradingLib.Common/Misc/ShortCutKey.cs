using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    public class ShortCutKey
    {
        string _funcode = "";
        string _funname = "";
        string _keycode = "";
        string _defaultkeycode = "";

        public string FunctionCode { get { return _funcode; } set { _funcode = value; } }
        public string FunctionName { get { return _funname; } set { _funname = value; } }
        public string KeyCode { get { return _keycode; } set { _keycode = value; } }
        public string DefaultKeyCode { get { return _defaultkeycode; } set { _defaultkeycode = value; } }

        public ShortCutKey(string funcode, string funname, string keycode, string defaultkeycode)
        {
            FunctionCode = funcode;
            FunctionName = funname;
            KeyCode = keycode;
            DefaultKeyCode = defaultkeycode;
        }

    }
}
