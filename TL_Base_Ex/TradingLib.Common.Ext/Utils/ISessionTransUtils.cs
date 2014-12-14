using System;
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
        /// 获得Sessoin对应的Manager
        /// 如果是ClientManager则返回null
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static Manager GetManager(this ISession session)
        {
            if (session.IsManager() && session is Client2Session)
            {
                return (session as Client2Session).Manager;
            }
            return null;
        }

        /// <summary>
        /// 发送客户端逻辑数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        private static void SendPacket(this ISession session,IPacket packet)
        {
            TLCtxHelper.Ctx.MessageExchange.Send(packet);
        }

        /// <summary>
        /// 发送管理端逻辑数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        private static void SendPacketMgr(this ISession session, IPacket packet)
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

            session.SendPacketMgr(response);
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

            session.SendPacketMgr(response);
        }


        /// <summary>
        /// 向某地址列表发送通知 如果地址列表为null,则发送到ISession对应的地址
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cmdstr"></param>
        /// <param name="obj"></param>
        /// <param name="targets"></param>
        public static void NotifyMgr(this ISession session, string cmdstr, object obj, IEnumerable<ILocation> targets = null)
        {
            //通知方式 request获得对应的判断谓词 用于判断哪个客户端需要通知，然后再投影获得对应的地址集合
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(targets == null ? new ILocation[] { session.GetLocation() } : targets);
            response.ModuleID = session.ContirbID;
            response.CMDStr = cmdstr;
            response.Result = new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(obj).End().ToString();
            session.SendPacketMgr(response);
        }



        public static ILocation GetLocation(this ISession session)
        {
            ILocation location = new Location();
            location.ClientID = session.ClientID;
            location.FrontID = session.FrontID;
            return location;
        }

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
    }
}
