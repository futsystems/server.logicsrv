using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using TradingLib.ServiceManager;
using TradingLib.API;
using TradingLib.Common;
using System.IO;
using System.Text;
using TradingLib.BrokerXAPI;

using System.Runtime.InteropServices;

namespace TraddingSrvCLI
{
    /// <summary>
    /// 委托结构体
    /// .net mono 默认对齐是2字节对齐
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XOrderField00
    {
        /// <summary>
        /// 日期
        /// </summary>
        public int Date;//4

        /// <summary>
        /// 时间
        /// </summary>
        public int Time;//4

        /// <summary>
        /// 委托数量
        /// </summary>
        public int TotalSize;//4

        /// <summary>
        /// 成交数量
        /// </summary>
        public int FilledSize;//4

        /// <summary>
        /// 未成交数量
        /// </summary>
        public int UnfilledSize;//4

        /// <summary>
        /// limit价格
        /// </summary>
        public double LimitPrice;//8

        /// <summary>
        /// stop价格
        /// </summary>
        public double StopPrice;//8
        ////36
        ///// <summary>
        ///// 合约
        ///// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Symbol;

        ///// <summary>
        ///// 交易所
        ///// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Exchange;



        ///// <summary>
        ///// 委托状态消息
        ///// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string StatusMsg;


        /// <summary>
        /// 系统唯一委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string ID;

        /// <summary>
        /// 向远端发单时 生成的本地OrderRef 比如CTP 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string BrokerLocalOrderID;

        /// <summary>
        /// 远端交易所返回的编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string BrokerRemoteOrderID;

        /// <summary>
        /// 开平标识
        /// </summary>
        public QSEnumOffsetFlag OffsetFlag;//1

        /// <summary>
        /// 委托状态
        /// </summary>
        public QSEnumOrderStatus OrderStatus;//1

        /// <summary>
        /// 方向 //400
        /// </summary>
        public bool Side;//1
    }

    class Program
    {

        const string PROGRAME = "LogicSrv";

        static void debug(string message)
        {
            Console.WriteLine(message);
        }

        static void printleft(string msg, string msg1)
        {
            int len = Console.LargestWindowWidth / 2;
            int len2 = (len - msg.Length);
            string s =msg +msg1.PadLeft(len2-1);
            Console.WriteLine(s);
        }
        static void Main(string[] args)
        {
            debug("intsize:" + sizeof(int).ToString() + " doublesize:" + sizeof(double).ToString() + " boolsize:" + sizeof(bool).ToString());
            debug("OrderSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderField)).ToString());
            debug("TradeSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XTradeField)).ToString());
            debug("ErrorSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XErrorField)).ToString());
            //XServerInfoField
            //debug("XServerInfoFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XServerInfoField)).ToString());
            ////XUserInfoField
            debug("XUserInfoFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XUserInfoField)).ToString());
            debug("XRspUserLoginFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XRspUserLoginField)).ToString());
            debug("XOrderActionFieldSize:" + System.Runtime.InteropServices.Marshal.SizeOf(typeof(XOrderActionField)).ToString());
            //return;
            //Util.Debug("Orders:" +TradingLib.Mixins.LitJson.JsonMapper.ToJson(new XOrderField()),QSEnumDebugLevel.WARNING);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            try
            {
                debug("********* start core daemon *********");
                CoreDaemon cd = new CoreDaemon();
                //cd.SendDebugEvent +=new DebugDelegate(debug);
                //启动核心守护
                cd.Start();
                
            }
            catch (Exception ex)
            {
                debug("error:" + ex.ToString());
                Util.Debug(ex.ToString() + ex.StackTrace.ToString());
            }
        }

 
       
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = (Exception)e.ExceptionObject;
            Util.Debug(ex.ToString());
        }
    }

}
