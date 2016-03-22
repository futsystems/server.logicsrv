using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class SecurityUtils_Ex
    {


        static QSEnumActionCheckResult MarketTimeCheck(IExchange exchange,DateTime extime,TradingRange range,out int settleday)
        {
            settleday = 0;
            if (range == null)
            {
                settleday = 0;
                return QSEnumActionCheckResult.RangeNotExist;
            }

            //获得当前交易小节所属交易日 如果该交易日放假
            DateTime tradingday = range.TradingDay(extime);
            if (exchange.IsInHoliday(tradingday))
            {
                settleday = 0;
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
            settleday = tradingday.ToTLDate();
            return QSEnumActionCheckResult.Allowed;
        }

        /// <summary>
        /// 判定某个品种几分钟后是否可以下单
        /// </summary>
        /// <param name="sec"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        public static bool IsMarketOpenAfterTime(this SecurityFamily sec,TimeSpan span)
        {
            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime() + span;//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            //交易小节不存在表面 对应时间不在交易时间小节之内
            if (range == null) return false;

            //获得当前交易小节所属交易日 如果该交易日放假则不能交易
            DateTime tradingday = range.TradingDay(extime);
            if (exchange.IsInHoliday(tradingday)) return false;

            return true;
        }



        /// <summary>
        /// 检查品种当前是否可以提交委托
        /// 交易小节完善后 可以通过交易小节具体判断 当前是否是否可以交易或撤单
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static QSEnumActionCheckResult CheckPlaceOrder(this SecurityFamily sec,out int settleday)
        {
            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            return MarketTimeCheck(exchange, extime, range,out settleday);
            
        
        }

        public static QSEnumActionCheckResult CheckCancelOrder(this SecurityFamily sec,out int settleday)
        {
            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节

            return MarketTimeCheck(exchange, extime, range ,out settleday);
            
        }


        /// <summary>
        /// 特殊假日判定
        /// 如果当前允许交易则返回true 遇到特殊假日当前不能交易返回false
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static bool CheckSpecialHoliday(this SecurityFamily sec)
        {
            if (sec.Exchange.Country == Country.CN && sec.Currency == CurrencyType.RMB) return true;//国内交易所没有特殊假日

            IExchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            DateTime nextday = extime.AddDays(1);

            //判定明天是否是特殊假日
            bool special = exchange.IsInSpecialHoliday(nextday);
            if (!special) return true;
            
            //香港交易所判定
            if (sec.Exchange.Country == Country.CN && sec.Currency == CurrencyType.HKD)
            {
                //交易所时间在12点以后 即午后无交易
                if (extime.Hour > 12)
                {
                    return false;
                }
            }
            //新加坡交易所判定
            if (sec.Exchange.Country == Country.SG)
            { 
            
            }

            //美国交易所判定
            if (sec.Exchange.Country == Country.USA)
            { 
            
            }


            return true;
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
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
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
