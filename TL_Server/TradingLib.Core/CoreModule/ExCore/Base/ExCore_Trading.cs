using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ExCore
    {

        /// <summary>
        /// 订阅某个合约行情
        /// </summary>
        /// <param name="sym"></param>
        public void RegisterSymbol(Symbol sym)
        {
            SymbolBasket b = new SymbolBasketImpl(sym);
            TLCtxHelper.ModuleDataRouter.RegisterSymbols(b);
        }

        /// <summary>
        /// 内部委托错误发送 
        /// 指定是否需要通知清算中心或者管理中心
        /// </summary>
        /// <param name="notify"></param>
        protected void ReplyErrorOrder(Order order, RspInfo info, bool needlog = true)
        {
            OnOrderErrorEvent(order, info, needlog);
        }

        /// <summary>
        /// 内部委托发送
        /// 当委托通过风控检查则需要通知客户端和清算中心
        /// </summary>
        /// <param name="o"></param>
        /// <param name="needsend"></param>
        protected void ReplyOrder(Order o)
        {
            OnOrderEvent(o);
        }


        #region 交易操作入口

        /// <summary>
        /// 对外提供取消委托操作
        /// 管理端取消委托
        /// </summary>
        /// <param name="val"></param>
        public void CancelOrder(long val)
        {
            try
            {
                debug("Got CancelOrder :" + val, QSEnumDebugLevel.INFO);
                Order o = TLCtxHelper.ModuleClearCentre.SentOrder(val);
                //如果委托处于pending状态
                if (o.IsPending())
                {
                    //如果委托状态表面需要通过broker来取消委托 则通过broker来进行撤单
                    if (o.CanCancel())//opened partfilled
                    {
                        TLCtxHelper.ModuleBrokerRouter.CancelOrder(o.id);
                    }
                    else if (o.Status == QSEnumOrderStatus.Submited)//已经通过broker提交 该状态无法立即撤单 需要等待委托状态更新为Opened或者 被定时程序发现是一个错误委托
                    {
                        debug("委托:" + val.ToString() + "处于Submited,等待broker返回", QSEnumDebugLevel.INFO);
                    }
                    else if (o.Status == QSEnumOrderStatus.Placed)//
                    {
                        debug("委托:" + val.ToString() + "处于Placed,等待系统返回", QSEnumDebugLevel.INFO);
                    }
                }
                else
                {
                    debug("委托:" + val.ToString() + "不可撤销", QSEnumDebugLevel.WARNING);
                }

            }
            catch (Exception ex)
            {
                debug("取消委托出错:" + ex.ToString());
                throw (new QSTradingServerCancleError(ex));
            }
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
        #endregion




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
        protected void OrderRequestHandler(Order o, bool inter = false, bool riskcheck = true)
        {
            try
            {
                //给委托绑定唯一的委托编号
                AssignOrderID(ref o);

                //检查交易帐户
                IAccount acc = TLCtxHelper.ModuleAccountManager[o.Account];
                if (acc == null)
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    ReplyErrorOrder(o, RspInfoEx.Fill("TRADING_ACCOUNT_NOT_FOUND"), false);
                    return;
                }

                //执行常规检查,step1检查不涉及帐户类的检查不用加锁 常规检查部分拒绝的委托不记录到数据库 避免记录很多无效委托
                if (riskcheck)
                {
                    debug("Got Order[Check1]:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                    string errortitle = string.Empty;
                    bool needlog = true;

                    if (!TLCtxHelper.ModuleRiskCentre.CheckOrderStep1(ref o, acc, out needlog, out errortitle, inter))
                    {
                        o.Status = QSEnumOrderStatus.Reject;
                        RspInfo info = RspInfoEx.Fill(errortitle);

                        o.Comment = "风控拒绝:" + info.ErrorMessage;
                        ReplyErrorOrder(o, info, needlog);

                        debug("委托(" + o.id.ToString() + ")被拒绝,ErrorID:" + errortitle + " ErrorMesssage:" + info.ErrorMessage + " needlog:" + needlog.ToString(), QSEnumDebugLevel.WARNING);
                        return;
                    }
                }


                //锁定该账户,表明同一时刻只有一个线程可以对某个特定account进行下列操作 对于单个account只能处理一个委托
                lock (acc)
                {
                    //委托风控检查
                    string msg = "";
                    if (riskcheck)
                    {
                        debug("Got Order[Check2]:" + o.id.ToString(), QSEnumDebugLevel.INFO);
                        if (!TLCtxHelper.ModuleRiskCentre.CheckOrderStep2(ref o, acc, out msg, inter))
                        {
                            o.Status = QSEnumOrderStatus.Reject;
                            RspInfo info = RspInfoEx.Fill("RISKCENTRE_CHECK_ERROR");
                            info.ErrorMessage = string.IsNullOrEmpty(msg) ? info.ErrorMessage : msg;//错误代码替换
                            o.Comment = "风控拒绝:" + info.ErrorMessage;
                            ReplyErrorOrder(o, info);

                            debug("委托(" + o.id.ToString() + ")被拒绝,ErrorID:" + info.ErrorID.ToString() + " ErrorMesssage:" + info.ErrorMessage, QSEnumDebugLevel.WARNING);
                            return;
                        }
                    }

                    //通过了风控检查的委托
                    o.Status = QSEnumOrderStatus.Placed;
                    //debug("####################### gotorderevent Placed", QSEnumDebugLevel.INFO);
                    //向客户端发送委托提交回报 这里已经将委托提交到清算中心做记录,没有通过委托检查的委托 通过ReplyErrorOrder进行回报
                    ReplyOrder(o);

                    //debug("####################### brokerrouter send order", QSEnumDebugLevel.INFO);
                    //委托通过风控检查,则通过brokerrouter路由到对应的下单接口
                    if (o.Status == QSEnumOrderStatus.Placed)
                        TLCtxHelper.ModuleBrokerRouter.SendOrder(o);
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
