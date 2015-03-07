﻿using System;
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
