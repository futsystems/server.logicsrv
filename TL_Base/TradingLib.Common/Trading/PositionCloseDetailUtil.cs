using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class PositionCloseDetailUtil
    {

        /// <summary>
        /// 判断是否是平历史持仓
        /// </summary>
        /// <param name="close"></param>
        /// <returns></returns>
        public static bool IsCloseHist(this PositionCloseDetail close)
        {
            //如果交易日没有标注 表明该持平仓明细为平今仓 如果是平昨仓则tradingday会被赋值成历史持仓的开仓交易日信息
            if (close.Tradingday == 0)
            {
                return false;
            }

            //如果交易日等于对应的结算日则为平当日仓 否则为平历史仓
            if (close.Tradingday == close.Settleday)
            {
                return false;
            }
            return true;
        }

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
            sb.Append(string.Format("{0} {1}手@{2} PreS:{3}", d.Side ? "买入" : "卖出", d.CloseVolume, d.ClosePrice, d.LastSettlementPrice));
            return sb.ToString();
        }
    }
}
