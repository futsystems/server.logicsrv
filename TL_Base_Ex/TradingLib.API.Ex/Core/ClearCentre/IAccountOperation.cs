using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 定义了账户操作接口
    /// </summary>
    public interface IAccountOperation
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
    }

}