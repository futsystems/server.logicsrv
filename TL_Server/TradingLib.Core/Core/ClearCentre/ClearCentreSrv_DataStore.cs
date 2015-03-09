using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {


        /// <summary>
        /// �������н����˻�
        /// �������
        /// 1.����lastequity + realizedpl + unrealizedpl - commission + cashin - cashout = now equity ���ݿ����ͨ��
        /// 2.���ս�����Ϻ��nowequity��Ϊ�˻����е�lastequity ���ݿ����ͨ��
        /// 3.�������������ĳЩ�����¼������Ȩ�治���ڸ��˻���nowequity
        /// 4.�˻�����ʱ��Ҫ��� �˻�������Ȩ���Ƿ������ݿ��¼������Ȩ��
        /// </summary>
        void SettleAccount()
        {
            debug(string.Format("#####SettleAccount: Start Settele Account,Current Tradingday:{0}", TLCtxHelper.ModuleSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                try
                {
                    ORM.MSettlement.SettleAccount(acc);
                }
                catch (Exception ex)
                {
                    debug(string.Format("SettleError,Account:{0} errors:{1}", acc.ID, ex.ToString()), QSEnumDebugLevel.ERROR);
                }
            }

            //�������������
            debug(string.Format("Update lastsettleday as:{0}", TLCtxHelper.ModuleSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            ORM.MSettlement.UpdateSettleday(TLCtxHelper.ModuleSettleCentre.CurrentTradingday);
            debug("Settlement Done", QSEnumDebugLevel.INFO);
        }

    }
}
