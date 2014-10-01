using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public class DBContractBankTracker
    {
        Dictionary<string, ContractBank> bankmap = new Dictionary<string, ContractBank>();

        public DBContractBankTracker()
        {
            foreach (ContractBank b in ORM.MBasicInfo.SelectContractBanks())
            {
                bankmap[b.BrankID] = b;
            }
        }

        public ContractBank this[string bankid]
        {
            get
            {
                if (bankmap.Keys.Contains(bankid))
                    return bankmap[bankid];
                return null;
            }
        }


        public string DefaultBankID
        {
            get
            {
                try
                {
                    return bankmap.Keys.First();
                }
                catch (Exception ex)
                {
                    return "0";
                }
            }
        }
        public bool HaveBankID(string bankid)
        {
            if (bankmap.Keys.Contains(bankid))
                return true;
            return false;
        }

        /// <summary>
        /// 返回所有银行列表
        /// </summary>
        public ContractBank[] Banks
        {
            get
            {
                return bankmap.Values.ToArray();
            }
        }
    }
}
