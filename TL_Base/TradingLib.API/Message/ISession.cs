using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace TradingLib.API
{
    public delegate void ISessionDel(ISession session);
    public delegate void IPacketSessionDelegate(IPacket packet,ISession session);

    public enum QSEnumSessionType
    { 
        /// <summary>
        /// 交易客户端session
        /// </summary>
        CLIENT,
        /// <summary>
        /// 管理端session
        /// </summary>
        MANAGER,

    }
    /// <summary>
    /// 在扩展模块相应网络端消息时,函数调用需要提供一个支持ISession接口的对象,用于标注客户端位置并向该客户端端发送消息
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// 客户端通话唯一标识,内部采用GUID进行标识,每个交易客户端或者管理端注册到系统时,系统会自动给其分配一个
        /// 唯一的会话编号来标记与该客户端的通讯
        /// </summary>
        //int iSessionID { get; }

        /// <summary>
        /// 前置编号
        /// </summary>
        //int iFrontID { get; set; }

        
        /// <summary>
        /// 客户端ID
        /// </summary>
        string ClientID { get; }

        /// <summary>
        /// 前置编号,标注了该客户端是从哪个前置连接到交易系统
        /// </summary>
        string FrontID { get; }

        /// <summary>
        /// 回话类型
        /// </summary>
        QSEnumSessionType SessionType { get; set; }


        /// <summary>
        /// 交易帐号ID
        /// </summary>
        string AccountID { get; }

        /// <summary>
        /// 管理员ID
        /// </summary>
        string ManagerID { get; }



        /// <summary>
        /// 对应的扩展模块编号
        /// </summary>
        string ContirbID { get; set; }

        /// <summary>
        /// 对应的扩展模块命令
        /// </summary>
        string CMDStr { get; set; }

        /// <summary>
        /// 对端请求编号
        /// </summary>
        int RequestID { get; set; }

        /// <summary>
        /// 是否登入
        /// </summary>
        bool IsLoggedIn { get; }
    }
}
