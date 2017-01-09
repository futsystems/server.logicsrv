using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static partial class DomainUtils
    {
        /// <summary>
        /// 返回该域的所有权限模板
        /// </summary>
        /// <param name="domain"></param>
        /// <returns></returns>
        public static IEnumerable<JsonWrapperReceivableAccount> GetRecvBanks(this Domain domain)
        {
            return BasicTracker.ContractBankTracker.ReceivableAccounts.Where(b => b.Domain_ID == domain.ID);
        }

        /// <summary>
        /// 获得某个域下的收款银行信息
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static JsonWrapperReceivableAccount GetRecvBank(this Domain domain,int id)
        {
            JsonWrapperReceivableAccount bank= BasicTracker.ContractBankTracker.GetRecvBankAccount(id);
            if (bank.Domain_ID == domain.ID)
                return bank;
            return null;
        }

        /// <summary>
        /// 更新银行帐户
        /// </summary>
        /// <param name="domain"></param>
        /// <param name="bank"></param>
        public static void UpdateRecvBanks(this Domain domain, JsonWrapperReceivableAccount bank)
        {
            bank.Domain_ID = domain.ID;
            BasicTracker.ContractBankTracker.UpdateRecvBank(bank);
        }
    }
}
