using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
        static string AccountTypeName = "";
        static Type AccountType = null;
        static Dictionary<string, Type> accounttypemap = new Dictionary<string, Type>();
        static ConfigDB  _cfgdb = new ConfigDB("Account");
            
        /// <summary>
        /// 静态初始化
        /// </summary>
        static AccountBase()
        {
            foreach (Type t in PluginHelper.LoadAccountType())
            {
                accounttypemap.Add(t.FullName, t);
            }

            if (!_cfgdb.HaveConfig("AccountType"))
            {
                _cfgdb.UpdateConfig("AccountType", QSEnumCfgType.String, "Lottoqq.Account.AccountImpl", "系统使用的帐户类型");
            }
            string acctype = _cfgdb["AccountType"].AsString();
            SetAccountType(acctype);
        }


        /// <summary>
        /// 设定帐户类型
        /// </summary>
        /// <param name="typename"></param>
        static void SetAccountType(string typename)
        {
            if (string.IsNullOrEmpty(typename))
            {
                Util.Debug(Util.GlobalPrefix + "AccountType not setted,used default AccountBase");
                return;
            }

            if (string.IsNullOrEmpty(AccountTypeName) && AccountType == null)
            {
                if (accounttypemap.Keys.Contains(typename))
                {
                    AccountTypeName = typename;
                    AccountType = accounttypemap[AccountTypeName];
                    Util.Debug(Util.GlobalPrefix + "use user-defined AccountType:" + AccountTypeName);
                }
                else
                {
                    Util.Debug(Util.GlobalPrefix + "Account Type:" + typename + " can not be loaded form dll,please check");
                }
            }
            
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IAccount CreateAccount(string id)
        {
            if (AccountType == null)
            {
                if (accounttypemap.Keys.Contains(AccountTypeName))
                {
                    //检查是否有以单个string为参数的构造函数

                    AccountType = accounttypemap[AccountTypeName];
                    Util.Debug(Util.GlobalPrefix + "use user-defined AccountType:" + AccountTypeName);
                }
                else
                {
                    AccountType = typeof(AccountBase);
                }
            }
            return (IAccount)Activator.CreateInstance(AccountType, new object[] { id});
            
        }

    }
}
