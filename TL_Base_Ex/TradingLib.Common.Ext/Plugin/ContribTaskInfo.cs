using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TradingLib.Common
{
    public struct TaskInfo
    {
        public MethodInfo MethodInfo;
        public TaskAttr Attr;
        public TaskInfo(MethodInfo info, TaskAttr attr)
        {
            MethodInfo = info;
            Attr = attr;
        }
    }
}
