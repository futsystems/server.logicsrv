using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{

    public class ContribSrvObject:BaseSrvObject
    {
        public ContribSrvObject(string programe)
            :base(programe)
        { 

            
        }

        
        ///// <summary>
        ///// 通过交易交换发送一个逻辑数据包
        ///// </summary>
        ///// <param name="packet"></param>
        //void SendPacket(IPacket packet)
        //{
        //    TLCtxHelper.Ctx.MessageExchange.Send(packet);
        //}

        ///// <summary>
        ///// 通过管理交换发送一个逻辑数据包
        ///// </summary>
        ///// <param name="packet"></param>
        //void SendPacketMgr(IPacket packet)
        //{
        //    TLCtxHelper.Ctx.MessageMgr.Send(packet);
        //}



        ////void Send(string message, MessageTypes type, string address)
        ////{
        ////    //TLCtxHelper.Ctx.MessageExchange.Send(message, type, address);
        ////}

        ///// <summary>
        ///// 向某个过滤条件的客户端发送一条消息
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="type"></param>
        ///// <param name="address"></param>
        ////public void Broadcast(string message, MessageTypes type, string filter)
        ////{
        ////    foreach (string address in TLCtxHelper.Ctx.MessageExchange.FilterClient(filter))
        ////    {
        ////        //TLCtxHelper.Ctx.MessageExchange.Send(message, type, address);
        ////    }
        ////}

        ///// <summary>
        ///// 向某个指定地址的客户端发送一条消息
        ///// </summary>
        ///// <param name="message"></param>
        ///// <param name="type"></param>
        ///// <param name="address"></param>
        ////public void Push(string message, MessageTypes type, string address)
        ////{
        ////    //TLCtxHelper.Ctx.MessageExchange.Send(message, type, address);
        ////}


        ///// <summary>
        ///// 向Session对应的客户端发送一条标准的Message
        ///// 发送Message的时候采用的是标准发送,通过生成Message发送一段内存块,对端需要通过
        ///// Message.gotmessage来解析该消息
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="type"></param>
        ///// <param name="message"></param>
        ////protected void Send(ISession session, string message, MessageTypes type)
        ////{
        ////    Send(message, type, session.ClientID);
        ////}





        ///// <summary>
        ///// 向Session对应的客户端发送一条文本字符串
        ///// 在发送该字符串消息的时候,系统会以格式 ContribID|CMDStr|Message 发送到对端
        ///// 用于告知对端从哪个扩展模块在哪个函数块发送的消息
        ///// </summary>
        ///// <param name="session"></param>
        ///// <param name="message"></param>
        //protected void SendContribResponse(ISession session, string jsonstr, bool islast = true)
        //{
        //    RspContribResponse response = ResponseTemplate<RspContribResponse>.SrvSendRspResponse(session);
        //    response.ModuleID = session.ContirbID;
        //    response.CMDStr = session.CMDStr;
        //    response.Result = jsonstr;
        //    response.IsLast = islast;
        //    SendPacket(response);
        //}

    }
}
