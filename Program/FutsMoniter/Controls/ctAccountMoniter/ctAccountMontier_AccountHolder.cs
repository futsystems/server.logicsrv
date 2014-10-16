using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;

namespace FutsMoniter.Controls
{
    public partial class ctAccountMontier
    {
        private ConcurrentDictionary<string, IAccountLite> accountmap = new ConcurrentDictionary<string, IAccountLite>();
        private ConcurrentDictionary<string, int> accountrowmap = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// 获得某个账户在datatable中的序号
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private int accountIdx(string account)
        {
            int rowid = -1;
            //map没有account键 还是会给out赋值,因此这里需要用if进行判断 来的到正确的逻辑 否则一致会返回0 出错
            if (!accountrowmap.TryGetValue(account, out rowid))
                return -1;
            else
                return rowid;
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

        IAccountLite accountselected = null;
        public IAccountLite AccountSetlected { get { return accountselected; } }

        //得到当前选择的行号
        private string CurrentAccount
        {

            get
            {
                int row =  (accountgrid.SelectedRows.Count > 0 ? accountgrid.SelectedRows[0].Index : -1);
                return accountgrid[0, row].Value.ToString();
            }
        }



        //通过行号得该行的Security
        IAccountLite GetVisibleAccount(string account)
        {
            if (HaveAccount(account))
                return accountmap[account];
            return null;
        }
    }
}
