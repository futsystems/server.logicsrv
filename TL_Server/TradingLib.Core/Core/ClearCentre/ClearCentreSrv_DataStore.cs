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
        /// 结算所有交易账户
        /// 结算分析
        /// 1.单日lastequity + realizedpl + unrealizedpl - commission + cashin - cashout = now equity 数据库检验通过
        /// 2.当日结算完毕后的nowequity即为账户表中的lastequity 数据库检验通过
        /// 3.产生错误就是在某些结算记录中上日权益不等于该账户的nowequity
        /// 4.账户结算时需要检查 账户的上日权益是否是数据库记录的上日权益
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

            //更新最近结算日
            debug(string.Format("Update lastsettleday as:{0}", TLCtxHelper.ModuleSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            ORM.MSettlement.UpdateSettleday(TLCtxHelper.ModuleSettleCentre.CurrentTradingday);
            debug("Settlement Done", QSEnumDebugLevel.INFO);
        }

    }
}
