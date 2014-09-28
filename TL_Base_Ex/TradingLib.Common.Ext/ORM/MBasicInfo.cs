﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;


namespace TradingLib.ORM
{
    internal class MarketTimeString
    {
        public string MarketTime { get; set; }
    }

    internal class SecForigenKey
    {
        public int exchange_fk { get; set; }
        public int mkttime_fk { get; set; }
    }


    /// <summary>
    /// Exchange数据库操作
    /// </summary>
    public class MBasicInfo : MBase
    {
        /// <summary>
        /// 返回帐户类别列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Exchange> SelectExchange()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM info_exchange";
                IEnumerable<Exchange> result = db.Connection.Query<Exchange>(query, null);
                return result;
            }

        }


        /// <summary>
        /// 返回帐户类别列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MarketTime> SelectSession()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT a.id,a.name,a.description,a.markettime FROM info_markettime a";
                IEnumerable<MarketTime> result = db.Connection.Query<MarketTime, MarketTimeString, MarketTime>(query, (mkttime, mkttimestr) => { mkttime.DeserializeMktTimeString(mkttimestr.MarketTime); return mkttime; }, null, null, false, "markettime", null, null).ToList<MarketTime>();
                return result;
            }

        }

        /// <summary>
        /// 返回帐户类别列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SecurityFamilyImpl> SelectSecurity()
        {
            using (DBMySql db = new DBMySql())
            {
                //const string query = "SELECT a.id,a.code,a.name,a.currency,a.type,a.multiple,a.pricetick,a.tradeable,a.underlaying_fk,a.entrycommission,a.exitcommission,a.margin,a.extramargin,a.maintancemargin,a.exchange_fk,a.mkttime_fk FROM info_security a";
                //IEnumerable<SecurityFamilyImpl> result = db.Connection.Query<SecurityFamilyImpl, SecForigenKey, SecurityFamilyImpl>(query, (sec, fk) => { sec.Exchange = BasicTracker.ExchagneTracker[fk.exchange_fk]; sec.MarketTime = BasicTracker.MarketTimeTracker[fk.mkttime_fk]; return sec; }, null, null, false, "exchange_fk", null, null).ToArray(); ;
                const string query = "SELECT * FROM info_security";
                IEnumerable<SecurityFamilyImpl> result = db.Connection.Query<SecurityFamilyImpl>(query, null);
                return result;
            }
        }

        public static bool UpdateSecurity(SecurityFamilyImpl sec)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE info_security SET code='{0}',name='{1}',currency='{2}',type='{3}',multiple='{4}',pricetick='{5}',underlaying_fk='{6}',entrycommission='{7}',exitcommission='{8}',margin='{9}',extramargin='{10}',maintancemargin='{11}',exchange_fk='{12}',mkttime_fk='{13}' ,tradeable ='{14}' WHERE id='{15}'", sec.Code, sec.Name, sec.Currency, sec.Type, sec.Multiple, sec.PriceTick, sec.UnderLayingFK, sec.EntryCommission, sec.ExitCommission, sec.Margin, sec.ExtraMargin, sec.MaintanceMargin, sec.ExchangeFK, sec.MarketTimeFK,sec.Tradeable?1:0 ,sec.ID);
                return db.Connection.Execute(query)>=0;
            }
        }

        /// <summary>
        /// 插入品种
        /// </summary>
        /// <param name="sec"></param>
        /// <returns></returns>
        public static bool InsertSecurity(SecurityFamilyImpl sec)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO info_security (`code`,`name`,`currency`,`type`,`multiple`,`pricetick`,`underlaying_fk`,`entrycommission`,`exitcommission`,`margin`,`extramargin`,`maintancemargin`,`exchange_fk`,`mkttime_fk`,`tradeable`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", sec.Code, sec.Name, sec.Currency, sec.Type, sec.Multiple, sec.PriceTick, sec.UnderLayingFK, sec.EntryCommission, sec.ExitCommission, sec.Margin, sec.ExtraMargin, sec.MaintanceMargin, sec.ExchangeFK, sec.MarketTimeFK, sec.Tradeable ? 1 : 0);
                int row =  db.Connection.Execute(query);
                SetIdentity(db.Connection, id => sec.ID = id, "id", "info_security");

                return row > 0;
            }
        }

        public static IEnumerable<SymbolImpl> SelectSymbol()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM info_symbols";
                IEnumerable<SymbolImpl> result = db.Connection.Query<SymbolImpl>(query);
                return result;
            }

        }

        public static bool UpdateSymbol(SymbolImpl sym)
        {
            using (DBMySql db = new DBMySql())
            {
                //TLCtxHelper.Ctx.debug("orm update mktimefk:" + sec.MarketTimeFK.ToString() + " exchangefk:" + sec.ExchangeFK.ToString() + " underfk:" + sec.UnderLayingFK.ToString());
                string query = string.Format("UPDATE info_symbols SET entrycommission='{0}',exitcommission='{1}',margin='{2}',extramargin='{3}',maintancemargin='{4}' ,tradeable='{5}' WHERE id='{6}'",sym._entrycommission,sym._extramargin,sym._margin,sym._extramargin,sym._maintancemargin,sym.Tradeable?1:0,sym.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }

        public static bool InsertSymbol(SymbolImpl sym)
        {
            using (DBMySql db = new DBMySql())
            {
                
                //string query = string.Format("INSERT INTO info_symbols (`symbol`,`entrycommission`,`exitcommission`,`margin`,`extramargin`,`maintancemargin`,`strike`,`optionside`,`expiremonth`,`expiredate`,`security_fk``underlaying_fk`,`underlayingsymbol_fk`,`tradeable`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", sym.Symbol, sym.EntryCommission, sym.ExitCommission, sym.Margin, sym.ExtraMargin, sym.MaintanceMargin, sym.Strike, sym.OptionSide, sym.ExpireMonth, sym.ExpireDate, sym.security_fk, sym.underlaying_fk, sym.underlayingsymbol_fk, sym.Tradeable ? 1 : 0);
                string query = string.Format("INSERT INTO info_symbols (`symbol`,`entrycommission`,`exitcommission`,`margin`,`extramargin`,`maintancemargin`,`strike`,`optionside`,`expiremonth`,`expiredate`,`security_fk`,`underlaying_fk`,`underlayingsymbol_fk`,`tradeable`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", sym.Symbol, sym._entrycommission, sym._exitcommission, sym._margin, sym._extramargin, sym._maintancemargin, sym.Strike, sym.OptionSide, sym.ExpireMonth, sym.ExpireDate, sym.security_fk, sym.underlaying_fk, sym.underlayingsymbol_fk, sym.Tradeable ? 1 : 0);
                
                //TLCtxHelper.Ctx.debug("query:" + query);
                int row = db.Connection.Execute(query);
                SetIdentity(db.Connection, id => sym.ID = id, "id", "info_symbols");

                return row > 0;
            }
        }

        /// <summary>
        /// 获得所有签约银行列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ContractBank> SelectContractBanks()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT * FROM info_contract_bank";
                IEnumerable<ContractBank> result = db.Connection.Query<ContractBank>(query);
                return result;
            }
        }
    }
        

    
}
