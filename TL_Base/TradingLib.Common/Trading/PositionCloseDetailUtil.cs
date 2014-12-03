using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class PositionCloseDetailUtil
    {

        ///// <summary>
        ///// 计算平仓明细盯市平仓盈亏
        ///// </summary>
        ///// <param name="close"></param>
        ///// <returns></returns>
        //public static decimal CalCloseProfitByDate(this PositionCloseDetail close,bool hispos)
        //{
        //    decimal profit = 0;
        //    //平今仓
        //    if (!hispos)
        //    {
        //        //今仓 平仓盈亏为平仓价-平仓价
        //        profit = (close.ClosePrice - close.OpenPrice) * close.CloseVolume * close.oSymbol.Multiple * (close.Side ? 1 : -1);
        //    }
        //    else
        //    {
        //        //昨仓 平仓盈亏为昨结算-平仓价
        //        profit = (close.ClosePrice - close.LastSettlementPrice) * close.CloseVolume * close.oSymbol.Multiple * (close.Side ? 1 : -1);
        //    }

        //    return profit;
        //}

        ///// <summary>
        ///// 计算平仓明细逐笔平仓盈亏
        ///// </summary>
        ///// <param name="close"></param>
        ///// <returns></returns>
        //public static decimal CalCloseProfitByTrade(this PositionCloseDetail close)
        //{
        //    return (close.ClosePrice - close.OpenPrice) * close.CloseVolume * close.oSymbol.Multiple * (close.Side ? 1 : -1);
        //}

        ///// <summary>
        ///// 计算平仓明细的平仓盈亏点数
        ///// </summary>
        ///// <param name="close"></param>
        ///// <param name="hispos"></param>
        ///// <returns></returns>
        //public static decimal CalClosePointByDate(this PositionCloseDetail close, bool hispos)
        //{
        //    decimal point = 0;
        //    //平今仓
        //    if (!hispos)
        //    {
        //        //今仓 平仓盈亏为平仓价-平仓价
        //        point = (close.ClosePrice - close.OpenPrice) * close.CloseVolume * (close.Side ? 1 : -1);
        //    }
        //    else
        //    {
        //        //昨仓 平仓盈亏为昨结算-平仓价
        //        point = (close.ClosePrice - close.LastSettlementPrice) * close.CloseVolume * (close.Side ? 1 : -1);
        //    }
        //    return point;
        //}

        /// <summary>
        /// 获得文字输出
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static string GetPositionCloseStr(this PositionCloseDetail d)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(d.Account + " ");
            sb.Append(d.Symbol + " ");
            sb.Append("Open:" + d.OpenDate.ToString() + " " + d.OpenPrice.ToString() + " ID:" + d.OpenTradeID + " ");
            sb.Append("Close:" + d.CloseDate.ToString() + " " + d.ClosePrice.ToString() + " ID:" + d.CloseTradeID + " ");
            sb.Append(string.Format("{0} {1}手@{2} PreS:{3}", d.Side ? "买平" : "卖平", d.CloseVolume, d.ClosePrice, d.LastSettlementPrice));
            sb.Append(string.Format(" CloseProfit:{0}", d.CloseProfitByDate));
            return sb.ToString();
        }
    }
}
