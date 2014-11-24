using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TradingLib.API;
using TradingLib.Common;

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
            Util.DestoryStatus("TLBrokerWrapperProxy");
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
        public TLBrokerWrapperProxy(string path, string dllname)
        {
            //1.加载dll
            NativeLib = new UnmanagedLibrary(path, dllname);
            //2.绑定导出函数到委托
            AssignCommonDelegates();

            //3:创建对应的handle
            _Wrapper = _CreateBrokerWrapper();
            //绑定事件
            //_RegRtnTrade(this.Wrapper, FireRtnTrade);
            
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

            _RegOnConnected = NativeLib.GetUnmanagedFunction<RegOnConnectedProc>("RegOnConnected");
            _RegOnDisconnected = NativeLib.GetUnmanagedFunction<RegOnDisconnectedProc>("RegOnDisconnected");
            _RegOnLogin = NativeLib.GetUnmanagedFunction<RegOnLoginProc>("RegOnLogin");
            _RegRtnTrade = NativeLib.GetUnmanagedFunction<RegRtnTradeProc>("RegRtnTrade");
            _RegRtnOrder = NativeLib.GetUnmanagedFunction<RegRtnOrderProc>("RegRtnOrder");
            _RegRtnOrderError = NativeLib.GetUnmanagedFunction<RegRtnOrderErrorProc>("RegRtnOrderError");

            //_reCBOnConnected = FireOnConnected;
            //_reOnRtnOrderErrorEvent = FireRtnOrderError;
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
            _Register(this.Wrapper, brokerproxy.Handle);

            //注册完毕具体的broker对象后 绑定事件注意 直接用函数名来进行绑定会造成回调函数被回收导致c++调用回调时报错 要用原始的事件声明方式
            //_RegOnConnected(this.Wrapper, FireOnConnected);
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
            _Connect(this.Wrapper, ref pServerInfo);
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
            _Disconnect(this.Wrapper);
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
            _Login(this.Wrapper,ref pUserInfo);
        }

        /// <summary>
        /// 提交委托
        /// </summary>
        /// <param name="pWrapper"></param>
        /// <param name="pOrder"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate string SendOrderProc(IntPtr pWrapper, ref XOrderField pOrder);
        SendOrderProc _SendOrder;
        public string SendOrder(ref XOrderField pOrder)
        {
            Util.Debug("BrokerProxy SendOrder",QSEnumDebugLevel.MUST);
            return _SendOrder(this.Wrapper, ref pOrder);
        }


        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool SendOrderActionProc(IntPtr pWrapper, ref XOrderActionField pAction);
        SendOrderActionProc _SendOrderAction;
        public bool SendOrderAction(ref XOrderActionField pAction)
        {
            Util.Debug("BrokerProxy SendOrderAction",QSEnumDebugLevel.MUST);
            return _SendOrderAction(this.Wrapper, ref pAction);
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
        #endregion


        #region 回调事件
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
        #endregion




        ///// <summary>
        ///// 字符串参数调用
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate string DemoStringCallProc(string input);
        //public DemoStringCallProc demostringcall;

        ///// <summary>
        ///// 整形参数调用
        ///// </summary>
        ///// <param name="x"></param>
        ///// <returns></returns>
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate int DemoIntCallProc(int x);
        //public DemoIntCallProc demointcall;

        ///// <summary>
        ///// 结构体参数调用
        ///// </summary>
        ///// <param name="error"></param>
        //[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        //public delegate void DemoStructCallProc(ref  ErrorField error);
        //public DemoStructCallProc demostructcall;
    }
}
