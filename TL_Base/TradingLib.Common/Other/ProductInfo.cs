using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.Common
{
    /// <summary>
    /// 终端类别信息
    /// 用于标注终端类别 通过客户端注册时提交到服务端
    /// </summary>
    public class ProductInfo
    {
        public const string T_XTRADER_STOCK = "XTrader.Stock";
        public const string T_XTRADER_FUTURE = "XTrader.Future";
    }
}
