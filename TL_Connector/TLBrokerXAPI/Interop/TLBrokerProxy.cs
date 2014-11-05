using System;
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
    /// 某个具体的Broker的封装Proxy
    /// </summary>
    public class TLBrokerProxy
    {
        private  readonly UnmanagedLibrary NativeLib;

        public IntPtr Handle { get { return _Broker; } }

        IntPtr _Broker = IntPtr.Zero;
        public TLBrokerProxy(string path, string dllname)
        {
            //1.加载dll
            NativeLib = new UnmanagedLibrary(path, dllname);
            //2.绑定导出函数到委托
            AssignCommonDelegates();

            //3.创建对应的Broker
            _Broker = CreateBroker();
        }

        /// <summary>
        /// 绑定dll导出的函数到对应的委托对象
        /// </summary>
        private  void AssignCommonDelegates()
        {
            CreateBroker = NativeLib.GetUnmanagedFunction<CreateBrokerProc>("CreateBroker");
        }

        /// <summary>
        /// 无参数调用
        /// </summary>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr CreateBrokerProc();
        public CreateBrokerProc CreateBroker;

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
