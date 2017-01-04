﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.Common;

using System.Diagnostics;//记得加入此引用
using System.Collections.Concurrent;
using TradingLib.API;
using System.Reflection;
using System.Threading;

namespace TradingLib.Core
{


    //服务端风险控制模块,根据每个账户的设定，实时的检查Order是否符合审查要求予以确认或者决绝
    public partial class RiskCentre
    {

        /// <summary>
        /// 获得取消
        /// 取消事务是在处理队列中进行异步处理
        /// 而强平事务的完成是在positionround回报中同步处理
        /// </summary>
        /// <param name="oid"></param>
        void GotOrder(Order o)
        {
            if (o.Status == QSEnumOrderStatus.Canceled)
            {
                foreach (RiskTaskSet ps in riskTasklist)
                {
                    if (ps.PendingOrders.Contains(o.id))
                    {
                        ps.PendingOrders.Remove(o.id);
                    }

                    if (ps.FlatOrderIDList.Contains(o.id))
                    {
                        ps.FlatOrderIDList.Remove(o.id);
                    }
                }
            }
        }

        /// <summary>
        /// 响应交易服务返回过来的ErrorOrder
        /// 比如风控中心强平 发送委托 但是委托被拒绝，则需要对该事件进行响应
        /// 否则超时后 会出现强平系统无法正常撤单的问题。而无法正常撤单则没有撤单回报,导致强平系统一直试图撤单
        /// </summary>
        /// <param name="error"></param>
        void GotOrderError(Order order, RspInfo info)
        {
            logger.Info("Got Orrder Error ID:" + order.id.ToString());
            foreach (RiskTaskSet ps in riskTasklist)
            {
                //如果委托被拒绝 并且委托ID是本地发送过去的ID 则将positionflatset的委托ID置0
                if (ps.FlatOrderIDList.Contains(order.id) && order.Status == QSEnumOrderStatus.Reject)
                    ps.FlatOrderIDList.Remove(order.id);
            }
        }

    }
}