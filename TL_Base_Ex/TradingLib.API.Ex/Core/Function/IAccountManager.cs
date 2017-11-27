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
        /// 删除交易帐户
        /// </summary>
        /// <param name="account"></param>
        void DelAccount(string account);

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
        /// 判定某个用户是否已经绑定了交易账户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool UserHaveAccount(int userID);

        /// <summary>
        /// 通过UserID查找对应账户
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        IAccount GetUserAccount(int userID);

        /// <summary>
        /// 为某个User添加交易账户
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        bool CreateAccountForUser(int userID, int agentID,CurrencyType currency, out string account);
        /// <summary>
        /// 直接进行出入金操作
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ammount"></param>
        /// <param name="comment"></param>
        void CashOperation(CashTransaction txn);


        /// <summary>
        /// 执行分红操作
        /// </summary>
        /// <param name="txn"></param>
        void PowerOperation(PowerTransaction txn);

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="template_id"></param>
        void UpdateAccountCommissionTemplate(string account, int template_id);

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="template_id"></param>
        void UpdateAccountMarginTemplate(string account, int template_id);


        /// <summary>
        /// 更新交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        void UpdateAccountExStrategyTemplate(string account, int template_id);


        void UpdateAccountConfigTemplate(string account, int template_id,bool force);


        /// <summary>
        /// 更新账户profile
        /// </summary>
        /// <param name="account"></param>
        /// <param name="name"></param>
        /// <param name="qq"></param>
        /// <param name="mobile"></param>
        /// <param name="idcard"></param>
        /// <param name="bank"></param>
        /// <param name="branch"></param>
        /// <param name="bankac"></param>
        void UpdateAccountProfile(string account, string name, string qq, string mobile, string idcard, int bank, string branch, string bankac);
    }
}
