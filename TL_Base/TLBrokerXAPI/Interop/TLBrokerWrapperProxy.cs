using System;
using System.IO;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

/*
 * 关于zmq的clr实现过程
 * 1.实现UnmanagedLibray 用于加载某个dll 提供导出函数到委托的转换
 * 2.具体业务单元TLBroker(zmq) 内部含有unmangedlibray 初始化时 加载dll 并将对应的导出函数绑定到委托
 * 3.在TLBroker(zmq)的基础上封装具体的业务类 实现功能单元
 * 4.业务部分利用功能单元 组成对应的功能对象
 * 
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.BrokerXAPI.Interop
{
    /// <summary>
    /// TLBrokerWrapper封装Proxy用于注入具体的Broker插件,形成通用的调用层
    /// </summary>
    internal class TLBrokerWrapperProxy : IDisposable
    {
        public static bool ValidWrapperProxy(string path,string dllname)
        {
            TLBrokerWrapperProxy proxy = null;
            try
            {
                proxy = new TLBrokerWrapperProxy(path, dllname);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                if(proxy != null)
                {
                    proxy.Dispose();
                }
            }
        }

        private bool _disposed;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_Wrapper == IntPtr.Zero)
                    return;
                _DestoryBrokerWrapper(_Wrapper);
            }

            _disposed = true;
        }


        private  readonly UnmanagedLibrary NativeLib;

        IntPtr _Wrapper = IntPtr.Zero;

        private IntPtr Wrapper { get { return _Wrapper; } }
        ILog logger = null;

        public TLBrokerWrapperProxy(string path, string dllname)
        {
            logger = LogManager.GetLogger(Path.GetFileNameWithoutExtension(dllname));
            //1.加载dll
            //Util.Info("Load Nativelib wrapper dll/so", this.GetType().Name);
            NativeLib = new UnmanagedLibrary(path, dllname);
            //2.绑定导出函数到委托
            AssignCommonDelegates();

            //3:创建对应的handle
            _Wrapper = _CreateBrokerWrapper();
        }

        /// <summary>
        /// 绑定dll导出的函数到对应的委托对象
        /// </summary>
        private  void AssignCommonDelegates()
        {
            _CreateBrokerWrapper = NativeLib.GetUnmanagedFunction<CreateBrokerWrapperProc>("CreateBrokerWrapper");
            _DestoryBrokerWrapper = NativeLib.GetUnmanagedFunction<DestoryBrokerWrapperProc>("DestoryBrokerWrapper");

            _Register = NativeLib.GetUnmanagedFunction<RegisterProc>("Register");
            _Connect = NativeLib.GetUnmanagedFunction<ConnectProc>("Connect");
            _Disconnect = NativeLib.GetUnmanagedFunction<DisconnectProc>("Disconnect");
            _Login = NativeLib.GetUnmanagedFunction<LoginProc>("Login");
            _SendOrder = NativeLib.GetUnmanagedFunction<SendOrderProc>("SendOrder");
            _SendOrderAction = NativeLib.GetUnmanagedFunction<SendOrderActionProc>("SendOrderAction");
            _QryInstrument = NativeLib.GetUnmanagedFunction<QryInstrumentProc>("QryInstrument");
            //_Restore = NativeLib.GetUnmanagedFunction<RestoreProc>("Restore");
            _QryAccountInfo = NativeLib.GetUnmanagedFunction<QryAccountInfoProc>("QryAccountInfo");
            _QryOrder = NativeLib.GetUnmanagedFunction<QryOrderProc>("QryOrder");
            _QryTrade = NativeLib.GetUnmanagedFunction<QryTradeProc>("QryTrade");
            _QryPositionDetail = NativeLib.GetUnmanagedFunction<QryPositionDetailProc>("QryPositionDetail");
            _Withdraw = NativeLib.GetUnmanagedFunction<WithdrawProc>("Withdraw");
            _Deposit = NativeLib.GetUnmanagedFunction<DepositProc>("Deposit");

            _RegOnConnected = NativeLib.GetUnmanagedFunction<RegOnConnectedProc>("RegOnConnected");
            _RegOnDisconnected = NativeLib.GetUnmanagedFunction<RegOnDisconnectedProc>("RegOnDisconnected");
            _RegOnLogin = NativeLib.GetUnmanagedFunction<RegOnLoginProc>("RegOnLogin");
            _RegRtnTrade = NativeLib.GetUnmanagedFunction<RegRtnTradeProc>("RegRtnTrade");
            _RegRtnOrder = NativeLib.GetUnmanagedFunction<RegRtnOrderProc>("RegRtnOrder");
            _RegRtnOrderError = NativeLib.GetUnmanagedFunction<RegRtnOrderErrorProc>("RegRtnOrderError");
            _RegRtnOrderActionError = NativeLib.GetUnmanagedFunction<RegRtnOrderActionErrorProc>("RegRtnOrderActionError");
            _RegOnSymbol = NativeLib.GetUnmanagedFunction<RegOnSymbolProc>("RegOnSymbol");
            _RegOnAccountInfo = NativeLib.GetUnmanagedFunction<RegOnAccountInfoProc>("RegOnAccountInfo");
            _RegOnQryOrder = NativeLib.GetUnmanagedFunction<RegOnQryOrderProc>("RegOnQryOrder");
            _RegOnQryTrade = NativeLib.GetUnmanagedFunction<RegOnQryTradeProc>("RegOnQryTrade");
            _RegOnQryPositionDetail = NativeLib.GetUnmanagedFunction<RegOnQryPositionDetailProc>("RegOnPositionDetail");
            _RegOnLog = NativeLib.GetUnmanagedFunction<RegOnLogProc>("RegOnLog");
            _RegOnMessage = NativeLib.GetUnmanagedFunction<RegOnMessageProc>("RegOnMessage");
            _RegOnTransfer = NativeLib.GetUnmanagedFunction<RegOnTransferProc>("RegOnTransfer");
        }



        /// <summary>
        /// 创建BrokerWrapper调用
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr CreateBrokerWrapperProc();
        CreateBrokerWrapperProc _CreateBrokerWrapper;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr DestoryBrokerWrapperProc(IntPtr pWrapper);
        DestoryBrokerWrapperProc _DestoryBrokerWrapper;

        /// <summary>
        /// 注册具体的Broker调用
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegisterProc(IntPtr pWrapper,IntPtr pBroker);
        RegisterProc _Register;
        public void Register(TLBrokerProxy brokerproxy)
        {
            try
            {
                logger.Info("BrokerProxy Register TLBroker");
                _Register(this.Wrapper, brokerproxy.Handle);
            }
            catch (Exception ex)
            {
                logger.Error("Register BrokerProxy Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 连接服务器调用
        /// </summary>
        /// <param name="pServerInfo"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void ConnectProc(IntPtr pWrapper, ref XServerInfoField pServerInfo);
        ConnectProc _Connect;
        public void Connect(ref XServerInfoField pServerInfo)
        {
            try
            {
                logger.Info("BrokerProxy Connect");
                _Connect(this.Wrapper, ref pServerInfo);
            }
            catch (Exception ex)
            {
                logger.Error("Connect Broker Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 断开服务端连接
        /// </summary>
        /// <param name="pWrappero"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void DisconnectProc(IntPtr pWrapper);
        DisconnectProc _Disconnect;
        public void Disconnect()
        {
            try
            {
                logger.Info("BrokerProxy Disconnect");
                _Disconnect(this.Wrapper);
            }
            catch (Exception ex)
            {
                logger.Error("Disconnect Broker Error:" + ex.ToString());
            }
        }


        /// <summary>
        /// 登入操作
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="pUserInfo"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void LoginProc(IntPtr pWrapper,ref XUserInfoField pUserInfo);
        LoginProc _Login;
        public void Login(ref XUserInfoField pUserInfo)
        {
            try
            {
                logger.Info("BrokerProxy Login");
                _Login(this.Wrapper, ref pUserInfo);
            }
            catch (Exception ex)
            {
                logger.Error("Login Error:" + ex.ToString());
            }
        }

        /// <summary>
        /// 提交委托
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="pOrder"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool SendOrderProc(IntPtr pWrapper, ref XOrderField pOrder);
        SendOrderProc _SendOrder;
        public bool SendOrder(ref XOrderField pOrder)
        {
            try
            {
                logger.Info(string.Format("SendOrder ID:{0} {1} {2} {3} {4}@{5}", pOrder.ID, (pOrder.Side ? "Buy" : "Sell"), pOrder.OffsetFlag, Math.Abs(pOrder.TotalSize), pOrder.Symbol, pOrder.LimitPrice));
                return  _SendOrder(this.Wrapper, ref pOrder);
            }
            catch (Exception ex)
            {
                logger.Error("SendOrder Error:" + ex.ToString());
                return false;
            }
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool SendOrderActionProc(IntPtr pWrapper, ref XOrderActionField pAction);
        SendOrderActionProc _SendOrderAction;
        public bool SendOrderAction(ref XOrderActionField pAction)
        {
            try
            {
                logger.Info(string.Format("SendOrderAction Action:{0} LocalID:{1} RemoteID:{2}", pAction.ActionFlag, pAction.BrokerLocalOrderID, pAction.BrokerRemoteOrderID));
                return _SendOrderAction(this.Wrapper, ref pAction);
            }
            catch (Exception ex)
            {
                logger.Error("SendOrderAction Error:" + ex.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool QryInstrumentProc(IntPtr pWrapper);
        QryInstrumentProc _QryInstrument;
        public bool QryInstrument()
        {
            try
            {
                logger.Info("BrokerProxy QryInstrument");
                bool x =  _QryInstrument(this.Wrapper);
                if(!x)
                {
                    logger.Warn("have not synced symbol data");
                }
                //Util.Error("**************** qry instrument return:" + x.ToString());
                return x;
            }
            catch (Exception ex)
            {
                Util.Error("QryInstrument Error:" + ex.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool QryAccountInfoProc(IntPtr pWrapper);
        QryAccountInfoProc _QryAccountInfo;
        public bool QryAccountInfo()
        {
            try
            {
                logger.Info("BrokerProxy QryAccountInfo");
                bool x = _QryAccountInfo(this.Wrapper);
                //Util.Info("**************** qry accountinfo return:" + x.ToString());
                return x;
            }
            catch (Exception ex)
            {
                logger.Error("QryAccountInfo Error:" + ex.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool QryOrderProc(IntPtr pWrapper);
        QryOrderProc _QryOrder;
        public bool QryOrder()
        {
            try
            {
                logger.Info("BrokerProxy QryOrder");
                bool x = _QryOrder(this.Wrapper);
                //Util.Info("**************** QryOrder return:" + x.ToString());
                return x;
            }
            catch (Exception ex)
            {
                logger.Error("QryOrder Error:" + ex.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool QryTradeProc(IntPtr pWrapper);
        QryTradeProc _QryTrade;
        public bool QryTrade()
        {
            try
            {
                logger.Info("BrokerProxy QryTrade");
                bool x = _QryTrade(this.Wrapper);
                //Util.Info("**************** QryTrade return:" + x.ToString());
                return x;
            }
            catch (Exception ex)
            {
                logger.Error("QryTrade Error:" + ex.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool QryPositionDetailProc(IntPtr pWrapper);
        QryPositionDetailProc _QryPositionDetail;
        public bool QryPositionDetail()
        {
            try
            {
                logger.Info("BrokerProxy QryPositionDetail");
                bool x = _QryPositionDetail(this.Wrapper);
                //Util.Info("**************** QryPositionDetail return:" + x.ToString());
                return x;
            }
            catch (Exception ex)
            {
                logger.Error("QryPositionDetail Error:" + ex.ToString());
                return false;
            }
        }



        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool DepositProc(IntPtr pWrapper,ref XCashOperation pCashOperation);
        DepositProc _Deposit;
        public bool Deposit(ref XCashOperation pCashOperation)
        {
            try
            {
                logger.Info("BrokerProxy Deposit");
                bool x = _Deposit(this.Wrapper, ref pCashOperation);
                //Util.Info("**************** deposit return:" + x.ToString());
                return x;
            }
            catch (Exception ex)
            {
                logger.Error("_Deposit Error:" + ex.ToString());
                return false;
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool WithdrawProc(IntPtr pWrapper, ref XCashOperation pCashOperation);
        WithdrawProc _Withdraw;
        public bool Withdraw(ref XCashOperation pCashOperation)
        {
            try
            {
                logger.Info("BrokerProxy Withdraw");
                bool x = _Withdraw(this.Wrapper, ref pCashOperation);
                return x;
            }
            catch (Exception ex)
            {
                logger.Error("Withdraw Error:" + ex.ToString());
                return false;
            }
        }

        #region 注册回调函数接口
        /// <summary>
        /// 注册连接建立回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnConnectedProc(IntPtr pWrapper,CBOnConnected cb);
        RegOnConnectedProc _RegOnConnected;


        /// <summary>
        /// 注册连接建立回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnDisconnectedProc(IntPtr pWrapper, CBOnDisconnected cb);
        RegOnDisconnectedProc _RegOnDisconnected;

        /// <summary>
        /// 注册登入回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnLoginProc(IntPtr pWrapper, CBOnLogin cb);
        RegOnLoginProc _RegOnLogin;




        /// <summary>
        /// 注册成交回报回调
        /// </summary>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegRtnTradeProc(IntPtr pWrapper,CBRtnTrade cb);
        RegRtnTradeProc _RegRtnTrade;

        /// <summary>
        /// 注册委托回报回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegRtnOrderProc(IntPtr pWrapper, CBRtnOrder cb);
        RegRtnOrderProc _RegRtnOrder;

        /// <summary>
        /// 注册委托错误回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegRtnOrderErrorProc(IntPtr pWrapper, CBRtnOrderError cb);
        RegRtnOrderErrorProc _RegRtnOrderError;

        /// <summary>
        /// 注册委托操作错误回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegRtnOrderActionErrorProc(IntPtr pWrapper, CBRtnOrderActionError cb);
        RegRtnOrderActionErrorProc _RegRtnOrderActionError;

        /// <summary>
        /// 注册合约回调
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnSymbolProc(IntPtr pWrapper, CBOnSymbol cb);
        RegOnSymbolProc _RegOnSymbol;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnAccountInfoProc(IntPtr pWrapper, CBOnAccountInfo cb);
        RegOnAccountInfoProc _RegOnAccountInfo;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnQryOrderProc(IntPtr pWrapper, CBOnQryOrder cb);
        RegOnQryOrderProc _RegOnQryOrder;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnQryTradeProc(IntPtr pWrapper, CBOnQryTrade cb);
        RegOnQryTradeProc _RegOnQryTrade;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnQryPositionDetailProc(IntPtr pWrapper, CBOnQryPositionDetail cb);
        RegOnQryPositionDetailProc _RegOnQryPositionDetail;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnLogProc(IntPtr pWrapper, CBOnLog cb);
        RegOnLogProc _RegOnLog;


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnMessageProc(IntPtr pWrapper, CBOnMessage cb);
        RegOnMessageProc _RegOnMessage;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegOnTransferProc(IntPtr pWrapper, CBOnTransfer cb);
        RegOnTransferProc _RegOnTransfer;

        #endregion


        #region 回调事件

        CBOnTransfer cbOnTransfer;
        public event CBOnTransfer OnTransferEvent
        {
            add { cbOnTransfer += value; _RegOnTransfer(this.Wrapper, cbOnTransfer); }
            remove { cbOnTransfer -= value; _RegOnTransfer(this.Wrapper, cbOnTransfer); }
        }


        CBOnMessage cbOnMessage;
        public event CBOnMessage OnMessageEvent
        {
            add { cbOnMessage += value; _RegOnMessage(this.Wrapper, cbOnMessage); }
            remove { cbOnMessage -= value; _RegOnMessage(this.Wrapper, cbOnMessage); }
        }

        CBOnLog cbOnLog;
        public event CBOnLog OnLogEvent
        {
            add { cbOnLog += value; _RegOnLog(this.Wrapper, cbOnLog); }
            remove { cbOnLog -= value; _RegOnLog(this.Wrapper, cbOnLog); }
        }

        CBOnConnected cbBOnConnected;
        public event CBOnConnected OnConnectedEvent
        {
            add { cbBOnConnected += value; _RegOnConnected(this.Wrapper, cbBOnConnected); }
            remove { cbBOnConnected -= value; _RegOnConnected(this.Wrapper, cbBOnConnected); }
        }

        CBOnDisconnected cbOnDisconnected;
        public event CBOnDisconnected OnDisconnectedEvent
        {
            add { cbOnDisconnected += value; _RegOnDisconnected(this.Wrapper, cbOnDisconnected); }
            remove { cbOnDisconnected -= value; _RegOnDisconnected(this.Wrapper, cbOnDisconnected); }
        }

        CBOnLogin cbOnLogin;
        public event CBOnLogin OnLoginEvent
        {
            add { cbOnLogin += value; _RegOnLogin(this.Wrapper, cbOnLogin); }
            remove { cbOnLogin -= value; _RegOnLogin(this.Wrapper, cbOnLogin); }
        }

        CBRtnTrade cbRtnTrade;
        public event CBRtnTrade OnRtnTradeEvent
        {
            add{cbRtnTrade += value;_RegRtnTrade(this.Wrapper,cbRtnTrade);}
            remove { cbRtnTrade -= value; _RegRtnTrade(this.Wrapper, cbRtnTrade); }
        }

        CBRtnOrder cbRtnOrder;
        public event CBRtnOrder OnRtnOrderEvent
        {
            add { cbRtnOrder += value; _RegRtnOrder(this.Wrapper, cbRtnOrder); }
            remove { cbRtnOrder -= value; _RegRtnOrder(this.Wrapper, cbRtnOrder); }
        }

        CBRtnOrderError cbRtnOrderError;
        public event CBRtnOrderError OnRtnOrderErrorEvent
        {
            add { cbRtnOrderError += value; _RegRtnOrderError(this.Wrapper, cbRtnOrderError); }
            remove { cbRtnOrderError -= value; _RegRtnOrderError(this.Wrapper, cbRtnOrderError); }
        }

        CBRtnOrderActionError cbRtnOrderActionError;
        public event CBRtnOrderActionError OnRtnOrderActionErrorEvent
        {
            add { cbRtnOrderActionError += value; _RegRtnOrderActionError(this.Wrapper, cbRtnOrderActionError); }
            remove { cbRtnOrderActionError -= value; _RegRtnOrderActionError(this.Wrapper, cbRtnOrderActionError); }
        }

        CBOnSymbol cbOnSymbol;
        public event CBOnSymbol OnSymbolEvent
        {
            add { cbOnSymbol += value; _RegOnSymbol(this.Wrapper, cbOnSymbol); }
            remove { cbOnSymbol -= value; _RegOnSymbol(this.Wrapper, cbOnSymbol); }
        }

        /// <summary>
        /// 帐户信息回报事件
        /// </summary>
        CBOnAccountInfo cbOnAccountInfo;
        public event CBOnAccountInfo OnAccountInfoEvent
        {
            add { cbOnAccountInfo += value; _RegOnAccountInfo(this.Wrapper, cbOnAccountInfo); }
            remove { cbOnAccountInfo -= value; _RegOnAccountInfo(this.Wrapper, cbOnAccountInfo); }
        }

        CBOnQryOrder cbOnQryOrder;
        public event CBOnQryOrder OnQryOrderEvent
        {
            add { cbOnQryOrder += value; _RegOnQryOrder(this.Wrapper, cbOnQryOrder); }
            remove { cbOnQryOrder -= value; _RegOnQryOrder(this.Wrapper, cbOnQryOrder); }
        }

        CBOnQryTrade cbOnQryTrade;
        public event CBOnQryTrade OnQryTradeEvent
        {
            add { cbOnQryTrade += value; _RegOnQryTrade(this.Wrapper, cbOnQryTrade); }
            remove { cbOnQryTrade -= value; _RegOnQryTrade(this.Wrapper, cbOnQryTrade); }
        }

        CBOnQryPositionDetail cbOnQryPositionDetail;
        public event CBOnQryPositionDetail OnQryPositionDetailEvent
        {
            add { cbOnQryPositionDetail += value; _RegOnQryPositionDetail(this.Wrapper, cbOnQryPositionDetail); }
            remove { cbOnQryPositionDetail -= value; _RegOnQryPositionDetail(this.Wrapper, cbOnQryPositionDetail); }
        }
        #endregion
    }
}
