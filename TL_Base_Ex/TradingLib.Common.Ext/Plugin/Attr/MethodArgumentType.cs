using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 方法参数
    /// </summary>
    public enum QSEnumMethodArgumentType
    {
        ISession,//消息系统的支持ISession的对象
        Double,//小数
        Integer,//整数
        Int64,//长整
        String,//字符串
        Enum,//枚举
        Boolean,//布尔
        Decimal,//
        UserDefined//用户自定义
    }
}
