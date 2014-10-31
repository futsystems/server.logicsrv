using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{

    public static class PositionDetailUtil
    {
        /// <summary>
        /// 用某个平仓成交区平当前持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static PositionCloseDetail ClosePositon(this PositionDetail pos, Trade f, ref int remainsize)
        {
            if (pos.IsClosed()) throw new Exception("can not close the closed position");
            if (f.IsEntryPosition) throw new Exception("entry trade can not close postion");
            if (pos.Account != f.Account) throw new Exception("postion's account do not match with trade");
            if (pos.Symbol != f.symbol) throw new Exception("position's symbol do not math with trade");
            if (pos.Side != f.PositionSide) throw new Exception(string.Format("position's side[{0}] do not math with trade's side[{1}]", pos.Side, f.PositionSide));

            //计算平仓量
            int closesize = pos.Volume >= remainsize ? remainsize : pos.Volume;

            //持仓明细的平仓量累加
            pos.CloseVolume += closesize;
            //持仓量累减
            pos.Volume -= closesize;
            //剩余平仓量累减
            remainsize -= closesize;

            PositionCloseDetail closedetail = new PositionCloseDetailImpl();
            //生成平仓明细数据
            closedetail.Account = pos.Account;
            closedetail.Symbol = pos.Symbol;

            //开仓所在交易日
            closedetail.Tradingday = pos.Tradingday;//如果是今仓则为0，在数据储存时候赋上具体的交易日信息
            //开仓所在结算日,在数据保存时赋上具体的交易日信息

            //设定方向
            closedetail.Side = pos.Side;

            //设定开仓时间
            closedetail.OpenDate = pos.OpenDate;
            closedetail.OpenTime = pos.OpenTime;
            closedetail.OpenTradeID = pos.TradeID;
            //设定平仓时间
            closedetail.CloseDate = f.xdate;
            closedetail.CloseTime = f.xtime;
            closedetail.CloseTradeID = f.BrokerKey;

            //设定开仓平仓价格信息
            closedetail.OpenPrice = pos.OpenPrice;
            closedetail.LastSettlementPrice = pos.LastSettlementPrice;
            closedetail.ClosePrice = f.xprice;
            closedetail.CloseVolume = closesize;

            //传递合约信息
            closedetail.oSymbol = f.oSymbol;

            //计算平仓明细的平仓盈亏和点数 平仓盈亏需要判断是今仓还是昨仓
            closedetail.CloseProfitByDate = closedetail.CalCloseProfitByDate(pos.IsHisPosition());
            closedetail.ClosePointByDate = closedetail.CalClosePointByDate(pos.IsHisPosition());
            closedetail.CloseProfitByTrade = closedetail.CalCloseProfitByTrade();

            //持仓明细的平仓盈亏累加 平仓金额累加
            pos.CloseProfitByDate += closedetail.CloseProfitByDate;
            pos.CloseAmount += closesize * f.xprice * f.oSymbol.Multiple;
            pos.CloseProfitByTrade += closedetail.CloseProfitByTrade;
            return closedetail;
        }


        /// <summary>
        /// 持仓价格
        /// 这个价格有别于开仓价，在结算后持仓价=昨日结算价
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal PositionPrice(this PositionDetail pos)
        {
            if (pos.IsHisPosition())
            {
                return pos.LastSettlementPrice;
            }
            else
            {
                return pos.OpenPrice;
            }
        }
        /// <summary>
        /// 判断是否是历史持仓
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="currtradingday"></param>
        /// <returns></returns>
        public static bool IsHisPosition(this PositionDetail pos)
        {
            //如果交易日没有标注 表明该持仓明细是当日新开持仓明细 不为历史持仓，如果是历史持仓在写入历史持仓明细表的时候会加入交易日信息
            if (pos.Tradingday == 0)
            {
                return false;
            }

            //如果交易日等于对应的结算日则为当日持仓，否则为历史持仓
            if (pos.Tradingday == pos.Settleday)
            {
                //如果持仓明细的交易日与当前交易日不同，则为历史持仓，否则为当日持仓
                return false;
            }
            return true;
        }

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
        /// 当前剩余持仓数量 开仓数量-平仓数量 即为该持仓当前持有的数量
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        //public static int HoldSize(this PositionDetail pos)
        //{
        //    return pos.Volume - pos.CloseVolume;
        //}

        /// <summary>
        /// 持仓成本
        /// 今仓的持仓成本为 当日开仓价格 昨仓的平仓成本为 结算价格
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal HoldPrice(this PositionDetail pos)
        {
            if (!pos.IsHisPosition())
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
            sb.Append(pos.Account + "-" + pos.Symbol);
            sb.Append(" ");
            sb.Append(" T:" + pos.GetDateTime().ToString());
            sb.Append(" S:" + (pos.Side ? "Long" : "Short"));
            sb.Append(string.Format(" {0}@{1}", pos.Volume, pos.OpenPrice));
            sb.Append(" HoldSize:" + pos.Volume +" TotalSize:"+(pos.Volume+pos.CloseVolume).ToString());
            sb.Append(" TradeID:" + pos.TradeID);
            sb.Append(string.Format(" PreS:{0} S:{1}", pos.LastSettlementPrice, pos.SettlementPrice));
            sb.Append(string.Format(" PL:{0} UnPL:{1}", pos.CloseProfitByDate, pos.PositionProfitByDate));
            sb.Append(string.Format(" His:{0}", pos.IsHisPosition() ? "YdPos" : "TdPos"));
            return sb.ToString();
        }


        



    }



}
