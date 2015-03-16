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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string ServerAddress;

        /// <summary>
        /// 服务端口
        /// </summary>
        public int ServerPort;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Field1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Field2;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
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

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Field1;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
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
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
        public string Field1;

        /// <summary>
        /// 预留字段2
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 100)]
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
    /// .net mono 默认对齐是2字节对齐
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XOrderField
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
        //36
        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Symbol;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Exchange;



        /// <summary>
        /// 委托状态消息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string StatusMsg;


        /// <summary>
        /// 系统唯一委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ID;

        /// <summary>
        /// 向远端发单时 生成的本地OrderRef 比如CTP 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BrokerLocalOrderID;

        /// <summary>
        /// 远端交易所返回的编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
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

        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;

    }
    
    /// <summary>
    /// 委托操作
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XOrderActionField
    {
        /// <summary>
        /// 未成交数量
        /// </summary>
        public int Size;//4

        /// <summary>
        /// 价格
        /// </summary>
        public double Price;//8

        /// <summary>
        /// 本地系统委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string ID;

        /// <summary>
        /// 相对于成交端 本地编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BrokerLocalOrderID;

        /// <summary>
        /// 交易所委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BrokerRemoteOrderID;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Exchange;

        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Symbol;

        /// <summary>
        /// 委托操作标识
        /// </summary>
        public QSEnumOrderActionFlag ActionFlag;//1


    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XTradeField
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
        /// 成交手数
        /// </summary>
        public int Size;//4

        /// <summary>
        /// 成交价格
        /// </summary>
        public double Price;//8

        /// <summary>
        /// 手续费
        /// </summary>
        public double Commission;//8

        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Symbol;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Exchange;

        /// <summary>
        /// 成交编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BrokerTradeID;

        /// <summary>
        /// 近端委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BrokerLocalOrderID;

        /// <summary>
        /// 远端委托编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string BrokerRemoteOrderID;

        /// <summary>
        /// 开平标识
        /// </summary>
        public QSEnumOffsetFlag OffsetFlag;//1

        /// <summary>
        /// 方向
        /// </summary>
        public bool Side;

        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
    }

    /// <summary>
    /// 交易帐户信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XAccountInfo
    {
        /// <summary>
        /// 昨日权益
        /// </summary>
        public double LastEquity;

        /// <summary>
        /// 入金
        /// </summary>
        public double Deposit;

        /// <summary>
        /// 出金
        /// </summary>
        public double WithDraw;

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public double ClosePorifit;

        /// <summary>
        /// 持仓盈亏
        /// </summary>
        public double PositoinProfit;

        /// <summary>
        /// 手续费
        /// </summary>
        public double Commission;
    }


    /// <summary>
    /// 合约结构体 用于传递合约信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct XSymbol
    {
        /// <summary>
        /// 合约
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string Symbol;

        /// <summary>
        /// 交易所
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string Exchange;

        /// <summary>
        /// 品种代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string SecurityCode;


        /// <summary>
        /// 品种类型
        /// </summary>
        public SecurityType SecurityType;

        /// <summary>
        /// 行权方向
        /// </summary>
        public QSEnumOptionSide OptionSide;

        /// <summary>
        /// 执行价
        /// </summary>
        public double StrikePrice;

        /// <summary>
        /// 保证金 额度或比例
        /// </summary>
        public double Margin;

        /// <summary>
        /// 开仓手续费
        /// </summary>
        public double EntryCommission;

        /// <summary>
        /// 平仓手续费
        /// </summary>
        public double ExitCommission;

        /// <summary>
        /// 平今手续费
        /// </summary>
        public double ExitTodayCommission;

        /// <summary>
        /// 到期日
        /// </summary>
        public int ExpireDate;
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

    public class XHistOrder
    {
        public XHistOrder(XOrderField order, bool islast)
        {
            this.Order = order;
            this.IsLast = islast;
        }

        public XOrderField Order;
        public bool IsLast;
    }

    public class XHistTrade
    {
        public XHistTrade(XTradeField trade, bool islast)
        {
            this.Trade = trade;
            this.IsLast = islast;
        }

        public XTradeField Trade;
        public bool IsLast;
    }


}
