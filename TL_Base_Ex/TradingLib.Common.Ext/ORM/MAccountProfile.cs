using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    //internal class demoxxx
    //{
    //    public string Account { get; set; }
    //    public string Name { get; set; }
    //}
    public class MAccountProfile : MBase
    {
        /// <summary>
        /// 获取所有交易帐户的附加信息
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AccountProfile> SelectAccountProfile()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM accounts_profile";
                return db.Connection.Query<AccountProfile>(query);//.Select(d => { AccountProfile p = new AccountProfile(); p.Account = d.Account; p.Name = d.Name; return p; });
            }
        }

        /// <summary>
        /// 删除某个交易帐户的个人信息
        /// </summary>
        /// <param name="account"></param>
        public static void DelAccountProfile(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM accounts_profile WHERE account = '{0}'", account);
                db.Connection.Execute(query);
            }
        }
        /// <summary>
        /// 插入一条交易帐户与成交通道的绑定关系
        /// </summary>
        /// <param name="pair"></param>
        public static void InsertAccountProfile(AccountProfile profile)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO accounts_profile (`account`,`name`,`mobile`,`qq`,`email`,`idcard`,`bank_id`,`branch`,`bankac`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}')", profile.Account, profile.Name, profile.Mobile, profile.QQ, profile.Email, profile.IDCard, profile.Bank_ID, profile.Branch, profile.BankAC);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新交易账户所绑定的通道
        /// </summary>
        /// <param name="pair"></param>
        public static void UpdateAccountProfile(AccountProfile profile)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE accounts_profile SET name='{0}',mobile='{1}',qq='{2}',email='{3}',idcard='{4}',bank_id='{5}',branch='{6}',bankac='{7}' WHERE account='{8}'", profile.Name, profile.Mobile, profile.QQ, profile.Email, profile.IDCard, profile.Bank_ID, profile.Branch, profile.BankAC, profile.Account);
                db.Connection.Execute(query);
            }
        }
    }
}
