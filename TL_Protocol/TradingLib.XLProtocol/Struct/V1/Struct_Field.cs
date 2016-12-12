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
    #endregion

    #region 查询成交
    #endregion 

    #region 查询持仓
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
