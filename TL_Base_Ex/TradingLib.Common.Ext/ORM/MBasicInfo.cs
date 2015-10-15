using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.ORM
{
    internal class MarketTimeDBRanges
    {
        public string Ranges { get; set; }
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
                const string query = "SELECT * FROM info_exchange WHERE avabile=1";//选择可用的交易所列表
                IEnumerable<Exchange> result = db.Connection.Query<Exchange>(query, null);
                return result;
            }
        }

        /// <summary>
        /// 更新交易所
        /// </summary>
        /// <param name="ex"></param>
        public static void UpdateExchange(Exchange ex)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE info_exchange SET country='{0}',excode='{1}',name='{2}',title='{3}',calendar='{4}',timezoneid='{5}',closetime='{6}' WHERE id='{7}'", ex.Country, ex.EXCode, ex.Name, ex.Title, ex.Calendar, ex.TimeZoneID,ex.CloseTime, ex.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入交易所
        /// </summary>
        /// <param name="ex"></param>
        public static void InsertExchange(Exchange ex)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO info_exchange (`country`,`excode`,`name`,`title`,`calendar`,`timezoneid`,`closetime`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}')", ex.Country, ex.EXCode, ex.Name, ex.Title, ex.Calendar, ex.TimeZoneID, ex.CloseTime);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => ex.ID = id, "id", "info_exchange");
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
                const string query = "SELECT a.id,a.name,a.description,a.closetime,a.ranges FROM info_markettime a";
                IEnumerable<MarketTime> result = db.Connection.Query<MarketTime, MarketTimeDBRanges, MarketTime>(query, (mkttime, dbranges) => { mkttime.DeserializeTradingRange(dbranges.Ranges); return mkttime; }, null, null, false, "ranges", null, null).ToList<MarketTime>();
                return result;
            }

        }

        /// <summary>
        /// 更新交易时间段
        /// </summary>
        /// <param name="mt"></param>
        public static void UpdateMarketTime(MarketTime mt)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE info_markettime SET name='{0}',description='{1}',ranges='{2}',closetime='{3}' WHERE id='{4}'", mt.Name, mt.Description, mt.SerializeTradingRange(),mt.CloseTime, mt.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入交易时间段
        /// </summary>
        /// <param name="mt"></param>
        public static void InsertMarketTime(MarketTime mt)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO info_markettime (`name`,`description`,`ranges`,`closetime`) VALUES ( '{0}','{1}','{2}','{3}')", mt.Name, mt.Description, mt.SerializeTradingRange(),mt.CloseTime);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => mt.ID = id, "id", "info_markettime");
            }
        }

        /// <summary>
        /// 返回帐户类别列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<SecurityFamilyImpl> SelectSecurity(int domainid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM info_security WHERE domain_id={0}",domainid);
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
                string query = string.Format("INSERT INTO info_security (`code`,`name`,`currency`,`type`,`multiple`,`pricetick`,`underlaying_fk`,`entrycommission`,`exitcommission`,`margin`,`extramargin`,`maintancemargin`,`exchange_fk`,`mkttime_fk`,`tradeable`,`domain_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}')", sec.Code, sec.Name, sec.Currency, sec.Type, sec.Multiple, sec.PriceTick, sec.UnderLayingFK, sec.EntryCommission, sec.ExitCommission, sec.Margin, sec.ExtraMargin, sec.MaintanceMargin, sec.ExchangeFK, sec.MarketTimeFK, sec.Tradeable ? 1 : 0,sec.Domain_ID);
                int row =  db.Connection.Execute(query);
                SetIdentity(db.Connection, id => sec.ID = id, "id", "info_security");

                return row > 0;
            }
        }

        /// <summary>
        /// 获得某个domain的所有域ID
        /// </summary>
        /// <param name="domainid"></param>
        /// <returns></returns>
        public static IEnumerable<SymbolImpl> SelectSymbol(int domainid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM info_symbols WHERE domain_id={0}",domainid);
                IEnumerable<SymbolImpl> result = db.Connection.Query<SymbolImpl>(query);
                return result;
            }

        }

        public static bool UpdateSymbol(SymbolImpl sym)
        {
            using (DBMySql db = new DBMySql())
            {
                //TLCtxHelper.Ctx.debug("orm update mktimefk:" + sec.MarketTimeFK.ToString() + " exchangefk:" + sec.ExchangeFK.ToString() + " underfk:" + sec.UnderLayingFK.ToString());
                string query = string.Format("UPDATE info_symbols SET entrycommission='{0}',exitcommission='{1}',margin='{2}',extramargin='{3}',maintancemargin='{4}' ,expiremonth='{5}',expiredate='{6}',tradeable='{7}' WHERE id='{8}'", sym._entrycommission, sym._exitcommission, sym._margin, sym._extramargin, sym._maintancemargin,0,sym.ExpireDate, sym.Tradeable ? 1 : 0, sym.ID);
                return db.Connection.Execute(query) >= 0;
            }
        }

        public static bool InsertSymbol(SymbolImpl sym)
        {
            using (DBMySql db = new DBMySql())
            {
                
                //string query = string.Format("INSERT INTO info_symbols (`symbol`,`entrycommission`,`exitcommission`,`margin`,`extramargin`,`maintancemargin`,`strike`,`optionside`,`expiremonth`,`expiredate`,`security_fk``underlaying_fk`,`underlayingsymbol_fk`,`tradeable`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}')", sym.Symbol, sym.EntryCommission, sym.ExitCommission, sym.Margin, sym.ExtraMargin, sym.MaintanceMargin, sym.Strike, sym.OptionSide, sym.ExpireMonth, sym.ExpireDate, sym.security_fk, sym.underlaying_fk, sym.underlayingsymbol_fk, sym.Tradeable ? 1 : 0);
                string query = string.Format("INSERT INTO info_symbols (`symbol`,`entrycommission`,`exitcommission`,`margin`,`extramargin`,`maintancemargin`,`strike`,`optionside`,`expiremonth`,`expiredate`,`security_fk`,`underlaying_fk`,`underlayingsymbol_fk`,`tradeable`,`domain_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}')", sym.Symbol, sym._entrycommission, sym._exitcommission, sym._margin, sym._extramargin, sym._maintancemargin, sym.Strike, sym.OptionSide,0, sym.ExpireDate, sym.security_fk, sym.underlaying_fk, sym.underlayingsymbol_fk, sym.Tradeable ? 1 : 0, sym.Domain_ID);
                
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

        /// <summary>
        /// 获得所有收款银行
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperReceivableAccount> SelectReceivableBanks()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "select a.id, a.name , a.bank_ac ,a.branch ,a.domain_id,a.bank_id,b.name as bankname from info_receivable_bankac a JOIN info_contract_bank b where a.bank_id = b.id";
                IEnumerable<JsonWrapperReceivableAccount> result = db.Connection.Query<JsonWrapperReceivableAccount>(query);
                return result;
            }
        }

        /// <summary>
        /// 更新收款银行信息
        /// </summary>
        /// <param name="bank"></param>
        public static void UpdateRecvBank(JsonWrapperReceivableAccount bank)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE info_receivable_bankac SET bank_id='{0}',name='{1}',bank_ac='{2}',branch='{3}' WHERE id='{4}'", bank.Bank_ID, bank.Name, bank.Bank_AC, bank.Branch, bank.ID);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 插入收款银行信息
        /// </summary>
        /// <param name="bank"></param>
        public static void InsertRecvBank(JsonWrapperReceivableAccount bank)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO info_receivable_bankac (`bank_id`,`name`,`bank_ac`,`branch`,`domain_id`) VALUES ( '{0}','{1}','{2}','{3}','{4}')", bank.Bank_ID, bank.Name, bank.Bank_AC, bank.Branch, bank.Domain_ID);
                db.Connection.Execute(query);
                SetIdentity(db.Connection, id => bank.ID = id, "id", "info_receivable_bankac");
            }
        }
    }
        

    
}
