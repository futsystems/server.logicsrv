using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public delegate void IAccountLiteDel(IAccountLite account);
    public delegate void JsonReplyDel(string jsonstr);
}
