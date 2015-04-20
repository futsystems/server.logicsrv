using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MsgExchServer
    {

        /// <summary>
        /// 出入金通知
        /// </summary>
        /// <param name="account"></param>
        /// <param name="type"></param>
        /// <param name="amount"></param>
        void NotifyCashOperation(string account, QSEnumCashOperation type, decimal amount)
        {
            Util.Debug(string.Format("帐户:{0}发生:{1} {2}",account,type,amount),QSEnumDebugLevel.INFO);
            CashOperationNotify notify = ResponseTemplate<CashOperationNotify>.SrvSendNotifyResponse(account);
            notify.Amount = amount;
            notify.OperationType = type;

            CachePacket(notify);

            TLCtxHelper.EventAccount.FireAccountTradingNoticeEvent(account, "出入金内容");
        }

        void NotifyTradingNotice(string account, string content)
        {
            Util.Debug(string.Format("发送通知到帐户:{0} 内容:{1}", account,content), QSEnumDebugLevel.INFO);
            TradingNoticeNotify notify = ResponseTemplate<TradingNoticeNotify>.SrvSendNotifyResponse(account);
            notify.SendTime = Util.ToTLTime();
            notify.NoticeContent = content;

            CachePacket(notify);
        }
    }
}
