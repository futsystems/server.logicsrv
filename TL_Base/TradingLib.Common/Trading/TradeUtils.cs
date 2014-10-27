using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class TradeUtils
    {

        public static long GetDateTime(this Trade f)
        {
            return Util.ToTLDateTime(f.xdate, f.xtime);
        }

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

        public static string GetTradeDetail(this Trade f)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(f.Account + " " + f.symbol + " ");
            sb.Append(" T:" + f.GetDateTime().ToString());
            sb.Append(" " + f.OffsetFlag.ToString());
            sb.Append(f.side ? " BOT" : " SOD");

            sb.Append(" " + Math.Abs(f.xsize).ToString());
            sb.Append("@" + f.oSymbol.FormatPrice(f.xprice));
            sb.Append(" C:" + f.Commission.ToString());
            sb.Append(" R:" + f.Broker + "/" + f.BrokerKey);

            return sb.ToString();
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

        /// <summary>
        /// 形成新的开仓明细
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static PositionDetail ToPositionDetail(this Trade f)
        {
            PositionDetail pos = new PositionDetailImpl();
            pos.Account = f.Account;
            pos.oSymbol = f.oSymbol;

            pos.OpenDate = f.xdate;
            pos.OpenTime = f.xtime;

            pos.Tradingday = 0;//从新的开仓成交记录生成的持仓明细持仓日期为当前结算日

            pos.Side = f.PositionSide;
            pos.Volume = Math.Abs(f.xsize);
            pos.OpenPrice = f.xprice;
            pos.TradeID = f.BrokerKey;//开仓明细中的开仓成交编号
            pos.HedgeFlag = "";
            

            return pos;
        }
    }
}
