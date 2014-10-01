using System;
using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 客户端会话事件
    /// </summary>
    public class SessionEvent<T>
        where T:ClientInfoBase,new()
    {
        /// <summary>
        /// 客户端连接事件
        /// </summary>
        public event ClientInfoDelegate<T> ClientRegisterEvent;

        /// <summary>
        /// 客户端断开事件
        /// </summary>
        public event ClientInfoDelegate<T> ClientUnregistedEvent;


        /// <summary>
        /// 客户端登入 退出事件
        /// </summary>
        public event ClientLoginInfoDelegate<T> ClientLoginInfoEvent;
        /// <summary>
        /// 交易帐号登入成功
        /// </summary>
        public event AccountIdDel AccountLoginSuccessEvent;

        /// <summary>
        /// 交易帐号登入失败
        /// </summary>
        public event AccountIdDel AccountLoginFailedEvent;

        /// <summary>
        /// 交易帐号登入成功后向对应客户端推送附加消息
        /// 1.当前账户财务信息
        /// 2.风控信息
        /// 3.比赛信息
        /// 4.配资信息
        /// </summary>
        public event AccountIdDel NotifyLoginSuccessEvent;
        /// <summary>
        /// 会话状态改变事件
        /// 1.客户端交易帐号登入
        /// 2.客户端交易帐号注销
        /// 3.客户端回报本地硬件编码
        /// </summary>
        public event ISessionDel SessionChangedEvent;


        /// <summary>
        /// 客户端回话统一认证回调
        /// 如果绑定该事件则进行统一用户中心认证,不绑定则通过本地数据库表进行认证
        /// </summary>
        public event LoginRequestDel<T> AuthUserEvent;



        internal void FireClientConnectedEvent(T c)
        {
            if (ClientRegisterEvent != null)
                ClientRegisterEvent(c);
        }

        internal void FireClientDisconnectedEvent(T c)
        {
            if (ClientUnregistedEvent != null)
                ClientUnregistedEvent(c);
        }

        internal void FireClientLoginInfoEvent(T c, bool islogin)
        {
            if (ClientLoginInfoEvent != null)
                ClientLoginInfoEvent(c, islogin);
        }
        internal void FireSessionChangedEvent(ISession session)
        {
            if (SessionChangedEvent != null)
                SessionChangedEvent(session);
        }


        internal void FireAccountLoginSuccessEvent(string account)
        {
            if (AccountLoginSuccessEvent != null)
                AccountLoginSuccessEvent(account);

        }

        internal void FireAccountLoginFailedEvent(string account)
        {
            if (AccountLoginFailedEvent != null)
                AccountLoginFailedEvent(account);
        }
        internal void FireNotifyLoginSuccessEvent(string account)
        {
            if (NotifyLoginSuccessEvent != null)
                NotifyLoginSuccessEvent(account);
        }

        public bool IsAuthEventBinded { get { return AuthUserEvent != null; } }

        internal void FireAuthUserEvent(T client,LoginRequest request,ref LoginResponse response)
        {
            //TLCtxHelper.Debug("authuserevent binded :" + (AuthUserEvent != null).ToString());
            if (AuthUserEvent != null)
                 AuthUserEvent(client, request, ref response);
            
        }
    }
}
