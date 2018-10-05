using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

/*
 *  XLProtocol 为头部定长协议
 * 
 *     协议头                 正文头             域头           域
 * |---XLProtocolHeader---|---XLDataHeader---|---XLFieldHeader--XLField---1|---XLFieldHeader--XLField---2|......
 *  
 * 
 * XLProtocolHeader 4个字节
 * XLDataHeader 16个字节
 * 
 * 
 * 
 * 
 * */
namespace TradingLib.XLProtocol
{
    /// <summary>
    /// 协议头 4字节
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLProtocolHeader
    {
        /// <summary>
        /// 数据包类型
        /// </summary>
        public short XLMessageType;

        /// <summary>
        /// 数据包长度 不包含协议头4个字节
        /// </summary>
        public ushort XLMessageLength;

    }


    /// <summary>
    /// 正文头 16个字节
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLDataHeader
    {
        /// <summary>
        /// 压缩类别
        /// </summary>
        public byte Enctype;

        /// <summary>
        /// 版本号
        /// </summary>
        public byte Version;

        /// <summary>
        /// 是否是最后一条回报
        /// </summary>
        public byte IsLast;

        /// <summary>
        /// 响应序列号类别
        /// </summary>
        public byte SeqType;

        /// <summary>
        /// 回报序列号
        /// </summary>
        public uint SeqNo;

        /// <summary>
        /// 请求编号
        /// </summary>
        public uint RequestID;

        /// <summary>
        /// 数据域个数
        /// </summary>
        public ushort FieldCount;

        /// <summary>
        /// 数据长度 未压缩长度 包含 FieldHeader, FieldData
        /// </summary>
        public ushort FieldLength;

        
    }

    /// <summary>
    /// 域头
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct XLFieldHeader
    {
        /// <summary>
        /// 业务结构体ID
        /// </summary>
        public ushort FieldID;

        /// <summary>
        /// 业务结构体长度
        /// </summary>
        public ushort FieldLength;
    }



    /// <summary>
    /// 数据域结构体FieldID接口
    /// </summary>
    public interface IXLField
    {
        ushort FieldID { get; }
    }


    /// <summary>
    /// 错误消息结构体
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ErrorField:IXLField
    {
        /// <summary>
        /// 错误代码
        /// </summary>
        public int ErrorID;
        /// <summary>
        /// 错误信息
        /// </summary>
        //[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        //public string ErrorMsg;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 81)]
        public byte[] ErrorMsg;

        /// <summary>
        /// 域ID
        /// </summary>
        public ushort FieldID { get { return (ushort)XLFieldType.F_ERROR; } }
    }

}
