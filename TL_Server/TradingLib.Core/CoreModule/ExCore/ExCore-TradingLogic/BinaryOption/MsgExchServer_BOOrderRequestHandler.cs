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
        void BOOrderRequestHandler(BinaryOptionOrder o, bool inter = false, bool riskcheck = true)
        {
            try
            {
                //给委托绑定唯一的委托编号
                AssignOrderID(ref o);

                //检查交易帐户
                IAccount acc = TLCtxHelper.ModuleAccountManager[o.Account];
                if (acc == null)
                {
                    o.Status = EnumBOOrderStatus.Reject;
                    ReplyBOOrderError(o, RspInfoEx.Fill("TRADING_ACCOUNT_NOT_FOUND"), false);
                    return;
                }

                logger.Info("Got Order[Check1]:" + o.ID.ToString());
                string errortitle = string.Empty;
                bool needlog = true;

                if (!TLCtxHelper.ModuleRiskCentre.CheckOrderStep(ref o, acc, out needlog, out errortitle, inter))
                {
                    o.Status = EnumBOOrderStatus.Reject;
                    RspInfo info = RspInfoEx.Fill(errortitle);

                    o.Comment = "风控拒绝:" + info.ErrorMessage;
                    ReplyBOOrderError(o, info, needlog);

                    logger.Warn(string.Format("Order[{0}] Is Reject / RspInfo:{1} NeedLog:{2}", o.ID, info, needlog));
                    return;
                }

                lock (acc)
                {
                    //进行二段检查 资金等

                    //通过了风控检查的委托
                    o.Status = EnumBOOrderStatus.Placed;
                    //向客户端发送委托提交回报 这里已经将委托提交到清算中心做记录,没有通过委托检查的委托 通过ReplyErrorOrder进行回报
                    ReplyOrder(o);

                    //debug("####################### brokerrouter send order", QSEnumDebugLevel.INFO);
                    //委托通过风控检查,则通过brokerrouter路由到对应的下单接口
                    if (o.Status == EnumBOOrderStatus.Placed)
                        TLCtxHelper.ModuleBrokerRouter.SendOrder(o);
                }


            }
            catch (Exception ex)
            {
                logger.Error("BOOrderRequestHandler Error:" + ex.ToString());
            }
        }

    }
}
