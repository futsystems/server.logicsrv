using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CTPService.Struct
{
    public interface IByteSwap
    {
        void Swap();
    }

    public interface IFieldId : IByteSwap
    {
        ushort FieldId { get; }

    }


    public interface ITFieldId : IByteSwap
    {
        ushort FieldId { get; }
    }

    /// <summary>
    /// FTD协议定长报头
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct proto_hdr : IByteSwap
    {
        /// <summary>
        /// FTD报文类型
        /// </summary>
        public byte bFtdtype;
        /// <summary>
        /// FTD扩充长度
        /// </summary>
        public byte bExLen;
        /// <summary>
        /// 信息正文长度
        /// </summary>
        public ushort wPktLen;

        public void Swap()
        {
            wPktLen = ByteSwapHelp.ReverseBytes(wPktLen);
        }
    }

    /// <summary>
    /// FTD正文报头
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ftd_hdr : IByteSwap
    {
        public byte bHead;

        /// <summary>
        /// 压缩类别 0为无,3为LZ
        /// </summary>
        public byte bEnctype;

        /// <summary>
        /// 版本号
        /// </summary>
        public byte bVersion;

        /// <summary>
        /// 报文链 
        /// 76 L
        /// 67 C
        /// 
        /// </summary>
        public byte bChain;

        /// <summary>
        /// 类别0-req,2-rtn,4-qry
        /// </summary>
        public ushort wSeqSn;

        /// <summary>
        /// tid
        /// </summary>
        public uint dTransId;

        /// <summary>
        /// 序列号
        /// </summary>
        public uint dSeqNo;

        /// <summary>
        /// 数据域数量
        /// </summary>
        public ushort wFiCount;

        /// <summary>
        /// FTDC信息正文长度 未压缩长度 ,包含(tid+len)
        /// </summary>
        public ushort wFtdcLen;

        /// <summary>
        /// 请求编号
        /// </summary>
        public uint dReqId;

        public void Swap()
        {
            wSeqSn = ByteSwapHelp.ReverseBytes(wSeqSn);
            dTransId = ByteSwapHelp.ReverseBytes(dTransId);
            dSeqNo = ByteSwapHelp.ReverseBytes(dSeqNo);
            wFiCount = ByteSwapHelp.ReverseBytes(wFiCount);
            wFtdcLen = ByteSwapHelp.ReverseBytes(wFtdcLen);
            dReqId = ByteSwapHelp.ReverseBytes(dReqId);
        }
    }

    /// <summary>
    /// 协议头 + FTD头 用于填充从后生成数据包
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct proftd_hdr
    {
        public proto_hdr proto_hdr;

        public ftd_hdr ftd_hdr;
    }
    /// <summary>
    /// ftdc头
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ftdc_hdr : IByteSwap
    {
        public ushort wFiId;

        public ushort wFiLen;

        public void Swap()
        {
            wFiId = ByteSwapHelp.ReverseBytes(wFiId);
            wFiLen = ByteSwapHelp.ReverseBytes(wFiLen);
        }
    }

    /// <summary>
    /// FTDC正文数据包
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PktData<T>
    {
        public ftdc_hdr FTDCHeader;

        public IFieldId FTDCData;

        public object Data;
    }
}
