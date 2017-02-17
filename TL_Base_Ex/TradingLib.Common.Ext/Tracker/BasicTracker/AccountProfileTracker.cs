using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class AccountProfileTracker
    {
        Dictionary<string, AccountProfile> profilemap = new Dictionary<string, AccountProfile>();

        public AccountProfileTracker()
        {
            foreach (var p in ORM.MAccountProfile.SelectAccountProfile())
            {
                profilemap[p.Account] = p;
            }
        }

        /// <summary>
        /// 获得所有Profile
        /// </summary>
        public IEnumerable<AccountProfile> Profiles
        {
            get { return profilemap.Values; }
        }

        /// <summary>
        /// 获得某个交易帐户的Profile
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountProfile this[string account]
        {
            get
            {
                if (string.IsNullOrEmpty(account)) return null;
                AccountProfile target = null;
                if (this.profilemap.TryGetValue(account, out target))
                {
                    return target;
                }
                else
                {
                    //如果不存在对应交易账户的profile信息 这里生成对应的默认profile加入 这样通过交易账号获得profile对象 均不可能为空，生成结算单时 避免了profile为空时的异常
                    AccountProfile profile = new AccountProfile();
                    profile.Account = account;
                    UpdateAccountProfile(profile);
                    return profilemap[account];
                }
            }
        }

        /// <summary>
        /// 删除交易账户对应的Profile信息
        /// </summary>
        /// <param name="account"></param>
        public void DropAccount(string account)
        {
            AccountProfile target = null;
            if (this.profilemap.Keys.Contains(account))
            {
                this.profilemap.Remove(account);
            }
        }

        /// <summary>
        /// 更新某个交易帐户的个人信息
        /// </summary>
        /// <param name="profile"></param>
        public void UpdateAccountProfile(AccountProfile profile)
        {
            AccountProfile target = null;
            
            //已经存在
            if (this.profilemap.TryGetValue(profile.Account, out target))
            {
                ORM.MAccountProfile.UpdateAccountProfile(profile);
                target.Bank_ID = profile.Bank_ID;
                target.BankAC = profile.BankAC;
                target.Branch = profile.Branch;
                target.Email = profile.Email;
                target.IDCard = profile.IDCard;
                target.Mobile = profile.Mobile;
                target.Name = profile.Name;
                target.QQ = profile.QQ;
                target.Broker = profile.Broker;
                target.Memo = profile.Memo;

            }
            else
            {
                ORM.MAccountProfile.InsertAccountProfile(profile);
                profilemap.Add(profile.Account, profile);
            }
        }

    }
}
