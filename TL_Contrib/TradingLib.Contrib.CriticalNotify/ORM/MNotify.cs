using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.ORM;
using TradingLib.Mixins.DataBase;

namespace TradingLib.Contrib.NotifyCentre.ORM
{

    internal class ContactNum
    {
        public int ContactCount { get; set; }
    }
    public class MNotify:MBase
    {
        /// <summary>
        /// 获得所有服务计划
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<AccountContact> SelectAccountContacts()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_ng_contact";
                return db.Connection.Query<AccountContact>(query, null);
            }
        }

        /// <summary>
        /// 插入联系方式
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static bool InsertContct(AccountContact ct)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_ng_contact (`account`,`email`,`mobile`,`weixin`) VALUES ( '{0}','{1}','{2}','{3}')", ct.Account, ct.Email, ct.Mobile, ct.WeiXin);
                return db.Connection.Execute(query) > 0;
            }
        }


        /// <summary>
        /// 查询数据库是否存在参数arg
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static bool HaveAccountContact(string account)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("SELECT count(*) as  ContactCount FROM contrib_ng_contact WHERE account='{0}'",account);
                ContactNum num = db.Connection.Query<ContactNum>(query).SingleOrDefault();
                if (num != null && num.ContactCount > 0)
                {
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// 更新邮件地址
        /// </summary>
        /// <param name="account"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool UpdateEmail(string account, string email)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveAccountContact(account))//更新
                {
                    string query = String.Format("UPDATE contrib_ng_contact SET email = '{0}' WHERE account = '{1}' ",email,account);
                    return db.Connection.Execute(query) >= 0;
                }
                else//插入
                {
                    AccountContact ct = new AccountContact(account) { Email = email };
                    string query = string.Format("INSERT INTO contrib_ng_contact (`account`,`email`,`mobile`,`weixin`) VALUES ( '{0}','{1}','{2}','{3}')", ct.Account, ct.Email, ct.Mobile, ct.WeiXin);
                    return db.Connection.Execute(query) >= 0;
                }
            }
        }

        /// <summary>
        /// 更新手机号码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool UpdateMobile(string account, string mobile)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveAccountContact(account))//更新
                {
                    string query = String.Format("UPDATE contrib_ng_contact SET mobile = '{0}' WHERE account = '{1}' ", mobile, account);
                    return db.Connection.Execute(query) >= 0;
                }
                else//插入
                {
                    AccountContact ct = new AccountContact(account) { Mobile = mobile };
                    string query = string.Format("INSERT INTO contrib_ng_contact (`account`,`email`,`mobile`,`weixin`) VALUES ( '{0}','{1}','{2}','{3}')", ct.Account, ct.Email, ct.Mobile, ct.WeiXin);
                    return db.Connection.Execute(query) >= 0;
                }
            }
        }

        /// <summary>
        /// 更新微信号码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="weixin"></param>
        /// <returns></returns>
        public static bool UpdateWeiXin(string account, string weixin)
        {
            using (DBMySql db = new DBMySql())
            {
                if (HaveAccountContact(account))//更新
                {
                    string query = String.Format("UPDATE contrib_ng_contact SET weixin = '{0}' WHERE account = '{1}' ", weixin, account);
                    return db.Connection.Execute(query) >= 0;
                }
                else//插入
                {
                    AccountContact ct = new AccountContact(account) { WeiXin = weixin };
                    string query = string.Format("INSERT INTO contrib_ng_contact (`account`,`email`,`mobile`,`weixin`) VALUES ( '{0}','{1}','{2}','{3}')", ct.Account, ct.Email, ct.Mobile, ct.WeiXin);
                    return db.Connection.Execute(query) >= 0;
                }
            }
        }


        /// <summary>
        /// 获得所有通知者列表
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<NotifyeeContact> SelectNotifyfeeContacts()
        {
            using (DBMySql db = new DBMySql())
            {
                const string query = "SELECT *  FROM contrib_ng_notifee";
                return db.Connection.Query<NotifyeeContact>(query, null);
            }
        }

        /// <summary>
        /// 插入通知者
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static bool InsertNotifeeContact(NotifyeeContact ct)
        {
            using (DBMySql db = new DBMySql())
            {
                string query = string.Format("INSERT INTO contrib_ng_notifee (`name`,`email`,`mobile`,`weixin`,`notifyee`) VALUES ( '{0}','{1}','{2}','{3}','{4}')", ct.Name, ct.Email, ct.Mobile, ct.WeiXin,ct.NotifeeType);
                return db.Connection.Execute(query) > 0;
            }
        }






    }
}
