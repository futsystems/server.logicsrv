using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.Mixins.JsonObject;


namespace TradingLib.Core
{
    /// <summary>
    /// 服务端的通知模块
    /// 1.交易帐户通过notifyresponse发送到管理端
    /// 2.其他管理端的操作 反应在交易帐户上 通过Account来建立通知target进行通知
    /// 3.其他管理端的操作 反应在其他对象上 需要通过建立单独的通知体系来进行通知
    /// </summary>
    public partial class MgrExchServer
    {

        /// <summary>
        /// 初始化服务端的通知
        /// </summary>
        void InitNotifySection()
        {
            TLCtxHelper.CashOperationEvent.CashOperationRequest += new EventHandler<CashOperationEventArgs>(CashOperationEvent_CashOperationRequest);
        }

        void CashOperationEvent_CashOperationRequest(object sender, CashOperationEventArgs e)
        {
            NotifyCashOperation(e.CashOperation);
        }

        /// <summary>
        /// 通过谓词顾虑出当前通知地址
        /// </summary>
        /// <param name="predictate"></param>
        /// <returns></returns>
        ILocation[] GetNotifyTargets(Predicate<CustInfoEx> predictate)
        {
            return this.NotifyTarges.Where(e => predictate(e)).Select(info => info.Location).ToArray();
        }


        /// <summary>
        /// 出入金状态通知
        /// </summary>
        /// <param name="op"></param>
        void NotifyCashOperation(JsonWrapperCashOperation op)
        {
            //通知方式 request获得对应的判断谓词 用于判断哪个客户端需要通知，然后再投影获得对应的地址集合
            //ILocation[] locations = this.NotifyTarges.Where(e => op.GetNotifyPredicate()(e)).Select(info => info.Location).ToArray();
            ILocation[] locations = GetNotifyTargets(op.GetNotifyPredicate());
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(locations);
            response.ModuleID = CoreName;
            response.CMDStr = "NotifyCashOperation";
            response.Result = new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(op).End().ToString();
            CachePacket(response);
            debug(" send out cashoperation notify");
        }


        /// <summary>
        /// 管理员更新通知
        /// </summary>
        /// <param name="mgr"></param>
        void NotifyManagerUpdate(Manager mgr)
        {
            ILocation[] locations = GetNotifyTargets(mgr.GetNotifyPredicate());
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(locations);
            response.ModuleID = CoreName;
            response.CMDStr = "NotifyManagerUpdate";
            response.Result = new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(mgr).End().ToString();
            CachePacket(response);
            debug(" send out managerupdate notify");

        }
    }
}
