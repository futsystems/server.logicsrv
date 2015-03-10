using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

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
            debug(string.Format("����Ա:{0} ������ӽ����ʺ�:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);

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


            AccountCreation create = new AccountCreation();
            create.Account = account;
            create.Category = category;
            create.Password = password;
            create.RouteGroup = BasicTracker.RouterGroupTracker[routergroup_id];
            create.RouterType = create.Category == QSEnumAccountCategory.SIMULATION ? QSEnumOrderTransferType.SIM : QSEnumOrderTransferType.LIVE;
            create.UserID = user_id;
            create.Domain = manager.Domain;
            create.BaseManager = manager.BaseManager;


            //ִ�в��� �������쳣 �����쳣���������ر�
            this.AddAccount(ref create);//�������ʻ����뵽����
            session.OperationSuccess("�����ʻ�:" + create.Account + "�ɹ�");
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
            debug(string.Format("����Ա:{0} ����ɾ���ʻ�:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
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
            debug(string.Format("����Ա:{0} ��������ʻ����:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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
            debug(string.Format("����Ա:{0} ������Ȩ�����:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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
            debug(string.Format("����Ա:{0} ����������ڽ���:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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
            debug(string.Format("����Ա:{0} �������·���౻:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateAccountRouterTransferType(account, routetype);
            }
        }

        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "UpdateAccountInvestor", "UpdateAccountInvestor - update account investor info", "�޸Ľ����ʺ�Ͷ������Ϣ", QSEnumArgParseType.Json)]
        public void CTE_UpdateAccountInvestor(ISession session, string json)
        {
            debug(string.Format("����Ա:{0} �����޸�Ͷ������Ϣ:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var name = req["name"].ToString();
            var broker = req["broker"].ToString();
            var bank_id = int.Parse(req["bank_id"].ToString());
            var bank_ac = req["bank_ac"].ToString();

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            if (acct != null)
            {
                this.UpdateInvestorInfo(account, name, broker, bank_id, bank_ac);
            }
        }



        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "AccountCashOperation", "AccountCashOperation - account cash operation", "�������ʻ�����ȥ��", QSEnumArgParseType.Json)]
        public void CTE_CashOperation(ISession session, string json)
        {
            debug(string.Format("����Ա:{0} �����������:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
            Manager manager = session.GetManager();

            var req = Mixins.Json.JsonMapper.ToObject(json);
            var account = req["account"].ToString();
            var amount = decimal.Parse(req["amount"].ToString());
            var txnref = req["txnref"].ToString();
            var comment = req["comment"].ToString();

            //var routetype = Util.ParseEnum<QSEnumOrderTransferType>(req["routertrype"].ToString());

            IAccount acct = TLCtxHelper.ModuleAccountManager[account];
            HandlerMixins.Valid_ObjectNotNull(acct);

            Manager manger = session.GetManager();

            if (!manager.RightAccessAccount(acct))
            {
                throw new FutsRspError("��Ȩ�������ʻ�");
            }

            //ִ�г�������
            this.CashOperation(account, amount, txnref, comment);

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
            debug(string.Format("����Ա:{0} �����޸Ľ�������:{1}", session.AuthorizedID, json), QSEnumDebugLevel.INFO);
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


        /// <summary>
        /// ��ѯ��������Ա��Ϣ
        /// </summary>
        /// <param name="session"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageMgr, "QryAccountLoginInfo", "QryAccountLoginInfo - query account logininfo", "�鿴�����ʻ�����")]
        public void CTE_QryAccountLoginInfo(ISession session, string account)
        {
            Manager manager = session.GetManager();
            if (manager.IsRoot())
            {
                IAccount acc = TLCtxHelper.ModuleAccountManager[account];
                if (acc == null)
                {
                    throw new FutsRspError("�����ʻ�������");
                }
                Protocol.LoginInfo logininfo = new Protocol.LoginInfo();
                logininfo.LoginID = account;
                logininfo.Pass = ORM.MAccount.GetAccountPass(account);
                session.ReplyMgr(logininfo);
            }
        }
















        //#endregion

    }
}
