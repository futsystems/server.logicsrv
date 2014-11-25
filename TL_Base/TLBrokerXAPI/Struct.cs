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
 * 关于本地交易帐户委托与接口侧委托的关系
 * 如果本地委托与接口侧委托是一对一的关系,则可以通过扩展原有委托字段来实现信息的共同存储，由于我们想设计成智能路由模式,即存在拆单的问题
 * 则Broker侧的委托 需要重新构建,即将本地帐户的委托映射成多条Broker侧的委托
 * 
 * Broker侧的委托 需要抽象出来形成统一的委托形式
 * 
 * BrokerOrder
 * ID,Date,Time,Token,Side,Size,Symbol,Exchange,LimitPrice,StopPrice,TrailPrice,TIF,OrderType,TotalSize,FilledSize,UnFilledSize,LocalID,RemoteID
 * 
 * 
 * 
 * */
namespace TradingLib.BrokerXAPI
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
    /// 登入回报结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XRspUserLoginField
    {
        /// <summary>
        /// 当前交易日
        /// </summary>
        public int Tradingday;

        /// <summary>
        /// 登入用户名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string UserID;

        /// <summary>
        /// 预留字段1
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field1;

        /// <summary>
        /// 预留字段2
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string Field2;

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
        /// 委托状态消息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string StatusMsg;


        /// <summary>
        /// 系统唯一委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string ID;

        /// <summary>
        /// 向远端发单时 生成的本地OrderRef 比如CTP 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerLocalOrderID;

        /// <summary>
        /// 远端交易所返回的编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerRemoteOrderID;
    }
    
    /// <summary>
    /// 委托操作
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XOrderActionField
    {
        /// <summary>
        /// 本地系统委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string ID;

        /// <summary>
        /// 相对于成交端 本地编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerLocalOrderID;


        /// <summary>
        /// 交易所委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerRemoteOrderID;


        /// <summary>
        /// 委托操作标识
        /// </summary>
        public QSEnumOrderActionFlag ActionFlag;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string Exchange;

        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string Symbol;

        /// <summary>
        /// 未成交数量
        /// </summary>
        public int Size;


        /// <summary>
        /// 价格
        /// </summary>
        public double Price;


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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerTradeID;

        /// <summary>
        /// 近端委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerLocalOrderID;

        /// <summary>
        /// 远端委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
        public string BrokerRemoteOrderID;
    }




    public class XOrderError
    {
        public XOrderError(XOrderField order, XErrorField error)
        {
            this.Order = order;
            this.Error = error;
        }
        public XOrderField Order;
        public XErrorField Error;
    }

    public class XOrderActionError
    {
        public XOrderActionError(XOrderActionField orderAction, XErrorField error)
        {
            this.OrderAction = orderAction;
            this.Error = error;
        }

        public XOrderActionField OrderAction;
        public XErrorField Error;
    }
}
