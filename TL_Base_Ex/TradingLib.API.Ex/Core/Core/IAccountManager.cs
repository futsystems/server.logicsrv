using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 交易帐户管理功能接口
    /// </summary>
    public interface IAccountManager
    {
        /// <summary>
        /// 获得清算中心下所有交易账户
        /// </summary>
        IEnumerable<IAccount> Accounts { get; }

        /// <summary>
        /// 返回某个交易账户,通过交易帐号
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        IAccount this[string accid] { get; }

        /// <summary>
        /// 禁止某个账户进行交易
        /// </summary>
        /// <param name="id"></param>
        void InactiveAccount(string id);

        /// <summary>
        /// 激活某个交易账户 允许其进行交易
        /// </summary>
        /// <param name="id"></param>
        void ActiveAccount(string id);

        /// <summary>
        /// 更新交易账户密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newpass"></param>
        void UpdateAccountPass(string account, string newpass);


        /// <summary>
        /// 验证交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        bool VaildAccount(string account, string pass);

        /// <summary>
        /// 直接进行出入金操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ammount"></param>
        /// <param name="comment"></param>
        void CashOperation(string id, decimal ammount, string transref, string comment);

    }
}
