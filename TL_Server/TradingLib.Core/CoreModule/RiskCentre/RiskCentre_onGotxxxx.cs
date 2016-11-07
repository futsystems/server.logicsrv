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



        #region
        /// <summary>
        /// 行情驱动止盈止损监控器进行工作,满足条件对外触发平仓指令
        /// </summary>
        /// <param name="k"></param>
        void GotTick(Tick k)
        {
            //_posoffsetracker.GotTick(k);
            _haltstatetracker.GotTick(k);
        }

        
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
                foreach (RiskTaskSet ps in posflatlist)
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
            foreach (RiskTaskSet ps in posflatlist)
            {
                //如果委托被拒绝 并且委托ID是本地发送过去的ID 则将positionflatset的委托ID置0
                if (ps.FlatOrderIDList.Contains(order.id) && order.Status == QSEnumOrderStatus.Reject)
                    ps.FlatOrderIDList.Remove(order.id);
            }
        }

        /// <summary>
        /// 当有持仓平调后 遍历当地平仓事务列表，如果在列表中 则直接删除该平仓事务,表明该平仓事务已经完成
        /// 注意平仓事务列表为线程安全的list可以同时被多个线程操作
        /// 如果统一在processpostionflat中检查持仓情况会出现以下问题：如果持仓平调 但是在平调后立马再次开仓，此时PostioinFlat并没有从队列中删除，当再次扫描到该持仓时 系统会认为该持仓没有被及时平调,从而尝试撤单并重新强平，最后单子又无法撤单成功
        /// 
        /// 只有在FlatPosition类型的任务中真正触发委托
        /// 平仓任务出队在持仓回合中进行
        /// 取消任务队列在处理程序中进行
        /// 
        /// </summary>
        /// <param name="pr"></param>
        /// <param name="pos"></param>
        void GotPostionRoundClosed(PositionRound pr, Position pos)
        {
            //string key = pos.GetPositionKey();

            //RiskTaskSet[] list = posflatlist.Where(task => task.TaskType == QSEnumRiskTaskType.FlatPosition && task.Position.GetPositionKey().Equals(key)).ToArray();

            //foreach (RiskTaskSet tmp in list)
            //{
            //    logger.Info("Position:" + tmp.Position.GetPositionKey() + " 已经平掉,从队列中移除");
            //    posflatlist.Remove(tmp);
            //    //通过事件中继触发事件
            //    TLCtxHelper.EventSystem.FirePositionFlatEvent(this, new PositionFlatEventArgs(tmp.Position));
            //    //if (PositionFlatEvent != null)
            //    //{
            //    //    PositionFlatEvent(this, new PositionFlatEventArgs(tmp.Position));
            //    //}
            //}
        }

        #endregion

    }
}