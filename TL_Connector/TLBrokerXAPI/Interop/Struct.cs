using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


/* 关于互通结构体的说明
 * 1.结构体排序规则设定为Sequential
 * 2.int double decimal long 等基本数据类型要与c++的长度一致
 * 3.字符串可以使用string类型 但是需要指定sizeconst，错误的长度不会影响数据的获取，比如c++中设定20位,传递过去为50位
 * 打印输出可以正常输出50位 应为字符串是以某个地址开始寻找\0结尾。因此超出长度部分仍然可以正常读取
 * 不过下一个字段就会出现无法读取或者获得数值异常。因此sizecount更多的作用是规范内存布局,确定变量开始的地址偏移，因此结构
 * 体中的字段长度需要与c++严格一致
 * 
 * 
 * 
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.BrokerXAPI.Interop
{
    /// <summary>
    /// 错误消息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XErrorField
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorID;
        /// <summary>
        /// 错误信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string ErrorMsg;

    }

    /// <summary>
    /// 服务端连接信息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XServerInfoField
    {
        /// <summary>
        /// 服务地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string ServerAddress;

        /// <summary>
        /// 服务端口
        /// </summary>
        public int ServerPort;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field3;

    }

    /// <summary>
    /// 登入信息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XUserInfoField
    {
        /// <summary>
        /// 登入用户名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string UserID;

        /// <summary>
        /// 登入密码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 40)]
        public string Password;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field2;

    }


    /// <summary>
    /// 委托结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XOrderField
    {
        /// <summary>
        /// 日期
        /// </summary>
        public int Date;

        /// <summary>
        /// 时间
        /// </summary>
        public int Time;

        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Symbol;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Exchange;

        /// <summary>
        /// 方向
        /// </summary>
        public bool Side;

        /// <summary>
        /// 委托数量
        /// </summary>
        public int TotalSize;

        /// <summary>
        /// 成交数量
        /// </summary>
        public int FilledSize;

        /// <summary>
        /// 未成交数量
        /// </summary>
        public int UnfilledSize;

        /// <summary>
        /// limit价格
        /// </summary>
        public double LimitPrice;

        /// <summary>
        /// stop价格
        /// </summary>
        public double StopPrice;

        /// <summary>
        /// 开平标识
        /// </summary>
        public QSEnumOffsetFlag OffsetFlag;


        /// <summary>
        /// 委托状态
        /// </summary>
        public QSEnumOrderStatus OrderStatus;

        /// <summary>
        /// 委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string OrderID;
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XTradeField
    {
        public int Date;

        public int Time;

        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Symbol;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Exchange;

        /// <summary>
        /// 方向
        /// </summary>
        public bool Side;

        /// <summary>
        /// 成交手数
        /// </summary>
        public int Size;


        /// <summary>
        /// 成交价格
        /// </summary>
        public double Price;

        /// <summary>
        /// 开平标识
        /// </summary>
        public QSEnumOffsetFlag OffsetFlag;

        /// <summary>
        /// 手续费
        /// </summary>
        public double Commission;

        /// <summary>
        /// 成交编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string TradeID;
    }







    /// <summary>
    /// 成交回调委托
    /// </summary>
    /// <param name="pTrade"></param>
    public delegate void CBRtnTrade(ref XTradeField pTrade);
}
