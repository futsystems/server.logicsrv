using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;


namespace TradingLib.Common.DataFarm
{
    /// <summary>
    /// 封装了扩展模块的命令结构
    /// 保存了通过反射获得的MethodInfo以及自定义特性
    /// </summary>
    public struct DataCommandInfo
    {
        public MethodInfo MethodInfo;
        public DataCommandAttr Attr;
        public object Target;
        public DataCommandInfo(MethodInfo info, DataCommandAttr attr, object target)
        {
            MethodInfo = info;
            Attr = attr;
            Target = target;
        }
    }
}
