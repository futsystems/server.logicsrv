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
        /// 插入Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public static void InsertIntradayBar(BarImpl bar)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into data_intraday (`tradingday`,`symbol`,`endtime`,`open`,`high`,`low`,`close`,`volume`,`openinterest`,`tradecount`,`intervaltype`,`interval` ) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", bar.TradingDay, bar.Symbol, bar.EndTime.ToTLDateTime(), bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.OpenInterest, bar.TradeCount, (int)bar.IntervalType, bar.Interval);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => bar.ID = id, "id", "data_intraday");
            }
        }

        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public static void UpdateIntradayBar(BarImpl bar)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Update  data_intraday SET `open`={0},`high`={1},`low`={2},`close`={3},`volume`={4},`openinterest`={5},`tradecount`={6}, `tradingday`={7} WHERE `id`='{8}'", bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.OpenInterest, bar.TradeCount, bar.TradingDay, bar.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某条记录
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteIntradayBar(int id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM data_intraday WHERE id = '{0}'", id); 
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
        public static IEnumerable<BarImpl> LoadIntradayBars(string symbol, BarInterval type, int interval, DateTime start)//;//, DateTime end, int maxcount)
        {
            using (DBMySql db = new DBMySql())
            {
                string qrystr = "SELECT ";
                qrystr += "a.id,a.tradingday,a.symbol,a.open,a.high,a.low,a.close,a.volume,a.openinterest,a.tradecount,a.interval,a.intervaltype,STR_TO_DATE(endtime,'%Y%m%d%H%i%s') as endtime FROM data_intraday a WHERE `symbol` = '{0}' AND `intervaltype` = {1} AND `interval` = {2} ".Put(symbol, (int)type, interval);

                if (start != DateTime.MinValue)
                {
                    qrystr += "AND `endtime`>={0} ".Put(start.ToTLDateTime());
                }
                //if (end != DateTime.MaxValue)
                //{
                //    qrystr += "AND `endtime`<={0} ".Put(end.ToTLDateTime());
                //}
                qrystr += "ORDER BY `endtime` ";
                //if (fromEnd)
                //{
                //qrystr += "DESC ";//降序截取 否则获得的数据是最老的数据
                //}
                //else
                //{
                qrystr += "ASC ";
                //}
                //if (maxcount > 0)
                //{
                //    qrystr += "LIMIT {0}".Put(maxcount);
                //}

                IEnumerable<BarImpl> bars = db.Connection.Query<BarImpl>(qrystr);//, (bar, fields) => { bar.StartTime = Util.ToDateTime(fields.StarTtime); bar.IntervalType = (BarInterval)fields.Intervaltype; return bar; }, null, null, false, "starttime", null, null);
                
                return bars;
            }
        }



        /// <summary>
        /// 插入Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public static void InsertEodBar(BarImpl bar)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into data_eod (`tradingday`,`symbol`,`endtime`,`open`,`high`,`low`,`close`,`volume`,`openinterest`,`tradecount`,`intervaltype`,`interval` ) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", bar.TradingDay, bar.Symbol, bar.EndTime.ToTLDateTime(), bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.OpenInterest, bar.TradeCount, (int)bar.IntervalType, bar.Interval);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => bar.ID = id, "id", "data_eod");
            }
        }

        /// <summary>
        /// 更新Bar数据
        /// </summary>
        /// <param name="bar"></param>
        public static void UpdateEodBar(BarImpl bar)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Update  data_eod SET `open`={0},`high`={1},`low`={2},`close`={3},`volume`={4},`openinterest`={5},`tradecount`={6}, `tradingday`={7} WHERE `id`='{8}'", bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.OpenInterest, bar.TradeCount, bar.TradingDay, bar.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 删除某条记录
        /// </summary>
        /// <param name="id"></param>
        public static void DeleteEodBar(int id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM data_eod WHERE id = '{0}'", id);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 加载日级别数据
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="start"></param>
        /// <param name="type"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static IEnumerable<BarImpl> LoadEodBars(string symbol, DateTime start, BarInterval type = BarInterval.Day, int interval = 1)
        {
            using (DBMySql db = new DBMySql())
            {
                string qrystr = "SELECT ";
                qrystr += "a.id,a.tradingday,a.symbol,a.open,a.high,a.low,a.close,a.volume,a.openinterest,a.tradecount,a.interval,a.intervaltype,STR_TO_DATE(endtime,'%Y%m%d%H%i%s') as endtime FROM data_eod a WHERE `symbol` = '{0}' AND `intervaltype` = {1} AND `interval` = {2} ".Put(symbol, (int)type, interval);

                if (start != DateTime.MinValue)
                {
                    qrystr += "AND `endtime`>={0} ".Put(start.ToTLDateTime());
                }
                qrystr += "ORDER BY `endtime` ";
                qrystr += "ASC ";
                IEnumerable<BarImpl> bars = db.Connection.Query<BarImpl>(qrystr);
                return bars;
            }
        }
    }
}
