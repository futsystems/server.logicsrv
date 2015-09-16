using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class AccountManager
    {
        /// <summary>
        /// 更改账户密码
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public void UpdateAccountPass(string account, string pass)
        {
            if (!HaveAccount(account)) return;
            ORM.MAccount.UpdateAccountPass(account, pass);
        }

        /// <summary>
        /// 更新账户类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ca"></param>
        public void UpdateAccountCategory(string account, QSEnumAccountCategory ca)
        {
            if (!HaveAccount(account)) return;
            this[account].Category = ca;
            ORM.MAccount.UpdateAccountCategory(account, ca);
            AccountChanged(account);
        }

        /// <summary>
        /// 修改账户路由类别
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        public void UpdateAccountRouterTransferType(string account, QSEnumOrderTransferType type)
        {
            if (!HaveAccount(account)) return;
            this[account].OrderRouteType = type;
            ORM.MAccount.UpdateAccountRouterTransferType(account, type);
            AccountChanged(account);
        }

        /// <summary>
        /// 修改投资者信息
        /// </summary>
        /// <param name="account"></param>
        /// <param name="name"></param>
        /// <param name="broker"></param>
        /// <param name="bankfk"></param>
        /// <param name="bankac"></param>
        public void UpdateInvestorInfo(string account, string name, string broker, int bankfk, string bankac)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.Name = name;
            acc.Broker = broker;
            acc.BankAC = bankac;
            acc.BankID = bankfk;
            ORM.MAccount.UpdateInvestorInfo(account, name, broker, bankfk, bankac);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新交易帐户的ManagerID
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateManagerID(string account, int id)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.Mgr_fk = id;
            ORM.MAccount.UpdateManagerID(account, id);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新交易账户交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateAccountExStrategyTemplate(string account, int id)
        {
            if (!HaveAccount(account)) return;
            IAccount acc = this[account];
            acc.ExStrategy_ID = id;
            ORM.MAccount.UpdateAccountExStrategyTemplate(account, id);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountCommissionTemplate(string account, int templateid)
        {
            if (!HaveAccount(account)) return;
            this[account].Commission_ID = templateid;
            ORM.MAccount.UpdateAccountCommissionTemplate(account, templateid);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountMarginTemplate(string account, int templateid)
        {
            if (!HaveAccount(account)) return;
            this[account].Margin_ID = templateid;
            ORM.MAccount.UpdateAccountMarginTemplate(account, templateid);
            AccountChanged(account);
        }


        /// <summary>
        /// 更新账户日内交易设置
        /// </summary>
        /// <param name="account"></param>
        /// <param name="intraday"></param>
        public void UpdateAccountIntradyType(string account, bool intraday)
        {
            if (!HaveAccount(account)) return;
            this[account].IntraDay = intraday;
            ORM.MAccount.UpdateAccountInterday(account, intraday);
            AccountChanged(account);
        }

        public void UpdateRouterGroup(string account, int gid)
        {
            if (!HaveAccount(account)) return;
            this[account].RG_FK = gid;
            ORM.MAccount.UpdateRouterGroup(account, gid);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新帐户路由组
        /// </summary>
        /// <param name="account"></param>
        /// <param name="rg"></param>
        public void UpdateRouterGroup(string account, RouterGroup rg)
        {
            logger.Info("修改帐户路由组为:" + rg.Name);
            if (!HaveAccount(account)) return;
            this[account].RG_FK = rg.ID;
            ORM.MAccount.UpdateRouterGroup(account, rg.ID);
            AccountChanged(account);
        }


        /// <summary>
        /// 激活某个交易账户 允许其进行交易
        /// 某个账户激活后需要调用风控中心重新加载该账户的风控规则，使得风控规则复位
        /// </summary>
        /// <param name="id"></param>
        public void ActiveAccount(string id)
        {
            logger.Info("激活帐户:" + id);
            if (!HaveAccount(id)) return;
            this[id].Execute = true;
            TLCtxHelper.EventAccount.FireAccountActiveEvent(id);
            AccountChanged(id);
        }


        /// <summary>
        /// 禁止某个账户进行交易
        /// </summary>
        /// <param name="id"></param>
        public void InactiveAccount(string id)
        {
            logger.Info("冻结账户:" + id);
            if (!HaveAccount(id)) return;
            this[id].Execute = false;
            TLCtxHelper.EventAccount.FireAccountInactiveEvent(id);
            AccountChanged(id);
        }

        /// <summary>
        /// 交易账户的资金操作
        /// amount带有符号，正数表示入金 负数表示出金
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public void CashOperation(string account, decimal amount,QSEnumEquityType equitytype, string transref, string comment)
        {

            logger.Info("CashOperation ID:" + account + " Amount:" + amount.ToString() + " Comment:" + comment);
            IAccount acc = this[account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            //帐户自有资金的出入金操作
            if (equitytype == QSEnumEquityType.OwnEquity)
            {
                //金额检查
                if (amount < 0)
                {
                    if (acc.NowEquity < Math.Abs(amount))
                    {
                        throw new FutsRspError("出金额度大于帐户权益");
                    }
                }

                //执行时间检查 
                if (TLCtxHelper.ModuleSettleCentre.IsInSettle)
                {
                    throw new FutsRspError("系统正在结算,禁止出入金操作");
                }

                if (amount > 0)
                {
                    acc.Deposit(amount);
                }
                else
                {
                    acc.Withdraw(Math.Abs(amount));
                }
                ORM.MAccount.CashOperation(account, amount,QSEnumEquityType.OwnEquity, transref, comment);
            }
            
            if (equitytype == QSEnumEquityType.CreditEquity)
            {
                if (amount < 0)
                {
                    if (acc.Credit< Math.Abs(amount))
                    {
                        throw new FutsRspError("出金额度大于优先资金权益");
                    }
                }

                //执行时间检查 
                if (TLCtxHelper.ModuleSettleCentre.IsInSettle)
                {
                    throw new FutsRspError("系统正在结算,禁止出入金操作");
                }

                if (amount > 0)
                {
                    acc.CreditDeposit(Math.Abs(amount));
                }
                else
                {
                    acc.CreditWithdraw(Math.Abs(amount));
                }
                ORM.MAccount.CashOperation(account, amount, QSEnumEquityType.CreditEquity, transref, comment);                
            }

            TLCtxHelper.EventAccount.FireAccountCashOperationEvent(acc.ID, amount > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw, Math.Abs(amount));
        }

        /// <summary>
        /// 验证某个交易账户是否有效
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool VaildAccount(string account, string pass)
        {
            bool v = ORM.MAccount.ValidAccount(account, pass);
            v = v && HaveAccount(account);//检查风控中心是否记录该账号
            return v;
        }


    }
}
