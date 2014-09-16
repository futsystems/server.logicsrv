using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class PostionUtils
    {

        static string PositionSideStr(bool side)
        {
            if (side)
            {
                return "Long";
            }
            else
            {
                return "Short";
            }
        }

        /// <summary>
        /// 获得posiiton的key值 用于对该positon进行唯一性标识
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static string GetKey(this Position pos,bool positionside)
        {
            StringBuilder sb = new StringBuilder();
            char d = '-';
            sb.Append(pos.Account);
            sb.Append(d);
            sb.Append(pos.Symbol);
            sb.Append(d);
            sb.Append(PositionSideStr(positionside));
            return sb.ToString();
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
                return p.UnsignedSize * p.LastPrice * p.oSymbol.Multiple * p.oSymbol.Margin;
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
            return p.SettleUnrealizedPL * p.oSymbol.Multiple;
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
            return p.UnsignedSize * p.SettlePrice * p.oSymbol.Multiple;
        }

        

        ///// <summary>
        ///// 计算开仓统计
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <returns></returns>
        //public static TradeStatistic GetEntryTradeStatistic(this Position pos)
        //{
        //    int volume = pos.Trades.Where(f => f.IsEntryPosition).Sum(f => f.UnsignedSize);
        //    decimal amount = pos.Trades.Where(f => f.IsEntryPosition).Sum(f => f.GetAmount());
        //    decimal avgprice = volume!=0?(amount / volume / pos.oSymbol.Multiple):0;
        //    return new TradeStatistic(amount, volume, avgprice);
        //}

        ///// <summary>
        ///// 计算平仓统计
        ///// </summary>
        ///// <param name="pos"></param>
        ///// <returns></returns>
        //public static TradeStatistic GetExitTradeStatistic(this Position pos)
        //{
        //    int volume = pos.Trades.Where(f => !f.IsEntryPosition).Sum(f => f.UnsignedSize);
        //    decimal amount = pos.Trades.Where(f => !f.IsEntryPosition).Sum(f => f.GetAmount());
        //    decimal avgprice = volume!=0?(amount / volume / pos.oSymbol.Multiple):0;
        //    return new TradeStatistic(amount, volume, avgprice);
        //}

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

    /// <summary>
    /// 成交统计
    /// </summary>
    //public  class TradeStatistic
    //{
    //    public TradeStatistic(decimal amount, int volume, decimal avgprice)
    //    {
    //        this.Amount = amount;
    //        this.Volume = volume;
    //        this.AVGPrice = avgprice;
    //    }
    //    /// <summary>
    //    /// 均价
    //    /// </summary>
    //    public decimal AVGPrice {get;set;}

    //    /// <summary>
    //    /// 开仓/平仓 金额
    //    /// </summary>
    //    public decimal Amount{get;set;}

    //    /// <summary>
    //    /// 开仓/平仓 累计数量
    //    /// </summary>
    //    public int Volume{get;set;}
    //}
}
