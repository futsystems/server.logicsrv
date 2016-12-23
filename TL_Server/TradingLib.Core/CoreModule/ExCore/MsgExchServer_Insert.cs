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
        /// 手工插入委托
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void ManualInsertOrder(Order o)
        {
            AssignOrderID(ref o);
            long ordid = o.id;
            //交易时间检查
            int settleday = 0;
            QSEnumActionCheckResult result = o.oSymbol.SecurityFamily.CheckPlaceOrder(out settleday);
            if (result != QSEnumActionCheckResult.Allowed)
            {
                o.Status = QSEnumOrderStatus.Reject;
                return;
            }

            o.SettleDay = settleday;

            //设定委托相关编号
            o.OrderSysID = o.OrderSeq.ToString();
            o.BrokerRemoteOrderID = o.OrderSysID;
            OnOrderEvent(o);
            logger.Info(string.Format("Insert Order:{0}", o.GetOrderInfo()));
        }

        /// <summary>
        /// 手工插入成交
        /// </summary>
        /// <param name="t"></param>
        public void ManualInsertTrade(Trade t)
        {
            int settleday = 0;
            Symbol symbol = BasicTracker.DomainTracker.SuperDomain.GetSymbol(t.Exchange,t.Symbol);
            if (symbol == null) return;

            QSEnumActionCheckResult result = symbol.SecurityFamily.CheckPlaceOrder(out settleday);
            if (result != QSEnumActionCheckResult.Allowed)
            {
                return;
            }
            t.SettleDay = settleday;

            OnFillEvent(t);
            logger.Info(string.Format("Insert Trade:{0}", t.GetTradeInfo()));
        }
    }
}
