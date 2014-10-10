using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class DBAccountLastEquity
    {
        public string Account { get; set; }

        public decimal LastEquity { get; set; }

    }

    internal class settlementconfirm
    {
        /// <summary>
        /// 交易帐号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 确认日期
        /// </summary>
        public int Settleday { get; set; }
    }


    public class MSettlement:MBase
    {

        /// <summary>
        /// 插入结算持仓
        /// </summary>
        /// <param name="p"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static bool InsertHoldPosition(Position p, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into hold_positions (`account`,`symbol`,`size`,`avgprice`,`settleday`,`settleprice`,`multiple`,`securitycode`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", p.Account, p.Symbol, p.Size, p.AvgPrice, settleday, p.SettlePrice, p.oSymbol.Multiple,p.oSymbol.SecurityFamily.Code);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 插入结算持仓回合数据
        /// </summary>
        /// <param name="pr"></param>
        /// <returns></returns>
        public static bool InsertHoldPositionRound(IPositionRound pr, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into hold_postransactions (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exittime`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`holdsize`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`,`type`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',{23})", pr.Account, pr.Symbol, pr.Security, pr.Multiple.ToString(), pr.EntryTime.ToString(), pr.EntrySize.ToString(), pr.EntryPrice.ToString(), pr.EntryCommission.ToString(), pr.ExitTime.ToString(), pr.ExitSize.ToString(), pr.ExitPrice.ToString(), pr.ExitCommission.ToString(), pr.Highest.ToString(), pr.Lowest.ToString(), pr.Size.ToString(), pr.HoldSize.ToString(), pr.Side.ToString(), pr.WL.ToString(), pr.TotalPoints.ToString(), pr.Profit.ToString(), pr.Commissoin.ToString(), pr.NetProfit.ToString(), pr.Type.ToString(), settleday);
                return db.Connection.Execute(query) > 0;
            }
        }


        //检查某个Account是否存在当天的结算记录,如果没有我们则可以对该交易账户进行结算
        /// <summary>
        /// 检查账户当天是否已经结算过了，当天结算过的账户不能再进行结算
        /// 检查当前交易日是否已经结算过
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool IsAccountSettled(string account)
        {
            return IsAccountSettled(account,TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
        }
        /// <summary>
        /// 检查账户是否结算过,搜索结算信息表,如果该日有结算信息,则结算过,没有则没有结算过
        /// </summary>
        /// <param name="account"></param>
        /// <param name="day"></param>
        /// <returns></returns>
        public static  bool IsAccountSettled(string account,int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("select account from settlement  where `account` = '{0}' and `settleday` = '{1}'", account,settleday);
                return db.Connection.Query(query).Count() > 0;
            }
        }

        /// <summary>
        /// 结算某个交易账户
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public static bool SettleAccount(IAccount acc)
        {
            if (IsAccountSettled(acc.ID)) return true;//如果该账户已经结算过，则直接返回
            using (DBMySql db = new DBMySql())
            {

                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;

                    Settlement settle = AccountBase.GenSettle(acc);
                    settle.SettleDay = TLCtxHelper.Ctx.SettleCentre.CurrentTradingday;//结算日为当前交易日
                    settle.SettleTime = TLCtxHelper.Ctx.SettleCentre.SettleTime;//获得结算时间

                    //1.插入某账户的结算信息(当前财务信息)平仓盈亏,持仓盈亏,手续费,入金,出金,昨日权益,当前权益
                    TLCtxHelper.Debug(string.Format("account:{0} lastequity:{1} nowequity:{2}", settle.Account, settle.LastEqutiy, settle.NowEquity));
                    string query = String.Format("Insert into settlement (`account`,`settleday`,`realizedpl`,`unrealizedpl`,`commission`,`cashin`,`cashout`,`lastequity`,`nowequity`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", settle.Account, settle.SettleDay, settle.RealizedPL,settle.UnRealizedPL, settle.Commission, settle.CashIn,settle.CashOut,settle.LastEqutiy,settle.NowEquity);
                    istransok =  istransok &&  (db.Connection.Execute(query) > 0);

                    //3.更新账户表中的上期权益数据 将结算数据的当前权益更新为帐户的昨日权益
                    query = String.Format("UPDATE accounts SET lastequity = '{0}' WHERE account = '{1}'", settle.NowEquity, settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    //4.更新账户结算时间,以后计算就只需要读取该账户这个时间段之后的交易信息并在当前权益基础上进行权益计算。
                    query = String.Format("UPDATE accounts SET settledatetime= '{0}' WHERE account = '{1}'",Util.ToDateTime(settle.SettleDay,settle.SettleTime),settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();

                    return istransok;
                }
            }
        }

        /// <summary>
        /// 获得交易帐户昨日权益
        /// </summary>
        /// <returns></returns>
        public static IList<DBAccountLastEquity> SelectAccountLastEquity()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT account,lastequity from accounts";
                IList<DBAccountLastEquity> list = db.Connection.Query<DBAccountLastEquity>(query, null).ToArray<DBAccountLastEquity>();
                return list;
            }
        }

        /// <summary>
        /// 获取系统最近一次的结算日期
        /// </summary>
        /// <returns></returns>
        public static int GetLastSettleday()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM system";
                systeminformation info = db.Connection.Query<systeminformation>(query).SingleOrDefault<systeminformation>();
                return info.LastSettleday;
            }
        }

        /// <summary>
        /// 更新最近结算日期
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static bool UpdateSettleday(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE system SET lastsettleday = '{0}'",settleday);
                return db.Connection.Execute(query) >= 0;
            }
        }



        /// <summary>
        /// 获得某交易帐户未确认结算
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Settlement SelectSettlementInfoUnconfirmed(string account,int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM settlement WHERE account = '{0}' AND confrim_timestamp = 0 AND settleday = {1}", account, tradingday);
                Settlement settlement = db.Connection.Query<SettlementImpl>(query, null).SingleOrDefault();
                return settlement;
            }
        }

        /// <summary>
        /// 获得某个交易帐号最近一条结算记录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static Settlement SelectSettlementInfoUnconfirmed(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM settlement WHERE account = '{0}' AND confrim_timestamp = 0 ORDER BY  settleday DESC limit 1", account);
                Settlement settlement = db.Connection.Query<SettlementImpl>(query, null).SingleOrDefault();
                return settlement;
            }
        }

        /// <summary>
        /// 查询某个交易帐号 某个交易日的结算单
        /// </summary>
        /// <param name="account"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static Settlement SelectSettlement(string account,int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM settlement WHERE account = '{0}' AND settleday = '{1}'", account,settleday);
                Settlement settlement = db.Connection.Query<SettlementImpl>(query, null).SingleOrDefault();
                return settlement;
            }
        }

        /// <summary>
        /// 确认交易帐户某日结算单
        /// </summary>
        /// <param name="account"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static bool ConfirmeSettle(string account,int tradingday,long timestamp)
        {
            using (DBMySql db = new DBMySql())
            {
                bool istransok = false;
                using (var transaction = db.Connection.BeginTransaction())
                {
                    
                    string query = String.Format("UPDATE settlement SET confrim_timestamp='{0}' WHERE account = '{1}' AND settleday < '{2}'",timestamp, account, tradingday);
                    istransok =  (db.Connection.Execute(query) > 0);

                    query = String.Format("UPDATE accounts SET confrim_timestamp = '{0}' WHERE account = '{1}'",timestamp, account);
                    istransok = istransok && (db.Connection.Execute(query) > 0);

                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();
                    return istransok;
                }


            }
        }


        /// <summary>
        /// 查询某个帐户某个交易日是否有结算确认
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        //public static bool IsSettlementInfoConfirmed(string account,int day)
        //{
        //    using (DBMySql db = new DBMySql())
        //    {
        //        string query = string.Format("SELECT * FROM settlement_confirm WHERE account='{0}' AND settleday='{1}'", account, day);

        //        return db.Connection.Query<settlementconfirm>(query, null).Count() > 0;
        //    }
        
        //}
    }

    

}
