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
        /// @������ӽ����ʻ�
        /// ����˲����������·�ʽ����
        /// 1.Ȩ�޳�����
        /// 2.ִ�в���ʱ�ڲ�ͨ��FutsRspErro�׳��쳣�ķ�ʽ ���ͨ�������쳣�����쳣��Ϣ�ر����ͻ���
        /// 
        /// ����ʻ��Ĳ������ջᴥ�������ʺŲ����������ʺ��¼��Ὣ�ʻ�֪ͨ��������Ȩ�޲鿴�Ĺ����
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddAccount", "AddAccount - add account", "��ӽ����ʻ�", QSEnumArgParseType.Json)]
        public void CTE_AddAccount(ISession session, string json)
        {
            logger.Info(string.Format("����Ա:{0} ������ӽ����ʺ�:{1}", session.AuthorizedID, json));
            
            Manager manager = session.GetManager();
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var category = Util.ParseEnum<QSEnumAccountCategory>(req["category"].ToString());
            var password = req["password"].ToString();
            var routergroup_id = int.Parse(req["routergroup_id"].ToString());
            var user_id = int.Parse(req["user_id"].ToString());
            var manager_id = int.Parse(req["manager_id"].ToString());

            //���ʻ���Ŀ���
            if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("�ʻ���Ŀ�ﵽ����:" + manager.Domain.AccLimit.ToString());
            }

            //�������RootȨ�޵�Manager��Ҫ����ִ��Ȩ�޼��
            if (!manager.IsInRoot())
            {
                //�������Ϊ����������ʻ�,��������Ҫ�жϵ�ǰManager�������Ƿ�ӵ�����������Ȩ��
                if (manager.BaseMgrID != manager_id)
                {
                    if (!manager.IsParentOf(manager_id))
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

            Manager targetmgr = BasicTracker.ManagerTracker[manager_id];
            if (targetmgr == null)
            {
                //���ָ���Ĺ����򲻴��ڣ���Ĭ��Ϊ��ǰ������manager����
                targetmgr = manager.BaseManager;
            }

            //ָ����manager��͵�ǰ������һ�� ���жϵ�ǰ�������Ƿ���Ȩ��ָ�������ڿ���
            if (targetmgr.BaseMgrID != manager.BaseMgrID)
            {
                if (!manager.RightAccessManager(targetmgr))
                {
                    throw new FutsRspError("��ȨΪ�ù���Ա����ʻ�");
                }
            }

            AccountCreation create = new AccountCreation();
            create.Account = account;
            create.Category = category;
            create.Password = password;
            //create.RouteGroup = BasicTracker.RouterGroupTracker[routergroup_id];
            create.RouterType = QSEnumOrderTransferType.LIVE;
            create.UserID = user_id;
            //create.Domain = manager.Domain;
            create.BaseManagerID = targetmgr.BaseMgrID;


            //ִ�в��� �������쳣 �����쳣���������ر�
            this.AddAccount(ref create);//�������ʻ����뵽����
            session.OperationSuccess("�����ʻ�:" + create.Account + "�ɹ�");
        }


        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddFinServiceAccount", "AddFinServiceAccount - add finservice account", "������ʿͻ�", QSEnumArgParseType.Json)]
        public void CTE_AddFinServiceAccount(ISession session, string json)
        {
            logger.Info(string.Format("����Ա:{0} ����������ʿͻ��ʺ�:{1}", session.AuthorizedID, json));

            Manager manager = session.GetManager();
            var profile = Mixins.Json.JsonMapper.ToObject<AccountProfile>(json);
            var account = profile.Account;
            QSEnumAccountCategory category = QSEnumAccountCategory.MONITERACCOUNT;
            var password = string.Empty;
            var routergroup_id = 0;
            var user_id = 0;
            var manager_id = manager.BaseManager.ID;

            //���ʻ���Ŀ���
            if (manager.Domain.GetAccounts().Count() >= manager.Domain.AccLimit)
            {
                throw new FutsRspError("�ʻ���Ŀ�ﵽ����:" + manager.Domain.AccLimit.ToString());
            }

            //�������RootȨ�޵�Manager��Ҫ����ִ��Ȩ�޼��
            if (!manager.IsInRoot())
            {
                //�������Ϊ����������ʻ�,��������Ҫ�жϵ�ǰManager�������Ƿ�ӵ�����������Ȩ��
                if (manager.BaseMgrID != manager_id)
                {
                    if (!manager.IsParentOf(manager_id))
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


            AccountCreation create = new AccountCreation();
            create.Account = account;
            create.Category = category;
            create.Password = password;
            //create.RouteGroup = BasicTracker.RouterGroupTracker[routergroup_id];
            create.RouterType = QSEnumOrderTransferType.SIM;
            create.UserID = user_id;
            //create.Domain = manager.Domain;
            //create.BaseManager = manager.BaseManager;


            //ִ�в��� �������쳣 �����쳣���������ر�
            this.AddAccount(ref create);//�������ʻ����뵽����

            profile.Account = create.Account;//�����ӵĽ����ʻ�
            //�����µ�profile
            BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);

            session.OperationSuccess("�������ʿͻ�:" + create.Account + "�ɹ�");

        }


        /// <summary>
        /// ͳһʹ��AccountCreation���󴴽������ʻ�
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AddAccountFacde", "AddAccountFacde - add  account", "��ӽ����ʺ�", QSEnumArgParseType.Json)]
        public void CTE_AddAccountFacde(ISession session, string json)
        {
            logger.Info(string.Format("����Ա:{0} ��������ʺ�:{1}", session.AuthorizedID, json));

            Manager manager = session.GetManager();
            var creation = Mixins.Json.JsonMapper.ToObject<AccountCreation>(json);
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

            session.OperationSuccess("���������ʺ�:" + creation.Account + "�ɹ�");

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

            var profile = Mixins.Json.JsonMapper.ToObject<AccountProfile>(json);
            IAccount account = TLCtxHelper.ModuleAccountManager[profile.Account];

            if (account != null)
            {
                BasicTracker.AccountProfileTracker.UpdateAccountProfile(profile);
            }
            //���������ʻ��䶯�¼�
            TLCtxHelper.EventAccount.FireAccountChangeEent(account);

            session.OperationSuccess("���¸�����Ϣ�ɹ�");

        }

        /// <summary>
        /// ����ɾ�������ʻ�
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "DelAccount", "DelAccount - del account", "ɾ�������ʻ�", QSEnumArgParseType.Json)]
        public void CTE_DelAccount(ISession session, string json)
        {
            logger.Info(string.Format("����Ա:{0} ����ɾ���ʻ�:{1}", session.AuthorizedID, json));

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
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

            session.OperationSuccess("�����ʻ�:" + account + " ɾ���ɹ�");
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
            logger.Info(string.Format("����Ա:{0} ��������ʻ����:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
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
            logger.Info(string.Format("����Ա:{0} ������Ȩ�����:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
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
            logger.Info(string.Format("����Ա:{0} ����������ڽ���:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
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
            logger.Info(string.Format("����Ա:{0} �������·���౻:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountRouterTransferType(account, routetype);
            }
        }

        //[ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountInvestor", "UpdateAccountInvestor - update account investor info", "�޸Ľ����ʺ�Ͷ������Ϣ", QSEnumArgParseType.Json)]
        //public void CTE_UpdateAccountInvestor(ISession session, string json)
        //{
        //    logger.Info(string.Format("����Ա:{0} �����޸�Ͷ������Ϣ:{1}", session.AuthorizedID, json));
        //    var req = Mixins.Json.JsonMapper.ToObject(json);
        //    var account = req["account"].ToString();
        //    var name = req["name"].ToString();
        //    var broker = req["broker"].ToString();
        //    var bank_id = int.Parse(req["bank_id"].ToString());
        //    var bank_ac = req["bank_ac"].ToString();

        //    IAccount acct = TLCtxHelper.ModuleAccountManager[account];
        //    if (acct != null)
        //    {
        //        this.UpdateInvestorInfo(account, name, broker, bank_id, bank_ac);
        //    }
        //}

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountCurrency", "UpdateAccountCurrency - update account currency", "���½����ʻ��������", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountCurrency(ISession session, string json)
        {
            logger.Info(string.Format("����Ա:{0} �����޸�Ͷ������Ϣ:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
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

            session.OperationSuccess("�����ʻ����»������ͳɹ�");
        }



        /// <summary>
        /// �����˻� �����
        /// </summary>
        /// <param name="session"></param>
        /// <param name="json"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AccountCashOperation", "AccountCashOperation - account cash operation", "�������ʻ�����ȥ��", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            logger.Info(string.Format("����Ա:{0} �����������:{1}", session.AuthorizedID, json));
            Manager manager = session.GetManager();

            var req = Mixins.Json.JsonMapper.ToObject(json);
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
            //            //session.OperationSuccess("����������ύ,���ѯ���ʻ���Ϣ");
            //        }
            //    }
            //}
            //���������󷵻��ʻ���Ϣ����
            session.NotifyMgr("NotifyAccountFinInfo", acct.GenAccountInfo());
            session.OperationSuccess("���������ɹ�");
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
            logger.Info(string.Format("����Ա:{0} �����޸Ľ�������:{1}", session.AuthorizedID, json));
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var newpass = req["newpass"].ToString();

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountPass(account, newpass);
                session.OperationSuccess("�޸�����ɹ�");
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
            session.OperationSuccess("�����ʻ�������ģ��ɹ�");
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
            session.OperationSuccess("�����ʻ���֤��ģ��ɹ�");
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
            session.OperationSuccess("�����ʻ����ײ���ģ��ɹ�");
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
            session.OperationSuccess("�����ʻ�·����ɹ�");
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

    }
}
