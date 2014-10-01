using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Quant.Base
{
    /// <summary>
    /// 指标构造可以使用的参数类型
    /// </summary>
    public enum ConstructorArgumentType
    {
        String,//字符串
        Integer,//整数
        Double,//小数
        BarElement,//Bar数据项
        Enum,//枚举
        ChartPane,//chartPane
        UserDefined,//用户自定义
        Boolean,//布尔
        Int64//长整
    }

 

}
