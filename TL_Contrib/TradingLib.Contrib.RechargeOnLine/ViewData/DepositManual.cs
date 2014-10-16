using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotLiquid;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.RechargeOnLine
{
    public class DepositManualViewData : Drop
    {
        public static DepositManualViewData GetDepositManualViewData()
        {
            DepositManualViewData viewdata = new DepositManualViewData();
            viewdata.BankAccountList = BasicTracker.ContractBankTracker.ReceivableAccounts.Select(a => a.ToDrop()).ToList();
            //viewdata.Name = "xyz";
            //Util.Debug("got bank account list:" + viewdata.BankAccountList.Count().ToString());
            return viewdata;
                
        }

        //public string Name { get; set; }
        /// <summary>
        /// 收款银行列表
        /// </summary>
        public List<ReceiveableAccount> BankAccountList { get; set; }
    }
}
