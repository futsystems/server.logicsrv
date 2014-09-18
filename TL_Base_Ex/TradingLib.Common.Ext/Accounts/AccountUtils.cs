using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    public static class AccountUtils
    {

        public static string GetCustName(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.Name))
            {
                return LibUtil.GetEnumDescription(account.Category) + "[" + account.ID + "]";
            }
            return account.Name;
        }

        public static string GetCustBroker(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.Broker))
            {
                return GlobalConfig.DefaultBroker;
            }
            return account.Broker;
        }

        public static string GetCustBankID(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.BankID))
            {
                return GlobalConfig.DefaultBankID;
            }

            return account.BankID;
        }

        public static string GetCustBankAC(this IAccount account)
        {
            if (string.IsNullOrEmpty(account.BankAC))
            {
                return GlobalConfig.DefaultBankAC;
            }
            return account.BankAC;
        }
    }
}
