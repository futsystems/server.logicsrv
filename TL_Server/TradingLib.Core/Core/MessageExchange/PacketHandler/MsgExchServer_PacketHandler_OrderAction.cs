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
        /// 响应客户端委托操作
        /// </summary>
        /// <param name="request"></param>
        void tl_newOrderActionRequest(OrderActionRequest request)
        {
            OrderAction action = request.OrderAction;
            Order o = null;

            IAccount account = TLCtxHelper.ModuleAccountManager[request.OrderAction.Account];

            //1.通过交易系统分配的全局委托ID进行识别委托
            if (action.OrderID != 0)
            {

                o = TLCtxHelper.ModuleClearCentre.SentOrder(action.OrderID);
            }
            else//通过OrderRef FrontID SessionID 或者 OrderSysID进行查找委托
            {
                debug("OrderAction OrderRef:" + action.OrderRef + " Front:" + action.FrontID.ToString() + " Session:" + action.SessionID.ToString() + " OrderSysID:" + action.OrderExchID + " OrderRef:" + action.OrderRef + " Request:" + action.RequestID, QSEnumDebugLevel.INFO);
                o = account.Orders.FirstOrDefault(tmp=>(tmp.OrderRef == action.OrderRef && tmp.FrontIDi == action.FrontID && tmp.SessionIDi == action.SessionID) || (tmp.OrderSysID == action.OrderExchID));
            }

            if (o != null)
            {
                //撤销委托操作
                if (action.ActionFlag == QSEnumOrderActionFlag.Delete)
                {
                    //如果委托处于pending状态
                    if (o.IsPending())
                    {
                        //如果委托状态表面需要通过broker来取消委托 则通过broker来进行撤单
                        if (o.CanCancel())//opened partfilled
                        {
                            //委托操作回报
                            OrderActionNotify notify = ResponseTemplate<OrderActionNotify>.SrvSendNotifyResponse(action.Account);
                            notify.BindRequest(request);

                            //通过brokerrouter取消委托

                            TLCtxHelper.ModuleBrokerRouter.CancelOrder(o.id);
                        }
                        //处于中间状态Placed或Submited由系统单独逻辑进行定时检查 用于清除处于未知状态的委托
                        else if (o.Status == QSEnumOrderStatus.Submited || o.Status == QSEnumOrderStatus.Placed)//已经通过broker提交 该状态无法立即撤单 需要等待委托状态更新为Opened或者 被定时程序发现是一个错误委托
                        {
                            debug(string.Format("委托:{0} 处于:{1},等待broker返回", o.id, o.Status), QSEnumDebugLevel.INFO);
                            NotifyOrderActionError(action, RspInfoEx.Fill("ORDER_IN_PRESTAGE"));
                        }
                    }
                    else
                    {
                        //委托不可撤销
                        debug("对应委托不可撤销:"+o.GetOrderInfo(), QSEnumDebugLevel.WARNING);
                        NotifyOrderActionError(action, RspInfoEx.Fill("ORDER_CAN_NOT_BE_DELETE"));
                    }
                }
            }
            else//委托操作所指定的委托不存在 委托操作字段错误
            {
                debug("对应委托没有找到", QSEnumDebugLevel.WARNING);
                NotifyOrderActionError(action, RspInfoEx.Fill("ORDERACTION_BAD_FIELD"));
            }

        }

    }
}
