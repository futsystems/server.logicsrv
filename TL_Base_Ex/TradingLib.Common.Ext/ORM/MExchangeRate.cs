﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MExchangeRate : MBase
    {
        /// <summary>
        /// 获得所有汇率信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ExchangeRate> SelectExchangeRates()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM log_exchange_rate";
                return db.Connection.Query<ExchangeRate>(query);
            }
        }

        /// <summary>
        /// 获取某个分区的汇率设置
        /// </summary>
        /// <param name="domain_id"></param>
        /// <returns></returns>
        public static IEnumerable<ExchangeRate> SelectExchangeRates(int domain_id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM log_exchange_rate WHERE domain_id='{0}'", domain_id);
                return db.Connection.Query<ExchangeRate>(query);
            }
        }
        /// <summary>
        /// 获取某个结算日的汇率信息
        /// </summary>
        /// <param name="settleday"></param>
        /// <returns></returns>
        public static IEnumerable<ExchangeRate> SelectExchangeRates(int domain_id,int settleday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM log_exchange_rate WHERE settleday='{0}' AND domain_id='{1}'", settleday,domain_id);
                return db.Connection.Query<ExchangeRate>(query);
            }
        }

        /// <summary>
        /// 更新汇率信息
        /// </summary>
        /// <param name="t"></param>
        public static void UpdateExchangeRate(ExchangeRate rate)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE log_exchange_rate SET askrate='{0}',intermediaterate='{1}',bidrate='{2}',updatetime='{3}' , settleday='{4}' WHERE id='{5}'", rate.AskRate, rate.IntermediateRate, rate.BidRate, rate.UpdateTime, rate.Settleday, rate.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入一条汇率信息
        /// </summary>
        /// <param name="t"></param>
        public static void InsertExchangeRate(ExchangeRate rate)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO log_exchange_rate (`currency`,`askrate`,`intermediaterate`,`bidrate`,`updatetime`,`settleday`,`domain_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", rate.Currency, rate.AskRate, rate.IntermediateRate, rate.BidRate, rate.UpdateTime, rate.Settleday,rate.Domain_ID);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => rate.ID = id, "id", "log_exchange_rate");
            }
        }
    }
}