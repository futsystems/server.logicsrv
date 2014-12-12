using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.LitJson;
using TradingLib.Mixins.JsonObject;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        IdTracker accchashopid = new IdTracker();
        /// <summary>
        /// �ύ��� �� �������
        /// </summary>
        /// <param name="account"></param>
        /// <param name="amount"></param>
        /// <param name="op"></param>
        /// <returns></returns>
        public bool RequestCashOperation(string account, decimal amount, QSEnumCashOperation op,out string opref,QSEnumCashOPSource source= QSEnumCashOPSource.Unknown,string  recvinfo="")
        {
            opref = string.Empty;
            if (!this.HaveAccount(account)) return false;
            if (amount <= 0) return false;
            JsonWrapperCashOperation request = new JsonWrapperCashOperation();
            request.Account = account;
            request.Amount = amount;
            request.DateTime = Util.ToTLDateTime(DateTime.Now);
            request.Operation = op;
            request.Status = QSEnumCashInOutStatus.PENDING;
            request.Ref = accchashopid.AssignId.ToString();
            request.Source = source;
            if (request.Source == QSEnumCashOPSource.Online)
            {
                request.RecvInfo = "������֧��";
            }
            else
            {
                request.RecvInfo = recvinfo;
            }
            ORM.MCashOpAccount.InsertAccountCashOperation(request);
            //���ⲿ��¶����������Ref
            opref = request.Ref;
            return true;
        }

        /// <summary>
        /// ȷ��ĳ������¼ ����
        /// </summary>
        /// <param name="opref"></param>
        /// <returns></returns>
        public bool ConfirmCashOperation(string opref)
        {

            JsonWrapperCashOperation op = ORM.MCashOpAccount.GetAccountCashOperation(opref);
            IAccount account = this[op.Account];
            if (account == null)
            {
                throw new FutsRspError("�����ʻ�������");
            }

            if(op ==null)
            {
                throw new FutsRspError("��������󲻴���");
            }

            if(op.Status != QSEnumCashInOutStatus.PENDING)
            {
                throw new FutsRspError("���������״̬�쳣");
            }

            //����ִ�н����
            if (op.Operation == QSEnumCashOperation.WithDraw)
            {
                if (account.NowEquity < op.Amount)
                {
                    throw new FutsRspError("�����ȴ����ʻ�Ȩ��");
                }
            }

            //ִ��ʱ���� 
            if (TLCtxHelper.Ctx.SettleCentre.IsInSettle)
            {
                throw new FutsRspError("ϵͳ���ڽ���,��ֹ��������");
            }

            //ִ�����ݿ����
            bool ret = ORM.MCashOpAccount.ConfirmAccountCashOperation(op);
            
            //������ݿ�������� ��ͬ���ڴ�����
            if (ret)
            {
                if (op.Operation == QSEnumCashOperation.Deposit)
                {
                    account.Deposit(op.Amount);
                }
                else
                {
                    account.Withdraw(op.Amount);
                }
            }
            debug("Account:" + op.Account + " ȷ�����:" + op.Amount.ToString() + " �ɹ�!");
            return true;
        }
    }
}
