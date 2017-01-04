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
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        void SendOrder(Order o)
        {
            try
            {
                o.Date = Util.ToTLDate(DateTime.Now);
                o.Time = Util.ToTLTime(DateTime.Now);

                TLCtxHelper.ModuleExCore.SendOrderInternal(o);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Send Order:{0} Error:{1}", o.GetOrderInfo(), ex.ToString()));
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
                TLCtxHelper.ModuleExCore.CancelOrder(number);
            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Cancel Order:{0} Error:{1}", number, ex.ToString()));
            }
        }

    }
}
