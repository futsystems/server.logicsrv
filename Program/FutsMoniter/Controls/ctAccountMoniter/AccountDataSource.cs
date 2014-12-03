using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace FutsMoniter.Controls
{
    public class AccountDataSource
    {
        private ConcurrentDictionary<string, IAccountLite> accountmap = new ConcurrentDictionary<string, IAccountLite>();
        private ConcurrentDictionary<int, IAccountLite> accountrowmap = new ConcurrentDictionary<int, IAccountLite>();


        public List<string> GetAccountList()
        {
            return accountmap.Keys.ToList();
        }

         /// <summary>
        /// 查询是否存在某个交易帐号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private bool HaveAccount(string account)
        {
            return (accountmap.ContainsKey(account));
        }

        public string getaccount(int rowidx)
        {
            IAccountLite acc = accountrowmap[rowidx];
            return acc != null ? acc.Account : "wu";
        }
        int row = 0;
        public void GotAccountLite(IAccountLite acc)
        {
            if (HaveAccount(acc.Account))
                accountmap[acc.Account] = acc;
            else
            {
                accountmap.TryAdd(acc.Account, acc);
                
                accountrowmap.TryAdd(row,acc);
                row++;
            }
        }
            /// <summary>
        /// 获得某个账户在datatable中的序号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        //private int accountIdx(string account)
        //{
        //    int rowid = -1;
        //    //map没有account键 还是会给out赋值,因此这里需要用if进行判断 来的到正确的逻辑 否则一致会返回0 出错
        //    if (!accountrowmap.TryGetValue(account, out rowid))
        //        return -1;
        //    else
        //        return rowid;
        //}
        
    }
}
