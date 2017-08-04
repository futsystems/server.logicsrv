using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
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
        public void UpdateAccountPass(string id, string pass)
        {
            IAccount account = this[id];
            if (account == null) return;
            
            ORM.MAccount.UpdateAccountPass(id, pass);
            account.Pass = pass;
        }

        /// <summary>
        /// 更新账户类别
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ca"></param>
        public void UpdateAccountCategory(string id, QSEnumAccountCategory ca)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Category = ca;
            ORM.MAccount.UpdateAccountCategory(id, ca);
            AccountChanged(account);
        }

        /// <summary>
        /// 修改账户路由类别
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="type"></param>
        public void UpdateAccountRouterTransferType(string id, QSEnumOrderTransferType type)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.OrderRouteType = type;
            ORM.MAccount.UpdateAccountRouterTransferType(id, type);
            AccountChanged(account);
        }


        /// <summary>
        /// 更新交易帐户货币
        /// </summary>
        /// <param name="account"></param>
        /// <param name="currency"></param>
        public void UpdateAccountCurrency(string id, CurrencyType currency)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Currency = currency;
            ORM.MAccount.UpdateAccountCurrency(id, currency);
            AccountChanged(account);
        }
        /// <summary>
        /// 更新交易帐户的ManagerID
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateManagerID(string id, int mgr_id)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Mgr_fk = mgr_id;
            ORM.MAccount.UpdateManagerID(id, mgr_id);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新交易账户交易参数模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="id"></param>
        public void UpdateAccountExStrategyTemplate(string id, int tempate_id)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.ExStrategy_ID = tempate_id;
            ORM.MAccount.UpdateAccountExStrategyTemplate(id, tempate_id);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新手续费模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountCommissionTemplate(string id, int templateid)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Commission_ID = templateid;
            ORM.MAccount.UpdateAccountCommissionTemplate(id, templateid);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新保证金模板
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountMarginTemplate(string id, int templateid)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Margin_ID = templateid;
            ORM.MAccount.UpdateAccountMarginTemplate(id, templateid);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新配置模板
        /// </summary>
        /// <param name="id"></param>
        /// <param name="templateid"></param>
        public void UpdateAccountConfigTemplate(string id, int templateid)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Config_ID = templateid;
            ORM.MAccount.UpdateAccountConfigTemplate(id, templateid);
            AccountChanged(account);
        }

        /// <summary>
        /// 更新账户日内交易设置
        /// </summary>
        /// <param name="account"></param>
        /// <param name="intraday"></param>
        public void UpdateAccountIntradyType(string id, bool intraday)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.IntraDay = intraday;
            ORM.MAccount.UpdateAccountInterday(id, intraday);
            AccountChanged(account);
        }

        public void UpdateRouterGroup(string id, int gid)
        {
            IAccount account = this[id];
            if (account == null) return;
            (account as AccountImpl).RouteGroup = BasicTracker.RouterGroupTracker[gid];
            ORM.MAccount.UpdateRouterGroup(id, gid);
            
            AccountChanged(account);
        }

        /// <summary>
        /// 更新帐户路由组
        /// </summary>
        /// <param name="account"></param>
        /// <param name="rg"></param>
        public void UpdateRouterGroup(string id, RouterGroup rg)
        {
            IAccount account = this[id];
            if (account == null) return;
            (account as AccountImpl).RouteGroup = BasicTracker.RouterGroupTracker[rg.ID];
            ORM.MAccount.UpdateRouterGroup(id, rg.ID);
            AccountChanged(account);
        }


        /// <summary>
        /// 激活某个交易账户 允许其进行交易
        /// 某个账户激活后需要调用风控中心重新加载该账户的风控规则，使得风控规则复位
        /// </summary>
        /// <param name="id"></param>
        public void ActiveAccount(string id)
        {
            IAccount account = this[id];
            if (account == null) return;
            this[id].Execute = true;
            TLCtxHelper.EventAccount.FireAccountActiveEvent(account);
            AccountChanged(account);
        }


        /// <summary>
        /// 禁止某个账户进行交易
        /// </summary>
        /// <param name="id"></param>
        public void InactiveAccount(string id)
        {
            IAccount account = this[id];
            if (account == null) return;
            account.Execute = false;
            TLCtxHelper.EventAccount.FireAccountInactiveEvent(account);
            AccountChanged(account);
        }


       
        
        

        /// <summary>
        /// 执行分红操作
        /// 讲分红操作生成对应的出入金操作
        /// </summary>
        /// <param name="txn"></param>
        public void PowerOperation(PowerTransaction txn)
        {
            logger.Info("Dividend {0} into Account:{1}".Put(txn.Dividend, txn.Dividend));
            IAccount acc = this[txn.Account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            //没有金额调整
            if (txn.Dividend == 0 && txn.Amount == 0) return;


            CashTransaction cashtxn = new CashTransactionImpl();
            cashtxn.Account = txn.Account;

            bool isdeposit = txn.Dividend > 0;
            if (isdeposit)
            {
                cashtxn.Amount = txn.Dividend;
            }
            else
            {
                cashtxn.Amount = txn.Amount;
            }
            cashtxn.DateTime = Util.ToTLDateTime();
            cashtxn.EquityType = QSEnumEquityType.OwnEquity;
            cashtxn.Operator = "System";
            cashtxn.Settled = false;
            cashtxn.Settleday = txn.Settleday;

            cashtxn.TxnType = isdeposit ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw;

            cashtxn.Comment = isdeposit?("持仓{0}[{1}] 分红:{2}".Put(txn.Size, txn.Symbol, txn.Dividend)):("持仓{0}[{1}] 应付:{2}".Put(txn.Size,txn.Symbol,txn.Amount));

            cashtxn.TxnID = _txnTracker.AssignId.ToString();
            acc.LoadCashTrans(cashtxn);
            TLCtxHelper.ModuleDataRepository.NewCashTransaction(cashtxn);

        
        }
        /// <summary>
        /// 交易账户的资金操作
        /// amount带有符号，正数表示入金 负数表示出金
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="amount"></param>
        /// <param name="comment"></param>
        public void CashOperation(CashTransaction txn)
        {

            logger.Info("CashOperation ID:" + txn.Account + " Amount:" + txn.Amount.ToString() + " Txntype:" + txn.TxnType.ToString() + " EquityType:" + txn.EquityType.ToString());
            IAccount acc = this[txn.Account];
            if (acc == null)
            {
                throw new FutsRspError("交易帐户不存在");
            }

            //执行时间检查 
            if (TLCtxHelper.ModuleSettleCentre.SettleMode != QSEnumSettleMode.StandbyMode)
            {
                throw new FutsRspError("系统正在结算,禁止出入金操作");
            }

            if (txn.TxnType == QSEnumCashOperation.WithDraw)
            {
                if (txn.EquityType == QSEnumEquityType.OwnEquity && txn.Amount > acc.NowEquity)
                {
                    throw new FutsRspError("出金额度大于帐户权益");
                }
                if (txn.EquityType == QSEnumEquityType.CreditEquity && txn.Amount > acc.Credit)
                {
                    throw new FutsRspError("出金额度大于帐户信用额度");
                }
            }

            //生成唯一序列号
            txn.TxnID = _txnTracker.AssignId.ToString();

            acc.LoadCashTrans(txn);
            TLCtxHelper.ModuleDataRepository.NewCashTransaction(txn);
            //ORM.MCashTransaction.InsertCashTransaction(txn);
            //TLCtxHelper.EventAccount.FireAccountCashOperationEvent(txn.Account,txn., Math.Abs(amount));
        }

        /// <summary>
        /// 验证某个交易账户是否有效
        /// </summary>
        /// <param name="ac"></param>
        /// <param name="pass"></param>
        /// <returns></returns>
        public bool VaildAccount(string id, string pass)
        {
            IAccount account = this[id];
            if (account == null) return false;
            //数据库验证交易帐户ID与密码
            return ORM.MAccount.ValidAccount(id, pass);
        }


    }
}
