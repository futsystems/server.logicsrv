using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.DataBase;

namespace TradingLib.ORM
{
    public class MManagerProfile : MBase
    {

        public static IEnumerable<ManagerProfile> SelectManagerProfile()
        {
            using (DBMySql db = new DBMySql())
            {
                string query = "SELECT * FROM manager_profile";
                return db.Connection.Query<ManagerProfile>(query);
            }
        }


        public static void DelManagerProfile(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("DELETE FROM manager_profile WHERE account = '{0}'", account);
                db.Connection.Execute(query);
            }
        }


        public static void InsertManagerProfile(ManagerProfile profile)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = String.Format("INSERT INTO manager_profile (`account`,`name`,`mobile`,`qq`,`email`,`idcard`,`bank_id`,`branch`,`bankac`,`memo`) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", profile.Account, profile.Name, profile.Mobile, profile.QQ, profile.Email, profile.IDCard, profile.Bank_ID, profile.Branch, profile.BankAC, profile.Memo);
                db.Connection.Execute(query);
            }
        }

        /// <summary>
        /// 更新交易账户所绑定的通道
        /// </summary>
        /// <param name="pair"></param>
        public static void UpdateManagerProfile(ManagerProfile profile)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("UPDATE manager_profile SET name='{0}',mobile='{1}',qq='{2}',email='{3}',idcard='{4}',bank_id='{5}',branch='{6}',bankac='{7}',memo='{8}' WHERE account='{9}'", profile.Name, profile.Mobile, profile.QQ, profile.Email, profile.IDCard, profile.Bank_ID, profile.Branch, profile.BankAC, profile.Memo, profile.Account);
                db.Connection.Execute(query);
            }
        }
    }
}
