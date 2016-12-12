//////////////////////////////////////////////////////////////////////////
/// 定义了客户端接口使用的业务数据结构
///
/////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace TradingLib.XLProtocol.V1
{
    #region 请求登入
    /// <summary>
    /// 登入请求
    /// </summary>
    public struct XLReqLoginField : IXLField
    {
        /// <summary>
        /// 账户编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;

        /// <summary>
        /// 密码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string Password;

        /// <summary>
        /// 用户端产品信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string UserProductInfo;

        /// <summary>
        /// Mac地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string MacAddress;

        /// <summary>
        /// 终端IP地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 10)]
        public string ClientIPAddress;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_REQ_LOGIN; } }
    }

    /// <summary>
    /// 登入响应
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLRspLoginField : IXLField
    {
        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 姓名
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string Name;


        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_LOGIN; } }
    }

    #endregion

    #region 更新密码
    /// <summary>
    /// 用户口令变更
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLReqUserPasswordUpdateField : IXLField
    {
        /// <summary>
        /// 原来的口令
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string OldPassword;
        /// <summary>
        /// 新的口令
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string NewPassword;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_REQ_UPDATEPASS; } }
    }

    /// <summary>
    /// 用户口令变更
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLRspUserPasswordUpdateField : IXLField
    {
        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_UPDATEPASS; } }
    }
    #endregion

    #region 查询合约
    /// <summary>
    /// 查询合约
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLQrySymbolField : IXLField
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 产品代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SecurityID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_SYMBOL; } }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLSymbolField : IXLField
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 合约名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string SymbolName;

        /// <summary>
        /// 产品代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SecurityID;

        /// <summary>
        /// 产品类型
        /// </summary>
        public XLSecurityType SecurityType;

        /// <summary>
        /// 合约乘数
        /// </summary>
        public int Multiple;

        /// <summary>
        /// 最小变动价位
        /// </summary>
        public double PriceTick;

        /// <summary>
        /// 到期日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExpireDate;

        /// <summary>
        /// 计价货币
        /// </summary>
        public XLCurrencyType Currency;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_SYMBOL; } }

       
    }
    #endregion

    #region 查询委托
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLQryOrderField : IXLField
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_ORDER; } }
    }

    /// <summary>
    /// 报单
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLOrderField : IXLField
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;

        /// <summary>
        /// 报单日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string Date;

        /// <summary>
        /// 委托时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string Time;

        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string UserID;

        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;

        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;

        /// <summary>
        /// 价格
        /// </summary>
        public double LimitPrice;

        /// <summary>
        /// 止损价
        /// </summary>
        public double StopPrice;

        /// <summary>
        /// 报单类别
        /// </summary>
        public XLOrderType OrderType;

        /// <summary>
        /// 总数量
        /// </summary>
        public int VolumeTotal;

        /// <summary>
        /// 成交数量
        /// </summary>
        public int VolumeFilled;

        /// <summary>
        /// 未成交数量
        /// </summary>
        public int VolumeUnfilled;

        /// <summary>
        /// 买卖方向
        /// </summary>
        public XLDirectionType Direction;

        /// <summary>
        /// 开平标志
        /// </summary>
        public XLOffsetFlagType OffsetFlag;

        /// <summary>
        /// 组合投机套保标志
        /// </summary>
        public XLHedgeFlagType HedgeFlag;

       

        

        /// <summary>
        /// 本地报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderRef;

        /// <summary>
        /// 远端报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID;

        /// <summary>
        /// 柜台委托全局编号
        /// </summary>
        public long OrderID;

        /// <summary>
        /// 报单状态
        /// </summary>
        public XLOrderStatus OrderStatus;

        /// <summary>
        /// 委托状态消息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string StatusMsg;

        /// <summary>
        /// 用户强评标志
        /// </summary>
        public int ForceClose;

        /// <summary>
        /// 强平原因
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string ForceCloseReason;



        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_ORDER; } }
    }

    #endregion

    #region 查询成交
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLQryTradeField : IXLField
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 成交编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string TradeID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_TRADE; } }
    }

    /// <summary>
    /// 成交
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLTradeField : IXLField
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;

        /// <summary>
        /// 成交时期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string Date;

        /// <summary>
        /// 成交时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string Time;

        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string UserID;


        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;

        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;

        /// <summary>
        /// 报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderRef;

        /// <summary>
        /// 报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;

        /// <summary>
        /// 成交编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string TradeID;

        /// <summary>
        /// 买卖方向
        /// </summary>
        public XLDirectionType Direction;

        /// <summary>
        /// 开平标志
        /// </summary>
        public XLOffsetFlagType OffsetFlag;

        /// <summary>
        /// 投机套保标志
        /// </summary>
        public XLHedgeFlagType HedgeFlag;

        /// <summary>
        /// 价格
        /// </summary>
        public double Price;

        /// <summary>
        /// 数量
        /// </summary>
        public int Volume;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_TRADE; } }
    }
    #endregion 

    #region 查询持仓
    /// <summary>
    /// 查询投资者持仓
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLQryPositionField : IXLField
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_POSITION; } }
    }

    /// <summary>
    /// 投资者持仓
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLPositionField : IXLField
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;

        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string UserID;

        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;

        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;

        /// <summary>
        /// 持仓多空方向
        /// </summary>
        public XLPosiDirectionType PosiDirection;

        /// <summary>
        /// 投机套保标志
        /// </summary>
        public XLHedgeFlagType HedgeFlag;

        /// <summary>
        /// 上日持仓
        /// </summary>
        public int YdPosition;

        /// <summary>
        /// 今日持仓
        /// </summary>
        public int Position;

        /// <summary>
        /// 今日持仓
        /// </summary>
        public int TodayPosition;

        /// <summary>
        /// 开仓量
        /// </summary>
        public int OpenVolume;

        /// <summary>
        /// 平仓量
        /// </summary>
        public int CloseVolume;

        /// <summary>
        /// 开仓成本
        /// </summary>
        public double OpenCost;

        /// <summary>
        /// 持仓成本
        /// </summary>
        public double PositionCost;

        /// <summary>
        /// 占用的保证金
        /// </summary>
        public double Margin;

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public double CloseProfit;

        /// <summary>
        /// 持仓盈亏
        /// </summary>
        public double PositionProfit;

        /// <summary>
        /// 手续费
        /// </summary>
        public double Commission;


        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_POSITION; } }

       
    }
    #endregion

    #region 查询账户资金
    #endregion

    #region 查询最大报单数量
    #endregion

    #region 提交委托
    #endregion

    #region 提交委托操作
    #endregion

    #region 委托回报
    #endregion

    #region 成交回报
    #endregion

    #region 持仓更新回报
    #endregion


    #region 订阅行情
    #endregion

    #region 查询分时
    #endregion

    #region 查询K线
    #endregion






}
