﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// OnRouterEvent
    /// 这部分是用于响应路由侧回报过来的事件
    /// 1.调用清算中心进行记录
    /// 2.对外部暴露这些交易事件
    /// </summary>
    public partial class ExCore
    {
       
        public virtual void OnTickEvent(Tick k)
        {
            //2.清算中心响应Tick事件
            TLCtxHelper.ModuleClearCentre.GotTick(k);
            //2.对外触发Tick事件 用于被其他组件监听
            TLCtxHelper.EventIndicator.FireTickEvent(k);
            //对外通知
            this.NotifyTick(k);
        }

        public void OnOrderActionErrorEvent(OrderAction action, RspInfo info)
        {
            //对外通知
            this.NotifyOrderActionError(action, info);
        }

        /// <summary>
        /// 响应路由中心委托错误回报
        /// </summary>
        /// <param name="notify"></param>
        public void OnOrderErrorEvent(Order order, RspInfo info)
        {
            this.OnOrderErrorEvent(order, info, true);
        }

        public void OnOrderErrorEvent(Order order, RspInfo info, bool needlog = true)
        {
            //清算中心响应委托错误回报
            //如果需要记录该委托错误 则需要调用清算中心的goterrororder进行处理
            if (needlog)
            {
                TLCtxHelper.ModuleClearCentre.GotOrderError(order, info);
            }
            //对外触发委托错误事件
            TLCtxHelper.EventIndicator.FireOrderErrorEvent(order,info);
            //对外通知
            this.NotifyOrderError(order, info);
        }

        protected virtual void AmendOrderComment(ref Order o)
        {
            //更新委托状态
            switch (o.Status)
            {
                case QSEnumOrderStatus.Filled:
                    o.Comment = string.IsNullOrEmpty(o.Comment) ? commentFilled : o.Comment;
                    break;
                case QSEnumOrderStatus.PartFilled:
                    o.Comment = string.IsNullOrEmpty(o.Comment) ? commentPartFilled : o.Comment;
                    break;
                case QSEnumOrderStatus.Canceled:
                    o.Comment = string.IsNullOrEmpty(o.Comment) ? commentCanceled : o.Comment;
                    break;
                case QSEnumOrderStatus.Placed:
                    o.Comment = string.IsNullOrEmpty(o.Comment) ? commentPlaced : o.Comment;
                    break;
                case QSEnumOrderStatus.Submited:
                    o.Comment = string.IsNullOrEmpty(o.Comment) ? commentSubmited : o.Comment;
                    break;
                case QSEnumOrderStatus.Opened:
                    o.Comment = string.IsNullOrEmpty(o.Comment) ? commentOpened : o.Comment;
                    break;
                default:
                    break;
            }
        }
        public void OnOrderEvent(Order o)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];

            AmendOrderComment(ref o);
            
            //清算中心响应委托回报
            TLCtxHelper.ModuleClearCentre.GotOrder(o);

            //触发交易账户委托事件
            account.FireOrderEvent(o);

            //?如何完善cancel机制
            if (o.Status == QSEnumOrderStatus.Canceled)
            {
                //清算中心响应取消回报
                OnCancelEvent(o.id);
            }
            //对外触发委托事件
            TLCtxHelper.EventIndicator.FireOrderEvent(o);
            //对外通知
            this.NotifyOrder(o);
        }


        /// <summary>
        /// 响应成交路由返回的成交回报
        /// </summary>
        /// <param name="t"></param>
        public void OnFillEvent(Trade t)
        {
            //设定系统内成交编号
            AssignTradeID(ref t);

            IAccount account = TLCtxHelper.ModuleAccountManager[t.Account];

            bool accept = false;
            //清算中心响应成交回报
            TLCtxHelper.ModuleClearCentre.GotFill(t,out accept);//注这里的成交没有结算手续费,成交部分我们需要在结算中心结算结算完手续费后再向客户端发送

            //如果清算中心无法处理该成交 则直接返回 不用向系统触发成交事件或通知客户端
            if (!accept) return;

            //对外通知成交 Indicator总线
            TLCtxHelper.EventIndicator.FireFillEvent(t);

            //触发交易账户成交事件
            account.FireFillEvent(t);

            //对外通知
            this.NotifyFill(t);

            //对外通知持仓更新
            if (account != null)
            {
                //有新的成交数据后,系统自动发送对应的持仓信息
                Position pos = account.GetPosition(t.Symbol, t.PositionSide);
                if (pos != null)
                {
                    //对外通知
                    NotifyPositionUpdate(pos);
                }
            }
        }

        public void OnCancelEvent(long oid)
        {
            //清算中心响应取消回报
            TLCtxHelper.ModuleClearCentre.GotCancel(oid);
            //对外触发取消事件
            TLCtxHelper.EventIndicator.FireCancelEvent(oid);
            //对外通知
            this.NotifyCancel(oid);
        }
    }
}