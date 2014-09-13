//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MySql.Data.MySqlClient;

//using TradingLib.API;
//using TradingLib.Common;
////using CTP;
//using System.Data;

//namespace TradingLib.MySql
//{
//    /// <summary>
//    /// 用于记录交易配对记录,TransactionLoger实现缓存,线程安全
//    /// 同时用于ClearCenter记录交易信息
//    /// </summary>
//    public class mysqlDBTransaction : mysqlDBBase
//    {

//        //通过Order返回对应Order表的index
//        public int getOrderIdx(Order o)
//        {
//            this.SqlReady();
//            string sql = String.Format("select id from orders where `account` = '{0}' and `ordid` = '{1}'", o.Account, o.id);

//            cmd.CommandText = sql;
//            MySqlDataReader myReader = cmd.ExecuteReader();
//            try
//            {
//                myReader.Read();
//                return myReader.GetInt32("id");

//            }
//            catch (Exception ex)
//            {
//                return -1;
//            }
//            finally
//            {
//                myReader.Close();

//            }

//        }

//        //插入Order委托指令
//        /// <summary>
//        /// 将委托插入到数据库
//        /// </summary>
//        /// <param name="o"></param>
//        /// <returns></returns>
//        public bool insertOrder(Order o)
//        {
//            this.SqlReady();
//            //if (getOrderIdx(o) > 0) return true;//如果数据库中有该委托，则我们直接返回 这样可以保证数据库记录的委托编号是为唯一的(这里增加了数据库的占用时间,影响了效率)
//            string sql = String.Format("Insert into orders (`symbol`,`size`,`price`,`stop`,`comment`,`ex`,`account`,`security`,`currency`,`localsymbol`,`ordid`,`tif`,`date`,`time`,`trail`,`side`,`broker`,`brokerkey`,`status`,`source`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}')", o.symbol, (o.UnsignedSize * (o.side ? 1 : -1)).ToString(), o.price.ToString(System.Globalization.CultureInfo.InvariantCulture), o.stopp.ToString(System.Globalization.CultureInfo.InvariantCulture), o.comment, o.Exchange, o.Account, o.Security.ToString(), o.Currency.ToString(), o.LocalSymbol, o.id.ToString(), o.TIF, o.date.ToString(), o.time.ToString(), o.trail.ToString(System.Globalization.CultureInfo.InvariantCulture), (o.side == true ? "1" : "-1"), o.Broker, o.BrokerKey, o.Status.ToString(),o.OrderSource.ToString());
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }
//        /// <summary>
//        /// 更新委托状态,委托状态Placed,Accepted,Filled,PartFilled,Cancel,Reject
//        /// </summary>
//        /// <param name="o"></param>
//        /// <returns></returns>
//        public bool updateOrderStatus(Order o)
//        {
//            this.SqlReady();
//            //if (getOrderIdx(o) < 0) return true;//如果数据库中有该委托，则我们直接返回 这样可以保证数据库记录的委托编号是为唯一的(这里增加了数据库的占用时间,影响了效率)
//            string sql = String.Format("UPDATE orders SET status = '{0}',broker = '{1}',brokerkey='{2}' WHERE ordid = '{3}'",o.Status.ToString(),o.Broker,o.BrokerKey,o.id);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() > 0);
//        }

//        /// <summary>
//        /// 将成交插入到数据库
//        /// </summary>
//        /// <param name="f"></param>
//        /// <returns></returns>
//        public bool insertTrade(Trade f)//外加一个重复插入验证
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into trades (`ordid`,`xsize`,`xprice`,`xdate`,`xtime`,`symbol`,`account`,`fillid`,`broker`,`brokerkey`,`posoperation`,`commission`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}')", f.id.ToString(), f.xsize.ToString(), f.xprice.ToString(), f.xdate.ToString(), f.xtime.ToString(), f.symbol.ToString(), f.Account.ToString(), f.id.ToString(), f.Broker, f.BrokerKey, f.PositionOperation.ToString(),f.Commission.ToString());
//            cmd.CommandText = sql;
//            return ((cmd.ExecuteNonQuery() > 0));// && (fillOrder(f) > 0));
//        }
       

//        /// <summary>
//        /// 将取消插入到委托
//        /// </summary>
//        /// <param name="date"></param>
//        /// <param name="time"></param>
//        /// <param name="oid"></param>
//        /// <returns></returns>
//        public bool insertCancel(int date, int time, long oid)
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into cancles (`date`,`time`,`ordid`) values('{0}','{1}','{2}')", date.ToString(), time.ToString(), oid.ToString());
//            cmd.CommandText = sql;
//            return ((cmd.ExecuteNonQuery() > 0));
//        }

//        public bool insertPositionTransaction(string account,string symbol,string security,int multiple,DateTime entrytime,int entrysize,decimal entryprice,decimal entrycommission,DateTime exittime,int exitsize,decimal exitprice,decimal exitcommissoin,decimal highest,decimal lowest,int size,int hold,bool side,bool wl,decimal totalpoints,decimal profit,decimal commission,decimal netprofit)
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into postransactions (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exittime`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`hold`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')",account,symbol,security,multiple.ToString(),entrytime.ToString(),entrysize.ToString(),entryprice.ToString(),entrycommission.ToString(),exittime.ToString(),exitsize.ToString(),exitprice.ToString(),exitcommissoin.ToString(),highest.ToString(),lowest.ToString(),size.ToString(),hold.ToString(),side.ToString(),wl.ToString(),totalpoints.ToString(),profit.ToString(),commission.ToString(),netprofit.ToString());
//            cmd.CommandText = sql;
//            return ((cmd.ExecuteNonQuery() > 0));
//        }

//        public bool insertPositionTransactionOpened(string account, string symbol, string security, int multiple, DateTime entrytime, int entrysize, decimal entryprice, decimal entrycommission, DateTime exittime, int exitsize, decimal exitprice, decimal exitcommissoin, decimal highest, decimal lowest, int size, int hold, bool side, bool wl, decimal totalpoints, decimal profit, decimal commission, decimal netprofit)
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into postransactionsopened (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exittime`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`hold`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')", account, symbol, security, multiple.ToString(), entrytime.ToString(), entrysize.ToString(), entryprice.ToString(), entrycommission.ToString(), exittime.ToString(), exitsize.ToString(), exitprice.ToString(), exitcommissoin.ToString(), highest.ToString(), lowest.ToString(), size.ToString(), hold.ToString(), side.ToString(), wl.ToString(), totalpoints.ToString(), profit.ToString(), commission.ToString(), netprofit.ToString());
//            cmd.CommandText = sql;
//            return ((cmd.ExecuteNonQuery() > 0));
//        }



//        /// <summary>
//        /// 将pr数据插入到race_transaction表格中
//        /// </summary>
//        public bool insertPositionTransactionToRace(string account, string symbol, string security, int multiple, DateTime entrytime, int entrysize, decimal entryprice, decimal entrycommission, DateTime exittime, int exitsize, decimal exitprice, decimal exitcommissoin, decimal highest, decimal lowest, int size, int hold, bool side, bool wl, decimal totalpoints, decimal profit, decimal commission, decimal netprofit)
//        {
//            this.SqlReady();
//            string sql = String.Format("Insert into race_postransactions (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exittime`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`hold`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}')", account, symbol, security, multiple.ToString(), entrytime.ToString(), entrysize.ToString(), entryprice.ToString(), entrycommission.ToString(), exittime.ToString(), exitsize.ToString(), exitprice.ToString(), exitcommissoin.ToString(), highest.ToString(), lowest.ToString(), size.ToString(), hold.ToString(), side.ToString(), wl.ToString(), totalpoints.ToString(), profit.ToString(), commission.ToString(), netprofit.ToString());
//            cmd.CommandText = sql;
//            return ((cmd.ExecuteNonQuery() > 0));
//        }

//        /// <summary>
//        /// 删除某个选手某个时间之前的比赛positionround数据
//        /// </summary>
//        /// <param name="account"></param>
//        /// <param name="time"></param>
//        /// <returns></returns>
//        public bool deleteRacePositionTransaction(string account, DateTime time)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from race_postransactions WHERE account = '{0}' AND entrytime < '{1}'", account,time.ToString());
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }
//        /// <summary>
//        /// 删除某个账户的参赛positionround信息
//        /// </summary>
//        /// <param name="account"></param>
//        /// <returns></returns>
//        public bool deleteRacePositionTransaction(string account)
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from race_postransactions WHERE account = '{0}'",account);
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }

       
//        #region


//        /// <summary>
//        /// 清空positionTransactionOpened表,用于插清空后插入当日未关闭positionround
//        /// </summary>
//        /// <returns></returns>
//        public bool ClearPositionTransactionOpened()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from postransactionsopened");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }
//        /// <summary>
//        /// 清空日内仓位操作数据
//        /// </summary>
//        /// <returns></returns>
//        public bool ClearIntradayPositionTransaction()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from postransactions");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }

//        /// <summary>
//        /// 转储positiontransaction
//        /// </summary>
//        /// <returns></returns>
//        public bool DumpIntradayPositionTransaction(out int rows)
//        {
//            rows = 0;
//            this.SqlReady();
//            string sql = String.Format("replace into log_postransactions select * from postransactions");
//            cmd.CommandText = sql;
//            rows = cmd.ExecuteNonQuery();
//            return (rows >= 0);
//        }


//        /// <summary>
//        /// 清空日内委托数据
//        /// </summary>
//        /// <returns></returns>
//        public bool ClearIntradayOrders()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from orders");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }
//        /// <summary>
//        /// 转储orders
//        /// </summary>
//        /// <returns></returns>
//        public bool DumpIntradayOrders(out int rows)
//        {
//            rows = 0;
//            this.SqlReady();
//            string sql = String.Format("replace into log_orders select * from orders");
//            cmd.CommandText = sql;
//            rows = cmd.ExecuteNonQuery();
//            return ( rows>= 0);
//        }

//        /// <summary>
//        /// 恢复某日的委托记录
//        /// </summary>
//        /// <returns></returns>
//        public bool RestoreIntradayOrders(DateTime day)
//        {
//            if (day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday) return true;
//            DateTime end = day;
//            DateTime start = end.AddDays(-1);
//            if (end.DayOfWeek == DayOfWeek.Monday)
//                start = end.AddDays(-3);
//            this.SqlReady();
//            string sql = String.Format("replace into orders SELECT * FROM log_orders where ADDtime(date,time) >='{0} 20:30:00' and ADDtime(date,time) <='{1} 15:30:00'",start.ToShortDateString(),end.ToShortDateString());
//            cmd.CommandText = sql;
//            int rows = cmd.ExecuteNonQuery();
//            return (rows>=0);
//        }

//        /// <summary>
//        /// 恢复某日的成交记录
//        /// </summary>
//        /// <returns></returns>
//        public bool RestoreIntradayTrades(DateTime day)
//        {
//            if (day.DayOfWeek == DayOfWeek.Sunday || day.DayOfWeek == DayOfWeek.Saturday) return true;
//            DateTime end = day;
//            DateTime start = end.AddDays(-1);
//            if (end.DayOfWeek == DayOfWeek.Monday)
//                start = end.AddDays(-3);
//            this.SqlReady();
//            string sql = String.Format("replace into trades SELECT * FROM log_trades where ADDtime(xdate,xtime) >='{0} 20:30:00' and ADDtime(xdate,xtime) <='{1} 15:30:00'", start.ToShortDateString(), end.ToShortDateString());
//            cmd.CommandText = sql;
//            int rows = cmd.ExecuteNonQuery();
//            return (rows >= 0);
//        }


//        /// <summary>
//        /// 清空日内成交数据
//        /// </summary>
//        /// <returns></returns>
//        public bool ClearIntradayTrades()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from trades");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }
//        /// <summary>
//        /// 转储成交数据
//        /// </summary>
//        /// <returns></returns>
//        public bool DumpIntradayTrades(out int rows)
//        {
//            rows = 0;
//            this.SqlReady();
//            string sql = String.Format("replace into log_trades select * from trades");
//            cmd.CommandText = sql;
//            rows = cmd.ExecuteNonQuery();
//            return (rows >= 0);
//        }

//        /// <summary>
//        /// 清空日内取消数据
//        /// </summary>
//        /// <returns></returns>
//        public bool ClearIntradayCancels()
//        {
//            this.SqlReady();
//            string sql = String.Format("delete from cancles");
//            cmd.CommandText = sql;
//            return (cmd.ExecuteNonQuery() >= 0);
//        }

//        public bool DumpIntradayCancels(out int rows)
//        {
//            rows = 0;
//            this.SqlReady();
//            string sql = String.Format("replace into log_cancles select * from cancles");
//            cmd.CommandText = sql;
//            rows = cmd.ExecuteNonQuery();
//            return (rows >= 0);
//        }


//        #endregion



//    }
//}
