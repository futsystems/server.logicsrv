using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TradingLib.Common
{
    public class ContribEventInfo
    {
        public ContribEventAttr Attr;
        public EventInfo EventInfo;
        public object Target;

        public ContribEventInfo(EventInfo info, ContribEventAttr attr,object target)
        {
            Attr = attr;
            EventInfo = info;
            Target = target;
        }
    }
}
