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
        public QSEnumAccountCategory Account_Category { get; set; }
        public QSEnumOrderTransferType Order_Route_Type { get; set; }
        public bool IntraDay { get; set; }
        
        public DateTime CreatedTime { get; set; }
        public DateTime SettleDateTime { get; set; }
        public decimal LastEquity { get; set; }
        public int User_ID { get; set; }
        public long Confrim_TimeStamp { get; set; }
        public string MAC { get; set; }
        public string Token { get; set; }
        public bool PosLock { get; set; }

    }

    internal class AccountLastEquity
    {
        public AccountLastEquity()
        {
            this.Account = string.Empty;
            this.LastEquity = 0M;
        }
        public string Account { get; set; }
        public decimal LastEquity { get; set; }
    }


    internal class TransRefFields
    {
        public string Account { get; set; }
        public string TransRef { get; set; }
    }
    public class MAccount:MBase
    {
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
                try
                {
                    string query = String.Format("SELECT a.account,a.pass FROM accounts a WHERE account = '{0}'", account);
                    AccountAuth auth = db.Connection.Query<AccountAuth>(query, null).Single<AccountAuth>();
                    return auth.Pass.Equals(pass);
                }
                catch (Exception ex)
                {
                    return false;
                }
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
        public static bool UpdateAccountToken(string account, string token)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET token = '{0}' WHERE account = '{1}'", token, account);
                return db.Connection.Execute(query) >= 0;
            }
        }


        /// <summary>
        /// 更新帐户的锁仓权限
        /// </summary>
        /// <param name="account"></param>
        /// <param name="poslock"></param>
        /// <returns></returns>
        public static bool UpdateAccountPosLock(string account, bool poslock)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("UPDATE accounts SET poslock = '{0}' WHERE account = '{1}'", poslock?1:0, account);
                return db.Connection.Execute(query) >= 0;
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
        #endregion

        #region 出入金操作与统计
        /// <summary>
        /// 插入出入金记录
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static bool CashOperation(string account, decimal amount, string transref,string comment)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("Insert into transactions (`datetime`,`amount`,`comment`,`account`,`transref`,`settleday`) values('{0}','{1}','{2}','{3}','{4}','{5}')", DateTime.Now.ToString(), amount.ToString(), comment, account.ToString(),transref,TLCtxHelper.Ctx.SettleCentre.CurrentTradingday);
                return db.Connection.Execute(query) > 0;
            }
        }

        /// <summary>
        /// 帐户下某个transref是否存在
        /// </summary>
        /// <param name="account"></param>
        /// <param name="transref"></param>
        /// <returns></returns>
        public static bool IsTransRefExist(string account, string transref)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT * FROM transactions WHERE account={0} AND transref ={1}",account,transref);
                return db.Connection.Query<TransRefFields>(query, null).ToArray().Length > 0;
            }
        }

        /// <summary>
        /// 获得某个交易日的所有入金
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static decimal CashInOfTradingDay(string accId,int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT Sum(amount) as total FROM transactions where settleday ='{0}' and account='{1}' and amount>0",tradingday, accId);
                TotalCashAmount total = db.Connection.Query<TotalCashAmount>(query, null).Single<TotalCashAmount>();
                return total.Total;
            }
        }

        /// <summary>
        /// 某个时间端内所有入金之和
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static decimal CashIn(string accId, DateTime start, DateTime end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT Sum(amount) as total FROM transactions where datetime >='{0}'and datetime <= '{1}' and account='{2}' and amount>0", start.ToString(), end.ToString(), accId);
                TotalCashAmount total = db.Connection.Query<TotalCashAmount>(query, null).Single<TotalCashAmount>();
                return total.Total;
            }
        }

        /// <summary>
        /// 获得结算以来的出金
        /// </summary>
        /// <param name="accID"></param>
        /// <param name="start"></param>
        /// <returns></returns>
        public static decimal CashOutOfTradingDay(string accId,int tradingday)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT Sum(amount) as total FROM transactions where settleday ='{0}' and account='{1}' and amount<0", tradingday, accId);
                TotalCashAmount total = db.Connection.Query<TotalCashAmount>(query, null).Single<TotalCashAmount>();
                return total.Total;
            }
        }
        /// <summary>
        /// 统计某个时间段内所有出金总合
        /// </summary>
        /// <param name="accId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static decimal CashOut(string accId, DateTime start, DateTime end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("SELECT Sum(amount) as total FROM transactions where datetime >='{0}'and datetime <= '{1}' and account='{2}' and amount<0", start.ToString(), end.ToString(), accId);
                TotalCashAmount total = db.Connection.Query<TotalCashAmount>(query, null).Single<TotalCashAmount>();
                return total.Total;
            }

        }

        public static IList<CashTransaction> SelectHistCashTransaction(string account, int begin, int end)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT * FROM  {0}  WHERE settleday >='{1}' AND settleday <='{2}' AND account='{3}'", "transactions", begin, end, account);
                IList<CashTransaction> cts = db.Connection.Query<CashTransaction>(query).ToArray();

                return cts;
            }
        }
        #endregion


        #region 添加帐户操作 

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
        /// 获得某个类型的帐户的最大值
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static int MaxAccountRef(QSEnumAccountCategory category)
        {
            using (DBMySql db = new DBMySql())
            {

                //如果该类别的交易帐户存在，则以递增的方式插入新帐户
                if (HaveAnyAccount(category))
                {
                    string query = String.Format("select max(account) as AccountRef from accounts where account_category='{0}'", category.ToString());
                    MaxAccountRef acref = db.Connection.Query<MaxAccountRef>(query, null).Single<MaxAccountRef>();
                    if (!(acref == null || acref.AccountRef == null))
                    {
                        return int.Parse(acref.AccountRef);
                    }
                }
                //如果没有插入过交易帐户 则以类别字头进行插入
                int code = (int)category;
                return code * 10000;
            }
        }

        /// <summary>
        /// 某个UserID只能申请一个某个类型的交易帐号
        /// </summary>
        /// <param name="user_id"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private static bool HaveRequested(string user_id, QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("select user_id as id from accounts where user_id='{0}' and account_category='{1}'", user_id, category.ToString());
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
        public static bool AddAccount(out string account, string user_id = "0",string setaccount="", string pass = "123456", QSEnumAccountCategory category = QSEnumAccountCategory.DEALER)
        {
            using (DBMySql db = new DBMySql())
            {
                account = string.Empty;
                try
                {
                    //如果指定的交易帐号为空 则通过数据库内存放的帐号信息进行递增来获得当前添加帐号
                    if (string.IsNullOrEmpty(setaccount))
                    {
                        int acref = MaxAccountRef(category);
                        //生成当前交易帐号
                        account = (acref + 1).ToString();
                    }
                    else
                    {
                        if (setaccount.StartsWith("9"))
                        {
                            TLCtxHelper.Debug("自定义帐号不能以9开头");
                            return false;
                        }
                        if (ExistAccount(setaccount))
                        {
                            TLCtxHelper.Debug("设定的交易帐号:" + setaccount + "已经存在");
                            return false;
                        }
                        else
                        {
                            account = setaccount;
                        }
                    }
                    if (string.IsNullOrEmpty(pass))
                    {
                        pass = GlobalConfig.DefaultPassword;
                    }
                    //如果user_id为非0编号 表明是由前端web网站调用的添加帐号,因此需要检查user_id是否已经申请过帐号
                    if (user_id != "0")
                    { 
                        if(HaveRequested(user_id, category))
                        {
                            TLCtxHelper.Ctx.debug(string.Format("UserID:{0} have already register account:{1}",user_id,category));
                            return false;
                        }
                    }
                    string query = String.Format("Insert into accounts (`account`,`user_id`,`createdtime`,`pass`,`account_category`,`settledatetime`) values('{0}','{1}','{2}','{3}','{4}','{5}')", account, user_id.ToString(), DateTime.Now.ToString(), pass, category, DateTime.Now - new TimeSpan(1, 0, 0, 0, 0));
                    return db.Connection.Execute(query) > 0;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }

        public static int GetAccountUserID(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                try
                {
                    string query = String.Format("select user_id as id from accounts where `account` = '{0}'", account);
                    UserID userid = db.Connection.Query<UserID>(query, null).SingleOrDefault<UserID>();
                    return userid.ID;
                }
                catch (Exception ex)
                {
                    return 0;
                }
            }
        }



        static IAccount AccountFields2IAccount(AccountFields fields)
        {
            IAccount account = AccountHelper.CreateAccount(fields.Account);
            account.LastEquity = fields.LastEquity;
            account.UserID = fields.User_ID;
            account.CreatedTime = fields.CreatedTime;
            account.SettleDateTime = fields.SettleDateTime;//Util.ToDateTime(fields.SettleDateTime);
            account.OrderRouteType = fields.Order_Route_Type;
            account.IntraDay = fields.IntraDay;
            account.Category = fields.Account_Category;
            account.SettlementConfirmTimeStamp = fields.Confrim_TimeStamp;
            account.MAC = fields.MAC;
            account.Token = fields.Token;
            account.PosLock = fields.PosLock;
            //TLCtxHelper.Debug("fileds route:" + fields.Order_Router_Type.ToString() +" category:"+fields.Account_Category.ToString()) ;
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
        /// 查询某个交易帐户的昨日权益
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static decimal GetAccountLastEquity(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT account,lastequity FROM accounts WHERE account = '{0}'",account);
                AccountLastEquity lastequity = db.Connection.Query<AccountLastEquity>(query).SingleOrDefault();

                return lastequity.LastEquity;
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

    }
}
