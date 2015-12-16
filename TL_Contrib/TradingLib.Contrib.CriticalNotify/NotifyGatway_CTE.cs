using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Contrib.NotifyCentre
{
    public partial class NotifyGatway
    {
        /// <summary>
        /// 暴露成被webmessage调用的方式，可以在其他模块通过 TLCtxHelper.Ctx.MessageWebHandler("NotifyGatway", "UpdateNotifyEmail", account + "," + email); 进行调用
        /// </summary>
        /// <param name="account"></param>
        /// <param name="email"></param>
        [ContribCommandAttr(QSEnumCommandSource.MessageWeb, "UpdateNotifyEmail", "UpdateNotifyEmail - 更新交易帐户的通知邮件地址", "更新交易帐户的通知邮件地址")]
        public void CTE_QryFinService(string account,string email)
        {
            logger.Info("更新交易帐户:" + account + "的通知邮件地址为:" + email);
            IAccount acc = TLCtxHelper.ModuleAccountManager[account];
            if (acc != null)
            {
                ContactTracker.UpdateEmail(account, email);
                EmailDrop drop = new AccountContactChangedDrop(account);
                foreach (IEmail e in drop.GenerateEmails())
                {
                    SendEmail(e);
                }
            }          
        }
    }
}
