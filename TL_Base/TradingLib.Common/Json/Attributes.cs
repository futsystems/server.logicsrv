using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common.Json
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = false)]//该特性只能用于类
    public class NoJsonExportAttr:Attribute
    {
    }
}
