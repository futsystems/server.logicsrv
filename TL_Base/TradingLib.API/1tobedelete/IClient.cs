//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace TradingLib.API
//{
//    /// <summary>
//    /// Generic interface for  Clients.  
//    /// </summary>
//    public interface TLClient:IDebug
//    {
//        /// <summary>
//        /// 客户端所连接对端的类型,数据,交易,还是2者同时存在
//        /// </summary>
//        QSEnumProviderType ProviderType { get; set; }

//        /// <summary>
//        /// 是否处于连接状态
//        /// </summary>
//        bool IsConnected { get; }

//        #region 交易操作部分 提交委托 取消委托

//        /// <summary>
//        /// 发送委托
//        /// </summary>
//        /// <param name="order"></param>
//        /// <returns></returns>
//        int SendOrder (Order order);

//        /// <summary>
//        /// 取消委托
//        /// </summary>
//        /// <param name="id"></param>
//        void CancelOrder (long id);

//        #endregion

//        #region 行情部分 注册行情

//        /// <summary>
//        /// 请求行情数据
//        /// </summary>
//        /// <param name="mb"></param>
//        void Subscribe (SymbolBasket mb);

//        /// <summary>
//        /// 注销行情数据
//        /// </summary>
//        void Unsubscribe ();

//        #endregion

//        /// <summary>
//        /// 获得服务商所提供的FeatureList
//        /// </summary>
//        List<MessageTypes> RequestFeatureList { get; }

//        /// <summary>
//        /// 请求功能特征代码
//        /// </summary>
//        void RequestFeatures ();

//        /// <summary>
//        /// 注册到服务器
//        /// </summary>
//        //void Register();

//        /// <summary>
//        /// 向服务端发送心跳响应 告诉服务端客户端存活
//        /// </summary>
//        /// <returns></returns>
//        //void HeartBeat();
       

//        /// <summary>
//        /// 发送Message 底层的Register HearBeat等常用操作只存在于client内部,外层操作通过封装后由TLSend发送,同时保留TLSend提高了接口的可扩展性
//        /// </summary>
//        /// <param name="type"></param>
//        /// <param name="message"></param>
//        /// <returns></returns>
//        long TLSend (MessageTypes type, string message);

//        /// <summary>
//        /// 获得客户端编号
//        /// </summary>
//        string Name { get; set; }

//        /// <summary>
//        /// 获得服务器版本
//        /// </summary>
//        int ServerVersion { get; }

//        /// <summary>
//        /// 获得服务名
//        /// </summary>
//        Providers BrokerName { get; }

//        /// <summary>
//        /// get providers available
//        /// </summary>
//        Providers[] ProvidersAvailable { get; }

//        /// <summary>
//        /// 获得选当前的服务商
//        /// </summary>
//        int ProviderSelected { get; }

//        #region 客户端的启动 停止 与连接

//        /// <summary>
//        /// 停止客户端连接
//        /// </summary>
//        void Stop ();

//        /// <summary>
//        /// 启动客户端连接,并连接到currentprovider,若为-1,表示第一次进行连接
//        /// </summary>
//        void Start ();

//        /// <summary>
//        /// 连接到指定的服务端
//        /// </summary>
//        /// <param name="ProviderIndex"></param>
//        /// <param name="showwarning"></param>
//        /// <returns></returns>
//        bool Mode (int ProviderIndex, bool showwarning);

//        /// <summary>
//        /// 重新连接到服务端,同时重新对serverlist进行服务搜索
//        /// </summary>
//        /// <returns></returns>
//        bool Mode ();

//        #endregion

//        #region 客户端事件

//        /// <summary>
//        /// 获得Tick
//        /// </summary>
//        event TickDelegate gotTick;
//        /// <summary>
//        /// 获得持仓数据回报
//        /// </summary>
//        event PositionDelegate gotPosition;
//        /// <summary>
//        /// 获得委托回报
//        /// </summary>
//        event OrderDelegate gotOrder;
//        /// <summary>
//        /// 获得委托取消回报
//        /// </summary>
//        event LongDelegate gotOrderCancel;
//        /// <summary>
//        /// 获得成交回报
//        /// </summary>
//        event FillDelegate gotFill;
//        /// <summary>
//        /// 获得FeatureList回报
//        /// </summary>
//        event MessageTypesMsgDelegate gotFeatures;
//        /// <summary>
//        /// 获得其他扩展消息回报
//        /// </summary>
//        event MessageDelegate gotUnknownMessage;
//        /// <summary>
//        /// 获得登入回报
//        /// </summary>
//        event LoginResponseDel gotLoginRep;
//        /// <summary>
//        /// 连接建立回报
//        /// </summary>
//        event ConnectDel GotConnectEvent;
//        /// <summary>
//        /// 连接断开回报
//        /// </summary>
//        event DisconnectDel GotDisconnectEvent;
//        /// <summary>
//        /// Tick Publisher 服务有效 用于通知客户端注册市场数据
//        /// </summary>
//        event DataPubConnectDel DataPubConnectEvent;
////Tick publisher成功
//        /// <summary>
//        /// tick publisher连接断开
//        /// </summary>
//        event DataPubDisconnectDel DataPubDisconnectEvent;
////tick publisher连接断开

//        #endregion

//    }

//    /// <summary>
//    /// Used to indicate that a  Server was not running.
//    /// </summary>
//    public class TLServerNotFound : Exception
//    {

//    }
//}
