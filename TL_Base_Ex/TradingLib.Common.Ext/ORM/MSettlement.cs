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
        #region 持仓明细


        /// <summary>
        /// 插入持仓明细
        /// </summary>
        /// <param name="?"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static void InsertPositionDetail(PositionDetail p)
        {
            using (DBMySql db = new DBMySql())
            {
                //如果对应的持仓明细不存在 则插入该持仓明细数据
                if (!IsPositionDetailExist(p))
                {
                    string query = String.Format("Insert into log_position_detail_hist (`account`,`opendate`,`opentime`,`closeamount`,`settleday`,`side`,`volume`,`openprice`,`tradeid`,`lastsettlementprice`,`settlementprice`,`closevolume`,`hedgeflag`,`margin`,`exchange`,`symbol`,`seccode`,`closeprofitbydate`,`closeprofitbytrade`,`positionprofitbydate`,`positionprofitbytrade`,`ishisposition`,`broker`,`breed`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}')", p.Account, p.OpenDate, p.OpenTime, p.CloseAmount, p.Settleday, p.Side ? 1 : 0, p.Volume, p.OpenPrice, p.TradeID, p.LastSettlementPrice, p.SettlementPrice, p.CloseVolume, p.HedgeFlag, p.Margin, p.Exchange, p.Symbol, p.SecCode, p.CloseProfitByDate, p.CloseProfitByTrade, p.PositionProfitByDate, p.PositionProfitByTrade, p.IsHisPosition ? 1 : 0, p.Broker, p.Breed);
                    db.Connection.Execute(query);
                }
            }
        }


        /// <summary>
        /// 检查某个持仓明细是否已经存在
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool IsPositionDetailExist(PositionDetail p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("select account from log_position_detail_hist  where `account` = '{0}' AND `settleday` = '{1}' AND  `symbol`='{2}' AND `tradeid`='{3}'", p.Account, p.Settleday, p.Symbol,p.TradeID);
                return db.Connection.Query(query).Count() > 0;
            }
        }
        /// <summary>
        /// 获得分帐户侧所有持仓明细
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<PositionDetail> SelectAccountPositionDetails(int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_detail_hist WHERE settleday = {0} AND  breed='{1}'", tradingday, QSEnumOrderBreedType.ACCT);
                return db.Connection.Query<PositionDetailImpl>(query);
            }
        }

        /// <summary>
        /// 获得某个分帐户的持仓明细
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<PositionDetail> SelectAccountPositionDetails(string account,int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_detail_hist WHERE settleday = {0} AND  breed='{1}' AND account='{2}'", tradingday, QSEnumOrderBreedType.ACCT, account);
                return db.Connection.Query<PositionDetailImpl>(query);
            }
        }
        /// <summary>
        /// 获得接口侧所有持仓明细数据
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<PositionDetail> SelectBrokerPositionDetails(int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_detail_hist WHERE settleday = {0} AND  breed='{1}'", tradingday, QSEnumOrderBreedType.BROKER);
                return db.Connection.Query<PositionDetailImpl>(query);
            }
        }


        /// <summary>
        /// 从数据库加载某个交易帐号或通道的持仓明细某个结算日的所有持仓明细
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<PositionDetail> SelectPositionDetails(string account,int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_detail_hist WHERE settleday = '{0}' AND account='{1}'", tradingday,account);
                return db.Connection.Query<PositionDetailImpl>(query);
            }
        }


         

        /// <summary>
        /// 插入平仓明细
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool InsertPositionCloseDetail(PositionCloseDetail p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into log_position_close_detail (`account`,`settleday`,`side`,`opendate`,`opentime`,`closedate`,`closetime`,`openprice`,`lastsettlementprice`,`closeprice`,`closevolume`,`closeprofitbydate`,`exchange`,`symbol`,`seccode`,`opentradeid`,`closetradeid`,`closepointbydate`,`closeprofitbytrade`,`iscloseydpositoin`,`closeamount`,`broker`,`breed`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')", p.Account, p.Settleday, p.Side ? 1 : 0, p.OpenDate, p.OpenTime, p.CloseDate, p.CloseTime, p.OpenPrice, p.LastSettlementPrice, p.ClosePrice, p.CloseVolume, p.CloseProfitByDate, p.Exchange, p.Symbol, p.SecCode, p.OpenTradeID, p.CloseTradeID, p.ClosePointByDate, p.CloseProfitByTrade, p.IsCloseYdPosition ? 1 : 0, p.CloseAmount,p.Broker,p.Breed);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 查询某个交易日的所有平仓明细
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<PositionCloseDetail> SelectPositionCloseDetail(int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_close_detail WHERE settleday = {0}", tradingday);
                return db.Connection.Query<PositionCloseDetailImpl>(query);
            }
        }

        /// <summary>
        /// 查询某个交易日某个交易帐户的平仓明细
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<PositionCloseDetail> SelectPositionCloseDetail(string account,int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_close_detail WHERE settleday = '{0}' AND account='{1}'", tradingday,account);
                return db.Connection.Query<PositionCloseDetailImpl>(query);
            }
        }



        /// <summary>
        /// 删除某个交易日的持仓明细
        /// </summary>
        /// <param name="settleday"></param>
        public static void DeletePositionDetails(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM  log_position_detail_hist WHERE settleday = {0}", settleday);
                db.Connection.Execute(query);
            }
            
        }

        #endregion

        /// <summary>
        /// 插入结算持仓回合数据
        /// </summary>
        /// <param name="pr"></param>
        /// <returns></returns>
        public static void InsertHoldPositionRound(PositionRound pr, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                if (!IsPositoinRoundExist(pr, settleday))
                {
                    string query = String.Format("Insert into hold_postransactions (`account`,`symbol`,`security`,`multiple`,`entrytime`,`entrysize`,`entryprice`,`entrycommission`,`exitsize`,`exitprice`,`exitcommission`,`highest`,`lowest`,`size`,`holdsize`,`side`,`wl`,`totalpoints`,`profit`,`commission`,`netprofit`,`type`,`settleday`) values('{0}','{1}','{2}','{3}',{4},'{5}','{6}','{7}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}',{23})", pr.Account, pr.Symbol, pr.Security, pr.Multiple.ToString(), pr.EntryTime, pr.EntrySize.ToString(), pr.EntryPrice.ToString(), pr.EntryCommission, pr.ExitTime, pr.ExitSize, pr.ExitPrice, pr.ExitCommission, pr.Highest, pr.Lowest, pr.Size, pr.HoldSize.ToString(), pr.Side ? 1 : 0, pr.WL?1:0, pr.TotalPoints, pr.Profit, pr.Commissoin, pr.NetProfit, pr.Type, settleday);
                    db.Connection.Execute(query);
                }
            }
        }

        /// <summary>
        /// 判断某个持仓回合数据是否存在
        /// 持仓回合是只开仓到平仓的一个整体过程，所以每个交易帐户不可能同一个合约有2条持仓回合记录
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static bool IsPositoinRoundExist(PositionRound pr, int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT *  FROM hold_postransactions  where `account` = '{0}' AND `settleday` = '{1}' AND  `symbol`='{2}' ", pr.Account, settleday, pr.Symbol);
                return db.Connection.Query(query).Count() > 0;
            }
        }
        /// <summary>
        /// 删除某个交易日的开启的持仓回合信息
        /// </summary>
        /// <param name="settleday"></param>
        public static void DeleteHoldPositionRound(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("DELETE FROM hold_postransactions WHERE settleday={0}", settleday);
                db.Connection.Execute(query);
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
                string query = String.Format("select account from log_settlement  where `account` = '{0}' and `settleday` = '{1}'", account,settleday);
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
            //如果该账户已经结算过，则直接返回 判断某帐户是当前交易日是否已经结算过 是通过 查询对应的结算记录来判断
            if (IsAccountSettled(acc.ID)) return true;
            using (DBMySql db = new DBMySql())
            {
                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;

                    Settlement settle = acc.ToSettlement();
                    settle.SettleDay = TLCtxHelper.Ctx.SettleCentre.NextTradingday;//结算日为当前交易日
                    settle.SettleTime = TLCtxHelper.Ctx.SettleCentre.SettleTime;//获得结算时间

                    //1.插入某账户的结算信息(当前财务信息)平仓盈亏,持仓盈亏,手续费,入金,出金,昨日权益,当前权益
                    if (acc.LastEquity != acc.NowEquity)
                    {
                        Util.Debug(string.Format("account:{0} lastequity:{1} nowequity:{2}", settle.Account, settle.LastEquity, settle.NowEquity),QSEnumDebugLevel.DEBUG);
                    }
                    string query = String.Format("Insert into log_settlement (`account`,`settleday`,`realizedpl`,`unrealizedpl`,`commission`,`cashin`,`cashout`,`lastequity`,`nowequity`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", settle.Account, settle.SettleDay, settle.RealizedPL, settle.UnRealizedPL, settle.Commission, settle.CashIn, settle.CashOut, settle.LastEquity, settle.NowEquity);
                    istransok =  istransok &&  (db.Connection.Execute(query) > 0);

                    //2.更新账户表中的上期权益数据 将结算数据的当前权益更新为帐户的昨日权益
                    query = String.Format("UPDATE accounts SET lastequity = '{0}' WHERE account = '{1}'", settle.NowEquity, settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    //3.更新账户结算时间,以后计算就只需要读取该账户这个时间段之后的交易信息并在当前权益基础上进行权益计算。
                    query = String.Format("UPDATE accounts SET settledatetime= '{0}' WHERE account = '{1}'",Util.ToTLDateTime(settle.SettleDay,settle.SettleTime),settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();

                    return istransok;
                }
            }
        }

        /// <summary>
        /// 删除某个交易日的结算记录
        /// </summary>
        /// <param name="settleday"></param>
        public static void DeleteSettlement(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("DELETE FROM log_settlement WHERE settleday={0}", settleday);
                db.Connection.Execute(query);
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
                SystemInformation info = db.Connection.Query<SystemInformation>(query).SingleOrDefault<SystemInformation>();
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
        /// 查询某个交易帐号 某个交易日的结算单
        /// </summary>
        /// <param name="account"></param>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static Settlement SelectSettlement(string account,int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_settlement WHERE account = '{0}' AND settleday = '{1}'", account,settleday);
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
                    
                    string query = String.Format("UPDATE log_settlement SET confrim_timestamp='{0}' WHERE account = '{1}' AND settleday < '{2}'",timestamp, account, tradingday);
                    istransok =  (db.Connection.Execute(query) >= 0);

                    query = String.Format("UPDATE accounts SET confrim_timestamp = '{0}' WHERE account = '{1}'",timestamp, account);
                    istransok = istransok && (db.Connection.Execute(query) > 0);

                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();
                    return istransok;
                }


            }
        }



        #region 结算价信息管理

        /// <summary>
        /// 从数据库加载某个结算日的所有结算价信息
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static IEnumerable<SettlementPrice> SelectSettlementPrice(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_settlement_price WHERE  settleday = '{0}'",settleday);
                return db.Connection.Query<SettlementPrice>(query);
            }
        }

        /// <summary>
        /// 向数据库插入一条结算价格记录
        /// </summary>
        /// <param name="price"></param>
        public static void InsertSettlementPrice(SettlementPrice price)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO log_settlement_price (`symbol`,`settleday`,`price`) values('{0}','{1}','{2}')", price.Symbol,price.SettleDay,price.Price);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新结算价信息
        /// </summary>
        /// <param name="price"></param>
        public static void UpdateSettlementPrice(SettlementPrice price)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE log_settlement_price SET price = '{0}' WHERE symbol = '{1}' AND settleday = '{2}'", price.Price, price.Symbol, price.SettleDay);
                db.Connection.Execute(query);
            }  
        }

        #endregion

    }

    

}
