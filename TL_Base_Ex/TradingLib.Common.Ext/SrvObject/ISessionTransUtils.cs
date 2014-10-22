﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    public static class ISessionTransUtils
    {
        /// <summary>
        /// 发送客户端逻辑数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public static void SendPacket(this ISession session,IPacket packet)
        {
            TLCtxHelper.Ctx.MessageExchange.Send(packet);
        }

        /// <summary>
        /// 发送管理端逻辑数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        public static void SendPacketMgr(this ISession session, IPacket packet)
        {
            TLCtxHelper.Ctx.MessageMgr.Send(packet);
        }

        /// <summary>
        /// 向管理端发送一个jsonreply回报
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reply"></param>
        /// <param name="islast"></param>
        public static  void SendJsonReplyMgr(this ISession session, TradingLib.Mixins.JsonReply reply, bool islast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = islast;

            response.RspInfo.ErrorID = reply.Code;
            response.RspInfo.ErrorMessage = reply.Message;
            response.Result = new Mixins.ReplyWriter().Start().FillReply(reply).End().ToString();

            SendPacketMgr(session, response);
        }

        /// <summary>
        /// 操作错误回报
        /// 通过FutsRspErro携带具体的错误消息生成RspMGROperationResponse 发送给对应的客户端
        /// </summary>
        /// <param name="session"></param>
        /// <param name="error"></param>
        public static void OperationError(this ISession session,FutsRspError error)
        {
            RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(session);
            response.RspInfo.Fill(error);

            SendPacketMgr(session, response);
        }

        /// <summary>
        /// 错误成功回报
        /// 某种操作如果没有特定的回报类型,则通过通用Response进行回报 并携带具体的成功消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="successmessage"></param>
        public static void OperationSuccess(this ISession session, string successmessage)
        {
            RspMGROperationResponse response = ResponseTemplate<RspMGROperationResponse>.SrvSendRspResponse(session);
            response.RspInfo.ErrorMessage = successmessage;

            SendPacketMgr(session, response);
        }




        /// <summary>
        /// 向一组管理端发送通知
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reply"></param>
        /// <param name="locations"></param>
        /// <param name="islast"></param>
        //public static void SendJsonNotifyMgr(this ISession session, string notifyname, TradingLib.Mixins.JsonReply reply, ILocation[] locations, bool islast = true)
        //{
        //    NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(locations);
        //    response.ModuleID = session.ContirbID;
        //    response.CMDStr = notifyname;
        //    response.Result = new Mixins.ReplyWriter().Start().FillReply(reply).End().ToString();

        //    SendPacketMgr(session, response);
        //}
        /// <summary>
        /// 向管理端发送一个jsonwrapper对象
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj"></param>
        /// <param name="islast"></param>
        public static void SendJsonReplyMgr(this ISession session, object obj, bool islast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = islast;
            response.Result = new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(obj).End().ToString();

            SendPacketMgr(session, response);
        }


        /// <summary>
        /// 向一组管理端发送通知
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj"></param>
        /// <param name="locations"></param>
        /// <param name="islast"></param>
        //public static void SendJsonNotifyMgr(ISession session,string notifyname, object obj,ILocation[] locations, bool islast = true)
        //{
        //    NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(locations);
        //    response.ModuleID = session.ContirbID;
        //    response.CMDStr = notifyname;
        //    response.Result = new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(obj).End().ToString();

        //    SendPacketMgr(session, response);
        //}

    }
}
