using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Quant.Base
{
    public class ResultUtils
    {

        public static double GetExposure(Position position, Security security)
        {
            /*
            if (security.Margin > 1)
                return position.UnsignedSize * (double)security.Margin;//如果保证金>1则用手数*保证金来计算总的风险值
            else
            {
                //手数*合约价格*合约乘数*保证金比例
                return (double)(position.UnsignedSize * position.AvgPrice * security.Multiple);
            }**/
            return (double)(position.UnsignedSize * position.AvgPrice * security.Multiple);
            
        }
        /// <summary>
        /// 获得某个仓位的当前价值 
        /// 平掉该仓位 所获得的 账户收益 保证金占用 以及 unrealizedPL
        /// </summary>
        /// <param name="position"></param>
        /// <param name="security"></param>
        /// <returns></returns>
        public static double GetCurrentValue(Position position, Security security)
        {
            return (double)position.UnRealizedPL * security.Multiple + GetPositonMarginUsed(position, security);
            //持仓手数*当前价格*合约乘数 就为当前持有合约的价值
            //return (double)(position.UnsignedSize * position.LastPrice*security.Multiple);
        }
        /// <summary>
        /// 计算由于某个成交所获得的利润或亏损
        /// </summary>
        /// <param name="trade"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static double GetRealizedPL(Trade trade, double cost)
        { 
            //开仓/加仓不会实现平仓利润 只有减仓或者直接平仓才会实现平仓利润
            if (trade.PositionOperation == QSEnumPosOperation.DelPosition || trade.PositionOperation == QSEnumPosOperation.ExitPosition)
            {
                //通过成交价格 与 持仓成本来计算 实现的利润
                
                return -1 * ((double)(trade.xprice - (decimal)cost) * trade.xsize);
            }
            return 0;
        }
        /// <summary>
        /// 计算某个成交的保证金 占用 释放 数额 负数 占用 正数 释放
        /// </summary>
        /// <param name="fill"></param>
        /// <param name="sec"></param>
        /// <param name="positionCost"></param>
        /// <returns></returns>
        public static double GetTradeMarginChange(Trade fill, Security sec,double positionCost)
        {
            if (fill.PositionOperation == QSEnumPosOperation.AddPosition || fill.PositionOperation == QSEnumPosOperation.EntryPosition)
            {
                if (sec.Margin > 1)
                {
                    return (double)(sec.Margin * fill.UnsignedSize)*-1;
                }
                else
                {
                    return (double)(fill.UnsignedSize * fill.xprice * sec.Multiple * sec.Margin)*-1;
                }
            }
            else
            {
                if (sec.Margin > 1)
                {
                    return (double)(sec.Margin * fill.UnsignedSize);
                }
                else
                {
                    return (double)(fill.UnsignedSize * (decimal)positionCost * sec.Multiple * sec.Margin);
                }
            }
            
        }

        public static bool TradeUseOrReleaseMargin(Trade fill)
        {
            if (fill.PositionOperation == QSEnumPosOperation.AddPosition || fill.PositionOperation == QSEnumPosOperation.EntryPosition)
                return true;
            else
                return false;
        }
        public static double GetPositonMarginUsed(PositionRound pr,Security sec)
        { 
            if(sec.Margin >1)
            {
                return Math.Abs((double)sec.Margin*pr.Size);
            
            }
            else
            {
                return Math.Abs((double)(pr.Size * pr.EntryPrice * sec.Multiple *sec.Margin));
            }
        }

        public static double GetPositonMarginUsed(Position position, Security sec)
        {
            if (sec.Margin > 1)
            {
                return Math.Abs((double)sec.Margin * position.Size);

            }
            else
            {
                return Math.Abs((double)(position.Size * position.AvgPrice * sec.Multiple * sec.Margin));
            }
        }


        public static void ClosePosition(PositionDataPair data, double closePrice,DateTime datetime, out Position position, out PositionRound pr,out Trade trade)
        {
            position = data.Position;
            pr = data.PositionRound;
            int beforesize = position.UnsignedSize;

            trade = new TradeImpl();
            trade.xprice = (decimal)closePrice;
            trade.xsize = position.FlatSize;
            trade.xdate = Util.ToTLDate(datetime);
            trade.xtime = Util.ToTLTime(datetime);
            trade.symbol = position.symbol;

            position.Adjust(trade);

            int aftersize = position.UnsignedSize;



                 
        }
 

    }
}
