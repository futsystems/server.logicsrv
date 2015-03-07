using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class RiskCentre
    {
        /// <summary>
        /// 分配委托ID
        /// </summary>
        public event AssignOrderIDDel AssignOrderIDEvent;

        /// <summary>
        /// 对外发送委托
        /// </summary>
        //public event OrderDelegate newSendOrderRequest;

        /// <summary>
        /// 对外取消委托
        /// </summary>
        //public event LongDelegate newOrderCancelRequest;

        /// <summary>
        /// 用于提前分配委托ID 便于跟踪委托
        /// </summary>
        /// <param name="o"></param>
        void AssignOrderID(ref Order o)
        {
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);
        }

        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o)
        {
            try
            {
                o.Date = Util.ToTLDate(DateTime.Now);
                o.Time = Util.ToTLTime(DateTime.Now);

                TLCtxHelper.CmdUtils.SendOrderInternal(o);
                //if (newSendOrderRequest != null)
                //    newSendOrderRequest(o);
            }
            catch (Exception ex)
            {
                debug("发送委托异常:" + o.ToString() + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="number"></param>
        void CancelOrder(long number)
        {
            try
            {
                TLCtxHelper.CmdUtils.CancelOrder(number);

                //if (newOrderCancelRequest != null)
                //    newOrderCancelRequest(number);
            }
            catch (Exception ex)
            {
                debug("取消委托异常:" + number.ToString() + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

    }
}
