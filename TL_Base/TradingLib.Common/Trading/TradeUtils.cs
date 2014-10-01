using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class TradeUtils
    {

        public static decimal GetCommission(this Trade f)
        {
            if (f.Commission >= 0)
                return f.Commission;
            return f.Commission;
        }

        /// <summary>
        /// 返回某个成交的成交金额
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal GetAmount(this Trade f)
        {
            return Math.Abs(f.xsize)* f.xprice * f.oSymbol.Multiple;
        }

        /// <summary>
        /// 获得成交字符串用于保存到文本
        /// </summary>
        /// <param name="f"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string GetTradStr(this Trade f, string delimiter=",")
        {
            string[] trade = new string[] { f.xdate.ToString(), f.xtime.ToString(), f.symbol, (f.side ? "BUY" : "SELL"), f.UnsignedSize.ToString(), f.oSymbol.FormatPrice(f.xprice), f.comment };
            return string.Join(delimiter,trade);
        }

        public static string GetTradeInfo(this Trade f)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(f.side ? "BOT" : "SOD");
            sb.Append(" "+f.OffsetFlag.ToString());
            sb.Append(" " + Math.Abs(f.xsize).ToString());
            sb.Append(" "+f.symbol);
            sb.Append("  @" + f.oSymbol.FormatPrice(f.xprice));
            sb.Append(" C:"+f.Commission.ToString());
            sb.Append(" R:" + f.Broker+"/"+f.BrokerKey);

            return sb.ToString();
        }
    }
}
