using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    /// <summary>
    /// 委托编号
    /// 从委托进入系统的第一个入口 
    /// 设定委托唯一编号id和日内流水OrderSeq
    /// 
    /// </summary>
    public partial class MsgExchServer
    {

        /// <summary>
        /// 客户端提交委托
        /// </summary>
        /// <param name="o"></param>
        void SrvOnOrderRequest(ISession session,OrderInsertRequest request,IAccount account)
        {
            logger.Info("Got Order:" + request.Order.GetOrderInfo());

            //检查插入委托请求是否有效
            if (!request.IsValid)
            {
                logger.Warn("请求无效");
                return;
            }
            //如果请求没有指定交易帐号 则根据对应的Client信息自动绑定交易帐号
            if (string.IsNullOrEmpty(request.Order.Account))
            {
                request.Order.Account = account.ID;
            }
            //如果指定交易帐号与登入交易帐号不符 则回报异常
            if (account.ID != request.Order.Account)//客户端没有登入或者登入ID与委托ID不符
            {
                logger.Warn("客户端对应的帐户:" + account.ID + " 与委托帐户:" + request.Order.Account + " 不符合");
                return;
            }

            //标注来自客户端的原始委托
            Order order = new OrderImpl(request.Order);//复制委托传入到逻辑层
            order.id = 0;
            order.OrderSource = QSEnumOrderSource.CLIENT;

            //设定委托达到服务器时间
            order.Date = Util.ToTLDate();
            order.Time = Util.ToTLTime();

            //设定TotalSize为 第一次接受到委托时候的Size
            order.TotalSize = order.Size;

            //设定分帐户端信息
            order.FrontIDi = session.FrontIDi;
            order.SessionIDi = session.SessionIDi;
            order.RequestID = request.RequestID;


            order.id = 0;//从外部传入的委托 统一置委托编号为0,由内部统一进行委托编号

            OrderRequestHandler(order, false, true);//外部委托
        }

        void OrderRequestHandler(Order o, bool inter = false, bool riskcheck = true)
        {
            try
            {
                //给委托绑定唯一的委托编号
                AssignOrderID(ref o);

                //检查交易帐户
                IAccount account = TLCtxHelper.ModuleAccountManager[o.Account];
                if (account == null)
                {
                    o.Status = QSEnumOrderStatus.Reject;
                    OnOrderErrorEvent(o, RspInfoEx.Fill("TRADING_ACCOUNT_NOT_FOUND"), false);
                    return;
                }

                //执行常规检查,step1检查不涉及帐户类的检查不用加锁 常规检查部分拒绝的委托不记录到数据库 避免记录很多无效委托
                if (riskcheck)
                {
                    logger.Info("Got Order[Check1]:" + o.id.ToString());
                    string errortitle = string.Empty;
                    bool needlog = true;

                    if (!TLCtxHelper.ModuleRiskCentre.CheckOrderStep1(ref o, account, out needlog, out errortitle, inter))
                    {
                        o.Status = QSEnumOrderStatus.Reject;
                        RspInfo info = RspInfoEx.Fill(errortitle);

                        o.Comment = "风控拒绝:" + info.ErrorMessage;
                        OnOrderErrorEvent(o, info, needlog);

                        logger.Warn(string.Format("Order[{0}] Is Reject / RspInfo:{1} NeedLog:{2}", o.id, info, needlog));
                        return;
                    }
                }


                //锁定该账户,表明同一时刻只有一个线程可以对某个特定account进行下列操作 对于单个account只能处理一个委托
                lock (account)
                {
                    //委托风控检查
                    string msg = "";
                    if (riskcheck)
                    {
                        logger.Info("Got Order[Check2]:" + o.id.ToString());
                        if (!TLCtxHelper.ModuleRiskCentre.CheckOrderStep2(ref o, account, out msg, inter))
                        {
                            o.Status = QSEnumOrderStatus.Reject;
                            RspInfo info = RspInfoEx.Fill("RISKCENTRE_CHECK_ERROR");
                            info.ErrorMessage = string.IsNullOrEmpty(msg) ? info.ErrorMessage : msg;//错误代码替换
                            o.Comment = "风控拒绝:" + info.ErrorMessage;
                            OnOrderErrorEvent(o, info);

                            logger.Warn(string.Format("Order[{0}] Is Reject / RspInfo:{1}", o.id, info));
                            return;
                        }
                    }

                    //通过了风控检查的委托
                    o.Status = QSEnumOrderStatus.Placed;

                    //向客户端发送委托提交回报 这里已经将委托提交到清算中心做记录,没有通过委托检查的委托 通过ReplyErrorOrder进行回报
                    OnOrderEvent(o);

                    //委托通过风控检查,则通过brokerrouter路由到对应的下单接口
                    TLCtxHelper.ModuleBrokerRouter.SendOrder(o);
                }
            }
            catch (Exception ex)
            {
                logger.Error("OrderRequestHandler error:" + ex.ToString());
                throw ex;
            }

        }

    }
}
