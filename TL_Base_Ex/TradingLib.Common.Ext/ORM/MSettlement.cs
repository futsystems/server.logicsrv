﻿using System;
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
                string query = String.Format("select account from log_position_detail_hist  where `account` = '{0}' AND `settleday` = '{1}' AND  `symbol`='{2}' AND `tradeid`='{3}' AND `side`='{4}' AND `opendate`='{5}'", p.Account, p.Settleday, p.Symbol, p.TradeID, p.Side ? 1 : 0, p.OpenDate);
                return db.Connection.Query(query).Count() > 0;
            }
        }

        /// <summary>
        /// 标注某个持仓明细已结算
        /// </summary>
        /// <param name="p"></param>
        public static void MarkPositionDetailSettled(PositionDetail p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE log_position_detail_hist SET settled='1',settledinday='{0}' WHERE `account` = '{1}' AND `settleday` = '{2}' AND  `symbol`='{3}' AND `tradeid`='{4}' AND `side`='{5}' AND `opendate`='{6}'",TLCtxHelper.ModuleSettleCentre.Tradingday, p.Account, p.Settleday, p.Symbol, p.TradeID, p.Side ? 1 : 0, p.OpenDate);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新交易所结算为已结算
        /// </summary>
        /// <param name="settle"></param>
        public static void MarkExchangeSettlementSettled(ExchangeSettlement settle)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE log_settlement_exchange SET settled='1' WHERE `account` = '{0}' AND `settleday` = '{1}'  AND `exchange`='{2}' ", settle.Account, settle.Settleday, settle.Exchange);
                db.Connection.Execute(query);
            }
        }
        /// <summary>
        /// 查询未结算隔夜持仓明细
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PositionDetail> SelecteAccountPositionDetailsUnSettled()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  log_position_detail_hist WHERE settled = {0} AND  breed='{1}'",0, QSEnumOrderBreedType.ACCT);
                return db.Connection.Query<PositionDetailImpl>(query);
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
                string query = String.Format("Insert into log_position_close_detail (`account`,`settleday`,`side`,`opendate`,`opentime`,`closedate`,`closetime`,`openprice`,`lastsettlementprice`,`closeprice`,`closevolume`,`closeprofitbydate`,`exchange`,`symbol`,`seccode`,`opentradeid`,`closetradeid`,`closepointbydate`,`closeprofitbytrade`,`iscloseydpositoin`,`closeamount`,`broker`,`breed`,`closepointbytrade`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}')", p.Account, p.Settleday, p.Side ? 1 : 0, p.OpenDate, p.OpenTime, p.CloseDate, p.CloseTime, p.OpenPrice, p.LastSettlementPrice, p.ClosePrice, p.CloseVolume, p.CloseProfitByDate, p.Exchange, p.Symbol, p.SecCode, p.OpenTradeID, p.CloseTradeID, p.ClosePointByDate, p.CloseProfitByTrade, p.IsCloseYdPosition ? 1 : 0, p.CloseAmount, p.Broker, p.Breed,p.ClosePointByTrade);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 更新反转平仓明细
        /// </summary>
        /// <param name="p"></param>
        public static void UpdatePositionCloseDetailReversed(PositionCloseDetail p)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE log_position_close_detail SET closeprofitbydate = '{0}',closepointbydate = '{1}',closeprofitbytrade = '{2}' ,side='{3}' WHERE settleday = '{4}' AND opentradeid='{5}' AND closetradeid='{6}'", p.CloseProfitByDate, p.ClosePointByDate, p.CloseProfitByTrade,p.Side?1:0, p.Settleday, p.OpenTradeID, p.CloseTradeID);
                db.Connection.Execute(query);
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
            return IsAccountSettled(account, TLCtxHelper.ModuleSettleCentre.Tradingday);
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
        /// 插入交易帐户结算记录
        /// 同时更新交易账户对应的结算字段
        /// </summary>
        /// <param name="settle"></param>
        public static void InsertAccountSettlement(AccountSettlement settle)
        {
            using (DBMySql db = new DBMySql())
            {
                using (var transaction = db.Connection.BeginTransaction())
                {
                    bool istransok = true;
                    string query = string.Format("INSERT INTO log_settlement (`account`,`settleday`,`closeprofitbydate`,`positionprofitbydate`,`commission`,`cashin`,`cashout`,`lastequity`,`equitysettled`,`lastcredit`,`creditsettled`,`creditcashin`,`creditcashout`,`assetbuyamount`,`assetsellamount`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", settle.Account, settle.Settleday, settle.CloseProfitByDate, settle.PositionProfitByDate, settle.Commission, settle.CashIn, settle.CashOut, settle.LastEquity, settle.EquitySettled, settle.LastCredit, settle.CreditSettled, settle.CreditCashIn, settle.CreditCashOut,settle.AssetBuyAmount,settle.AssetSellAmount);
                    istransok = istransok && (db.Connection.Execute(query) > 0);

                    query = string.Format("UPDATE accounts SET lastequity = '{0}',lastcredit='{1}' WHERE account = '{2}'", settle.EquitySettled,settle.CreditSettled, settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    query = string.Format("UPDATE accounts SET settledtime= '{0}' WHERE account = '{1}'", Util.ToTLDateTime(), settle.Account);
                    istransok = istransok && (db.Connection.Execute(query) >= 0);

                    //如果所有操作均正确,则提交数据库transactoin
                    if (istransok)
                        transaction.Commit();

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
        public static AccountSettlement SelectSettlement(string account,int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_settlement WHERE account = '{0}' AND settleday = '{1}'", account,settleday);
                AccountSettlement settlement = db.Connection.Query<AccountSettlementImpl>(query, null).SingleOrDefault();
                return settlement;
            }
        }

        /// <summary>
        /// 查询一个时间段内投资者账户结算记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="startSettleday"></param>
        /// <param name="endSettleday"></param>
        /// <returns></returns>
        public static IEnumerable<AccountSettlement> SelectSettlements(string account, int startSettleday, int endSettleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM log_settlement WHERE account = '{0}' AND settleday >= '{1}' AND settleday <= '{2}'", account, startSettleday,endSettleday);
                return db.Connection.Query<AccountSettlementImpl>(query, null);
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

        /// <summary>
        /// 获得某个结算日权益统计数据
        /// </summary>
        /// <param name="tradingday"></param>
        /// <returns></returns>
        public static IEnumerable<EquityReport> SelectEquityReport(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT account,equitysettled as equity,creditsettled as credit FROM log_settlement WHERE settleday = '{0}'", settleday);
                return db.Connection.Query<EquityReport>(query);//包含多个元素则异常
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
                string query = String.Format("SELECT * FROM log_settlement_price WHERE  settleday = '{0}'", settleday);
                return db.Connection.Query<SettlementPrice>(query);
            }
        }

        /// <summary>
        /// 向数据库插入一条结算价格记录
        /// </summary>
        /// <param name="price"></param>
        public static void InsertSettlementPrice(SettlementPrice data)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO log_settlement_price (`settleday`,`symbol`,`askprice`,`asksize`,`bidprice`,`bidsize`,`upperlimit`,`lowerlimit`,`presettlement`,`settlement`,`preoi`,`oi`,`open`,`high`,`low`,`close`,`vol`,`exchange`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}')", data.SettleDay, data.Symbol, data.AskPrice, data.AskSize, data.BidPrice, data.BidSize, data.UpperLimit, data.LowerLimit, data.PreSettlement, data.Settlement, data.PreOI, data.OI, data.Open, data.High, data.Low, data.Close, data.Vol, data.Exchange);
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
                string query = String.Format("UPDATE log_settlement_price SET settlement = '{0}' WHERE symbol = '{1}' AND settleday = '{2}'", price.Settlement, price.Symbol, price.SettleDay);
                db.Connection.Execute(query);
            }
        }

        #endregion


        #region 交易所结算

        public static void InsertExchangeSettlement(ExchangeSettlement settlement)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT into log_settlement_exchange (`settleday`,`account`,`closeprofitbydate`,`positionprofitbydate`,`commission`,`exchange`,`assetbuyamount`,`assetsellamount`) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')", settlement.Settleday, settlement.Account, settlement.CloseProfitByDate, settlement.PositionProfitByDate, settlement.Commission, settlement.Exchange,settlement.AssetBuyAmount,settlement.AssetSellAmount);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 获得没有结算的交易所结算记录
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExchangeSettlementImpl> SelectPendingExchangeSettlement()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM log_settlement_exchange WHERE settled ='0'";
                IEnumerable<ExchangeSettlementImpl> result = db.Connection.Query<ExchangeSettlementImpl>(query);
                return result;
            }
        }

        #endregion


        #region 回滚到某个交易日

        /// <summary>
        /// 将交易记录回滚到某个结算日
        /// </summary>
        /// <param name="settleday"></param>
        public static void RollBackToSettleday(int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                //将该交易日及以后的交易记录标注成未结算
                string query = string.Format("UPDATE tmp_orders SET settled=0 WHERE settleday >= '{0}' ", settleday);
                db.Connection.Execute(query);

                query = string.Format("UPDATE tmp_trades SET settled=0 WHERE settleday >= '{0}' ", settleday);
                db.Connection.Execute(query);

                //将该结算日及以后的出入金记录标注为未结算
                query = string.Format("UPDATE log_cashtrans SET settled=0 WHERE settleday >='{0}'", settleday);
                db.Connection.Execute(query);

                //将该交易日及以后的结算持仓删除
                query = string.Format("DELETE from log_position_detail_hist WHERE settleday >= '{0}'", settleday);
                db.Connection.Execute(query);

                //将在该交易日结算的结算持仓 标注成未结算
                query = string.Format("UPDATE log_position_detail_hist SET settled=0 WHERE settledinday ='{0}'", settleday);
                db.Connection.Execute(query);

                //删除该结算日及以后的交易所结算记录
                query = string.Format("DELETE from log_settlement_exchange WHERE settleday >= '{0}'", settleday);
                db.Connection.Execute(query);

                //删除结算日及以后的计算记录
                query = string.Format("DELETE from log_settlement WHERE settleday >= '{0}'", settleday);
                db.Connection.Execute(query);

                //代理手续费拆分
                query = string.Format("UPDATE log_agent_commission_split SET settled=0 WHERE settleday >= '{0}' ", settleday);
                db.Connection.Execute(query);

                //删除结算自动产生的出入金记录 普通代理结算日当天的手续费差以入金的方式计入代理结算账户
                query = string.Format("DELETE from log_agent_cashtrans WHERE settleday >= '{0}' AND operator = 'SETTLE'", settleday);
                db.Connection.Execute(query);
                
                //标注代理出入金未结算
                query = string.Format("UPDATE log_agent_cashtrans SET settled=0 WHERE settleday >= '{0}' ", settleday);
                db.Connection.Execute(query);

                //删除结算日及以后的计算记录
                query = string.Format("DELETE from log_agent_settlement WHERE settleday >= '{0}'", settleday);
                db.Connection.Execute(query);
            }
        }

        #endregion

    }

    

}
