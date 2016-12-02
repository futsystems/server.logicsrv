﻿using System;
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

    #region 查询投资者结算确认

    /// <summary>
    /// 查询结算信息确认域
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQrySettlementInfoConfirmField : ITFieldId
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
            get { return 9304; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQrySettlementInfoConfirmField(LCThostFtdcQrySettlementInfoConfirmField input)
        {
            CThostFtdcQrySettlementInfoConfirmField ret = new CThostFtdcQrySettlementInfoConfirmField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQrySettlementInfoConfirmField : IFieldId
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
            get { return 9304; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQrySettlementInfoConfirmField(CThostFtdcQrySettlementInfoConfirmField input)
        {
            LCThostFtdcQrySettlementInfoConfirmField ret = new LCThostFtdcQrySettlementInfoConfirmField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;

            return ret;
        }
    }

    /// <summary>
    /// 投资者结算结果确认信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcSettlementInfoConfirmField : ITFieldId
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
        /// <summary>
        /// 确认日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ConfirmDate;
        /// <summary>
        /// 确认时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ConfirmTime;

        public ushort FieldId
        {
            get { return 1039; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcSettlementInfoConfirmField(LCThostFtdcSettlementInfoConfirmField input)
        {
            CThostFtdcSettlementInfoConfirmField ret = new CThostFtdcSettlementInfoConfirmField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.ConfirmDate = input.ConfirmDate;
            ret.ConfirmTime = input.ConfirmTime;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcSettlementInfoConfirmField : IFieldId
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
        /// <summary>
        /// 确认日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ConfirmDate;
        /// <summary>
        /// 确认时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ConfirmTime;

        public ushort FieldId
        {
            get { return 1039; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcSettlementInfoConfirmField(CThostFtdcSettlementInfoConfirmField input)
        {
            LCThostFtdcSettlementInfoConfirmField ret = new LCThostFtdcSettlementInfoConfirmField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.ConfirmDate = input.ConfirmDate;
            ret.ConfirmTime = input.ConfirmTime;

            return ret;
        }
    }
    #endregion

    #region 查询通知
    /// <summary>
    /// 查询客户通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQryNoticeField : ITFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;

        public ushort FieldId
        {
            get { return 9301; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQryNoticeField(LCThostFtdcQryNoticeField input)
        {
            CThostFtdcQryNoticeField ret = new CThostFtdcQryNoticeField();
            ret.BrokerID = input.BrokerID;

            return ret;
        }
    }
    /// <summary>
    /// 查询客户通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQryNoticeField : IFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;

        public ushort FieldId
        {
            get { return 9301; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQryNoticeField(CThostFtdcQryNoticeField input)
        {
            LCThostFtdcQryNoticeField ret = new LCThostFtdcQryNoticeField();
            ret.BrokerID = input.BrokerID;

            return ret;
        }
    }

    /// <summary>
    /// 客户通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcNoticeField : ITFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 消息正文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 501)]
        public string Content;
        /// <summary>
        /// 经纪公司通知内容序列号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string SequenceLabel;

        public ushort FieldId
        {
            get { return 9302; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcNoticeField(LCThostFtdcNoticeField input)
        {
            CThostFtdcNoticeField ret = new CThostFtdcNoticeField();
            ret.BrokerID = input.BrokerID;
            ret.Content = input.Content;
            ret.SequenceLabel = input.SequenceLabel;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcNoticeField : IFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 消息正文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 501)]
        public string Content;
        /// <summary>
        /// 经纪公司通知内容序列号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string SequenceLabel;

        public ushort FieldId
        {
            get { return 9302; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcNoticeField(CThostFtdcNoticeField input)
        {
            LCThostFtdcNoticeField ret = new LCThostFtdcNoticeField();
            ret.BrokerID = input.BrokerID;
            ret.Content = input.Content;
            ret.SequenceLabel = input.SequenceLabel;

            return ret;
        }
    }
    #endregion

    #region 交易通知
    /// <summary>
    /// 查询交易事件通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQryTradingNoticeField
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
            get { return 9350; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQryTradingNoticeField(LCThostFtdcQryTradingNoticeField input)
        {
            CThostFtdcQryTradingNoticeField ret = new CThostFtdcQryTradingNoticeField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;

            return ret;
        }

    }

    /// <summary>
    /// 查询交易事件通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQryTradingNoticeField : IFieldId
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
            get { return 9350; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQryTradingNoticeField(CThostFtdcQryTradingNoticeField input)
        {
            LCThostFtdcQryTradingNoticeField ret = new LCThostFtdcQryTradingNoticeField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;

            return ret;
        }

    }


    /// <summary>
    /// 用户事件通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcTradingNoticeField : IFieldId
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 投资者范围
        /// </summary>
        public TThostFtdcInvestorRangeType InvestorRange;
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorID;
        /// <summary>
        /// 序列系列号
        /// </summary>
        public short SequenceSeries;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 发送时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SendTime;
        /// <summary>
        /// 序列号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 消息正文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 501)]
        public string FieldContent;


        public ushort FieldId
        {
            get { return 9350; }
        }

        public void Swap()
        { }
    }

    /// <summary>
    /// 用户事件通知
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcTradingNoticeField
    {
        /// <summary>
        /// 经纪公司代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string BrokerID;
        /// <summary>
        /// 投资者范围
        /// </summary>
        public TThostFtdcInvestorRangeType InvestorRange;
        /// <summary>
        /// 投资者代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string InvestorID;
        /// <summary>
        /// 序列系列号
        /// </summary>
        public short SequenceSeries;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 发送时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SendTime;
        /// <summary>
        /// 序列号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 消息正文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 501)]
        public string FieldContent;
    }



    #endregion

    #region 查询投资者结算结果信息
    /// <summary>
    /// 查询投资者结算结果
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQrySettlementInfoField : ITFieldId
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
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;

        public ushort FieldId
        {
            get { return 1804; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQrySettlementInfoField(LCThostFtdcQrySettlementInfoField input)
        {
            CThostFtdcQrySettlementInfoField ret = new CThostFtdcQrySettlementInfoField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.TradingDay = input.TradingDay;

            return ret;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQrySettlementInfoField : IFieldId
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
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;

        public ushort FieldId
        {
            get { return 1804; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQrySettlementInfoField(CThostFtdcQrySettlementInfoField input)
        {
            LCThostFtdcQrySettlementInfoField ret = new LCThostFtdcQrySettlementInfoField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.TradingDay = input.TradingDay;

            return ret;
        }
    }


    /// <summary>
    /// 投资者结算结果
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcSettlementInfoField : ITFieldId
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 结算编号
        /// </summary>
        public int SettlementID;
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
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 消息正文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 501)]
        public string Content;

        public ushort FieldId
        {
            get { return 27; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcSettlementInfoField(LCThostFtdcSettlementInfoField input)
        {
            CThostFtdcSettlementInfoField ret = new CThostFtdcSettlementInfoField();
            ret.TradingDay = input.TradingDay;
            ret.SettlementID = input.SettlementID;
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.SequenceNo = input.SequenceNo;
            ret.Content = input.Content;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcSettlementInfoField : IFieldId
    {
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 结算编号
        /// </summary>
        public int SettlementID;
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
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 消息正文
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 501)]
        public string Content;

        public ushort FieldId
        {
            get { return 27; }
        }

        public void Swap()
        {
            SettlementID = ByteSwapHelp.ReverseBytes(SettlementID);
            SequenceNo = ByteSwapHelp.ReverseBytes(SequenceNo);
        }

        public static implicit operator LCThostFtdcSettlementInfoField(CThostFtdcSettlementInfoField input)
        {
            LCThostFtdcSettlementInfoField ret = new LCThostFtdcSettlementInfoField();
            ret.TradingDay = input.TradingDay;
            ret.SettlementID = input.SettlementID;
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.SequenceNo = input.SequenceNo;
            ret.Content = input.Content;

            return ret;
        }
    }
    #endregion

    #region 查询合约
    /// <summary>
    /// 查询合约
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQryInstrumentField : ITFieldId
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 产品代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ProductID;

        public ushort FieldId
        {
            get { return 1815; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQryInstrumentField(LCThostFtdcQryInstrumentField input)
        {
            CThostFtdcQryInstrumentField ret = new CThostFtdcQryInstrumentField();
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.ProductID = input.ProductID;

            return ret;
        }
    }
    /// <summary>
    /// 查询合约
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQryInstrumentField : IFieldId
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 产品代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ProductID;

        public ushort FieldId
        {
            get { return 1815; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQryInstrumentField(CThostFtdcQryInstrumentField input)
        {
            LCThostFtdcQryInstrumentField ret = new LCThostFtdcQryInstrumentField();
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.ProductID = input.ProductID;

            return ret;
        }
    }

    /// <summary>
    /// 合约
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcInstrumentField : ITFieldId
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 合约名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string InstrumentName;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 产品代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ProductID;
        /// <summary>
        /// 产品类型
        /// </summary>
        public TThostFtdcProductClassType ProductClass;
        /// <summary>
        /// 交割年份
        /// </summary>
        public int DeliveryYear;
        /// <summary>
        /// 交割月
        /// </summary>
        public int DeliveryMonth;
        /// <summary>
        /// 市价单最大下单量
        /// </summary>
        public int MaxMarketOrderVolume;
        /// <summary>
        /// 市价单最小下单量
        /// </summary>
        public int MinMarketOrderVolume;
        /// <summary>
        /// 限价单最大下单量
        /// </summary>
        public int MaxLimitOrderVolume;
        /// <summary>
        /// 限价单最小下单量
        /// </summary>
        public int MinLimitOrderVolume;
        /// <summary>
        /// 合约数量乘数
        /// </summary>
        public int VolumeMultiple;
        /// <summary>
        /// 最小变动价位
        /// </summary>
        public double PriceTick;
        /// <summary>
        /// 创建日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string CreateDate;
        /// <summary>
        /// 上市日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string OpenDate;
        /// <summary>
        /// 到期日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExpireDate;
        /// <summary>
        /// 开始交割日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string StartDelivDate;
        /// <summary>
        /// 结束交割日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string EndDelivDate;
        /// <summary>
        /// 合约生命周期状态
        /// </summary>
        public TThostFtdcInstLifePhaseType InstLifePhase;
        /// <summary>
        /// 当前是否交易
        /// </summary>
        public int IsTrading;
        /// <summary>
        /// 持仓类型
        /// </summary>
        public TThostFtdcPositionTypeType PositionType;
        /// <summary>
        /// 持仓日期类型
        /// </summary>
        public TThostFtdcPositionDateTypeType PositionDateType;
        /// <summary>
        /// 多头保证金率
        /// </summary>
        public double LongMarginRatio;
        /// <summary>
        /// 空头保证金率
        /// </summary>
        public double ShortMarginRatio;
        /// <summary>
        /// 是否使用大额单边保证金算法
        /// </summary>
        public TThostFtdcMaxMarginSideAlgorithmType MaxMarginSideAlgorithm;

        ///基础商品代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string UnderlyingInstrID;
        ///执行价
        public double StrikePrice;
        ///期权类型
        public TThostFtdcOptionsTypeType OptionsType;
        ///合约基础商品乘数
        public double UnderlyingMultiple;
        ///组合类型
        public TThostFtdcCombinationTypeType CombinationType;


        public ushort FieldId
        {
            get { return 3; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcInstrumentField(LCThostFtdcInstrumentField input)
        {
            CThostFtdcInstrumentField ret = new CThostFtdcInstrumentField();
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.InstrumentName = input.InstrumentName;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.ProductID = input.ProductID;
            ret.ProductClass = input.ProductClass;
            ret.DeliveryYear = input.DeliveryYear;
            ret.DeliveryMonth = input.DeliveryMonth;
            ret.MaxMarketOrderVolume = input.MaxMarketOrderVolume;
            ret.MinMarketOrderVolume = input.MinMarketOrderVolume;
            ret.MaxLimitOrderVolume = input.MaxLimitOrderVolume;
            ret.MinLimitOrderVolume = input.MinLimitOrderVolume;
            ret.VolumeMultiple = input.VolumeMultiple;
            ret.PriceTick = input.PriceTick;
            ret.CreateDate = input.CreateDate;
            ret.OpenDate = input.OpenDate;
            ret.ExpireDate = input.ExpireDate;
            ret.StartDelivDate = input.StartDelivDate;
            ret.EndDelivDate = input.EndDelivDate;
            ret.InstLifePhase = input.InstLifePhase;
            ret.IsTrading = input.IsTrading;
            ret.PositionType = input.PositionType;
            ret.PositionDateType = input.PositionDateType;
            ret.LongMarginRatio = input.LongMarginRatio;
            ret.ShortMarginRatio = input.ShortMarginRatio;
            ret.MaxMarginSideAlgorithm = input.MaxMarginSideAlgorithm;
            ret.UnderlyingInstrID = input.UnderlyingInstrID;
            ret.StrikePrice = input.StrikePrice;
            ret.OptionsType = input.OptionsType;
            ret.UnderlyingMultiple = input.UnderlyingMultiple;
            ret.CombinationType = input.CombinationType;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcInstrumentField : IFieldId, ICloneable
    {
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 合约名称
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string InstrumentName;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 产品代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ProductID;
        /// <summary>
        /// 产品类型
        /// </summary>
        public TThostFtdcProductClassType ProductClass;
        /// <summary>
        /// 交割年份
        /// </summary>
        public int DeliveryYear;
        /// <summary>
        /// 交割月
        /// </summary>
        public int DeliveryMonth;
        /// <summary>
        /// 市价单最大下单量
        /// </summary>
        public int MaxMarketOrderVolume;
        /// <summary>
        /// 市价单最小下单量
        /// </summary>
        public int MinMarketOrderVolume;
        /// <summary>
        /// 限价单最大下单量
        /// </summary>
        public int MaxLimitOrderVolume;
        /// <summary>
        /// 限价单最小下单量
        /// </summary>
        public int MinLimitOrderVolume;
        /// <summary>
        /// 合约数量乘数
        /// </summary>
        public int VolumeMultiple;
        /// <summary>
        /// 最小变动价位
        /// </summary>
        public double PriceTick;
        /// <summary>
        /// 创建日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string CreateDate;
        /// <summary>
        /// 上市日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string OpenDate;
        /// <summary>
        /// 到期日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExpireDate;
        /// <summary>
        /// 开始交割日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string StartDelivDate;
        /// <summary>
        /// 结束交割日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string EndDelivDate;
        /// <summary>
        /// 合约生命周期状态
        /// </summary>
        public TThostFtdcInstLifePhaseType InstLifePhase;
        /// <summary>
        /// 当前是否交易
        /// </summary>
        public int IsTrading;
        /// <summary>
        /// 持仓类型
        /// </summary>
        public TThostFtdcPositionTypeType PositionType;
        /// <summary>
        /// 持仓日期类型
        /// </summary>
        public TThostFtdcPositionDateTypeType PositionDateType;
        /// <summary>
        /// 多头保证金率
        /// </summary>
        public double LongMarginRatio;
        /// <summary>
        /// 空头保证金率
        /// </summary>
        public double ShortMarginRatio;
        /// <summary>
        /// 是否使用大额单边保证金算法
        /// </summary>
        public TThostFtdcMaxMarginSideAlgorithmType MaxMarginSideAlgorithm;

        ///基础商品代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string UnderlyingInstrID;
        ///执行价
        public double StrikePrice;
        ///期权类型
        public TThostFtdcOptionsTypeType OptionsType;
        ///合约基础商品乘数
        public double UnderlyingMultiple;
        ///组合类型
        public TThostFtdcCombinationTypeType CombinationType;


        public ushort FieldId
        {
            get { return 3; }
        }

        public void Swap()
        {
            DeliveryYear = ByteSwapHelp.ReverseBytes(DeliveryYear);
            DeliveryMonth = ByteSwapHelp.ReverseBytes(DeliveryMonth);
            MaxMarketOrderVolume = ByteSwapHelp.ReverseBytes(MaxMarketOrderVolume);
            MinMarketOrderVolume = ByteSwapHelp.ReverseBytes(MinMarketOrderVolume);
            MaxLimitOrderVolume = ByteSwapHelp.ReverseBytes(MaxLimitOrderVolume);
            MinLimitOrderVolume = ByteSwapHelp.ReverseBytes(MinLimitOrderVolume);
            VolumeMultiple = ByteSwapHelp.ReverseBytes(VolumeMultiple);
            PriceTick = ByteSwapHelp.ReverseBytes(PriceTick);
            IsTrading = ByteSwapHelp.ReverseBytes(IsTrading);
            LongMarginRatio = ByteSwapHelp.ReverseBytes(LongMarginRatio);
            ShortMarginRatio = ByteSwapHelp.ReverseBytes(ShortMarginRatio);

            StrikePrice = ByteSwapHelp.ReverseBytes(StrikePrice);
            UnderlyingMultiple = ByteSwapHelp.ReverseBytes(UnderlyingMultiple);
        }

        public static implicit operator LCThostFtdcInstrumentField(CThostFtdcInstrumentField input)
        {
            LCThostFtdcInstrumentField ret = new LCThostFtdcInstrumentField();
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.InstrumentName = input.InstrumentName;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.ProductID = input.ProductID;
            ret.ProductClass = input.ProductClass;
            ret.DeliveryYear = input.DeliveryYear;
            ret.DeliveryMonth = input.DeliveryMonth;
            ret.MaxMarketOrderVolume = input.MaxMarketOrderVolume;
            ret.MinMarketOrderVolume = input.MinMarketOrderVolume;
            ret.MaxLimitOrderVolume = input.MaxLimitOrderVolume;
            ret.MinLimitOrderVolume = input.MinLimitOrderVolume;
            ret.VolumeMultiple = input.VolumeMultiple;
            ret.PriceTick = input.PriceTick;
            ret.CreateDate = input.CreateDate;
            ret.OpenDate = input.OpenDate;
            ret.ExpireDate = input.ExpireDate;
            ret.StartDelivDate = input.StartDelivDate;
            ret.EndDelivDate = input.EndDelivDate;
            ret.InstLifePhase = input.InstLifePhase;
            ret.IsTrading = input.IsTrading;
            ret.PositionType = input.PositionType;
            ret.PositionDateType = input.PositionDateType;
            ret.LongMarginRatio = input.LongMarginRatio;
            ret.ShortMarginRatio = input.ShortMarginRatio;
            ret.MaxMarginSideAlgorithm = input.MaxMarginSideAlgorithm;
            ret.UnderlyingInstrID = input.UnderlyingInstrID;
            ret.StrikePrice = input.StrikePrice;
            ret.OptionsType = input.OptionsType;
            ret.UnderlyingMultiple = input.UnderlyingMultiple;
            ret.CombinationType = input.CombinationType;

            return ret;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    #endregion 

    #region 查询委托
    /// <summary>
    /// 查询报单
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQryOrderField : ITFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
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
        /// 开始时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertTimeStart;
        /// <summary>
        /// 结束时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertTimeEnd;

        public ushort FieldId
        {
            get { return 1792; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQryOrderField(LCThostFtdcQryOrderField input)
        {
            CThostFtdcQryOrderField ret = new CThostFtdcQryOrderField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.OrderSysID = input.OrderSysID;
            ret.InsertTimeStart = input.InsertTimeStart;
            ret.InsertTimeEnd = input.InsertTimeEnd;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQryOrderField : IFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
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
        /// 开始时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertTimeStart;
        /// <summary>
        /// 结束时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertTimeEnd;

        public ushort FieldId
        {
            get { return 1792; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQryOrderField(CThostFtdcQryOrderField input)
        {
            LCThostFtdcQryOrderField ret = new LCThostFtdcQryOrderField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.OrderSysID = input.OrderSysID;
            ret.InsertTimeStart = input.InsertTimeStart;
            ret.InsertTimeEnd = input.InsertTimeEnd;

            return ret;
        }
    }


    /// <summary>
    /// 报单
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcOrderField : ITFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderRef;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 报单价格条件
        /// </summary>
        public TThostFtdcOrderPriceTypeType OrderPriceType;
        /// <summary>
        /// 买卖方向
        /// </summary>
        public TThostFtdcDirectionType Direction;
        /// <summary>
        /// 组合开平标志
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string CombOffsetFlag;
        /// <summary>
        /// 组合投机套保标志
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string CombHedgeFlag;
        /// <summary>
        /// 价格
        /// </summary>
        public double LimitPrice;
        /// <summary>
        /// 数量
        /// </summary>
        public int VolumeTotalOriginal;
        /// <summary>
        /// 有效期类型
        /// </summary>
        public TThostFtdcTimeConditionType TimeCondition;
        /// <summary>
        /// GTD日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string GTDDate;
        /// <summary>
        /// 成交量类型
        /// </summary>
        public TThostFtdcVolumeConditionType VolumeCondition;
        /// <summary>
        /// 最小成交量
        /// </summary>
        public int MinVolume;
        /// <summary>
        /// 触发条件
        /// </summary>
        public TThostFtdcContingentConditionType ContingentCondition;
        /// <summary>
        /// 止损价
        /// </summary>
        public double StopPrice;
        /// <summary>
        /// 强平原因
        /// </summary>
        public TThostFtdcForceCloseReasonType ForceCloseReason;
        /// <summary>
        /// 自动挂起标志
        /// </summary>
        public int IsAutoSuspend;
        /// <summary>
        /// 业务单元
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string BusinessUnit;
        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID;
        /// <summary>
        /// 本地报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderLocalID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 会员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ParticipantID;
        /// <summary>
        /// 客户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClientID;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 交易所交易员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string TraderID;
        /// <summary>
        /// 安装编号
        /// </summary>
        public int InstallID;
        /// <summary>
        /// 报单提交状态
        /// </summary>
        public TThostFtdcOrderSubmitStatusType OrderSubmitStatus;
        /// <summary>
        /// 报单提示序号
        /// </summary>
        public int NotifySequence;
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 结算编号
        /// </summary>
        public int SettlementID;
        /// <summary>
        /// 报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;
        /// <summary>
        /// 报单来源
        /// </summary>
        public TThostFtdcOrderSourceType OrderSource;
        /// <summary>
        /// 报单状态
        /// </summary>
        public TThostFtdcOrderStatusType OrderStatus;
        /// <summary>
        /// 报单类型
        /// </summary>
        public TThostFtdcOrderTypeType OrderType;
        /// <summary>
        /// 今成交数量
        /// </summary>
        public int VolumeTraded;
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int VolumeTotal;
        /// <summary>
        /// 报单日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertDate;
        /// <summary>
        /// 委托时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertTime;
        /// <summary>
        /// 激活时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ActiveTime;
        /// <summary>
        /// 挂起时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SuspendTime;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string UpdateTime;
        /// <summary>
        /// 撤销时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string CancelTime;
        /// <summary>
        /// 最后修改交易所交易员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string ActiveTraderID;
        /// <summary>
        /// 结算会员编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClearingPartID;
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 前置编号
        /// </summary>
        public int FrontID;
        /// <summary>
        /// 会话编号
        /// </summary>
        public int SessionID;
        /// <summary>
        /// 用户端产品信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string UserProductInfo;
        /// <summary>
        /// 状态信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string StatusMsg;
        /// <summary>
        /// 用户强评标志
        /// </summary>
        public int UserForceClose;
        /// <summary>
        /// 操作用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ActiveUserID;
        /// <summary>
        /// 经纪公司报单编号
        /// </summary>
        public int BrokerOrderSeq;
        /// <summary>
        /// 相关报单
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string RelativeOrderSysID;
        /// <summary>
        /// 郑商所成交数量
        /// </summary>
        public int ZCETotalTradedVolume;
        /// <summary>
        /// 互换单标志
        /// </summary>
        public int IsSwapOrder;

        ///营业部编号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string BranchID;
        ///投资单元代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string InvestUnitID;
        ///资金账号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string AccountID;
        ///币种代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string CurrencyID;
        ///IP地址
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string IPAddress;
        ///Mac地址
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string MacAddress;

        public ushort FieldId
        {
            get { return 1025; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcOrderField(LCThostFtdcOrderField input)
        {
            CThostFtdcOrderField ret = new CThostFtdcOrderField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.OrderRef = input.OrderRef;
            ret.UserID = input.UserID;
            ret.OrderPriceType = input.OrderPriceType;
            ret.Direction = input.Direction;
            ret.CombOffsetFlag = input.CombOffsetFlag;
            ret.CombHedgeFlag = input.CombHedgeFlag;
            ret.LimitPrice = input.LimitPrice;
            ret.VolumeTotalOriginal = input.VolumeTotalOriginal;
            ret.TimeCondition = input.TimeCondition;
            ret.GTDDate = input.GTDDate;
            ret.VolumeCondition = input.VolumeCondition;
            ret.MinVolume = input.MinVolume;
            ret.ContingentCondition = input.ContingentCondition;
            ret.StopPrice = input.StopPrice;
            ret.ForceCloseReason = input.ForceCloseReason;
            ret.IsAutoSuspend = input.IsAutoSuspend;
            ret.BusinessUnit = input.BusinessUnit;
            ret.RequestID = input.RequestID;
            ret.OrderLocalID = input.OrderLocalID;
            ret.ExchangeID = input.ExchangeID;
            ret.ParticipantID = input.ParticipantID;
            ret.ClientID = input.ClientID;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.TraderID = input.TraderID;
            ret.InstallID = input.InstallID;
            ret.OrderSubmitStatus = input.OrderSubmitStatus;
            ret.NotifySequence = input.NotifySequence;
            ret.TradingDay = input.TradingDay;
            ret.SettlementID = input.SettlementID;
            ret.OrderSysID = input.OrderSysID;
            ret.OrderSource = input.OrderSource;
            ret.OrderStatus = input.OrderStatus;
            ret.OrderType = input.OrderType;
            ret.VolumeTraded = input.VolumeTraded;
            ret.VolumeTotal = input.VolumeTotal;
            ret.InsertDate = input.InsertDate;
            ret.InsertTime = input.InsertTime;
            ret.ActiveTime = input.ActiveTime;
            ret.SuspendTime = input.SuspendTime;
            ret.UpdateTime = input.UpdateTime;
            ret.CancelTime = input.CancelTime;
            ret.ActiveTraderID = input.ActiveTraderID;
            ret.ClearingPartID = input.ClearingPartID;
            ret.SequenceNo = input.SequenceNo;
            ret.FrontID = input.FrontID;
            ret.SessionID = input.SessionID;
            ret.UserProductInfo = input.UserProductInfo;
            ret.StatusMsg = input.StatusMsg;
            ret.UserForceClose = input.UserForceClose;
            ret.ActiveUserID = input.ActiveUserID;
            ret.BrokerOrderSeq = input.BrokerOrderSeq;
            ret.RelativeOrderSysID = input.RelativeOrderSysID;
            ret.ZCETotalTradedVolume = input.ZCETotalTradedVolume;
            ret.IsSwapOrder = input.IsSwapOrder;

            ret.BranchID = input.BranchID;
            ret.InvestUnitID = input.InvestUnitID;
            ret.AccountID = input.AccountID;
            ret.CurrencyID = input.CurrencyID;
            ret.IPAddress = input.IPAddress;
            ret.MacAddress = input.MacAddress;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcOrderField : IFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderRef;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
        /// <summary>
        /// 报单价格条件
        /// </summary>
        public TThostFtdcOrderPriceTypeType OrderPriceType;
        /// <summary>
        /// 买卖方向
        /// </summary>
        public TThostFtdcDirectionType Direction;
        /// <summary>
        /// 组合开平标志
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string CombOffsetFlag;
        /// <summary>
        /// 组合投机套保标志
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 5)]
        public string CombHedgeFlag;
        /// <summary>
        /// 价格
        /// </summary>
        public double LimitPrice;
        /// <summary>
        /// 数量
        /// </summary>
        public int VolumeTotalOriginal;
        /// <summary>
        /// 有效期类型
        /// </summary>
        public TThostFtdcTimeConditionType TimeCondition;
        /// <summary>
        /// GTD日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string GTDDate;
        /// <summary>
        /// 成交量类型
        /// </summary>
        public TThostFtdcVolumeConditionType VolumeCondition;
        /// <summary>
        /// 最小成交量
        /// </summary>
        public int MinVolume;
        /// <summary>
        /// 触发条件
        /// </summary>
        public TThostFtdcContingentConditionType ContingentCondition;
        /// <summary>
        /// 止损价
        /// </summary>
        public double StopPrice;
        /// <summary>
        /// 强平原因
        /// </summary>
        public TThostFtdcForceCloseReasonType ForceCloseReason;
        /// <summary>
        /// 自动挂起标志
        /// </summary>
        public int IsAutoSuspend;
        /// <summary>
        /// 业务单元
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string BusinessUnit;
        /// <summary>
        /// 请求编号
        /// </summary>
        public int RequestID;
        /// <summary>
        /// 本地报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderLocalID;
        /// <summary>
        /// 交易所代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ExchangeID;
        /// <summary>
        /// 会员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ParticipantID;
        /// <summary>
        /// 客户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClientID;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 交易所交易员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string TraderID;
        /// <summary>
        /// 安装编号
        /// </summary>
        public int InstallID;
        /// <summary>
        /// 报单提交状态
        /// </summary>
        public TThostFtdcOrderSubmitStatusType OrderSubmitStatus;
        /// <summary>
        /// 报单提示序号
        /// </summary>
        public int NotifySequence;
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 结算编号
        /// </summary>
        public int SettlementID;
        /// <summary>
        /// 报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;
        /// <summary>
        /// 报单来源
        /// </summary>
        public TThostFtdcOrderSourceType OrderSource;
        /// <summary>
        /// 报单状态
        /// </summary>
        public TThostFtdcOrderStatusType OrderStatus;
        /// <summary>
        /// 报单类型
        /// </summary>
        public TThostFtdcOrderTypeType OrderType;
        /// <summary>
        /// 今成交数量
        /// </summary>
        public int VolumeTraded;
        /// <summary>
        /// 剩余数量
        /// </summary>
        public int VolumeTotal;
        /// <summary>
        /// 报单日期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertDate;
        /// <summary>
        /// 委托时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string InsertTime;
        /// <summary>
        /// 激活时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string ActiveTime;
        /// <summary>
        /// 挂起时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string SuspendTime;
        /// <summary>
        /// 最后修改时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string UpdateTime;
        /// <summary>
        /// 撤销时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string CancelTime;
        /// <summary>
        /// 最后修改交易所交易员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string ActiveTraderID;
        /// <summary>
        /// 结算会员编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClearingPartID;
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 前置编号
        /// </summary>
        public int FrontID;
        /// <summary>
        /// 会话编号
        /// </summary>
        public int SessionID;
        /// <summary>
        /// 用户端产品信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string UserProductInfo;
        /// <summary>
        /// 状态信息
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string StatusMsg;
        /// <summary>
        /// 用户强评标志
        /// </summary>
        public int UserForceClose;
        /// <summary>
        /// 操作用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ActiveUserID;
        /// <summary>
        /// 经纪公司报单编号
        /// </summary>
        public int BrokerOrderSeq;
        /// <summary>
        /// 相关报单
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string RelativeOrderSysID;
        /// <summary>
        /// 郑商所成交数量
        /// </summary>
        public int ZCETotalTradedVolume;
        /// <summary>
        /// 互换单标志
        /// </summary>
        public int IsSwapOrder;

        ///营业部编号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string BranchID;
        ///投资单元代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 17)]
        public string InvestUnitID;
        ///资金账号
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string AccountID;
        ///币种代码
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 4)]
        public string CurrencyID;
        ///IP地址
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string IPAddress;
        ///Mac地址
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string MacAddress;
        public ushort FieldId
        {
            get { return 1025; }
        }

        public void Swap()
        {
            LimitPrice = ByteSwapHelp.ReverseBytes(LimitPrice);
            VolumeTotalOriginal = ByteSwapHelp.ReverseBytes(VolumeTotalOriginal);
            MinVolume = ByteSwapHelp.ReverseBytes(MinVolume);
            StopPrice = ByteSwapHelp.ReverseBytes(StopPrice);
            IsAutoSuspend = ByteSwapHelp.ReverseBytes(IsAutoSuspend);
            RequestID = ByteSwapHelp.ReverseBytes(RequestID);
            InstallID = ByteSwapHelp.ReverseBytes(InstallID);
            NotifySequence = ByteSwapHelp.ReverseBytes(NotifySequence);
            SettlementID = ByteSwapHelp.ReverseBytes(SettlementID);
            VolumeTraded = ByteSwapHelp.ReverseBytes(VolumeTraded);
            VolumeTotal = ByteSwapHelp.ReverseBytes(VolumeTotal);
            SequenceNo = ByteSwapHelp.ReverseBytes(SequenceNo);
            FrontID = ByteSwapHelp.ReverseBytes(FrontID);
            SessionID = ByteSwapHelp.ReverseBytes(SessionID);
            UserForceClose = ByteSwapHelp.ReverseBytes(UserForceClose);
            BrokerOrderSeq = ByteSwapHelp.ReverseBytes(BrokerOrderSeq);
            ZCETotalTradedVolume = ByteSwapHelp.ReverseBytes(ZCETotalTradedVolume);
            IsSwapOrder = ByteSwapHelp.ReverseBytes(IsSwapOrder);
        }

        public static implicit operator LCThostFtdcOrderField(CThostFtdcOrderField input)
        {
            LCThostFtdcOrderField ret = new LCThostFtdcOrderField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.OrderRef = input.OrderRef;
            ret.UserID = input.UserID;
            ret.OrderPriceType = input.OrderPriceType;
            ret.Direction = input.Direction;
            ret.CombOffsetFlag = input.CombOffsetFlag;
            ret.CombHedgeFlag = input.CombHedgeFlag;
            ret.LimitPrice = input.LimitPrice;
            ret.VolumeTotalOriginal = input.VolumeTotalOriginal;
            ret.TimeCondition = input.TimeCondition;
            ret.GTDDate = input.GTDDate;
            ret.VolumeCondition = input.VolumeCondition;
            ret.MinVolume = input.MinVolume;
            ret.ContingentCondition = input.ContingentCondition;
            ret.StopPrice = input.StopPrice;
            ret.ForceCloseReason = input.ForceCloseReason;
            ret.IsAutoSuspend = input.IsAutoSuspend;
            ret.BusinessUnit = input.BusinessUnit;
            ret.RequestID = input.RequestID;
            ret.OrderLocalID = input.OrderLocalID;
            ret.ExchangeID = input.ExchangeID;
            ret.ParticipantID = input.ParticipantID;
            ret.ClientID = input.ClientID;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.TraderID = input.TraderID;
            ret.InstallID = input.InstallID;
            ret.OrderSubmitStatus = input.OrderSubmitStatus;
            ret.NotifySequence = input.NotifySequence;
            ret.TradingDay = input.TradingDay;
            ret.SettlementID = input.SettlementID;
            ret.OrderSysID = input.OrderSysID;
            ret.OrderSource = input.OrderSource;
            ret.OrderStatus = input.OrderStatus;
            ret.OrderType = input.OrderType;
            ret.VolumeTraded = input.VolumeTraded;
            ret.VolumeTotal = input.VolumeTotal;
            ret.InsertDate = input.InsertDate;
            ret.InsertTime = input.InsertTime;
            ret.ActiveTime = input.ActiveTime;
            ret.SuspendTime = input.SuspendTime;
            ret.UpdateTime = input.UpdateTime;
            ret.CancelTime = input.CancelTime;
            ret.ActiveTraderID = input.ActiveTraderID;
            ret.ClearingPartID = input.ClearingPartID;
            ret.SequenceNo = input.SequenceNo;
            ret.FrontID = input.FrontID;
            ret.SessionID = input.SessionID;
            ret.UserProductInfo = input.UserProductInfo;
            ret.StatusMsg = input.StatusMsg;
            ret.UserForceClose = input.UserForceClose;
            ret.ActiveUserID = input.ActiveUserID;
            ret.BrokerOrderSeq = input.BrokerOrderSeq;
            ret.RelativeOrderSysID = input.RelativeOrderSysID;
            ret.ZCETotalTradedVolume = input.ZCETotalTradedVolume;
            ret.IsSwapOrder = input.IsSwapOrder;

            ret.BranchID = input.BranchID;
            ret.InvestUnitID = input.InvestUnitID;
            ret.AccountID = input.AccountID;
            ret.CurrencyID = input.CurrencyID;
            ret.IPAddress = input.IPAddress;
            ret.MacAddress = input.MacAddress;


            return ret;
        }
    }

    #endregion

    #region 查询成交
    /// <summary>
    /// 查询成交
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcQryTradeField : ITFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
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
        /// 开始时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeTimeStart;
        /// <summary>
        /// 结束时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeTimeEnd;

        public ushort FieldId
        {
            get { return 1793; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcQryTradeField(LCThostFtdcQryTradeField input)
        {
            CThostFtdcQryTradeField ret = new CThostFtdcQryTradeField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.TradeID = input.TradeID;
            ret.TradeTimeStart = input.TradeTimeStart;
            ret.TradeTimeEnd = input.TradeTimeEnd;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcQryTradeField : IFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
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
        /// 开始时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeTimeStart;
        /// <summary>
        /// 结束时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeTimeEnd;

        public ushort FieldId
        {
            get { return 1793; }
        }

        public void Swap()
        { }

        public static implicit operator LCThostFtdcQryTradeField(CThostFtdcQryTradeField input)
        {
            LCThostFtdcQryTradeField ret = new LCThostFtdcQryTradeField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.ExchangeID = input.ExchangeID;
            ret.TradeID = input.TradeID;
            ret.TradeTimeStart = input.TradeTimeStart;
            ret.TradeTimeEnd = input.TradeTimeEnd;

            return ret;
        }
    }

    /// <summary>
    /// 成交
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CThostFtdcTradeField : ITFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderRef;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
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
        /// 买卖方向
        /// </summary>
        public TThostFtdcDirectionType Direction;
        /// <summary>
        /// 报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;
        /// <summary>
        /// 会员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ParticipantID;
        /// <summary>
        /// 客户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClientID;
        /// <summary>
        /// 交易角色
        /// </summary>
        public TThostFtdcTradingRoleType TradingRole;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 开平标志
        /// </summary>
        public TThostFtdcOffsetFlagType OffsetFlag;
        /// <summary>
        /// 投机套保标志
        /// </summary>
        public TThostFtdcHedgeFlagType HedgeFlag;
        /// <summary>
        /// 价格
        /// </summary>
        public double Price;
        /// <summary>
        /// 数量
        /// </summary>
        public int Volume;
        /// <summary>
        /// 成交时期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeDate;
        /// <summary>
        /// 成交时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeTime;
        /// <summary>
        /// 成交类型
        /// </summary>
        public TThostFtdcTradeTypeType TradeType;
        /// <summary>
        /// 成交价来源
        /// </summary>
        public TThostFtdcPriceSourceType PriceSource;
        /// <summary>
        /// 交易所交易员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string TraderID;
        /// <summary>
        /// 本地报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderLocalID;
        /// <summary>
        /// 结算会员编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClearingPartID;
        /// <summary>
        /// 业务单元
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string BusinessUnit;
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 结算编号
        /// </summary>
        public int SettlementID;
        /// <summary>
        /// 经纪公司报单编号
        /// </summary>
        public int BrokerOrderSeq;
        /// <summary>
        /// 成交来源
        /// </summary>
        public TThostFtdcTradeSourceType TradeSource;

        public ushort FieldId
        {
            get { return 1033; }
        }

        public void Swap()
        { }

        public static implicit operator CThostFtdcTradeField(LCThostFtdcTradeField input)
        {
            CThostFtdcTradeField ret = new CThostFtdcTradeField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.OrderRef = input.OrderRef;
            ret.UserID = input.UserID;
            ret.ExchangeID = input.ExchangeID;
            ret.TradeID = input.TradeID;
            ret.Direction = input.Direction;
            ret.OrderSysID = input.OrderSysID;
            ret.ParticipantID = input.ParticipantID;
            ret.ClientID = input.ClientID;
            ret.TradingRole = input.TradingRole;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.OffsetFlag = input.OffsetFlag;
            ret.HedgeFlag = input.HedgeFlag;
            ret.Price = input.Price;
            ret.Volume = input.Volume;
            ret.TradeDate = input.TradeDate;
            ret.TradeTime = input.TradeTime;
            ret.TradeType = input.TradeType;
            ret.PriceSource = input.PriceSource;
            ret.TraderID = input.TraderID;
            ret.OrderLocalID = input.OrderLocalID;
            ret.ClearingPartID = input.ClearingPartID;
            ret.BusinessUnit = input.BusinessUnit;
            ret.SequenceNo = input.SequenceNo;
            ret.TradingDay = input.TradingDay;
            ret.SettlementID = input.SettlementID;
            ret.BrokerOrderSeq = input.BrokerOrderSeq;
            ret.TradeSource = input.TradeSource;

            return ret;
        }
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LCThostFtdcTradeField : IFieldId
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
        /// <summary>
        /// 合约代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string InstrumentID;
        /// <summary>
        /// 报单引用
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderRef;
        /// <summary>
        /// 用户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string UserID;
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
        /// 买卖方向
        /// </summary>
        public TThostFtdcDirectionType Direction;
        /// <summary>
        /// 报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string OrderSysID;
        /// <summary>
        /// 会员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ParticipantID;
        /// <summary>
        /// 客户代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClientID;
        /// <summary>
        /// 交易角色
        /// </summary>
        public TThostFtdcTradingRoleType TradingRole;
        /// <summary>
        /// 合约在交易所的代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 31)]
        public string ExchangeInstID;
        /// <summary>
        /// 开平标志
        /// </summary>
        public TThostFtdcOffsetFlagType OffsetFlag;
        /// <summary>
        /// 投机套保标志
        /// </summary>
        public TThostFtdcHedgeFlagType HedgeFlag;
        /// <summary>
        /// 价格
        /// </summary>
        public double Price;
        /// <summary>
        /// 数量
        /// </summary>
        public int Volume;
        /// <summary>
        /// 成交时期
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeDate;
        /// <summary>
        /// 成交时间
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradeTime;
        /// <summary>
        /// 成交类型
        /// </summary>
        public TThostFtdcTradeTypeType TradeType;
        /// <summary>
        /// 成交价来源
        /// </summary>
        public TThostFtdcPriceSourceType PriceSource;
        /// <summary>
        /// 交易所交易员代码
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string TraderID;
        /// <summary>
        /// 本地报单编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 13)]
        public string OrderLocalID;
        /// <summary>
        /// 结算会员编号
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 11)]
        public string ClearingPartID;
        /// <summary>
        /// 业务单元
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 21)]
        public string BusinessUnit;
        /// <summary>
        /// 序号
        /// </summary>
        public int SequenceNo;
        /// <summary>
        /// 交易日
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 9)]
        public string TradingDay;
        /// <summary>
        /// 结算编号
        /// </summary>
        public int SettlementID;
        /// <summary>
        /// 经纪公司报单编号
        /// </summary>
        public int BrokerOrderSeq;
        /// <summary>
        /// 成交来源
        /// </summary>
        public TThostFtdcTradeSourceType TradeSource;

        public ushort FieldId
        {
            get { return 1033; }
        }

        public void Swap()
        {
            Price = ByteSwapHelp.ReverseBytes(Price);
            Volume = ByteSwapHelp.ReverseBytes(Volume);
            SequenceNo = ByteSwapHelp.ReverseBytes(SequenceNo);
            SettlementID = ByteSwapHelp.ReverseBytes(SettlementID);
            BrokerOrderSeq = ByteSwapHelp.ReverseBytes(BrokerOrderSeq);
        }

        public static implicit operator LCThostFtdcTradeField(CThostFtdcTradeField input)
        {
            LCThostFtdcTradeField ret = new LCThostFtdcTradeField();
            ret.BrokerID = input.BrokerID;
            ret.InvestorID = input.InvestorID;
            ret.InstrumentID = input.InstrumentID;
            ret.OrderRef = input.OrderRef;
            ret.UserID = input.UserID;
            ret.ExchangeID = input.ExchangeID;
            ret.TradeID = input.TradeID;
            ret.Direction = input.Direction;
            ret.OrderSysID = input.OrderSysID;
            ret.ParticipantID = input.ParticipantID;
            ret.ClientID = input.ClientID;
            ret.TradingRole = input.TradingRole;
            ret.ExchangeInstID = input.ExchangeInstID;
            ret.OffsetFlag = input.OffsetFlag;
            ret.HedgeFlag = input.HedgeFlag;
            ret.Price = input.Price;
            ret.Volume = input.Volume;
            ret.TradeDate = input.TradeDate;
            ret.TradeTime = input.TradeTime;
            ret.TradeType = input.TradeType;
            ret.PriceSource = input.PriceSource;
            ret.TraderID = input.TraderID;
            ret.OrderLocalID = input.OrderLocalID;
            ret.ClearingPartID = input.ClearingPartID;
            ret.BusinessUnit = input.BusinessUnit;
            ret.SequenceNo = input.SequenceNo;
            ret.TradingDay = input.TradingDay;
            ret.SettlementID = input.SettlementID;
            ret.BrokerOrderSeq = input.BrokerOrderSeq;
            ret.TradeSource = input.TradeSource;

            return ret;
        }
    }
    #endregion

}
