﻿using System;
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
            return Util.ToTLDateTime(f.xDate, f.xTime);
        }

        /// <summary>
        /// 获得某个成交的手续费
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal GetCommission(this Trade f)
        {
            return f.Commission >= 0 ? f.Commission : 0;
        }

        /// <summary>
        /// 返回某个成交的成交金额
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public static decimal GetAmount(this Trade f)
        {
            return Math.Abs(f.xSize) * f.xPrice * f.oSymbol.Multiple;
        }

        /// <summary>
        /// 获得成交字符串用于保存到文本
        /// </summary>
        /// <param name="f"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string GetTradStr(this Trade f, string delimiter=",")
        {
            string[] trade = new string[] { f.xDate.ToString(), f.xTime.ToString(), f.Symbol, (f.Side ? "BUY" : "SELL"), f.UnsignedSize.ToString(), f.oSymbol.FormatPrice(f.xPrice)};
            return string.Join(delimiter,trade);
        }

        public static string GetTradeDetail(this Trade f)
        {
            Util.Debug("TradeDetail~~~~~~~~~~~~ 00");
            StringBuilder sb = new StringBuilder();
            sb.Append(f.Account + " " + f.Symbol + " ");
            Util.Debug("~~~~~~~~~~~~ 00-1");
            sb.Append(" T:" + f.GetDateTime().ToString());
            Util.Debug("~~~~~~~~~~~~ 00-2");
            sb.Append(" " + f.OffsetFlag.ToString());
            Util.Debug("~~~~~~~~~~~~ 00-3");
            sb.Append(f.Side ? " BOT" : " SOD");
            Util.Debug("~~~~~~~~~~~~ 01");
            sb.Append(" " + Math.Abs(f.xSize).ToString());
            Util.Debug("~~~~~~~~~~~~ 02");
            sb.Append("@" + f.oSymbol.FormatPrice(f.xPrice));
            Util.Debug("~~~~~~~~~~~~ 03");
            sb.Append(" C:" + f.Commission.ToString());
            Util.Debug("~~~~~~~~~~~~ 04");
            sb.Append(string.Format("Broker:{0} BLocalID:{1} BRemoteID:{2} TradeID:{3} OrderSysID:{4} Breed:{5}", f.Broker, f.BrokerLocalOrderID, f.BrokerRemoteOrderID, f.TradeID, f.OrderSysID, f.Breed));
            //sb.Append(" R:" + f.Broker + "/" + f.TradeID);

            return sb.ToString();
        }
        public static string GetTradeInfo(this Trade f)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(f.Side ? "BOT" : "SOD");
            sb.Append(" "+f.OffsetFlag.ToString());
            sb.Append(" " + Math.Abs(f.xSize).ToString());
            sb.Append(" " + f.Symbol);
            sb.Append("  @" + f.oSymbol.FormatPrice(f.xPrice));
            sb.Append(" C:"+f.Commission.ToString());
            sb.Append(" R:" + f.Broker+"/"+f.TradeID);

            return sb.ToString();
        }
    }
}
