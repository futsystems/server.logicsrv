using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using FutSystems.GUI;

namespace FutsMoniter.Controls
{
    /// <summary>
    /// 向服务端提交委托或提交委托操作
    /// </summary>
    public partial class ctAccountMontier
    {
        void CancelOrder(long oid)
        {
            OrderAction actoin = new OrderActionImpl();
            actoin.Account = AccountSetlected.Account;
            actoin.ActionFlag = QSEnumOrderActionFlag.Delete;
            actoin.OrderID = oid;
            SendOrderAction(actoin);
        }
        void SendOrderAction(OrderAction action)
        {
            Globals.TLClient.ReqOrderAction(action);
        }

        void SendOrder(Order o)
        {
            o.Account = AccountSetlected.Account;
            Globals.TLClient.ReqOrderInsert(o);
        }
    }
}
