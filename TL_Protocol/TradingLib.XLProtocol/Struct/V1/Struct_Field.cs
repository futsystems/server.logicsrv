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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
        /// 账户货币
        /// </summary>
        public XLCurrencyType Currency;

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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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


        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
        public string TradingSession;
        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_SYMBOL; } }

       
    }
    #endregion

    #region 查询委托
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
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
        /// 今日总持仓
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
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLQryTradingAccountField : IXLField
    {
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string UserID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_ACCOUNT; } }
    }

    /// <summary>
    /// 资金账户
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLTradingAccountField : IXLField
    {
        /// <summary>
        /// 上日信用额度
        /// </summary>
        public double PreCredit;

        /// <summary>
        /// 当前信用额度
        /// </summary>
        public double Credit;

        /// <summary>
        /// 上日权益
        /// </summary>
        public double PreEquity;

        /// <summary>
        /// 入金金额
        /// </summary>
        public double Deposit;

        /// <summary>
        /// 出金金额
        /// </summary>
        public double Withdraw;

        /// <summary>
        /// 冻结的保证金
        /// </summary>
        public double FrozenMargin;

        /// <summary>
        /// 占用保证金
        /// </summary>
        public double Margin;

        /// <summary>
        /// 手续费
        /// </summary>
        public double Commission;

        /// <summary>
        /// 平仓盈亏
        /// </summary>
        public double CloseProfit;

        /// <summary>
        /// 持仓盈亏
        /// </summary>
        public double PositionProfit;

        /// <summary>
        /// 当前权益
        /// </summary>
        public double NowEquity;

        /// <summary>
        /// 可用资金
        /// </summary>
        public double Available;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_ACCOUNT; } }

    }
    #endregion

    #region 查询汇率
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLQryExchangeRateField : IXLField
    {
        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_EXCHANGERATE; } }
    }

    /// <summary>
    /// 汇率数据
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLExchangeRateField : IXLField
    {
        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay;
        /// <summary>
        /// 上日信用额度
        /// </summary>
        public double IntermediateRate;

        /// <summary>
        /// 当前信用额度
        /// </summary>
        public XLCurrencyType Currency;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_EXCHANGERATE; } }

    }
    #endregion



    #region 查询最大报单数量
    /// <summary>
    /// 查询最大报单数量
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLQryMaxOrderVolumeField : IXLField
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
        /// 最大允许报单数量
        /// </summary>
        public int MaxVolume;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_MAXORDVOL; } }
        
    }
        
    #endregion

    #region 提交委托
    /// <summary>
    /// 输入报单
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLInputOrderField : IXLField
    {
       
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
        /// 报单类别
        /// </summary>
        public XLOrderType OrderType;

        /// <summary>
        /// 买卖方向
        /// </summary>
        public XLDirectionType Direction;

        /// <summary>
        /// 组合开平标志
        /// </summary>
        public XLOffsetFlagType OffsetFlag;
       
        /// <summary>
        /// 组合投机套保标志
        /// </summary>
        public XLHedgeFlagType HedgeFlag;
       
        /// <summary>
        /// 价格
        /// </summary>
        public double LimitPrice;

        /// <summary>
        /// 止损价
        /// </summary>
        public double StopPrice;

        /// <summary>
        /// 数量
        /// </summary>
        public int VolumeTotalOriginal;

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_REQ_INSERTORDER; } }
        

    }
    #endregion

    #region 提交委托操作
    /// <summary>
    /// 输入报单操作
    /// 报单引用3钟方式
    /// 1.提交报单后系统会分配给该委托一个唯一ID作为编号 通过该编号可以进行撤单
    /// 2.通过委托ExchangeID + OrderSysID 组合键形成委托编号
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLInputOrderActionField : IXLField
    {
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string UserID;

        /// <summary>
        /// 委托系统编号
        /// </summary>
        public long OrderID;

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
        /// 操作标志
        /// </summary>
        public XLActionFlagType ActionFlag;

        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_REQ_ORDERACTION; } }


    }
    #endregion


    #region 订阅行情

    /// <summary>
    /// 订阅合约
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLSpecificSymbolField : IXLField
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_SYMBOL; } }
    }


     /// <summary>
    /// 深度行情
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLDepthMarketDataField : IXLField
    {
        /// <summary>
        /// 业务日期
        /// </summary>
        public int Date;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public int Time;
        /// <summary>
        /// 交易日
        /// </summary>
        public int TradingDay;
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
        /// 最新价
        /// </summary>
        public double LastPrice;
        /// <summary>
        /// 上次结算价
        /// </summary>
        public double PreSettlementPrice;
        /// <summary>
        /// 昨收盘
        /// </summary>
        public double PreClosePrice;
        /// <summary>
        /// 昨持仓量
        /// </summary>
        public double PreOpenInterest;
        /// <summary>
        /// 今开盘
        /// </summary>
        public double OpenPrice;
        /// <summary>
        /// 最高价
        /// </summary>
        public double HighestPrice;
        /// <summary>
        /// 最低价
        /// </summary>
        public double LowestPrice;
        /// <summary>
        /// 数量
        /// </summary>
        public int Volume;
        /// <summary>
        /// 持仓量
        /// </summary>
        public double OpenInterest;
        /// <summary>
        /// 今收盘
        /// </summary>
        public double ClosePrice;
        /// <summary>
        /// 本次结算价
        /// </summary>
        public double SettlementPrice;
        /// <summary>
        /// 涨停板价
        /// </summary>
        public double UpperLimitPrice;
        /// <summary>
        /// 跌停板价
        /// </summary>
        public double LowerLimitPrice;

        /// <summary>
        /// 申买价一
        /// </summary>
        public double BidPrice1;
        /// <summary>
        /// 申买量一
        /// </summary>
        public int BidVolume1;
        /// <summary>
        /// 申卖价一
        /// </summary>
        public double AskPrice1;
        /// <summary>
        /// 申卖量一
        /// </summary>
        public int AskVolume1;
        /// <summary>
        /// 申买价二
        /// </summary>
        public double BidPrice2;
        /// <summary>
        /// 申买量二
        /// </summary>
        public int BidVolume2;
        /// <summary>
        /// 申卖价二
        /// </summary>
        public double AskPrice2;
        /// <summary>
        /// 申卖量二
        /// </summary>
        public int AskVolume2;
        /// <summary>
        /// 申买价三
        /// </summary>
        public double BidPrice3;
        /// <summary>
        /// 申买量三
        /// </summary>
        public int BidVolume3;
        /// <summary>
        /// 申卖价三
        /// </summary>
        public double AskPrice3;
        /// <summary>
        /// 申卖量三
        /// </summary>
        public int AskVolume3;
        /// <summary>
        /// 申买价四
        /// </summary>
        public double BidPrice4;
        /// <summary>
        /// 申买量四
        /// </summary>
        public int BidVolume4;
        /// <summary>
        /// 申卖价四
        /// </summary>
        public double AskPrice4;
        /// <summary>
        /// 申卖量四
        /// </summary>
        public int AskVolume4;
        /// <summary>
        /// 申买价五
        /// </summary>
        public double BidPrice5;
        /// <summary>
        /// 申买量五
        /// </summary>
        public int BidVolume5;
        /// <summary>
        /// 申卖价五
        /// </summary>
        public double AskPrice5;
        /// <summary>
        /// 申卖量五
        /// </summary>
        public int AskVolume5;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_MarketData; } }
    }
    #endregion

    #region 查询分时
    /// <summary>
    /// 查询分时数据请求
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLQryMinuteDataField : IXLField
    {
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;

        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;

        /// <summary>
        /// 交易日
        /// 查询某个交易日的分时数据
        /// </summary>
        public int TradingDay;

        /// <summary>
        /// 起始查询时间
        /// 查询某个时间点之后的所有分时数据
        /// </summary>
        public long Start;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_QRY_MINUTEDATA; } }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLMinuteDataField : IXLField
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
        /// 成交量
        /// </summary>
        public int Vol;

        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_MINUTEDATA; } }
    }

    #endregion

    #region 查询K线
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLQryBarDataField : IXLField
    {
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;

        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string SymbolID;

        /// <summary>
        /// 间隔数
        /// </summary>
        public int Interval;

        /// <summary>
        /// 开始时间
        /// </summary>
        public long Start;

        /// <summary>
        /// 结束时间
        /// </summary>
        public long End;

        /// <summary>
        /// 最大返回数量
        /// </summary>
        public int MaxCount;

        /// <summary>
        /// 返回数据开始位置
        /// </summary>
        public int StartIndex;

        /// <summary>
        /// 是否查询EOD日线数据
        /// </summary>
        public bool IsEOD;

        /// <summary>
        /// 是否包含Partial
        /// </summary>
        public bool HavePartial;

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_Qry_BARDATA; } }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct XLBarDataField : IXLField
    {
        /// <summary>
        /// 日期
        /// </summary>
        public int Date { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public int Time { get; set; }

        /// <summary>
        /// 开盘价
        /// </summary>
        public double Open { get; set; }

        /// <summary>
        /// 最高价
        /// </summary>
        public double High { get; set; }

        /// <summary>
        /// 最低价
        /// </summary>
        public double Low { get; set; }

        /// <summary>
        /// 收盘价
        /// </summary>
        public double Close { get; set; }

        /// <summary>
        /// 成交量
        /// </summary>
        public double Vol { get; set; }

        /// <summary>
        /// 持仓量
        /// </summary>
        public double OI { get; set; }

        /// <summary>
        /// 域类别
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_RSP_BARDATA; } }

    }
    #endregion






}
