using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class MgrExchServer
    {
        void SrvOnOrderInsert(OrderInsertRequest request, ISession session, Manager manager)
        {
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.OrderSource = QSEnumOrderSource.QSMONITER;
            order.TotalSize = order.Size;
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();
            TLCtxHelper.ModuleExCore.SendOrderInternal(order);
        }

        void SrvOnOrderActionInsert(OrderActionRequest request, ISession session, Manager manager)
        {
            if (request.OrderAction.ActionFlag == QSEnumOrderActionFlag.Delete)
            {
                if (request.OrderAction.OrderID != 0)
                {
                    TLCtxHelper.ModuleExCore.CancelOrder(request.OrderAction.OrderID);
                }
            }
        }
    }
}
