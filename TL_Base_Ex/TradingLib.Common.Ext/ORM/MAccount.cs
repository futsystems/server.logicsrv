using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    internal class MaxAccountRef
    {
        public string AccountRef {get;set;}
    }

    internal class AccountAuth
    {
        public string Account { get; set; }
        public string Pass { get; set; }

    }

    internal class TotalCashAmount
    {
        public decimal Total { get; set; }
    }


    internal class UserID
    {
        public int ID { get; set; }
    }

    internal class TotalAccountNum
    {
        public int TotalNum { get; set; }
    }

    /// <summary>
    /// 数据库交易帐户field,用于生成IAccount对象
    /// </summary>
    internal class AccountFields
    {
        public string Account { get; set; }
        public string Pass { get; set; }
        public QSEnumAccountCategory Account_Category { get; set; }
        public QSEnumOrderTransferType Order_Route_Type { get; set; }

        public long CreatedTime { get; set; }
        public long SettleDateTime { get; set; }

        public decimal LastEquity { get; set; }
        public decimal LastCredit { get; set; }

        
        public bool IntraDay { get; set; }
        public CurrencyType Currency { get; set; }
        
        
        public int User_ID { get; set; }
        public int Mgr_fk { get; set; }
        public int rg_fk { get; set; }
        public int Margin_ID { get; set; }
        public int Commission_ID { get; set; }
        public int exstrategy_id { get; set; }
        public int config_id { get; set; }
        public int domain_id { get; set; }
        
        public long Confrim_TimeStamp { get; set; }

        public bool Deleted { get; set; }

        public int DeletedSettleday { get; set; }

    }

    internal class AccountLastEquity
    {
        public AccountLastEquity()
        {
            this.Account = string.Empty;
            this.NowEquity = 0M;
        }
        public string Account { get; set; }
        public decimal NowEquity { get; set; }
    }

    internal class AccountLastCredit
    {
        public AccountLastCredit()
        {
            this.Account = string.Empty;
            this.NowCredit = 0M;
        }

        public string Account { get; set; }

        public decimal NowCredit { get; set; }
    }

    internal class TransRefFields
    {
        public string Account { get; set; }
        public string TransRef { get; set; }
    }
    public class MAccount:MBase
    {

        #region 加载投资者账户
        static IAccount AccountFields2IAccount(AccountFields fields)
        {
            AccountImpl account = new AccountImpl(fields.Account);
            account.Pass = fields.Pass;
            account.UserID = fields.User_ID;
            account.CreatedTime = Util.ToDateTime(fields.CreatedTime);
            account.SettleDateTime = Util.ToDateTime(fields.SettleDateTime);
            account.OrderRouteType = fields.Order_Route_Type;
            account.IntraDay = fields.IntraDay;
            account.Category = fields.Account_Category;
            account.SettlementConfirmTimeStamp = fields.Confrim_TimeStamp;
            account.Mgr_fk = fields.Mgr_fk;
            account.Commission_ID = fields.Commission_ID;
            account.Margin_ID = fields.Margin_ID;
            account.Config_ID = fields.config_id;
            account.ExStrategy_ID = fields.exstrategy_id;
            account.Currency = fields.Currency;
            account.Deleted = fields.Deleted;
            account.DeletedSettleday = fields.DeletedSettleday;


            //绑定对象
            account.Domain = BasicTracker.DomainTracker[fields.domain_id];
            account.RouteGroup = BasicTracker.RouterGroupTracker[fields.rg_fk];
            
            return account;
        }


        /// <summary>
        /// 获得所有交易帐户
        /// </summary>
        /// <returns></returns>
        public static IList<IAccount> SelectAccounts()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM accounts");
                IList<IAccount> acclist = (from fileds in (db.Connection.Query<AccountFields>(query, null).ToList<AccountFields>())
                                           select AccountFields2IAccount(fileds)).ToList();

                return acclist;
            }
        }

        /// <summary>
        /// 获得某个交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static IAccount SelectAccount(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = query = string.Format("SELECT * FROM accounts WHERE account ='{0}'", account);

                AccountFields fields = db.Connection.Query<AccountFields>(query, null).Single<AccountFields>();
                if (fields != null)
                {
                    return AccountFields2IAccount(fields);
                }
                return null;
            }
        }

        #endregion

        #region 交易帐号相关操作
        /// <summary>
        /// 验证交易帐户和密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static bool ValidAccount(string account, string pass)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT a.account,a.pass FROM accounts a WHERE account = '{0}'", account);
                AccountAuth auth = db.Connection.Query<AccountAuth>(query, null).FirstOrDefault<AccountAuth>();
                if (auth == null)
                    return false;
                return auth.Pass.Equals(pass);
            }
        }

        /// <summary>
        /// 查询某个Manger的密码
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static string GetAccountPass(string login)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT a.account,a.pass FROM accounts a WHERE account = '{0}'", login);
                AccountAuth auth = db.Connection.Query<AccountAuth>(query, null).Single<AccountAuth>();
                return auth.Pass;
            }
        }


        /// <summary>
        /// 更新交易帐户密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public static bool UpdateAccountPass(string account, string pass)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET pass = '{0}' WHERE account = '{1}'", pass, account);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新路由组
        /// </summary>
        /// <param name="account"></param>
        /// <param name="gid"></param>
        /// <returns></returns>
        public static bool UpdateRouterGroup(string account,int gid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET rg_fk = '{0}' WHERE account = '{1}'", gid, account);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新账户类型
        /// </summary>
        /// <param name="account"></param>
        /// <param name="acctype"></param>
        /// <returns></returns>
        public static bool UpdateAccountRouterTransferType(string account, QSEnumOrderTransferType acctype)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET order_route_type = '{0}' WHERE account = '{1}'", acctype.ToString(), account);
                return db.Connection.Execute(query) >= 0;
            }
        }
        /// <summary>
        /// 更新账户类别 交易员，配资
        /// </summary>
        /// <param name="account"></param>
        /// <param name="ca"></param>
        /// <returns></returns>
        public static bool UpdateAccountCategory(string account, QSEnumAccountCategory ca)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET account_category = '{0}' WHERE account = '{1}'", ca.ToString(), account);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新账户类型
        /// </summary>
        /// <param name="account"></param>
        /// <param name="acctype"></param>
        /// <returns></returns>
        public static bool UpdateAccountInterday(string account, bool intraday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET intraday = '{0}' WHERE account = '{1}'", intraday?1:0, account);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新交易帐户标识
        /// </summary>
        /// <param name="account"></param>
        /// <param name="intraday"></param>
        /// <returns></returns>
        public static bool UpdateInvestorInfo(string account, string name,string broker,int bankfk,string bankac)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET name = '{0}',broker= '{1}' ,bankid='{2}', bankac='{3}' WHERE account = '{4}'", name, broker, bankfk, bankac, account);
                return db.Connection.Execute(query) >= 0;
            }
        }

        /// <summary>
        /// 更新交易帐户的代理ID
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool UpdateManagerID(string account, int id)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET mgr_fk = '{0}' WHERE account = '{1}'", id, account);
                return db.Connection.Execute(query) >= 0;
            }
        }


        /// <summary>
        /// 更新帐户手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAccountCommissionTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET commission_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新帐户保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAccountMarginTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET margin_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新账户交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAccountExStrategyTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET exstrategy_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新账户配置模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public static void UpdateAccountConfigTemplate(string account, int templateid)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET config_id = {0} WHERE account = '{1}'", templateid, account);
                db.Connection.Execute(query);
            }
        }


        /// <summary>
        /// 更新帐户的MAC地址
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mac"></param>
        /// <returns></returns>
        public static bool UpdateAccountMAC(string account, string mac)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET mac = '{0}' WHERE account = '{1}'", mac, account);
                return db.Connection.Execute(query) >= 0;
            }
        }

       

        /// <summary>
        /// 更新交易账户货币
        /// </summary>
        /// <param name="account"></param>
        /// <param name="currency"></param>
        public static void UpdateAccountCurrency(string account, CurrencyType currency)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET currency = '{0}' WHERE account = '{1}'", currency, account);
                db.Connection.Execute(query);
            }
        }
        #endregion

        #region 添加/删除 帐户操作 

        /// <summary>
        /// 检查某个类型的帐户是否申请了交易帐号
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        static bool HaveAnyAccount(QSEnumAccountCategory category)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("select count(*) as TotalNum from accounts where account_category='{0}'", category);
                TotalAccountNum num = db.Connection.Query<TotalAccountNum>(query, null).Single<TotalAccountNum>();
                if (num.TotalNum > 0)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 获得交易帐户前缀
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        static string GetPrefix(QSEnumAccountCategory category)
        {
            switch (category)
            {
                case QSEnumAccountCategory.SUBACCOUNT:
                    return GlobalConfig.SubPrefix;
                case QSEnumAccountCategory.SIGACCOUNT:
                    return "S";
                case QSEnumAccountCategory.MONITERACCOUNT:
                    return "M";
                case QSEnumAccountCategory.STRATEGYACCOUNT:
                    return "ST";
                default:
                    return GlobalConfig.SubPrefix;
            }
        }

        /// <summary>
        /// 获得某个类型的帐户的最大值
        /// 正则搜索 select * from accounts where account REGEXP '^98'
        ///  '^92[0-9]{4,10}$'
        ///  以92开头其余为[0-9],长度在4-10之间{』表示的是除掉prefi的长度
        ///  '^[A-Z]{2}55[0-9]{5}$'
        ///  以大写A-Z2位开头 接55 其余是5位0-9数字的 正则匹配
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static int MaxAccountRef(QSEnumAccountCategory category)
        {
            using (DBMySql db = new DBMySql())
            {
                string prefix = GetPrefix(category);
                //如果该类别的交易帐户存在，则以递增的方式插入新帐户
                if (HaveAnyAccount(category))
                {
                    string query = "select max(account) as AccountRef from accounts where account  REGEXP '^"+prefix+"[0-9]{"+(GlobalConfig.DefaultAccountLen-prefix.Length).ToString()+"}$'";
                    MaxAccountRef acref = db.Connection.Query<MaxAccountRef>(query, null).Single<MaxAccountRef>();
                    if (!(acref == null || acref.AccountRef == null))
                    {
                        return int.Parse(acref.AccountRef);
                    }
                }
                //如果没有插入过交易帐户 则以类别字头进行插入
                int code = int.Parse(prefix);
                int firstacc =  (code*(int)Math.Pow(10,GlobalConfig.DefaultAccountLen-prefix.Length));
                return firstacc;
            }
        }

        /// <summary>
        /// 某个UserID只能申请一个某个类型的交易帐号
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private static bool HaveRequested(int user_id, QSEnumAccountCategory category = QSEnumAccountCategory.SUBACCOUNT)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("select user_id as id from accounts where user_id='{0}' and account_category='{1}' and deleted='0'", user_id, category.ToString());
                return db.Connection.Query<UserID>(query, null).Count() > 0;
            }
        }

        /// <summary>
        /// 检查某个交易帐号是否已经存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private static bool ExistAccount(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("select user_id as id from accounts where account='{0}'",account);
                return db.Connection.Query<UserID>(query, null).Count() > 0;
            }
        }

        /// <summary>
        /// 某个User_id增加一个什么类型的帐号,并且密码设置为pass
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="pass"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public static bool AddAccount(ref AccountCreation create)
        {
            
            using (DBMySql db = new DBMySql())
            {
                //检查交易帐号 如果指定的交易帐号为空 则通过数据库内存放的帐号信息进行递增来获得当前添加帐号
                if (string.IsNullOrEmpty(create.Account))
                {
                    int acref = MaxAccountRef(create.Category);
                    //生成当前交易帐号
                    create.Account = (acref + 1).ToString();
                }
                else //指定添加的交易帐号
                {
                    //查看是否已经存在该帐号
                    if (ExistAccount(create.Account))
                    {
                        throw new FutsRspError("已经存在帐户:" + create.Account);  
                    }
                    //
                    //if (create.Account.StartsWith(GlobalConfig.SubPrefix))
                    //{
                    //    throw new FutsRspError("指定交易帐号不能使用默认前缀:" + GlobalConfig.SubPrefix);
                    //}
                }
                if (string.IsNullOrEmpty(create.Password))
                {
                    create.Password= GlobalConfig.DefaultPassword;
                }

                //如果user_id为非0编号 表明是由前端web网站调用的添加帐号,因此需要检查user_id是否已经申请过帐号
                if (create.UserID != 0)
                {
                    if (HaveRequested(create.UserID, create.Category))
                    {
                        throw new FutsRspError("用户已经申请过交易帐户");
                    }
                }
                Manager mgr = BasicTracker.ManagerTracker[create.BaseManagerID];

                string query = String.Format("Insert into accounts (`account`,`pass`,`account_category`,`order_route_type`,`createdtime`,`settledtime`,`user_id`,`mgr_fk`,`rg_fk`,`domain_id`,`currency` ) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}')", create.Account, create.Password, create.Category, create.RouterType, Util.ToTLDateTime(), Util.ToTLDateTime(DateTime.Now - new TimeSpan(1, 0, 0, 0, 0)), create.UserID, create.BaseManagerID, create.RouterID, mgr.Domain.ID,create.Currency);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 投资者账户 标注账户删除
        /// 账户删除为虚拟删除,记录账户删除时间，启动时候进行交易日判定，当投资者账户当前交易日已结算则不加载该账户
        /// 同时记录删除时的交易日，该交易结算之后就不用加载该账户交易记录
        /// </summary>
        /// <param name="account"></param>
        public static void MarkAccountDeleted(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET deleted = '1' , deletedtime='{0}', deletedsettleday='{1}' WHERE account = '{2}'", Util.ToTLDateTime(),TLCtxHelper.ModuleSettleCentre.Tradingday, account);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 数据库删除交易帐户 以及信息
        /// 删除帐户数据要彻底否则如果出现同名帐户，会出现错乱
        /// </summary>
        /// <param name="account"></param>
        public static void DelAccount(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string delquery = string.Empty;
                delquery = string.Format("DELETE FROM accounts WHERE account = '{0}'", account);//删除帐户列表
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM accounts_profile WHERE account='{0}'", account);//删除帐户Profile数据
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM cfg_rule WHERE account='{0}'", account);//删除帐户风控规则设置
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM hold_postransactions WHERE account='{0}'", account);//删除隔夜持仓
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM log_cashopreq WHERE account='{0}'", account);//删除出入金请求
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM log_cashtrans WHERE account='{0}'", account);//删除出入金记录
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM log_orderactions WHERE account='{0}'", account);//删除委托操作
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM log_orders WHERE account='{0}'", account);//删除委托
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM log_position_close_detail WHERE account='{0}'", account);//删除平仓明细
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM log_position_detail_hist WHERE account='{0}'", account);//删除持仓明细
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM log_postransactions WHERE account='{0}'", account);//删除交易回合
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM log_settlement WHERE account='{0}'", account);//删除帐户结算记录
                delquery = string.Format("DELETE FROM log_settlement_exchange WHERE account='{0}'", account);//删除帐户交易所结算记录

                delquery = string.Format("DELETE FROM log_trades WHERE account='{0}'", account);//删除交易回合
                db.Connection.Execute(delquery);

                delquery = string.Format("DELETE FROM tmp_orderactions WHERE account='{0}'", account);//删除日内交易记录
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM tmp_orders WHERE account='{0}'", account);//
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM tmp_trades WHERE account='{0}'", account);//
                db.Connection.Execute(delquery);
                delquery = string.Format("DELETE FROM tmp_postransactions WHERE account='{0}'", account);//
                db.Connection.Execute(delquery);
            }
        }

       

        #endregion

        


    }
}
