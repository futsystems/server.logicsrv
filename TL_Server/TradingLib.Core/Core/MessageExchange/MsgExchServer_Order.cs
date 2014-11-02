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
        /// 内部委托错误发送 
        /// 指定是否需要通知清算中心或者管理中心
        /// </summary>
        /// <param name="notify"></param>
        void ReplyErrorOrder(ErrorOrderNotify notify,bool needlog = true)
        {
            handler_GotOrderErrorNotify(notify,needlog);
            
        }

        /// <summary>
        /// 内部委托发送
        /// 当委托通过风控检查则需要通知客户端和清算中心
        /// </summary>
        /// <param name="o"></param>
        /// <param name="needsend"></param>
        void ReplyOrder(Order o)
        {
            handler_GotOrderEvent(o);
        }



        /// <summary>
        /// A:对外提供委托操作
        /// 暴露给 客户端快捷指令
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {
            OrderRequestHandler(o, false, true);//外部委托
        }
        /// <summary>
        /// B:直接向接口下单(跳过部分风控检查 账户冻结检查,强平时间外交易时间检查,帐户自定义委托检查等)
        /// 该函数只暴露给风控中心 用于执行强平
        /// </summary>
        /// <param name="o"></param>
        public void SendOrderInternal(Order o)
        {
            OrderRequestHandler(o, true, true);//内部委托
        }


        /// <summary>
        /// 客户端提交委托
        /// </summary>
        /// <param name="o"></param>
        void tl_newSendOrderRequest(Order o)
        {
            o.id = 0;//从外部传入的委托 统一置委托编号为0,由内部统一进行委托编号
            OrderRequestHandler(o, false, true);//外部委托
        }

        /// <summary>
        /// 绑定唯一的委托编号
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public void AssignOrderID(ref Order o)
        {
            if (o.id <= 0)
                o.id = _idt.AssignId;
            //如果委托编号已经被记录,则重新获得委托编号(防止产生重复委托编号)
          
            //委托编号唯一性检查 如果清算中心已经维护过该委托编号 则重新设定委托编号
            while (_clearcentre.IsOrderTracked(o.id))
            {
                o.id = _idt.AssignId;
            }

            //获得本地递增流水号
            o.OrderSeq = _clearcentre.NextOrderSeq;
        }


        /// <summary>
        /// 系统处理委托
        /// 单账户25秒 5000个委托 200个委托/秒
        /// 检查margin不拒绝委托/simbroker直接将委托写入cache         系统运行正常
        /// 检查margin 产生拒绝/simbroker直接将委托写入cache         系统运行正常
        /// 
        /// 关于大并发发送委托导致 datafeedrouter.gottick崩溃的问题。
        /// 委托:ansycserver->tlserver->tradingserver.tl_newSendOrderRequest(风控检查,需要调用清算中心检查保证金)
        /// </summary>
        /// <param name="o"></param>
        /// <param name="riskcheck"></param>
        void OrderRequestHandler(Order o, bool inter = false, bool riskcheck = true)
        {
            try
            {
                //给委托绑定唯一的委托编号
                AssignOrderID(ref o);
                IAccount acc = null;
                if (!_riskcentre.TrckerOrderAccount(o, out acc))
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    ErrorOrderNotify order = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(o.Account);
                    order.Order = o;
                    order.RspInfo.FillError("TRADING_ACCOUNT_NOT_FOUND");
                    ReplyErrorOrder(order,false);
                    return;
                }

                //执行常规检查,step1检查不涉及帐户类的检查不用加锁 常规检查部分拒绝的委托不记录到数据库 避免记录很多无效委托
                if (riskcheck)
                {
                    debug("Got Order[Check1]:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                    string errortitle = string.Empty;
                    bool needlog = true;
                    if (!_riskcentre.CheckOrderStep1(ref o, acc, out needlog,out errortitle, inter))
                    {
                        o.Status = QSEnumOrderStatus.Reject;
                        ErrorOrderNotify order = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(o.Account);
                        order.Order = o;
                        order.RspInfo.FillError(errortitle);
                        o.comment = "风控拒绝:" + order.RspInfo.ErrorMessage;
                        ReplyErrorOrder(order,needlog);
                        debug("委托(" + o.id.ToString() + ")被拒绝,ErrorID:" + order.RspInfo.ErrorID.ToString() + " ErrorMesssage:" + order.RspInfo.ErrorMessage +" needlog:"+needlog.ToString(),QSEnumDebugLevel.WARNING);
                        return;
                    }
                }

                
                //锁定该账户,表明同一时刻只有一个线程可以对某个特定account进行下列操作 对于单个account只能处理一个委托
                lock (acc)
                {
                    //委托风控检查
                    string msg = "";
                    bool riskcheckresuslt = true;
                    if (riskcheck)
                    {
                        debug("Got Order[Check2]:" + o.id.ToString(), QSEnumDebugLevel.INFO);
                        riskcheckresuslt = _riskcentre.CheckOrderStep2(ref o, acc,out msg, inter);
                        if (!riskcheckresuslt)
                        {
                            o.Status = QSEnumOrderStatus.Reject;
                            ErrorOrderNotify order = ResponseTemplate<ErrorOrderNotify>.SrvSendNotifyResponse(o.Account);
                            order.Order = o;
                            order.RspInfo.FillError("RISKCENTRE_CHECK_ERROR");
                            order.RspInfo.ErrorMessage = string.IsNullOrEmpty(msg) ? order.RspInfo.ErrorMessage : msg;//错误代码替换
                            o.comment = "风控拒绝:"+order.RspInfo.ErrorMessage;
                            ReplyErrorOrder(order);
                            debug("委托(" + o.id.ToString() + ")被拒绝,ErrorID:" + order.RspInfo.ErrorID.ToString() + " ErrorMesssage:" + order.RspInfo.ErrorMessage, QSEnumDebugLevel.WARNING);
                            return;
                        }
                    }

                    //通过了风控检查的委托 状态为Placed,若我们不进行风控检查 则直接设置委托为Placed
                    if (!riskcheck)
                        o.Status = QSEnumOrderStatus.Placed;

                    //debug("####################### gotorderevent Placed", QSEnumDebugLevel.INFO);
                    //向客户端发送委托提交回报 这里已经将委托提交到清算中心做记录,没有通过委托检查的委托 通过ReplyErrorOrder进行回报
                    ReplyOrder(o);

                    //debug("####################### brokerrouter send order", QSEnumDebugLevel.INFO);
                    //委托通过风控检查,则通过brokerrouter路由到对应的下单接口
                    if (o.Status == QSEnumOrderStatus.Placed)
                        _brokerRouter.SendOrder(o);
                }
            }
            catch (Exception ex)
            {
                //向外层抛出异常
                debug("OrderRequestHandler error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                throw (new QSTradingServerSendOrderError(ex));
            }

        }

    }
}
