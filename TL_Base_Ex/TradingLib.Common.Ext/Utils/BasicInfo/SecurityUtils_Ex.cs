using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public static class SecurityUtils_Ex
    {

        /// <summary>
        /// 判定交易小节是否可交易
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="extime"></param>
        /// <param name="range"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        static QSEnumActionCheckResult MarketTimeCheck(Exchange exchange,DateTime extime,TradingRange range,out int settleday)
        {
            settleday = 0;
            if (range == null)
            {
                settleday = 0;
                return QSEnumActionCheckResult.RangeNotExist;
            }

            //当前为工作日 且当前处于假期 则直接返回 InHoliday 不用判定交易小节到底在哪个交易日
            if (extime.IsWorkDay() && exchange.IsInHoliday(extime))
            {
                settleday = 0;
                return QSEnumActionCheckResult.InHoliday;
            }
            DateTime tradingday=DateTime.Now;
            //规则 1.当前属于假期 则都不交易 已被前面的判定规则覆盖 如果T+1交易日 则从当前日期来判定 而不是通过下一个交易日来判定
            if (exchange.EXCode == "HKEX")
            {
                //T交易小节 获取当前交易小节对应交易日 并做判定
                if (range.SettleFlag == QSEnumRangeSettleFlag.T)
                {
                    tradingday = range.TradingDay(extime);
                    if (exchange.IsInHoliday(tradingday))
                    {
                        settleday = 0;
                        return QSEnumActionCheckResult.InHoliday;
                    }
                }
                else
                { 
                    //T+1交易小节 获取当前主日期 判定是否可以交易
                    DateTime mainday = range.T1MainDay(extime); //夜盘跨越凌晨 则主交易日为当前小节的开始时间 同时要计算交易日 则也以开始时间来计算
                    if (exchange.IsInHoliday(mainday))
                    {
                        settleday = 0;
                        return QSEnumActionCheckResult.InHoliday;
                    }
                    //可以交易 则获得下一个交易日(非假日工作日)
                    tradingday = exchange.NextWorkDayWithoutHoliday(mainday);//同上夜盘跨越凌晨
                }
            }
                //交易日常规判定 当前T+1小节 属于紧挨的下一个交易日，如果对应该交易日不交易则该T+1交易小节不交易
            else
            {
                //获得当前交易小节所属交易日 如果该交易日放假则为Holiday
                tradingday = range.TradingDay(extime);
                if (exchange.IsInHoliday(tradingday))
                {
                    settleday = 0;
                    return QSEnumActionCheckResult.InHoliday;
                }
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
            Exchange exchange = sec.Exchange;
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
            Exchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节
            return MarketTimeCheck(exchange, extime, range,out settleday);
        }

        /// <summary>
        /// 获得品种当前所属交易日
        /// 委托检查时获得交易日判定
        /// 如果挂单在下个交易日成交则无法沿用委托对应的交易日，需要判定该成交对应的交易日
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static int CurrentTradingday(this SecurityFamily sec)
        {
            Exchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节
            if (range == null) return 0;

            int settleday = 0;
            var ret= MarketTimeCheck(exchange, extime, range, out settleday);

            return settleday;
        }

        public static int CurrentTradingday2(this SecurityFamily sec)
        {
            Exchange exchange = sec.Exchange;
            DateTime extime = exchange.GetExchangeTime();//获得交易所时间
            TradingRange range = sec.MarketTime.JudgeRange(extime);//根据交易所时间判定当前品种所属交易小节
            if (range == null) return 0;

            return range.TradingDay(extime).ToTLDate();
        }


        public static QSEnumActionCheckResult CheckCancelOrder(this SecurityFamily sec,out int settleday)
        {
            Exchange exchange = sec.Exchange;
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

            Exchange exchange = sec.Exchange;
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
            Exchange exchange = sec.Exchange;
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
