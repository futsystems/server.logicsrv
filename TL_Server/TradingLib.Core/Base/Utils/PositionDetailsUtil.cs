﻿using System;
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
        /// 计算持仓明细的盯市盈亏
        /// 如果没有结算价格 则以当前获得的最新价作为结算价 
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal CalUnRealizedProfitByDate(this PositionDetail pos)
        {
            IAccount account = TLCtxHelper.CmdAccount[pos.Account];
            Position rawpos = account.GetPosition(pos.Symbol, pos.Side);
            if (rawpos.SettlementPrice == null)
            {
                Util.Debug("postion:" + rawpos.GetPositionKey() + " have not got settlementprice,use lastprice:" + rawpos.LastPrice, QSEnumDebugLevel.WARNING);
                rawpos.SettlementPrice = rawpos.LastPrice;
            }
            //获得合约对象
            Symbol sym = pos.oSymbol != null ? pos.oSymbol : BasicTracker.SymbolTracker[pos.Symbol];

            decimal profit = 0;
            //今日新开持仓
            if (!pos.IsHisPosition)
            {
                //今仓 盯市浮动盈亏 = 今结算 - 开仓价
                profit = (pos.SettlementPrice - pos.OpenPrice) * pos.Volume * sym.Multiple * (pos.Side ? 1 : -1);
            }
            else
            {
                //昨仓 盯市浮动盈亏 = 今结算 - 昨结算
                profit = (pos.SettlementPrice - pos.LastSettlementPrice) * pos.Volume * sym.Multiple * (pos.Side ? 1 : -1);
            }

            return profit;
        }

        public static decimal CalMargin(this PositionDetail p)
        {


            //异化合约按照固定金额来计算
            if (p.oSymbol.SecurityType == SecurityType.INNOV)
            {
                return p.Volume * (p.oSymbol.Margin + (p.oSymbol.ExtraMargin > 0 ? p.oSymbol.ExtraMargin : 0));//通过固定保证金来计算持仓保证金占用
            }

            //其余品种保证金按照结算价格计算
            if (p.oSymbol.Margin <= 1)
                return p.Volume * (decimal)p.SettlementPrice * p.oSymbol.Multiple * p.oSymbol.Margin;
            else
                return p.oSymbol.Margin * p.Volume;
        }

    }
}