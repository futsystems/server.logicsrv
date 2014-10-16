using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.LitJson;

namespace TradingLib.Common
{

    public static class ISessionJsonUtils
    {
        /// <summary>
        /// 生成session所对应的jsonreply
        /// 需要标注code,message,以及模块ID,命令Str
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        //public static TradingLib.Mixins.ReplyWriter JsonReply(this ISession session)
        //{
        //    TradingLib.Mixins.ReplyWriter writer = new Mixins.ReplyWriter();
        //    writer.Start();
        //    writer.WritePropertyName("Code");
        //    writer.Write(0);
        //    writer.WritePropertyName("Message");
        //    writer.Write("");
        //    writer.WritePropertyName("ModuleID");
        //    writer.Write(session.ContirbID);
        //    writer.WritePropertyName("CMDStr");
        //    writer.Write(session.CMDStr);
        //    return writer;
        //}
    }


    public class ContribSrvObject:BaseSrvObject
    {
        public ContribSrvObject(string programe)
            :base(programe)
        { 

            
        }

        
        /// <summary>
        /// 发送一个逻辑数据包
        /// </summary>
        /// <param name="packet"></param>
        void SendPacket(IPacket packet)
        {
            TLCtxHelper.Ctx.MessageExchange.Send(packet);
        }



        void Send(string message, MessageTypes type, string address)
        {
            //TLCtxHelper.Ctx.MessageExchange.Send(message, type, address);
        }

        /// <summary>
        /// 向某个过滤条件的客户端发送一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="address"></param>
        public void Broadcast(string message, MessageTypes type, string filter)
        {
            foreach (string address in TLCtxHelper.Ctx.MessageExchange.FilterClient(filter))
            {
                //TLCtxHelper.Ctx.MessageExchange.Send(message, type, address);
            }
        }

        /// <summary>
        /// 向某个指定地址的客户端发送一条消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="address"></param>
        public void Push(string message, MessageTypes type, string address)
        {
            //TLCtxHelper.Ctx.MessageExchange.Send(message, type, address);
        }


        /// <summary>
        /// 向Session对应的客户端发送一条标准的Message
        /// 发送Message的时候采用的是标准发送,通过生成Message发送一段内存块,对端需要通过
        /// Message.gotmessage来解析该消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="type"></param>
        /// <param name="message"></param>
        protected void Send(ISession session, string message, MessageTypes type)
        {
            Send(message, type, session.ClientID);
        }


        void SendPacketMgr(IPacket packet)
        {
            TLCtxHelper.Ctx.MessageMgr.Send(packet);
        }


        /// <summary>
        /// 向Session对应的客户端发送一条文本字符串
        /// 在发送该字符串消息的时候,系统会以格式 ContribID|CMDStr|Message 发送到对端
        /// 用于告知对端从哪个扩展模块在哪个函数块发送的消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="message"></param>
        protected void SendContribResponse(ISession session, string jsonstr, bool islast = true)
        {
            RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.Result = jsonstr;
            response.IsLast = islast;
            SendPacket(response);
        }



        /// <summary>
        /// 向管理端发送一个jsonreply回报
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reply"></param>
        /// <param name="islast"></param>
        protected void SendJsonReplyMgr(ISession session, TradingLib.Mixins.JsonReply reply, bool islast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = islast;

            response.RspInfo.ErrorID = reply.Code;
            response.RspInfo.ErrorMessage = reply.Message;
            response.Result = new Mixins.ReplyWriter().Start().FillReply(reply).End().ToString();

            SendPacketMgr(response);
        }

        protected void SendJsonReplyMgr(ISession session, object obj, bool islast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = islast;
            response.Result = new Mixins.ReplyWriter().Start().FillReply(Mixins.JsonReply.GenericSuccess()).FillPlayload(obj).End().ToString();

            SendPacketMgr(response);
        }




        protected void SendJsonReply(ISession session, JsonReply reply,bool islast=true)
        {
            RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = islast;

            response.RspInfo.ErrorID = (int)reply.Code;
            response.RspInfo.ErrorMessage = reply.Message;

            SendPacket(response);
        }

        

        /// <summary>
        /// 向Session对应的客户端发送一个标准结构体
        /// 发送结构体时,通过将结构体序列化成Json字符串,然后以格式 ContribID|CMDStr|JsonString的格式发出
        /// 采用json发送消息的目的是对端处理可以通过解析json字符串然后方便的通过key获得对应的值
        /// 并且在系统升级或者扩充字段的时候不用考虑序列化时的顺序问题,值需要在原有结构体上进行增加字段即可
        /// 同时在公布ContribCommand API的时候只要同时公布返回数据的结构体字段与类型,对端就可以进行数据解析
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj">Struct结构体</param>

        protected void SendJsonObjs(ISession session, object[] objlist,bool islast =true)
        {
            JsonWriter w = ReplyHelper.NewJWriterWithSession(session);
            ReplyHelper.FillJWriter(objlist, w);
            ReplyHelper.EndWriter(w);
            SendContribResponse(session, w.ToString(),islast);
        }

        

        protected void SendJsonObj(ISession session, object obj,bool islast = true)
        {
            JsonWriter w = ReplyHelper.NewJWriterWithSession(session);
            ReplyHelper.FillJWriter(obj, w);
            ReplyHelper.EndWriter(w);
            SendContribResponse(session, w.ToString(),islast);
        }

        /// <summary>
        /// 循环发送一个组对象
        /// </summary>
        /// <param name="session"></param>
        /// <param name="objlist"></param>
        protected void MultiSendJsonObjs(ISession session, object[] objlist,bool setlast=true)
        {
            int num = objlist.Length;
            for (int i = 0; i < num;i++ )
            {
                SendJsonObj(session, objlist[i], setlast?(i == num - 1):false);
            }
        }




        /// <summary>
        /// 向一个地址列表中的客户端发送某个类型的消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="addresslist"></param>
        protected void Send(ISession session,string message, MessageTypes type, string[] addresslist)
        {
            foreach (string address in addresslist)
            {
                Send(message, type, address);
            }
        }

        protected void Send(ISession session, string message, string[] addresslist)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(session.ContirbID);
            sb.Append('|');
            sb.Append(session.CMDStr);
            sb.Append('|');
            sb.Append(message);
            Send(session, sb.ToString(), MessageTypes.CONTRIBRESPONSE, addresslist);
        }

        protected void Send(ISession session, object obj, string[] addresslist)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(session.ContirbID);
            sb.Append('|');
            sb.Append(session.CMDStr);
            sb.Append('|');
            sb.Append(JsonMapper.ToJson(obj));
            Send(session, sb.ToString(), MessageTypes.CONTRIBRESPONSE, addresslist);
        }

        
    }
}
