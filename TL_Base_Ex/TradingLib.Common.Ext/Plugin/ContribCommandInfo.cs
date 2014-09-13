using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace TradingLib.Common
{
    /// <summary>
    /// 封装了扩展模块的命令结构
    /// 保存了通过反射获得的MethodInfo以及自定义特性
    /// </summary>
    public struct  ContribCommandInfo
    {
        public MethodInfo MethodInfo;
        public ContribCommandAttr Attr;
        public object Target;
        public ContribCommandInfo(MethodInfo info, ContribCommandAttr attr,object target)
        {
            MethodInfo = info;
            Attr = attr;
            Target = target;
        }
    }
}
