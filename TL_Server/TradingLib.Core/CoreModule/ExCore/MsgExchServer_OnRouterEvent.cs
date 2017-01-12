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
        /// 响应路由事件中的实时行情数据
        /// </summary>
        /// <param name="k"></param>
        void OnTickEvent(Tick k)
        {
            TLCtxHelper.ModuleClearCentre.GotTick(k);
            TLCtxHelper.EventIndicator.FireTickEvent(k);
            //对外通知
            this.NotifyTick(k);
        }

        void OnOrderActionErrorEvent(OrderAction action, RspInfo info)
        {
            this.NotifyOrderActionError(action, info);
        }

        /// <summary>
        /// 响应路由中心委托错误回报
        /// </summary>
        /// <param name="notify"></param>
        void OnOrderErrorEvent(Order order, RspInfo info)
        {
            this.OnOrderErrorEvent(order, info, true);
        }

        /// <summary>
        /// needlog
        /// 清算中心将获得委托数据更新到数据库
        /// needlog为false 则不需要将委托数据写入数据库
        /// </summary>
        /// <param name="order"></param>
        /// <param name="info"></param>
        /// <param name="needlog"></param>
        public void OnOrderErrorEvent(Order order, RspInfo info, bool needlog = true)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[order.Account];
            if (account == null)
            {
                logger.Warn(string.Format("Account of Order:{0} do not exist", order.GetOrderInfo()));
            }

            //清算中心响应委托错误回报
            //如果需要记录该委托错误 则需要调用清算中心的GotOrderError进行处理
            if (needlog)
            {
                LogOrder(order);
                TLCtxHelper.ModuleClearCentre.GotOrderError(account,order, info);
            }

            TLCtxHelper.EventIndicator.FireOrderErrorEvent(order, info);

            this.NotifyOrderError(order, info);
        }

        void LogOrder(Order o)
        {
            if (!TLCtxHelper.ModuleClearCentre.IsTracked(o))
            {
                logger.Info("New Order:" + o.GetOrderInfo());
                TLCtxHelper.ModuleDataRepository.NewOrder(o);
            }
            else
            {
                logger.Info("Update Order:" + o.GetOrderStatus());
                TLCtxHelper.ModuleDataRepository.UpdateOrder(o);
            }
        }

        void LogOrderAction(OrderAction action)
        {
            logger.Info("New Cancel:" + action.OrderID);
            TLCtxHelper.ModuleDataRepository.NewOrderAction(action);
        }

        void LogTrade(Trade f)
        {
            logger.Info("New Fill:" + f.GetTradeInfo());
            //记录帐户成交记录
            TLCtxHelper.ModuleDataRepository.NewTrade(f);
        }


        /// <summary>
        /// 响应路由中心委托回报
        /// </summary>
        /// <param name="o"></param>
        void OnOrderEvent(Order o)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
            if (account == null)
            {
                logger.Warn(string.Format("Account of Order:{0} do not exist", o.GetOrderInfo()));
            }
            AmendOrderComment(ref o);

            LogOrder(o);

            //清算中心响应委托回报
            TLCtxHelper.ModuleClearCentre.GotOrder(account,o);

            //触发交易账户委托事件
            //account.FireOrderEvent(o);

            //?如何完善cancel机制
            if (o.Status == QSEnumOrderStatus.Canceled)
            {
                //清算中心响应取消回报
                OnCancelEvent(o.id);
            }

            //对外触发委托事件
            TLCtxHelper.EventIndicator.FireOrderEvent(o); //可以再某个组件 监听Indicator进行交易数据记录

            //对外通知
            this.NotifyOrder(o);
        }


        /// <summary>
        /// 响应成交路由返回的成交回报
        /// </summary>
        /// <param name="t"></param>
        void OnFillEvent(Trade t)
        {
            IAccount account = TLCtxHelper.ModuleAccountManager[t.Account];
            if (account == null)
            {
                logger.Warn(string.Format("Account of Trade:{0} do not exist", t.GetTradeInfo()));
            }
            //设定系统内成交编号
            AssignTradeID(ref t);

            bool accept = false;
            //清算中心响应成交回报
            TLCtxHelper.ModuleClearCentre.GotFill(account,t, out accept);//注这里的成交没有结算手续费,成交部分我们需要在结算中心结算结算完手续费后再向客户端发送

            LogTrade(t);

            //如果清算中心无法处理该成交 则直接返回 不用向系统触发成交事件或通知客户端
            if (!accept) return;

            //触发交易账户成交事件
            //account.FireFillEvent(t);

            //对外通知成交 Indicator总线
            TLCtxHelper.EventIndicator.FireFillEvent(t);

            //对外通知
            this.NotifyFill(t);

            //对外通知持仓更新
            Position pos = account.GetPosition(t.Symbol, t.PositionSide);
            if (pos != null)
            {
                //对外通知
                NotifyPositionUpdate(pos);
            }
            
        }

        void OnCancelEvent(long oid)
        {

            Order o = TLCtxHelper.ModuleClearCentre.SentOrder(oid);
            IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
            if (account == null)
            {
                logger.Warn(string.Format("Account of Cancel:{0} do not exist", oid));
            }

            if (o != null)
            {
                OrderAction action = new OrderActionImpl();
                action.Account = o.Account;
                action.ActionFlag = QSEnumOrderActionFlag.Delete;
                action.OrderID = o.id;
                this.LogOrderAction(action);

            }
            //清算中心响应取消回报
            TLCtxHelper.ModuleClearCentre.GotCancel(account,oid);
            //对外触发取消事件
            TLCtxHelper.EventIndicator.FireCancelEvent(oid);
            //对外通知
            this.NotifyCancel(oid);
        }

        /// <summary>
        /// 更新委托状态内容
        /// 1.未设定委托状态内容 根据委托状态更新默认状态内容
        /// 2.根据设定添加相关状态前缀
        /// </summary>
        /// <param name="o"></param>
        void AmendOrderComment(ref Order o)
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

            if (simpromptenable && o.Broker == "SIMBROKER")
            {
                o.Comment = simprompt + ":" + o.Comment;
            }
        }

        #region 二元期权回报处理
        void OnBOOrderErrorEvent(BinaryOptionOrder o, RspInfo info, bool needlog = true)
        {

            this.NotifyBOOrderError(o, info);
        }

        void OnBOOrderEvent(BinaryOptionOrder o)
        {

            this.NotifyBOOrder(o);
        }
        #endregion

    }
}
