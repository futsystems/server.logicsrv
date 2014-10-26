using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public static class PositionDetailsUtil_core
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
            if (pos.Side != f.PositionSide) throw new Exception("position's side do not math with trade's side");

            int closesize = pos.HoldSize() >= remainsize ? remainsize : pos.HoldSize();

            pos.CloseVolume += closesize;//持仓明细的平仓量累加
            remainsize -= closesize;//剩余平仓量累减

            PositionCloseDetail closedetail = new PositionCloseDetail();

            closedetail.Account = pos.Account;
            closedetail.Symbol = pos.Symbol;

            //交易日
            closedetail.Settleday = 0;

            //设定方向
            closedetail.Side = pos.Side;

            //设定开仓时间
            closedetail.OpenDate = pos.OpenDate;
            closedetail.OpenTime = pos.OpenTime;
            //设定平仓时间
            closedetail.CloseDate = f.xdate;
            closedetail.CloseTime = f.xtime;

            //设定开仓平仓价格信息
            closedetail.OpenPrice = pos.OpenPrice;
            closedetail.LastSettlementPrice = pos.LastSettlementPrice;
            closedetail.ClosePrice = f.xprice;
            closedetail.CloseVolume = closesize;

            //传递合约信息
            closedetail.oSymbol = f.oSymbol;

            return closedetail;
        }


    }
}
