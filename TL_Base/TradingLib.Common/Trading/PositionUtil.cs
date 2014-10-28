using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class PostionUtils
    {

        ///// <summary>
        ///// 获得posiiton的key值 用于对该positon进行唯一性标识
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <returns></returns>
        public static string GetKey(this Position pos,bool positionside)
        {
            StringBuilder sb = new StringBuilder();
            char d = '-';
            sb.Append(pos.Account);
            sb.Append(d);
            sb.Append(pos.Symbol);
            sb.Append(d);
            sb.Append(positionside?QSEnumPositionDirectionType.Long.ToString():QSEnumPositionDirectionType.Short.ToString());
            return sb.ToString();
        }

        public static string GetPositionKey(this Position pos)
        {
            return pos.Account + "-" + pos.Symbol + "-" + pos.DirectionType.ToString();
        }

        /// <summary>
        /// 计算持仓保证金
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcPositionMargin(this Position p)
        {
            //异化合约按照固定金额来计算
            if (p.oSymbol.SecurityType == SecurityType.INNOV)
            {
                return p.UnsignedSize * (p.oSymbol.Margin + (p.oSymbol.ExtraMargin > 0 ? p.oSymbol.ExtraMargin : 0));//通过固定保证金来计算持仓保证金占用
            }

            //其余品种保证金按照最新价格计算
            if (p.oSymbol.Margin <= 1)
            {
                //需要判断价格的有效性
                decimal m = p.UnsignedSize * p.LastPrice * p.oSymbol.Multiple * p.oSymbol.Margin;
                return m;
            }
            else
                return p.oSymbol.Margin * p.UnsignedSize;
        }

        /// <summary>
        /// 计算结算保证金
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcPositionSettleMargin(this Position p)
        {
            //异化合约按照固定金额来计算
            if (p.oSymbol.SecurityType == SecurityType.INNOV)
            {
                return p.UnsignedSize * (p.oSymbol.Margin + (p.oSymbol.ExtraMargin > 0 ? p.oSymbol.ExtraMargin : 0));//通过固定保证金来计算持仓保证金占用
            }

            //其余品种保证金按照结算价格计算
            if (p.oSymbol.Margin <= 1)
                return p.UnsignedSize * (decimal)p.SettlementPrice * p.oSymbol.Multiple * p.oSymbol.Margin;
            else
                return p.oSymbol.Margin * p.UnsignedSize;
        }
        /// <summary>
        /// 计算平仓盈亏
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcRealizedPL(this Position p)
        {
            return p.ClosedPL * p.oSymbol.Multiple;
        }

        /// <summary>
        /// 计算浮动盈亏
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcUnRealizedPL(this Position p)
        {
            return p.UnRealizedPL * p.oSymbol.Multiple;
        }

        /// <summary>
        /// 计算持仓的结算浮动盈亏 按照设定的结算价作为计算依据
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcSettleUnRealizedPL(this Position p)
        {
            return p.UnrealizedPLByDate * p.oSymbol.Multiple;
        }

        /// <summary>
        /// 计算持仓成本
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcPositionCost(this Position p)
        {
            return p.UnsignedSize * p.AvgPrice * p.oSymbol.Multiple;
        }

        /// <summary>
        /// 计算持仓市值
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcPositionValue(this Position p)
        {
            return p.UnsignedSize * p.LastPrice * p.oSymbol.Multiple;
        }

        /// <summary>
        /// 计算持仓结算市值
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcSettlePositionValue(this Position p)
        {
            return p.UnsignedSize * (decimal)p.SettlementPrice* p.oSymbol.Multiple;
        }

        
        /// <summary>
        /// 获得持仓内所有成交手续费
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal GetCommission(this Position pos)
        {
            return pos.Trades.Sum(fill => fill.GetCommission());
        }
        /// <summary>
        /// 生成PositionEx用于通知客户端
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static PositionEx GenPositionEx(this Position pos)
        {
            PositionEx p = new PositionEx();
            p.Account = pos.Account;
            p.Symbol = pos.Symbol;
            p.Multiple = pos.oSymbol.Multiple;
            p.UnsignedSize = pos.UnsignedSize;//总持仓数量
            p.AvgPrice = pos.AvgPrice;//持仓均价
            p.PositionCost = p.UnsignedSize * p.AvgPrice * p.Multiple;//总持仓金额
            p.Side = pos.isLong;
            p.ClosedPL = pos.ClosedPL;
            p.Size = pos.Size;
            
            p.OpenAmount = pos.OpenAmount;
            p.OpenVolume = pos.OpenVolume;
            p.OpenAVGPrice = pos.OpenVolume > 0 ? pos.OpenAmount / pos.oSymbol.Multiple / pos.OpenVolume : 0;

            p.CloseAmount = pos.CloseAmount;
            p.CloseVolume = pos.CloseVolume;
            p.CloseAVGPrice = pos.CloseVolume > 0 ? pos.CloseAmount / pos.oSymbol.Multiple / pos.CloseVolume : 0;
            p.DirectionType = pos.DirectionType;
            p.CloseProfit = pos.ClosedPL * pos.oSymbol.Multiple;

            p.UnRealizedPL = pos.UnRealizedPL;
            p.UnRealizedProfit = p.UnRealizedPL * p.Multiple;
            return p;
        }
    }

}
