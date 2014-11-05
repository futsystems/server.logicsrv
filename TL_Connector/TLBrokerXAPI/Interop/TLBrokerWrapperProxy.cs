﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

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
    public class TLBrokerWrapperProxy
    {
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
            _Register = NativeLib.GetUnmanagedFunction<RegisterProc>("Register");
            _Connect = NativeLib.GetUnmanagedFunction<ConnectProc>("Connect");
            _Disconnect = NativeLib.GetUnmanagedFunction<DisconnectProc>("Disconnect");
            _Login = NativeLib.GetUnmanagedFunction<LoginProc>("Login");
            _SendOrder = NativeLib.GetUnmanagedFunction<SendOrderProc>("SendOrder");
            _RegRtnTrade = NativeLib.GetUnmanagedFunction<RegRtnTradeProc>("RegRtnTrade");
        }



        /// <summary>
        /// 创建BrokerWrapper调用
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr CreateBrokerWrapperProc();
        CreateBrokerWrapperProc _CreateBrokerWrapper;

        /// <summary>
        /// 注册具体的Broker调用
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegisterProc(IntPtr pWrapper,IntPtr pBroker);
        RegisterProc _Register;
        public void Register(IntPtr pBroker)
        {
            _Register(this.Wrapper, pBroker);
            //注册完毕具体的broker对象后 绑定事件
            Console.WriteLine("register broker and then bind event");
            _RegRtnTrade(this.Wrapper, FireRtnTrade);
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
        public delegate void SendOrderProc(IntPtr pWrapper, ref XOrderField pOrder);
        SendOrderProc _SendOrder;
        public void SendOrder(ref XOrderField pOrder)
        {
            _SendOrder(this.Wrapper, ref pOrder);
        }

        /// <summary>
        /// 注册成交回报回调
        /// </summary>
        /// <param name="cb"></param>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void RegRtnTradeProc(IntPtr pWrapper,CBRtnTrade cb);
        RegRtnTradeProc _RegRtnTrade;



        #region 回调事件
        public event CBRtnTrade OnRtnTradeEvent;
        /// <summary>
        /// 触发成交回调事件,其余事件监听者绑定到OnRtnTradeEvent事件
        /// 将触发函数指针绑定到c++对应的回调对象
        /// </summary>
        /// <param name="pTrade"></param>
        void FireRtnTrade(ref XTradeField pTrade)
        {
            //Console.WriteLine("it is here for trade event");
            if (OnRtnTradeEvent != null)
                OnRtnTradeEvent(ref pTrade);
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
