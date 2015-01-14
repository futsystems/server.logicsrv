using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Mixins.Json
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]//该特性只能用于类
    public class NoJsonExportAttr:Attribute
    {
    }
}
