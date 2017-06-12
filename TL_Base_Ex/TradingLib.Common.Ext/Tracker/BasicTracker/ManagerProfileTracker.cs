using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public class ManagerProfileTracker
    {
        Dictionary<string, ManagerProfile> profilemap = new Dictionary<string, ManagerProfile>();

        public ManagerProfileTracker()
        {
            foreach (var p in ORM.MManagerProfile.SelectManagerProfile())
            {
                profilemap[p.Account] = p;
            }
        }

        /// <summary>
        /// 获得所有Profile
        /// </summary>
        public IEnumerable<ManagerProfile> Profiles
        {
            get { return profilemap.Values; }
        }

        /// <summary>
        /// 获得某个交易帐户的Profile
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public ManagerProfile this[string account]
        {
            get
            {
                if (string.IsNullOrEmpty(account)) return null;
                ManagerProfile target = null;
                if (this.profilemap.TryGetValue(account, out target))
                {
                    return target;
                }
                else
                {
                    //如果不存在对应交易账户的profile信息 这里生成对应的默认profile加入 这样通过交易账号获得profile对象 均不可能为空，生成结算单时 避免了profile为空时的异常
                    ManagerProfile profile = new ManagerProfile();
                    profile.Account = account;
                    UpdateManagerProfile(profile);
                    return profilemap[account];
                }
            }
        }

        /// <summary>
        /// 删除交易账户对应的Profile信息
        /// </summary>
        /// <param name="account"></param>
        public void DropManager(string loginid)
        {
            AccountProfile target = null;
            if (this.profilemap.Keys.Contains(loginid))
            {
                this.profilemap.Remove(loginid);
            }
        }

        /// <summary>
        /// 更新某个交易帐户的个人信息
        /// </summary>
        /// <param name="profile"></param>
        public void UpdateManagerProfile(ManagerProfile profile)
        {
            ManagerProfile target = null;
            
            //已经存在
            if (this.profilemap.TryGetValue(profile.Account, out target))
            {
                ORM.MManagerProfile.UpdateManagerProfile(profile);
                target.Bank_ID = profile.Bank_ID;
                target.BankAC = profile.BankAC;
                target.Branch = profile.Branch;
                target.Email = profile.Email;
                target.IDCard = profile.IDCard;
                target.Mobile = profile.Mobile;
                target.Name = profile.Name;
                target.QQ = profile.QQ;
                target.Memo = profile.Memo;

            }
            else
            {
                ORM.MManagerProfile.InsertManagerProfile(profile);
                profilemap.Add(profile.Account, profile);
            }
        }

    }
}
