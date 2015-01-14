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
        /// 交易帐号登入成功
        /// </summary>
        public event AccoundIDDel AccountLoginSuccessEvent;

        /// <summary>
        /// 交易帐号登入失败
        /// </summary>
        public event AccoundIDDel AccountLoginFailedEvent;

        /// <summary>
        /// 客户端登入 退出事件
        /// </summary>
        public event ClientLoginInfoDelegate<T> ClientLoginInfoEvent;


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


        public bool IsAuthEventBinded { get { return AuthUserEvent != null; } }

        internal void FireAuthUserEvent(T client,LoginRequest request,ref LoginResponse response)
        {
            //Util.Debug("authuserevent binded :" + (AuthUserEvent != null).ToString());
            if (AuthUserEvent != null)
                 AuthUserEvent(client, request, ref response);
            
        }
    }
}
