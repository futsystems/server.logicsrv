using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SuperSocket.SocketBase;
using SuperSocket.SocketBase.Protocol;
using SuperSocket.Facility.Protocol;
using TradingLib.API;
using TradingLib.Common;
using System.Runtime.InteropServices;


namespace CTPService
{
    /// <summary>
    /// 头部定长协议
    /// 用于头部长度固定，头部中包含消息体长度的协议类型
    /// </summary>
    public class TLReceiveFilter : FixedHeaderReceiveFilter<TLRequestInfo>
    {
        public TLReceiveFilter()
            : base(4)
        { 
            
        }
        const int HEADERSIZE =4;
        //const int LENGTHOFFSET = 0;
        //const int TYPEOFFSET = 4;

        const int FTDTYPEOFFSET = 0;
        const int EXLENOFFSET = 1;
        const int PKTLENOFFSET = 2;
        /// <summary>
        /// 从定长头部获得消息体长度
        /// TradingLib Message协议定义 4字节消息长度(包含头长度和消息体长度) 4字节消息类型 以及消息体
        /// length type body
        /// </summary>
        /// <param name="header"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            //MessageTypes type = (MessageTypes)BitConverter.ToInt32(header, offset + TYPEOFFSET);
            int bFtdtype = header[offset + FTDTYPEOFFSET];
            int bExLen = header[offset + EXLENOFFSET];
            int wPktLen = Endian.SwapInt16(BitConverter.ToInt16(header, offset + PKTLENOFFSET));

            return bExLen + wPktLen;
            //return 0;
            //返回消息体长度
            //return totallen - HEADERSIZE;
        }

        /// <summary>
        /// 从消息体数据中解析RequestInfo
        /// </summary>
        /// <param name="header"></param>
        /// <param name="bodyBuffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        protected override TLRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {

            int bFtdtype = header.Array[FTDTYPEOFFSET];
            int bExLen = header.Array[EXLENOFFSET];
            int wPktLen = Endian.SwapInt16((BitConverter.ToInt16(header.Array, PKTLENOFFSET)));//字节序变化

            byte[] data = new byte[length];
            Array.Copy(bodyBuffer, offset, data, 0, length);

            if (bExLen > 0)
            {
                int ftdTag = (int)data[0];
                if (ftdTag == (int)EnumFTDTagType.FTDTagKeepAlive)
                { 
                    //心跳
                }
                if (ftdTag == 7)
                { 
                    //连接后第一个数据包初始化
                }
            }

            if (data.Length > 100)
            {

                //解析FTD报头
                ftd_hdr ftd_hdr = ByteSwapHelp.BytesToStruct<ftd_hdr>(data);
                
                //解析FTDC报头
                ftdc_hdr ftdc_hdr = ByteSwapHelp.BytesToStruct<ftdc_hdr>(data, Constanst.FTD_HDRLEN);

                //解析FTDC中的业务结构体
                LCThostFtdcReqUserLoginField obj2 = ByteSwapHelp.BytesToStruct<LCThostFtdcReqUserLoginField>(data, Constanst.FTD_HDRLEN + Constanst.FTDC_HDRLEN);

                //解析后面4个公私有流的类型 此处一共5个FTDC包
                TdTpPkt topic = ByteSwapHelp.BytesToStruct<TdTpPkt>(data, Constanst.FTD_HDRLEN + Constanst.FTDC_HDRLEN + ftdc_hdr.wFiLen);

             
            }

            return null;// new TLRequestInfo(type.ToString(), content, message);
        }

          
    }
}
