using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class OrderHelper
    {
        /// <summary>
        /// 返回委托状态
        /// </summary>
        /// <param name="ot"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        public static QSEnumOrderStatus OrderStatus(OrderTracker ot, long id)
        {
            //QSEnumPopMessageType tt = QSEnumPopMessageType.Error;

            //long id = o.id;
            if (ot.isCanceled(id)) return QSEnumOrderStatus.Canceled;//取消
            if (ot.isCompleted(id)) return QSEnumOrderStatus.Filled;//全部成交
            if (ot.isPending(id))
            {
                if (ot.Filled(id) == 0)
                    return QSEnumOrderStatus.Opened;
                else
                    return QSEnumOrderStatus.PartFilled;
            }
            else
                return QSEnumOrderStatus.Unknown;
        }

        public static string OrderTypeString(Order o)
        { 
            string s =string.Empty;
            if (o.isMarket)
                s = s + "市价";
            else if (o.isLimit)
                s = s + "限价:" + string.Format("{0:F2}", o.price);
            else if(o.isStop)
                s = s + "追价:" + string.Format("{0:F2}", o.stopp);
            return s;
        }
    }
}
