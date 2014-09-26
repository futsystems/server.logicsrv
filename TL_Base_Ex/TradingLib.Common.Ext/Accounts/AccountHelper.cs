using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 用于按照设定生成对应的Account实例
    /// </summary>
    public class AccountHelper
    {
        
        static string AccountTypeName = "";
        static Type AccountType = null;
        static ConcurrentDictionary<string, Type> accounttypemap = new ConcurrentDictionary<string, Type>();

        static AccountHelper()
        {
            foreach (Type t in PluginHelper.LoadAccountType())
            {
                accounttypemap.TryAdd(t.FullName, t);
            }
        }



        public static void print()
        { 
            foreach(KeyValuePair<string,Type> v in accounttypemap)
            {
                TLCtxHelper.Debug(v.Key);
            }
        }

        /// <summary>
        /// 设定帐户类型
        /// </summary>
        /// <param name="typename"></param>
        public static void SetAccountType(string typename)
        {
            if (string.IsNullOrEmpty(typename))
            {
                TLCtxHelper.Debug(Util.GlobalPrefix+"AccountType not setted,used default AccountBase");
                return;
            }

            if (string.IsNullOrEmpty(AccountTypeName) && AccountType == null)
            {
                if (accounttypemap.Keys.Contains(typename))
                {
                    AccountTypeName = typename;
                    AccountType = accounttypemap[AccountTypeName];
                    TLCtxHelper.Debug(Util.GlobalPrefix + "use user-defined AccountType:" + AccountTypeName);
                }
                else
                {
                    TLCtxHelper.Debug(Util.GlobalPrefix + "Account Type:" + typename + " can not be loaded form dll,please check");
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
                    TLCtxHelper.Debug(Util.GlobalPrefix + "use user-defined AccountType:" + AccountTypeName);
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
