using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using CTPService.Struct;

namespace CTPService.Struct.V12
{

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct TopicData : IFieldId
    {
        public ushort wSeqSn;

        public uint dSeqNo;

        public void Swap()
        {
            wSeqSn = ByteSwapHelp.ReverseBytes(wSeqSn);
            dSeqNo = ByteSwapHelp.ReverseBytes(dSeqNo);
        }

        public ushort FieldId
        {
            get { return 4097; }
        }
    }

    /// <summary>
    /// 响应信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcRspInfoField : ITFieldId
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorID;
        /// <summary>
        /// 错误信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string ErrorMsg;

        public void Swap()
        {
            ErrorID = ByteSwapHelp.ReverseBytes(ErrorID);
        }

        public ushort FieldId
        {
            get { return 0; }
        }

        public static implicit operator CThostFtdcRspInfoField(LCThostFtdcRspInfoField input)
        {
            CThostFtdcRspInfoField ret = new CThostFtdcRspInfoField();
            ret.ErrorID = input.ErrorID;
            ret.ErrorMsg = input.ErrorMsg;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcRspInfoField : IFieldId
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorID;
        /// <summary>
        /// 错误信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string ErrorMsg;

        public void Swap()
        {
            ErrorID = ByteSwapHelp.ReverseBytes(ErrorID);
        }

        public ushort FieldId
        {
            get { return 0; }
        }

        public static implicit operator LCThostFtdcRspInfoField(CThostFtdcRspInfoField input)
        {
            LCThostFtdcRspInfoField ret = new LCThostFtdcRspInfoField();
            ret.ErrorID = input.ErrorID;
            ret.ErrorMsg = input.ErrorMsg;

            return ret;
        }
    }

    #region 用户登入
    /// <summary>
    /// 用户登录请求
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcReqUserLoginField : ITFieldId
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 用户代码
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
        /// 接口端产品信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string InterfaceProductInfo;
        /// <summary>
        /// 协议信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ProtocolInfo;
        /// <summary>
        /// Mac地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string MacAddress;
        /// <summary>
        /// 动态密码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string OneTimePassword;
        /// <summary>
        /// 终端IP地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ClientIPAddress;

        /// <summary>
        /// 登入备注
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string LoginRemark;

        public void Swap()
        { }

        public ushort FieldId
        {
            get { return (0x1002); }
        }

        public static implicit operator CThostFtdcReqUserLoginField(LCThostFtdcReqUserLoginField input)
        {
            CThostFtdcReqUserLoginField ret = new CThostFtdcReqUserLoginField();
            ret.TradingDay = input.TradingDay;
            ret.BrokerID = input.BrokerID;
            ret.UserID = input.UserID;
            ret.Password = input.Password;
            ret.UserProductInfo = input.UserProductInfo;
            ret.InterfaceProductInfo = input.InterfaceProductInfo;
            ret.ProtocolInfo = input.ProtocolInfo;
            ret.MacAddress = input.MacAddress;
            ret.OneTimePassword = input.OneTimePassword;
            ret.ClientIPAddress = input.ClientIPAddress;

            return ret;
        }
    }


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcReqUserLoginField : IFieldId
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 用户代码
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
        /// 接口端产品信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string InterfaceProductInfo;
        /// <summary>
        /// 协议信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ProtocolInfo;
        /// <summary>
        /// Mac地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string MacAddress;
        /// <summary>
        /// 动态密码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string OneTimePassword;
        /// <summary>
        /// 终端IP地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ClientIPAddress;

        /// <summary>
        /// 登入备注
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string LoginRemark;
        public void Swap()
        { }

        public ushort FieldId
        {
            get { return (0x1002); }
        }

        public static implicit operator LCThostFtdcReqUserLoginField(CThostFtdcReqUserLoginField input)
        {
            LCThostFtdcReqUserLoginField ret = new LCThostFtdcReqUserLoginField();
            ret.TradingDay = input.TradingDay;
            ret.BrokerID = input.BrokerID;
            ret.UserID = input.UserID;
            ret.Password = input.Password;
            ret.UserProductInfo = input.UserProductInfo;
            ret.InterfaceProductInfo = input.InterfaceProductInfo;
            ret.ProtocolInfo = input.ProtocolInfo;
            ret.MacAddress = input.MacAddress;
            ret.OneTimePassword = input.OneTimePassword;
            ret.ClientIPAddress = input.ClientIPAddress;

            return ret;
        }
    }

    /// <summary>
    /// 用户登录应答
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcRspUserLoginField : ITFieldId
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 登录成功时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string LoginTime;
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 交易系统名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string SystemName;
        /// <summary>
        /// 前置编号
        /// </summary>
        public int FrontID;
        /// <summary>
        /// 会话编号
        /// </summary>
        public int SessionID;
        /// <summary>
        /// 最大报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string MaxOrderRef;
        /// <summary>
        /// 上期所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SHFETime;
        /// <summary>
        /// 大商所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string DCETime;
        /// <summary>
        /// 郑商所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string CZCETime;
        /// <summary>
        /// 中金所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string FFEXTime;

        /// <summary>
        /// 能源中心
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string INETime;


        public void Swap()
        {
            FrontID = ByteSwapHelp.ReverseBytes(FrontID);
            SessionID = ByteSwapHelp.ReverseBytes(SessionID);

        }

        public ushort FieldId
        {
            get { return 0x1003; }
        }

        public static implicit operator CThostFtdcRspUserLoginField(LCThostFtdcRspUserLoginField input)
        {
            CThostFtdcRspUserLoginField ret = new CThostFtdcRspUserLoginField();
            ret.TradingDay = input.TradingDay;
            ret.LoginTime = input.LoginTime;
            ret.BrokerID = input.BrokerID;
            ret.UserID = input.UserID;
            ret.SystemName = input.SystemName;
            ret.FrontID = input.FrontID;
            ret.SessionID = input.SessionID;
            ret.MaxOrderRef = input.MaxOrderRef;
            ret.SHFETime = input.SHFETime;
            ret.DCETime = input.DCETime;
            ret.CZCETime = input.CZCETime;
            ret.FFEXTime = input.FFEXTime;
            ret.INETime = input.INETime;
            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcRspUserLoginField : IFieldId
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 登录成功时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string LoginTime;
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 交易系统名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string SystemName;
        /// <summary>
        /// 前置编号
        /// </summary>
        public int FrontID;
        /// <summary>
        /// 会话编号
        /// </summary>
        public int SessionID;
        /// <summary>
        /// 最大报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string MaxOrderRef;
        /// <summary>
        /// 上期所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SHFETime;
        /// <summary>
        /// 大商所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string DCETime;
        /// <summary>
        /// 郑商所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string CZCETime;
        /// <summary>
        /// 中金所时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string FFEXTime;

        /// <summary>
        /// 能源中心
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string INETime;


        public void Swap()
        {
            FrontID = ByteSwapHelp.ReverseBytes(FrontID);
            SessionID = ByteSwapHelp.ReverseBytes(SessionID);
        }

        public ushort FieldId
        {
            get { return 0x1003; }
        }

        public static implicit operator LCThostFtdcRspUserLoginField(CThostFtdcRspUserLoginField input)
        {
            LCThostFtdcRspUserLoginField ret = new LCThostFtdcRspUserLoginField();
            ret.TradingDay = input.TradingDay;
            ret.LoginTime = input.LoginTime;
            ret.BrokerID = input.BrokerID;
            ret.UserID = input.UserID;
            ret.SystemName = input.SystemName;
            ret.FrontID = input.FrontID;
            ret.SessionID = input.SessionID;
            ret.MaxOrderRef = input.MaxOrderRef;
            ret.SHFETime = input.SHFETime;
            ret.DCETime = input.DCETime;
            ret.CZCETime = input.CZCETime;
            ret.FFEXTime = input.FFEXTime;
            ret.INETime = input.INETime;

            return ret;
        }
    }

    #endregion


    #region 查询投资者
    /// <summary>
    /// 查询投资者
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQryInvestorField : ITFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorID;

        public ushort FieldId
        {
            get { return 1796; }
        }

        public void Swap()
        { }

        public static explicit operator CThostFtdcQryInvestorField(LCThostFtdcQryInvestorField input)
        {
            CThostFtdcQryInvestorField ret = new CThostFtdcQryInvestorField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;

            return ret;
        }
    }

    /// <summary>
    /// 查询投资者
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQryInvestorField : IFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorID;

        public ushort FieldId
        {
            get { return 1796; }
        }

        public void Swap()
        { }

        public static explicit operator LCThostFtdcQryInvestorField(CThostFtdcQryInvestorField input)
        {
            LCThostFtdcQryInvestorField ret = new LCThostFtdcQryInvestorField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;

            return ret;
        }
    }

    /// <summary>
    /// 投资者
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcInvestorField : ITFieldId
    {
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorID;
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 投资者分组代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorGroupID;
        /// <summary>
        /// 投资者名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string InvestorName;
        /// <summary>
        /// 证件类型
        /// </summary>
        public TThostFtdcIdCardTypeType IdentifiedCardType;
        /// <summary>
        /// 证件号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string IdentifiedCardNo;
        /// <summary>
        /// 是否活跃
        /// </summary>
        public int IsActive;
        /// <summary>
        /// 联系电话
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string Telephone;
        /// <summary>
        /// 通讯地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 101)]
        public string Address;
        /// <summary>
        /// 开户日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string OpenDate;
        /// <summary>
        /// 手机
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string Mobile;
        /// <summary>
        /// 手续费率模板代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string CommModelID;
        /// <summary>
        /// 保证金率模板代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string MarginModelID;

        public ushort FieldId
        {
            get { return 6; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcInvestorField(LCThostFtdcInvestorField input)
        {
            CThostFtdcInvestorField ret = new CThostFtdcInvestorField();
            ret.InvestorID = input.InvestorID;
            ret.BrokerID = input.BrokerID;
            ret.InvestorGroupID = input.InvestorGroupID;
            ret.InvestorName = input.InvestorName;
            ret.IdentifiedCardType = input.IdentifiedCardType;
            ret.IdentifiedCardNo = input.IdentifiedCardNo;
            ret.IsActive = input.IsActive;
            ret.Telephone = input.Telephone;
            ret.Address = input.Address;
            ret.OpenDate = input.OpenDate;
            ret.Mobile = input.Mobile;
            ret.CommModelID = input.CommModelID;
            ret.MarginModelID = input.MarginModelID;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcInvestorField : IFieldId
    {
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorID;
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 投资者分组代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorGroupID;
        /// <summary>
        /// 投资者名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string InvestorName;
        /// <summary>
        /// 证件类型
        /// </summary>
        public TThostFtdcIdCardTypeType IdentifiedCardType;
        /// <summary>
        /// 证件号码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 51)]
        public string IdentifiedCardNo;
        /// <summary>
        /// 是否活跃
        /// </summary>
        public int IsActive;
        /// <summary>
        /// 联系电话
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string Telephone;
        /// <summary>
        /// 通讯地址
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 101)]
        public string Address;
        /// <summary>
        /// 开户日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string OpenDate;
        /// <summary>
        /// 手机
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 41)]
        public string Mobile;
        /// <summary>
        /// 手续费率模板代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string CommModelID;
        /// <summary>
        /// 保证金率模板代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string MarginModelID;
        /// <summary>
        /// 
        /// </summary>
        public ushort FieldId
        {
            get { return 6; }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Swap()
        {
            IsActive = ByteSwapHelp.ReverseBytes(IsActive);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static implicit operator LCThostFtdcInvestorField(CThostFtdcInvestorField input)
        {
            LCThostFtdcInvestorField ret = new LCThostFtdcInvestorField();
            ret.InvestorID = input.InvestorID;
            ret.BrokerID = input.BrokerID;
            ret.InvestorGroupID = input.InvestorGroupID;
            ret.InvestorName = input.InvestorName;
            ret.IdentifiedCardType = input.IdentifiedCardType;
            ret.IdentifiedCardNo = input.IdentifiedCardNo;
            ret.IsActive = input.IsActive;
            ret.Telephone = input.Telephone;
            ret.Address = input.Address;
            ret.OpenDate = input.OpenDate;
            ret.Mobile = input.Mobile;
            ret.CommModelID = input.CommModelID;
            ret.MarginModelID = input.MarginModelID;

            return ret;
        }
    }

    #endregion




}
