﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Common
{
    /// <summary>
    /// ISession 扩展方法
    /// 发送消息,获得相关对象等
    /// </summary>
    public static class ISessionUtils
    {
        /// <summary>
        /// 是否是管理端
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsManager(this ISession session)
        {
            return session.SessionType == QSEnumSessionType.MANAGER;
        }


        /// <summary>
        /// 是否是管理端
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static bool IsClient(this ISession session)
        {
            return session.SessionType == QSEnumSessionType.CLIENT;
        }

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

        public static IAccount GetAccount(this ISession session)
        {
            if (session.IsClient() && session is Client2Session)
            {
                return (session as Client2Session).Account;
            }
            return null;
        }


        /// <summary>
        /// 发送管理端逻辑数据包
        /// </summary>
        /// <param name="session"></param>
        /// <param name="packet"></param>
        private static void SendPacketMgr(this ISession session, IPacket packet)
        {
            TLCtxHelper.ModuleMgrExchange.Send(packet);
        }

        /// <summary>
        /// 将某个对象放入JsonReply返回给管理端
        /// </summary>
        /// <param name="session"></param>
        /// <param name="obj"></param>
        /// <param name="islast"></param>
        public static void ReplyMgr(this ISession session, object obj, bool islast = true)
        {
            RspMGRContribResponse response = ResponseTemplate<RspMGRContribResponse>.SrvSendRspResponse(session);
            response.ModuleID = session.ContirbID;
            response.CMDStr = session.CMDStr;
            response.IsLast = islast;
            response.Result = obj.SerializeObject();

            session.SendPacketMgr(response);
        }

        /// <summary>
        /// 讲对象数组一个一个发给管理段
        /// 避免数据大时 讲所有数据放在一个jason arrary中导致数据传输问题
        /// </summary>
        /// <param name="session"></param>
        /// <param name="list"></param>
        //public static void ReplyMgrArray(this ISession session, object[] list)
        //{
        //    if (list.Length > 0)
        //    {
        //        for (int i = 0; i < list.Length; i++)
        //        {
        //            session.ReplyMgr(list[i], i == list.Length - 1);
        //        }
        //    }
        //    else
        //    {
        //        session.ReplyMgr(null);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="items"></param>
        public static void ReplyMgrArray(this ISession session, object[] items)
        {
            if (items.Length == 0)
            {
                session.ReplyMgr(null, true);
            }
            for (int i = 0; i < items.Length; i++)
            {
                session.ReplyMgr(items[i], i == items.Length - 1);
            }
        }
        /// <summary>
        /// 操作错误回报
        /// 通过FutsRspErro携带具体的错误消息生成RspMGROperationResponse 发送给对应的客户端
        /// </summary>
        /// <param name="session"></param>
        /// <param name="error"></param>
        public static void RspError(this ISession session,FutsRspError error)
        {
            RspMGRResponse response = ResponseTemplate<RspMGRResponse>.SrvSendRspResponse(session);
            response.RspInfo.Fill(error);
            Console.WriteLine("RspError:" + response.ToString());
            session.SendPacketMgr(response);
        }

        public static void RspError(this ISession session, string error_id)
        {
            RspMGRResponse response = ResponseTemplate<RspMGRResponse>.SrvSendRspResponse(session);
            response.RspInfo.Fill(error_id);
            session.SendPacketMgr(response);
        }

        /// <summary>
        /// 错误成功回报
        /// 某种操作如果没有特定的回报类型,则通过通用Response进行回报 并携带具体的成功消息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="successmessage"></param>
        public static void RspMessage(this ISession session, string message)
        {
            RspMGRResponse response = ResponseTemplate<RspMGRResponse>.SrvSendRspResponse(session);
            response.RspInfo.ErrorMessage = message;
            Console.WriteLine("测试 RspMessage:" + response.ToString());
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
            NotifyMGRContribNotify response = ResponseTemplate<NotifyMGRContribNotify>.SrvSendNotifyResponse(targets == null ? new ILocation[] { session.Location } : targets);
            response.ModuleID = session.ContirbID;
            response.CMDStr = cmdstr;
            response.Result = obj.SerializeObject();
            session.SendPacketMgr(response);
        }



        /// <summary>
        /// 想符合某个通知判定谓词的管理端列表发送通知
        /// 比如Manager发生更新,出入金请求记录等需要向特定的管理员发送通知，从而实现各个管理端界面同步更新
        /// </summary>
        /// <param name="session"></param>
        /// <param name="cmdstr"></param>
        /// <param name="obj"></param>
        /// <param name="predicate"></param>
        public static void NotifyMgr(this ISession session, string cmdstr, object obj, Predicate<Manager> predicate)
        {
            IEnumerable<ILocation> locations = TLCtxHelper.ModuleMgrExchange.GetNotifyTargets(predicate);
            if (locations.Count() > 0)
            {
                session.NotifyMgr(cmdstr,obj,locations);
            }
        }
    }
}
