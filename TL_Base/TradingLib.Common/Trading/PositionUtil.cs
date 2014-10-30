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
            
            p.Side = pos.isLong;
            
            p.Size = pos.Size;

            /*
             *  开仓金额	SUM（今日开仓数量 * 开仓价 * 合约乘数）	针对当前交易日的所有开仓
                开仓成本	（上日持仓 + 今日持仓）* 开仓价 * 合约乘数	等于逐笔持仓成本
                开仓均价	开仓成本/总持仓/合约乘数	

             * */
            p.OpenAmount = pos.OpenAmount;
            p.OpenVolume = pos.OpenVolume;
            p.OpenAVGPrice = pos.OpenVolume > 0 ? pos.OpenAmount / pos.oSymbol.Multiple / pos.OpenVolume : 0;

            //平仓金额	SUM（平仓数量 * 平仓价 * 合约乘数）	针对当前交易日的所有平仓
            p.CloseAmount = pos.CloseAmount;
            p.CloseVolume = pos.CloseVolume;
            p.CloseAVGPrice = pos.CloseVolume > 0 ? pos.CloseAmount / pos.oSymbol.Multiple / pos.CloseVolume : 0;

            //持仓成本
            /* 
             持仓成本	上日持仓 * 昨结算价 * 合约乘数 + SUM（今日持仓 * 开仓价 * 合约乘数）
             持仓均价	持仓成本/总持仓/合约乘数
             * */
            p.PositionCost = p.UnsignedSize * p.AvgPrice * p.Multiple;//总持仓成本  还有一个是 开仓成本
            //开仓成本
            /*
             * 开仓成本	（上日持仓 + 今日持仓）* 开仓价 * 合约乘数	等于逐笔持仓成本
                开仓均价	开仓成本/总持仓/合约乘数	
            **/
            p.OpenCost = pos.PositionDetailTotal.Sum(pd => pd.OpenPrice * pd.Volume * p.Multiple);
            p.DirectionType = pos.DirectionType;

            /*
                逐日平仓盈亏	
               "SUM（平昨量 *（平仓价 - 昨结算价）* 合约乘数）+SUM（平今量 *（平仓价 - 开仓价）* 合约乘数） -- 多头
                SUM（平昨量 *（昨结算价 - 平仓价）* 合约乘数）+SUM（平今量 *（开仓价 - 平仓价）* 合约乘数） -- 空头"	切过第二天后，平仓盈亏原值保留
             * */
            p.ClosedPL = pos.ClosedPL;
            p.CloseProfit = pos.ClosedPL * pos.oSymbol.Multiple;

            p.UnRealizedPL = pos.UnRealizedPL;
            p.UnRealizedProfit = p.UnRealizedPL * p.Multiple;

            p.Commission = pos.Trades.Sum(f => f.Commission);
            //今仓
            p.TodayPosition = pos.PositionDetailTodayNew.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume);//当日新开仓 持仓数量
            //昨仓 是初始状态的昨日持仓数量还是当前昨日持仓数量
            // p.YdPosition = pos.PositionDetailYdNew.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume);//昨仓数量
            p.YdPosition = pos.PositionDetailYdRef.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume);//昨仓数量
            

            p.LastSettlementPrice = (pos.LastSettlementPrice!=null?(decimal)pos.LastSettlementPrice:0);
            p.SettlementPrice = (pos.SettlementPrice != null?(decimal)pos.SettlementPrice:0);
            p.CloseProfitByDate = 0;
            p.Tradingday=0;
            return p;
        }
    }

}
