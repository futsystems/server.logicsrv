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

        /// <summary>
        /// 获得持仓键
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
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

        /// 计算结算时 持仓汇总的盯市盈亏 这里累加所有持仓明细的浮动盈亏获得
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static decimal CalcSettleUnRealizedPL(this Position p)
        {
            return p.PositionDetailTotal.Where(pos => !pos.IsClosed()).Sum(pos => pos.PositionProfitByDate);
            //return p.UnrealizedPLByDate * p.oSymbol.Multiple;
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
        public static decimal CalCommission(this Position pos)
        {
            return pos.Trades.Sum(fill => fill.GetCommission());
        }

        /// <summary>
        /// 累加所有持仓明细的逐日平仓盈亏
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal CalCloseProfitByDate(this Position pos)
        {
            return pos.PositionDetailTotal.Sum(pd => pd.CloseProfitByDate);
        }

        /// <summary>
        /// 累加所有持仓明细的逐笔平仓盈亏
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static decimal CalCloseProfitByTrade(this Position pos)
        {
            return pos.PositionDetailTotal.Sum(pd => pd.CloseProfitByTrade);
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
            
            p.AvgPrice = pos.AvgPrice;//持仓均价

            if (pos.DirectionType == QSEnumPositionDirectionType.Long)
            {
                p.Side = true;
            }
            else if(pos.DirectionType == QSEnumPositionDirectionType.Short)
            {
                p.Side = false;
            }

            p.DirectionType = pos.DirectionType;

            /*
             *  开仓金额=今日开仓数量 * 开仓价 * 合约乘数	                        针对当前交易日的所有开仓
             * */
            p.OpenAmount = pos.OpenAmount;
            p.OpenVolume = pos.OpenVolume;
            
            //平仓金额=平仓数量 * 平仓价 * 合约乘数                                针对当前交易日的所有平仓
            p.CloseAmount = pos.CloseAmount;
            p.CloseVolume = pos.CloseVolume;
            

            //持仓成本
            /* 
             持仓成本	上日持仓 * 昨结算价 * 合约乘数 + SUM（今日持仓 * 开仓价 * 合约乘数）
             持仓均价	持仓成本/总持仓/合约乘数 这里价格采用的持仓价，昨仓取的是昨日结算价
             * */
            p.PositionCost = pos.PositionDetailTotal.Where(pd => !pd.IsClosed()).Sum(pd => pd.PositionPrice() * pd.Volume * p.Multiple);
            
            //开仓成本
            /*
                开仓成本	（上日持仓 + 今日持仓）* 开仓价 * 合约乘数	等于逐笔持仓成本
                开仓均价	开仓成本/总持仓/合约乘数	
                指当前持仓对应的开仓成本 平掉的持仓不用计算入内 这里价格采用的是开仓价
            **/
            p.OpenCost = pos.PositionDetailTotal.Where(pd=>!pd.IsClosed()).Sum(pd => pd.OpenPrice * pd.Volume * p.Multiple);
            

            /*
                平仓盈亏	 按照不同的算法 计算出当日的平仓盈亏
               "SUM（平昨量 *（平仓价 - 昨结算价）* 合约乘数）+SUM（平今量 *（平仓价 - 开仓价）* 合约乘数） -- 多头
                SUM（平昨量 *（昨结算价 - 平仓价）* 合约乘数）+SUM（平今量 *（开仓价 - 平仓价）* 合约乘数） -- 空头"	切过第二天后，平仓盈亏原值保留
             * */
            p.ClosePL = pos.ClosedPL;//点数
            p.CloseProfit = pos.ClosedPL * p.Multiple;//盈亏金额

            p.UnRealizedPL = pos.UnRealizedPL;
            p.UnRealizedProfit = p.UnRealizedPL * p.Multiple;


            //总持仓数量 总的有效数量
            p.Position = pos.UnsignedSize;
            //今仓 有效数量 当日平仓会改变该数值
            p.TodayPosition = pos.PositionDetailTodayNew.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume);//当日新开仓 持仓数量
            //昨仓 是初始状态的昨日持仓数量 平仓后 不改变该数值
            p.YdPosition = pos.PositionDetailYdRef.Where(pd => !pd.IsClosed()).Sum(pd => pd.Volume);//昨仓数量


            //保证金
            p.Margin = pos.CalcPositionMargin();

            //持仓成交的手续费 累加所有成交的手续费
            p.Commission = pos.CalCommission();

            
            p.CloseProfitByDate = pos.CalCloseProfitByDate();
            p.CloseProfitByTrade = pos.CalCloseProfitByTrade();

            p.LastSettlementPrice = (pos.LastSettlementPrice != null ? (decimal)pos.LastSettlementPrice : 0);//获得的是持仓对象Position的昨日结算价格 这个价格是从行情产生的
            p.SettlementPrice = pos.LastPrice;// (pos.SettlementPrice != null ? (decimal)pos.SettlementPrice : 0);



            p.Tradingday=0;
            return p;
        }
    }

}
