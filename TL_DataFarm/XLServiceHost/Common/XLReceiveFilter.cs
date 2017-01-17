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
using Common.Logging;
using TradingLib.XLProtocol;


namespace XLServiceHost
{
    /// <summary>
    /// 头部定长协议
    /// 用于头部长度固定，头部中包含消息体长度的协议类型
    /// </summary>
    public class XLReceiveFilter : FixedHeaderReceiveFilter<XLRequestInfo>
    {
        ILog logger = LogManager.GetLogger("XLReceiveFilter");

        public XLReceiveFilter()
            : base(4)
        {

        }
        const int HEADERSIZE = 4;
        const int XLTYPEOFFSET = 0;
        const int XLLENOFFSET = 2;
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
            //int bFtdtype = header[offset + FTDTYPEOFFSET];
            int wPktLen = BitConverter.ToUInt16(header, offset + XLLENOFFSET);

            return wPktLen;
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
        protected override XLRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {

            try
            {
                XLProtocolHeader protoHeader = XLStructHelp.BytesToStruct<XLProtocolHeader>(header.Array, 0);
                XLDataHeader dataHeader;
                XLPacketData pkt = XLPacketData.Deserialize((XLMessageType)protoHeader.XLMessageType, bodyBuffer, offset, out dataHeader);

                string key = string.Format("XL Data Ver:{0} Tid:{1} SeqType:{2} SeqNo:{3} Enc:{4} ReqID:{5} Chain:{6} FieldCnt:{7} FtdcLen:{8}", dataHeader.Version, protoHeader.XLMessageType, (XLEnumSeqType)dataHeader.SeqType, dataHeader.SeqNo, dataHeader.Enctype, dataHeader.RequestID, dataHeader.IsLast, dataHeader.FieldCount, dataHeader.FieldLength);

                return new XLRequestInfo(key, dataHeader, pkt);
            }
            catch (Exception ex)
            {
                logger.Error("Request Parse Error:" + ex.ToString());
                return null;
            }
        }




    }
}
