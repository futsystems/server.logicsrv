using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;

namespace TradingLib.Common.DataFarm
{
    internal class BarFields
    {
        public int Tradingday { get; set; }
        public long StarTtime { get; set; }
        public string Symbol {get;set;}
        public double Open { get; set; }
        public double High { get; set; }
        public int Intervaltype { get; set; }


    }

    public class MBar : MBase
    {
        /// <summary>
        /// 将Bar插入数据库
        /// </summary>
        /// <param name="bar"></param>
        public static void InsertBar(Bar bar)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into data_bar (`tradingday`,`starttime`,`symbol`,`open`,`high`,`low`,`close`,`volume`,`openinterest`,`tradecount`,`intervaltype`,`interval` ) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", bar.TradingDay, bar.StartTime.ToTLDateTime(), bar.Symbol, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.OpenInterest, bar.TradeCount, (int)bar.IntervalType, bar.Interval);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 加载Bar数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="maxcount"></param>
        /// <param name="fromEnd"></param>
        /// <returns></returns>
        public static IEnumerable<BarImpl> LoadBars(string symbol, BarInterval type, int interval, DateTime start, DateTime end, int maxcount, bool fromEnd)
        {
            using (DBMySql db = new DBMySql())
            {
                string qrystr = "SELECT ";
                qrystr += "a.tradingday,a.symbol,a.open,a.high,a.low,a.close,a.volume,a.openinterest,a.tradecount,a.interval,a.intervaltype,STR_TO_DATE(starttime,'%Y%m%d%H%i%s') as starttime FROM data_bar a WHERE `symbol` = '{0}' AND `intervaltype` = {1} AND `interval` = {2} ".Put(symbol, (int)type, interval);

                if (start != DateTime.MinValue)
                {
                    qrystr += "AND `startime`>={0} ".Put(start.ToTLDateTime());
                }
                if (end != DateTime.MaxValue)
                {
                    qrystr += "AND `starttime`<={0} ".Put(end.ToTLDateTime());
                }
                qrystr += "ORDER BY `starttime` ";
                if (fromEnd)
                {
                    qrystr += "DESC ";
                }
                else
                {
                    qrystr += "ASC ";
                }
                if (maxcount > 0)
                {
                    qrystr += "LIMIT {0}".Put(maxcount);
                }

                IEnumerable<BarImpl> bars = db.Connection.Query<BarImpl>(qrystr);//, (bar, fields) => { bar.StartTime = Util.ToDateTime(fields.StarTtime); bar.IntervalType = (BarInterval)fields.Intervaltype; return bar; }, null, null, false, "starttime", null, null);
                return bars;
            }
        }
    }
}
