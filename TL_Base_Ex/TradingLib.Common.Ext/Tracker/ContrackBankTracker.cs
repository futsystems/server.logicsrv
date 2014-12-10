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
            foreach (JsonWrapperReceivableAccount a in ORM.MBasicInfo.SelectReceivableBanks())
            {
                recvaccidxmap.Add(a.ID, a);
            }
        }

        public void UpdateRecvBank(JsonWrapperReceivableAccount recvbank)
        {
            JsonWrapperReceivableAccount target = null;
            //更新
            if (recvaccidxmap.TryGetValue(recvbank.ID, out target))
            {
                //target.Domain_ID = recvbank.Domain_ID;
                target.Name = recvbank.Name;
                target.Bank_AC = recvbank.Bank_AC;
                target.Branch = recvbank.Branch;

                target.Bank_ID = recvbank.Bank_ID;

                ORM.MBasicInfo.UpdateRecvBank(target);
                target.BankName = this[target.Bank_ID].Name;
            }
            else
            {
                target = new JsonWrapperReceivableAccount();
                target.Bank_AC = recvbank.Bank_AC;
                target.Bank_ID = recvbank.Bank_ID;
                target.BankName = this[target.Bank_ID].Name;
                target.Branch = recvbank.Branch;
                target.Domain_ID = recvbank.Domain_ID;
                target.Name = recvbank.Name;

                ORM.MBasicInfo.InsertRecvBank(target);
                recvbank.ID = target.ID;//外传数据库全局ID
                recvaccidxmap[target.ID] = target;

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

        /// <summary>
        /// 获取银行对象
        /// </summary>
        /// <param name="bankid"></param>
        /// <returns></returns>
        public ContractBank this[string bankid]
        {
            get
            {
                if (bankid == null)
                    return null;
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

        /// <summary>
        /// 所有收款银行列表
        /// </summary>
        public IEnumerable<JsonWrapperReceivableAccount> ReceivableAccounts
        {
            get
            {
                return recvaccidxmap.Values;
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
