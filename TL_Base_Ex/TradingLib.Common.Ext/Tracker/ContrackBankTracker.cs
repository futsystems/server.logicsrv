using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Common
{
    public class DBContractBankTracker
    {
        Dictionary<string, ContractBank> bankmap = new Dictionary<string, ContractBank>();
        Dictionary<int, ContractBank> bankidxmap = new Dictionary<int, ContractBank>();
        Dictionary<int, JsonWrapperReceivableAccount> recvaccidxmap = new Dictionary<int, JsonWrapperReceivableAccount>();

        public DBContractBankTracker()
        {
            foreach (ContractBank b in ORM.MBasicInfo.SelectContractBanks())
            {
                bankmap[b.BrankID] = b;
                bankidxmap[b.ID] = b;
            }
            _receivableAccounts = ORM.MBasicInfo.SelectReceivableBanks();
            foreach (JsonWrapperReceivableAccount a in _receivableAccounts)
            {
                recvaccidxmap.Add(a.ID, a);
            }
        }

        /// <summary>
        /// 从收款银行编号获得对应的收款银行信息
        /// </summary>
        /// <param name="idx"></param>
        /// <returns></returns>
        public JsonWrapperReceivableAccount GetRecvBankAccount(int idx)
        { 
            JsonWrapperReceivableAccount tmp = null;
            if (recvaccidxmap.TryGetValue(idx, out tmp))
            {
                return tmp;
            }
            return null;
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

        public ContractBank this[int id]
        {
            get
            {
                if (bankidxmap.Keys.Contains(id))
                    return bankidxmap[id];
                return null;
            }
        }

        IEnumerable<JsonWrapperReceivableAccount> _receivableAccounts = null;
        public IEnumerable<JsonWrapperReceivableAccount> ReceivableAccounts
        {
            get
            {
                return _receivableAccounts;
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
