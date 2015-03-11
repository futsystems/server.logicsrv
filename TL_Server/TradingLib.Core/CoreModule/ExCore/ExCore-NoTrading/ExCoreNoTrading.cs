using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class ExCoreNoTrading : ExCore, IModuleExCore
    {
        const string CoreName = "MsgExch";

        public ExCoreNoTrading()
            : base(ExCoreNoTrading.CoreName)
        {


        }


        public void Start()
        { 
        
        }

        public void Stop()
        { 
        
        }


        /// <summary>
        /// 默认ExCore 发送委托 系统会分配 内部编号，同时调用ReplyOrder进行记录
        /// 在Passthrough下 在提交委托阶段不进行数据储存，只在接口侧返回数据时才进行处理
        /// </summary>
        /// <param name="o"></param>
        /// <param name="inter"></param>
        /// <param name="riskcheck"></param>
        protected override void  OrderRequestHandler(Order o, bool inter = false, bool riskcheck = true)
        {
            try
            {
                //给委托绑定唯一的委托编号
                //AssignOrderID(ref o);

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
                            ReplyErrorOrder(o, info,false);

                            debug("委托(" + o.id.ToString() + ")被拒绝,ErrorID:" + info.ErrorID.ToString() + " ErrorMesssage:" + info.ErrorMessage, QSEnumDebugLevel.WARNING);
                            return;
                        }
                    }

                    //通过了风控检查的委托
                    o.Status = QSEnumOrderStatus.Placed;

                    //debug("####################### gotorderevent Placed", QSEnumDebugLevel.INFO);
                    //向客户端发送委托提交回报 这里已经将委托提交到清算中心做记录,没有通过委托检查的委托 通过ReplyErrorOrder进行回报
                    //ReplyOrder(o);

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
