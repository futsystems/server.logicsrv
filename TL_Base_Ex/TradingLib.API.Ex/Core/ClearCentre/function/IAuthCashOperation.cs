using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    /// <summary>
    /// 认证和出入金请求接口
    /// </summary>
    public interface IAuthCashOperation
    {
        /// <summary>
        /// 请求一个资金操作
        /// 请求入金或出金
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        bool RequestCashOperation(string account, decimal amount, QSEnumCashOperation op,out string opref,QSEnumCashOPSource source= QSEnumCashOPSource.Unknown );

        /// <summary>
        /// 验证交易帐户
        /// </summary>
        /// <param name="account"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        bool VaildAccount(string account, string pass);
    }
}
