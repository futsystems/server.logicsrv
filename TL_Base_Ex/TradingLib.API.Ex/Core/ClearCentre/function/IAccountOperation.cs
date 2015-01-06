using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TradingLib.API
{
    public interface IAccountOperationCritical
    {
        /// <summary>
        /// 更新账户类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ca"></param>
        void UpdateAccountCategory(string id, QSEnumAccountCategory ca);

        /// <summary>
        /// 更新账户路由转发类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        void UpdateAccountRouterTransferType(string id, QSEnumOrderTransferType type);

        /// <summary>
        /// 进行出入金操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ammount"></param>
        /// <param name="comment"></param>
        void CashOperation(string id, decimal ammount, string transref,string comment);

        /// <summary>
        /// 将某个账户资金复位到value值
        /// </summary>
        /// <param name="account"></param>
        /// <param name="value"></param>
        void ResetEquity(string account, decimal value);
    }

    /// <summary>
    /// 定义了账户操作接口
    /// </summary>
    public interface IAccountOperation
    {
        /// 查询是否存在某AccountID的交易账户
        /// </summary>
        /// <param name="accid"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        bool HaveAccount(string accid, out IAccount acc);

        /// <summary>
        /// 是否加载某帐户
        /// </summary>
        /// <param name="accid"></param>
        /// <returns></returns>
        bool HaveAccount(string accid);

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
        /// 通过全局UserID以及服务类别返回对应的交易帐号
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        IAccount QryAccount(int uid, QSEnumAccountCategory category);


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
        /// 修改某个交易账户密码
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="newpass"></param>
        void ChangeAccountPass(string id, string newpass);

        /// <summary>
        /// 更新账户日内交易设置
        /// </summary>
        /// <param name="account"></param>
        /// <param name="intraday"></param>
        void UpdateAccountIntradyType(string account, bool intraday);
    }

}
