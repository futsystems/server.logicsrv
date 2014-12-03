using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public static class PositionDetailUtil
    {

        ///// <summary>
        ///// 判断是否是历史持仓
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <param name="currtradingday"></param>
        ///// <returns></returns>
        //public static bool IsHisPosition(this PositionDetail pos)
        //{
        //    //如果交易日没有标注 表明该持仓明细是当日新开持仓明细 不为历史持仓，如果是历史持仓在写入历史持仓明细表的时候会加入交易日信息
        //    if (pos.Settleday == 0)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        /// <summary>
        /// 获得开仓时间
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static long GetDateTime(this PositionDetail pos)
        {
            return Util.ToTLDateTime(pos.OpenDate, pos.OpenTime);
        }


        /// <summary>
        /// 该持仓是否已经被关闭
        /// 如果开仓量等于平仓量则该持仓关闭
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static bool IsClosed(this PositionDetail pos)
        {
            if (pos.Volume==0)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// 持仓成本
        /// 今仓的持仓成本为 当日开仓价格 昨仓的平仓成本为 结算价格
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal PositionPrice(this PositionDetail pos)
        {
            if (!pos.IsHisPosition)
            {
                return pos.OpenPrice;//开仓价
            }
            else
            {
                return pos.LastSettlementPrice;//昨日结算价
            }
        }

        /// <summary>
        /// 获得文字输出
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string GetPositionDetailStr(this PositionDetail pos)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}-{1} [{2}]",pos.Account,pos.Symbol,pos.Broker));
            sb.Append(" ");
            sb.Append(" T:" + pos.GetDateTime().ToString());
            sb.Append(" S:" + (pos.Side ? "Long" : "Short"));
            sb.Append(string.Format(" {0}@{1}", pos.Volume, pos.OpenPrice));
            sb.Append(" HoldSize:" + pos.Volume +" TotalSize:"+(pos.Volume+pos.CloseVolume).ToString());
            sb.Append(" TradeID:" + pos.TradeID);
            sb.Append(string.Format(" PreS:{0} S:{1}", pos.LastSettlementPrice, pos.SettlementPrice));
            sb.Append(string.Format(" PL:{0} UnPL:{1}", pos.CloseProfitByDate, pos.PositionProfitByDate));
            sb.Append(string.Format(" His:{0}", pos.IsHisPosition ? "YdPos" : "TdPos"));
            return sb.ToString();
        }


        



    }



}
