using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TradingLib.GUI
{
    public class HandlerInfo
    {
        public CallbackAttr Attr;
        public MethodInfo MethodInfo;
        public object Target;

        public HandlerInfo(MethodInfo info, CallbackAttr attr, object target)
        {
            this.Attr = attr;
            this.MethodInfo = info;
            this.Target = target;
        }
    }
}
