using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Mixins
{
    public delegate void IntDelegate(int val);
    public delegate void LongDelegate(long val);
    public delegate void StringDelegate(string param);
    public delegate void DebugDelegate(string msg);
    public delegate void LogDelegate(string msg);
    public delegate void VoidDelegate();

    public delegate string StatusDelegate();
}
