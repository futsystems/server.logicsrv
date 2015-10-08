using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class SecurityUtils_Ex
    {

        static QSEnumActionCheckResult MarketTimeCheck(IExchange exchange,DateTime extime,TradingRange range)
        {
            if (range == null)
            {
                return QSEnumActionCheckResult.RangeNotExist;
            }

            //获得当前交易小节所属交易日 如果该交易日放假
            DateTime tradingday = range.TradingDay(extime);
            if (exchange.IsInHoliday(tradingday))
            {
                return QSEnumActionCheckResult.InHoliday;
            }



            //如果是属于T交易日 则只要在交易小节内且交易所不放假 则都是可以交易的,T+1则需要判断下一个交易日是否交易
            //if (range.SettleFlag == QSEnumRangeSettleFlag.T1)
            //{
            //    if (exchange.IsInHoliday(t1_tradingday))
            //    {
            //        return QSEnumActionCheckResult.InHoliday;
            //    }

            //}
            return QSEnumActionCheckResult.Allowed;
        }
        /// <summary>
        /// 检查品种当前是否可以提交委托
        /// 交易小节完善后 可以通过交易小节具体判断 当前是否是否可以交易或撤单
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static QSEnumActionCheckResult CheckPlaceOrder(this SecurityFamily sec)
        {
            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetTargetTime(DateTime.Now);//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            return MarketTimeCheck(exchange, extime, range);
            
        
        }

        public static QSEnumActionCheckResult CheckCancelOrder(this SecurityFamily sec)
        {
            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetTargetTime(DateTime.Now);//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            return MarketTimeCheck(exchange, extime, range);
            
        }

        /// <summary>
        /// 品种在几分钟后收盘
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static bool CloseAfterTimeSpan(this SecurityFamily sec,int minute)
        {
            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetTargetTime(DateTime.Now);//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            if (range.MarketClose)
            {
                if (Util.FTDIFF(Util.ToTLTime(extime), range.EndTime) / 60 <= minute)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
