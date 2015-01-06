using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 用于保存插件的特性与类型
    /// 当查找实现接口的类型时 需要加载到plugin对应的map
    /// </summary>
    public class FinderPluginInfo
    {
        public TLAttribute Attribute;
        public Type Type;
    }
}
