using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace TradingLib.Contrib.NotifyCentre
{
    /// <summary>
    /// 联系方式维护器
    /// </summary>
    public class ContactTracker
    {

        /// <summary>
        /// 交易帐户与contact之间的映射
        /// </summary>
        ConcurrentDictionary<string, AccountContact> contactmap = new ConcurrentDictionary<string, AccountContact>();

        ConcurrentDictionary<int, NotifyeeContact> notifeemap = new ConcurrentDictionary<int, NotifyeeContact>();


        static ContactTracker defaultinstance = null;
        static ContactTracker()
        {
            defaultinstance = new ContactTracker();
        }

        private ContactTracker()
        {
            //从数据库加载contact到内存
            foreach (AccountContact c in ORM.MNotify.SelectAccountContacts())
            {
                contactmap.TryAdd(c.Account, c);
            }
            foreach (NotifyeeContact c in ORM.MNotify.SelectNotifyfeeContacts())
            {
                notifeemap.TryAdd(c.ID, c);
            }
        }

        /// <summary>
        /// 获得某个交易帐户的联系信息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static AccountContact GetAccountContract(string account)
        {
            return defaultinstance[account];
        }

        /// <summary>
        /// 返回某个类别的通知信息列表
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<NotifyeeContact> GetNotifeeContact(EnumNotifeeType type)
        {
            return defaultinstance.notifeemap.Values.Where(c => c.NotifeeType == type);
        }

        /// <summary>
        /// 获得某个帐户的联系方式
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public  AccountContact this[string account]
        {
            get
            {
                if (string.IsNullOrEmpty(account))
                    return null;
                AccountContact ct = null;
                if (contactmap.TryGetValue(account, out ct))
                {
                    return ct;
                }
                return null;
            }
        }

        /// <summary>
        /// 添加联系方式
        /// </summary>
        /// <param name="contact"></param>
        public static void AddContact(AccountContact contact)
        {
            if (defaultinstance.contactmap.Keys.Contains(contact.Account)) return;

            //插入联系方式
            ORM.MNotify.InsertContct(contact);
            //加入到缓存
            defaultinstance.contactmap.TryAdd(contact.Account, contact);
           
        }

        /// <summary>
        /// 更新某个交易帐户的手机号码信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="mobile"></param>
        public static void UpdateMobile(string account,string mobile)
        {
            if (defaultinstance.contactmap.Keys.Contains(account))
            {
                defaultinstance[account].Mobile = mobile;
                ORM.MNotify.UpdateMobile(account, mobile);
            }
            else
            {
                AccountContact ct = new AccountContact(account) { Mobile = mobile};
                AddContact(ct);
            }
        }

        /// <summary>
        /// 更新某个帐户的邮件信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="email"></param>
        public static void UpdateEmail(string account, string email)
        {
            if (defaultinstance.contactmap.Keys.Contains(account))
            {
                defaultinstance[account].Email = email;
                ORM.MNotify.UpdateEmail(account, email);
            }
            else
            {
                AccountContact ct = new AccountContact(account) { Email = email};
                AddContact(ct);
            }
        }

        /// <summary>
        /// 更新某个交易帐户的微信信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="weixin"></param>
        public static void UpdateWeiXin(string account, string weixin)
        {
            if (defaultinstance.contactmap.Keys.Contains(account))
            {
                defaultinstance[account].WeiXin = weixin;
                ORM.MNotify.UpdateWeiXin(account, weixin);
            }
            else
            {
                AccountContact ct = new AccountContact(account) { WeiXin=weixin };
                AddContact(ct);
            }
        }
    }
}
