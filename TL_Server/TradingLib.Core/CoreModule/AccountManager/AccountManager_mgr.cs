//Copyright 2013 by FutSystems,Inc.
//20170112 �������ò���

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class AccountManager
    {

        
        /// <summary>
        /// ͳһʹ��AccountCreation���󴴽������ʻ�
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddAccountFacde", "AddAccountFacde - add  account", "��ӽ����ʺ�", QSEnumArgParseType.Json)]
        public void CTE_AddAccountFacde(ISession session, string json)
        {
            Manager manager = session.GetManager();
            var creation = json.DeserializeObject<AccountCreation>();// Mixins.Json.JsonMapper.ToObject<AccountCreation>(json);
            var account = creation.Account;

            //���ʻ���Ŀ���
            if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("�ʻ���Ŀ�ﵽ����:" + manager.Domain.AccLimit.ToString());
            }

            if (creation.BaseManagerID == 0)
            {
                creation.BaseManagerID = manager.BaseMgrID;
            }

            //�������RootȨ�޵�Manager��Ҫ����ִ��Ȩ�޼��
            if (!manager.IsInRoot())
            {
                //�������Ϊ����������ʻ�,��������Ҫ�жϵ�ǰManager�������Ƿ�ӵ�����������Ȩ��
                if (manager.BaseMgrID != creation.BaseManagerID)
                {
                    if (!manager.IsParentOf(creation.BaseManagerID))
                    {
                        throw new FutsRspError("��Ȩ�ڸù��������ʻ�");
                    }
                }
            }

            //Manager�ʻ��������� ��������Լ�����������ӽ����ʻ� ����Ҫ����ʻ�����
            int limit = manager.BaseManager.AccLimit;

            int cnt = manager.GetVisibleAccount().Count();//��ø�manger�����������ʻ���Ŀ
            if (cnt >= limit)
            {
                throw new FutsRspError("�ɿ��ʻ�������������:" + limit.ToString());
            }


            //ִ�в��� �������쳣 �����쳣���������ر�
            this.AddAccount(ref creation);//�������ʻ����뵽����

            //�ʻ������Ϻ�ͬ�����profile��Ϣ
            creation.Profile.Account = creation.Account;

            //�����µ�profile
            BasicTracker.AccountProfileTracker.UpdateAccountProfile(creation.Profile);

            //���ⴥ�������ʺ�����¼�
            TLCtxHelper.EventAccount.FireAccountAddEvent(this[creation.Account]);

            session.RspMessage("���������ʺ�:" + creation.Account + "�ɹ�");

        }

        /// <summary>
        /// Ϊĳ��User���������˻�
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public bool CreateAccountForUser(int userID, int agentID,out string account)
        {
            account = string.Empty;
            try
            {
                AccountCreation creation = new AccountCreation();
                creation.BaseManagerID = agentID;
                creation.Category = QSEnumAccountCategory.SUBACCOUNT;
                creation.RouterType = QSEnumOrderTransferType.SIM;
                creation.UserID = userID;
                this.AddAccount(ref creation);
                //�ʻ������Ϻ�ͬ�����profile��Ϣ
                creation.Profile.Account = creation.Account;

                //�����µ�profile
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(creation.Profile);

                //���ⴥ�������ʺ�����¼�
                TLCtxHelper.EventAccount.FireAccountAddEvent(this[creation.Account]);

                account = creation.Account;
                return true;
            }
            catch (Exception ex)
            {
                logger.Error("Create Account Error:" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// ����ɾ�������ʻ�
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelAccount", "DelAccount - del account", "ɾ�������ʻ�", QSEnumArgParseType.CommaSeparated)]
        public void CTE_DelAccount(ISession session, string account)
        {
            IAccount acc = this[account];
            if (acc == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            //��齻���ʻ��ʽ�
            if (_deleteAccountCheckEquity && (acc.NowEquity > 1 || acc.Credit > 1))
            {
                throw new FutsRspError(string.Format(string.Format("�����ʻ�:{0} Ȩ��:{1} ���ö��:{2}δ���� �޷�ɾ��", account, acc.NowEquity, acc.Credit)));
            }

            this.DelAccount(account);

            session.RspMessage("�����ʻ�:" + account + " ɾ���ɹ�");
        }


        /// <summary>
        /// ��ѯ�����ʻ���Profile
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountProfile", "QryAccountProfile - qry profile account", "��ѯ�����ʻ�������Ϣ")]
        public void CTE_QryAccountProfile(ISession session, string account)
        {
            Manager mgr = session.GetManager();
            if (mgr == null) throw new FutsRspError("����Ա������");

            AccountProfile profile = BasicTracker.AccountProfileTracker[account];
            
            //���������Ϣ������ ����Ӹ�����Ϣ
            if (profile == null)
            {
                profile = new AccountProfile();
                profile.Account = account;

                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }
            session.ReplyMgr(profile);           

        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountProfile", "UpdateAccountProfile - update account profile", "���½����ʻ�������Ϣ",QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountProfile(ISession session, string json)
        {
            Manager mgr = session.GetManager();
            if (mgr == null) throw new FutsRspError("����Ա������");

            var profile = json.DeserializeObject<AccountProfile>();// Mixins.Json.JsonMapper.ToObject<AccountProfile>(json);
            IAccount account = TLCtxHelper.ModuleAccountManager[profile.Account];

            if (account != null)
            {
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }
            //���������ʻ��䶯�¼�
            TLCtxHelper.EventAccount.FireAccountChangeEent(account);

            session.RspMessage("���¸�����Ϣ�ɹ�");

        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCategory", "UpdateAccountCategory - change account category", "�޸��ʻ����", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCategory(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var category = Util.ParseEnum<QSEnumAccountCategory>(req["category"].ToString());
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountCategory(account, category);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountExecute", "UpdateAccountExecute - change account execute", "�޸��ʻ�����Ȩ��״̬", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountExecute(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var execute = bool.Parse(req["execute"].ToString());
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                if (execute && !acct.Execute)
                {
                    this.ActiveAccount(account);
                }
                if (!execute && acct.Execute)
                {
                    this.InactiveAccount(account);
                }

            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountIntraday", "UpdateAccountIntraday - change account intraday setting", "�޸��ʻ����ڽ�������", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountIntraday(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var intraday = bool.Parse(req["intraday"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountIntradyType(account, intraday);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateRouteType", "UpdateRouteType - change account route type", "�޸��ʻ���·������", QSEnumArgParseType.Json)]
        public void CTE_UpdateRouteType(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountRouterTransferType(account, routetype);
            }
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCurrency", "UpdateAccountCurrency - update account currency", "���½����ʻ��������", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCurrency(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var currency = (CurrencyType)Enum.Parse(typeof(CurrencyType),req["currency"].ToString());

            Manager mgr = session.GetManager();
            
            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            if (mgr == null || (!mgr.RightAccessAccount(acct)))
            {
                throw new FutsRspError("��Ȩ���������ʻ�");
            }

            this.UpdateAccountCurrency(account, currency);

            session.RspMessage("�����ʻ����»������ͳɹ�");
        }



        /// <summary>
        /// �����˻� �����
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AccountCashOperation", "AccountCashOperation - account cash operation", "�������ʻ�����ȥ��", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            Manager manager = session.GetManager();
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var amount = decimal.Parse(req["amount"].ToString());
            var txnref = req["txnref"].ToString();
            var comment = req["comment"].ToString();
            var equity_type = Util.ParseEnum<QSEnumEquityType>(req["equity_type"].ToString());
            var sync_mainacct = bool.Parse(req["sync_mainacct"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            HandlerMixins.Valid_ObjectNotNull(acct);

            Manager manger = session.GetManager();

            if (!manager.RightAccessAccount(acct))
            {
                throw new FutsRspError("��Ȩ�������ʻ�");
            }

            CashTransaction txn = new CashTransactionImpl();
            txn.Account = account;
            txn.Amount = Math.Abs(amount);
            txn.Comment = comment;
            txn.DateTime = Util.ToTLDateTime();
            txn.EquityType = equity_type;
            txn.Operator = manager.Login;
            txn.Settled = false;
            txn.Settleday = TLCtxHelper.ModuleSettleCentre.Tradingday;
            txn.TxnRef = txnref;
            txn.TxnType = amount > 0 ? QSEnumCashOperation.Deposit : QSEnumCashOperation.WithDraw;


            //ִ�г�������
            this.CashOperation(txn);

            ////���ʻ����
            //if (TLCtxHelper.Version.ProductType == QSEnumProductType.VendorMoniter)
            //{
            //    //ͬ���������������ʻ�
            //    if (sync_mainacct)
            //    {
            //        IBroker broker = BasicTracker.ConnectorMapTracker.GetBrokerForAccount(account);

            //        if (broker == null)
            //        {
            //            throw new FutsRspError("δ�����ʻ�,�޷�ͬ�������������ײ��ʻ�");
            //        }

            //        if (broker is TLBroker)
            //        {
            //            TLBroker b = broker as TLBroker;
            //            if (amount > 0)
            //            {
            //                //���
            //                b.Deposit((double)Math.Abs(amount), "");
            //            }
            //            else
            //            {
            //                //����
            //                b.Withdraw((double)Math.Abs(amount), "");
            //            }
            //            //session.RspMessage("����������ύ,���ѯ���ʻ���Ϣ");
            //        }
            //    }
            //}
            //���������󷵻��ʻ���Ϣ����
            session.NotifyMgr("NotifyAccountFinInfo", acct.GenAccountInfo());
            session.RspMessage("���������ɹ�");
        }






        /// <summary>
        /// @�޸Ľ����ʻ�����
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manger"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountPass", "UpdateAccountPass - update account password", "���½����ʻ���������", QSEnumArgParseType.Json)]
        public void CTE_ChangePassword(ISession session, string json)
        {
            var req = json.DeserializeObject();
            var account = req["account"].ToString();
            var newpass = req["newpass"].ToString();

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountPass(account, newpass);
                session.RspMessage("�޸�����ɹ�");
            }
            else
            {
                throw new FutsRspError("�����ʻ�������");
            }
        }












        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCommissionTemplate", "UpdateAccountCommissionTemplate - update account commission template set", "�����ʻ�������ģ��")]
        public void CTE_UpdateAccountCommissionTemplate(ISession session, string account, int templateid)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("��Ȩ�޸ĸý����ʻ�");
            }

            //����·����
            this.UpdateAccountCommissionTemplate(account, templateid);
            session.RspMessage("�����ʻ�������ģ��ɹ�");
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountMarginTemplate", "UpdateAccountMarginTemplate - update account margin template set", "�����ʻ���֤��ģ��")]
        public void CTE_UpdateAccountMarginTemplate(ISession session, string account, int templateid)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("��Ȩ�޸ĸý����ʻ�");
            }

            //����·����
            this.UpdateAccountMarginTemplate(account, templateid);
            session.RspMessage("�����ʻ���֤��ģ��ɹ�");
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountExStrategyTemplate", "UpdateAccountExStrategyTemplate - update account exstrategy template set", "�����ʻ����ײ���ģ��")]
        public void CTE_UpdateAccountExStrategyTemplate(ISession session, string account, int templateid)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("��Ȩ�޸ĸý����ʻ�");
            }

            //����·����
            this.UpdateAccountExStrategyTemplate(account, templateid);
            session.RspMessage("�����ʻ����ײ���ģ��ɹ�");
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountRouterGroup", "UpdateAccountRouterGroup - update account router group", "�����ʻ�·������Ϣ")]
        public void CTE_UpdateAccountRouterGroup(ISession session, string account, int gid)
        {
            Manager manager = session.GetManager();
            if (!manager.IsInRoot())
            {
                throw new FutsRspError("��Ȩ�޸��ʻ�·��������");
            }

            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            if (!manager.RightAccessAccount(acc))
            {
                throw new FutsRspError("��Ȩ�޸ĸý����ʻ�");
            }

            RouterGroup rg = manager.Domain.GetRouterGroup(gid);
            if (rg == null)
            {
                throw new FutsRspError("ָ��·���鲻����");
            }

            //����·����
            this.UpdateRouterGroup(account, rg);
            session.RspMessage("�����ʻ�·����ɹ�");
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountFinInfo", "QryAccountFinInfo - query account", "��ѯ�ʻ���Ϣ")]
        public void CTE_QryAccountFinInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (manager.RightAccessAccount(acc))
            {
                session.ReplyMgr(acc.GenAccountInfo());
            }
            else
            {
                throw new FutsRspError("��Ȩ�鿴���ʻ���Ϣ");
            }
        }

        /// <summary>
        /// ��ѯ��������Ա��Ϣ
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountLoginInfo", "QryAccountLoginInfo - query account logininfo", "�鿴�����ʻ�����")]
        public void CTE_QryAccountLoginInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();

            
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }
            if (manager.RightAccessAccount(acc))
            {
                Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
                logininfo.LoginID = account;
                logininfo.Pass = ORM.MAccount.GetAccountPass(account);
                session.ReplyMgr(logininfo);
            }
            else
            {
                throw new FutsRspError("��Ȩ�鿴�ʻ�");
            }

        }


        #region ��ʷ��¼��ѯ
        /// <summary>
        /// ��ѯ�����ʻ��ĳ�����¼
        /// </summary>
        /// <param name="session"></param>
        /// <param name="account"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountCashTxn", "QueryAccountCashTxn -query account cashtrans", "��ѯ�����ʻ�������¼", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountCashTrans(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                var data = json.DeserializeObject();
                string account = data["account"].ToString();
                long start = long.Parse(data["start"].ToString());
                long end = long.Parse(data["end"].ToString());

                CashTransactionImpl[] trans = ORM.MCashTransaction.SelectHistCashTransactions(account, start, end).ToArray();
                session.ReplyMgr(trans);
            }
        }

        /// <summary>
        /// ��ѯ�����˻����㵥
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountSettlement", "QueryAccountSettlement -query account settlement", "��ѯ�����ʻ����㵥", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountSettlement(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                var data = json.DeserializeObject();
                string account = data["account"].ToString();
                int tradingday = int.Parse(data["tradingday"].ToString());

                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                AccountSettlement settlement = ORM.MSettlement.SelectSettlement(account, tradingday);
                if (settlement != null)
                {
                    List<string> settlelist = SettlementFactory.GenSettlementFile(settlement, acc);
                    for (int i = 0; i < settlelist.Count; i++)
                    {
                        session.ReplyMgr(settlelist[i].Replace('|', '*'), i == settlelist.Count - 1);
                    }
                }
            }
        }

        /// <summary>
        /// ��ѯ�����˻�ί�м�¼
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountOrder", "QueryAccountOrder -query account order", "��ѯ�����ʻ�ί��", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountOrder(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                var data = json.DeserializeObject();
                string account = data["account"].ToString();
                int start = int.Parse(data["start"].ToString());
                int end = int.Parse(data["end"].ToString());

                IList<Order> orders = ORM.MTradingInfo.SelectOrders(account, start, end);

                int totalnum = orders.Count;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        session.ReplyMgr(OrderImpl.Serialize(orders[i]), i == totalnum - 1);
                    }
                }
                else
                {
                    session.ReplyMgr("");
                }
            }
        }
        /// <summary>
        /// ��ѯ�����˻��ɽ���¼
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountTrade", "QueryAccountTrade -query account trade", "��ѯ�����ʻ��ɽ�", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountTrade(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                var data = json.DeserializeObject();
                string account = data["account"].ToString();
                int start = int.Parse(data["start"].ToString());
                int end = int.Parse(data["end"].ToString());

                IList<Trade> trades = ORM.MTradingInfo.SelectTrades(account, start, end);

                int totalnum = trades.Count;
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        session.ReplyMgr(TradeImpl.Serialize(trades[i]), i == totalnum - 1);
                    }
                }
                else
                {
                    session.ReplyMgr("");
                }
            }
        }

        /// <summary>
        /// ��ѯ�����˻�����ֲ�
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QueryAccountPosition", "QueryAccountPosition -query account position", "��ѯ�����ʻ�����ֲ�", QSEnumArgParseType.Json)]
        public void CTE_QueryAccountPosition(ISession session, string json)
        {
            Manager manger = session.GetManager();
            if (manger != null)
            {
                var data = json.DeserializeObject();
                string account = data["account"].ToString();
                int tradingday = int.Parse(data["tradingday"].ToString());

                List<PositionDetail> positions = ORM.MSettlement.SelectAccountPositionDetails(account, tradingday).ToList();
                int totalnum = positions.Count();
                if (totalnum > 0)
                {
                    for (int i = 0; i < totalnum; i++)
                    {
                        session.ReplyMgr(PositionDetailImpl.Serialize(positions[i]), i == totalnum - 1);
                    }
                }
                else
                {
                    session.ReplyMgr("");
                }
            }
        }
        #endregion
    }
}
