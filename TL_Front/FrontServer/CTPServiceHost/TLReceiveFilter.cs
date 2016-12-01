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
using CTPService.Struct;
using CTPService.Struct.V12;


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

            EnumFTDType bFtdtype = (EnumFTDType)header.Array[FTDTYPEOFFSET];
            int bExLen = header.Array[EXLENOFFSET];
            int wPktLen = Endian.SwapInt16((BitConverter.ToInt16(header.Array, PKTLENOFFSET)));//字节序变化

            byte[] data = new byte[length];
            Array.Copy(bodyBuffer, offset, data, 0, length);
            
            string key = string.Empty;
            EnumFTDTagType ftdTag = EnumFTDTagType.FTDTagUnknown;
            ftd_hdr ftdhdr = new ftd_hdr();
            List<PktData<IFieldId>> fieldList = new List<PktData<IFieldId>>();

            if (bFtdtype == EnumFTDType.FTDTypeNone)
            {
                ftdTag = (EnumFTDTagType)(int)data[0];
                if (ftdTag ==  EnumFTDTagType.FTDTagRegister)
                { 
                    //连接后第一个数据包初始化
                    key = string.Format("REGISTER {0} {1}", bFtdtype, ftdTag);
                }
                if (ftdTag == EnumFTDTagType.FTDTagKeepAlive)
                {
                    //心跳
                    key = string.Format("HEARTBEAT {0} {1}",bFtdtype,ftdTag);
                }
            }


            if (bFtdtype == EnumFTDType.FTDTypeFTDC)
            {
                try
                {
                    //解析FTD报头
                    ftdhdr = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(data);
                    //根据tid进行数据解析
                    switch ((EnumTransactionID)ftdhdr.dTransId)
                    {

                        //用户登录请求 ReqUserLogin
                        case EnumTransactionID.T_REQ_LOGIN:
                            {
                                fieldList = ParsePktDataV12(data, ftdhdr.wFtdcLen, ftdhdr.wFiCount);
                                break;
                            }
                        //请求查询投资者 ReqQryInvestor
                        case EnumTransactionID.T_QRY_USRINF:
                            {
                                fieldList = ParsePktDataV12(data, ftdhdr.wFtdcLen, ftdhdr.wFiCount);
                                break;
                            }
                        default:
                            throw new Exception(string.Format("TransactionID:{0} pkt not handled", (EnumTransactionID)ftdhdr.dTransId));
                    }
                    key = string.Format("FTD Data Ver:{0} Tid:{1} SeqType:{2} SeqNo:{3} Enc:{4} ReqID:{5} Chain:{6} FieldCnt:{7} FtdcLen:{8}", ftdhdr.bVersion, ftdhdr.dTransId, (EnumSeqType)ftdhdr.wSeqSn, ftdhdr.dSeqNo, (EnumEncType)ftdhdr.bEnctype, ftdhdr.dReqId, (EnumChainType)ftdhdr.bChain, ftdhdr.wFiCount, ftdhdr.wFtdcLen);
                }
                catch (Exception ex)
                {
                    key = ex.ToString();
                }
            }
            return new TLRequestInfo(key, bFtdtype,ftdTag,ftdhdr,fieldList,data);
        }


        /// <summary>
        /// 从数据包中解析 业务数据
        /// </summary>
        /// <param name="data"></param>
        /// <param name="len"></param>
        /// <param name="cnt"></param>
        /// <returns></returns>
        List<PktData<IFieldId>> ParsePktDataV12(byte[] data,int len, int cnt)
        {
            List<PktData<IFieldId>> list = new List<PktData<IFieldId>>(); 
            int offset = 0;
            for (int i = 0; i < cnt; i++)
            {
                Struct.ftdc_hdr ftdc_hdr = ByteSwapHelp.BytesToStruct<Struct.ftdc_hdr>(data, offset + Constanst.FTD_HDRLEN);
                EnumFiledID fieldID = (EnumFiledID)ftdc_hdr.wFiId;
                IFieldId ftdc_data = StructHelperV12.BytesToStruct(data, offset + Constanst.FTD_HDRLEN + Constanst.FTDC_HDRLEN, fieldID);

                list.Add(new PktData<IFieldId>() { FTDCHeader = ftdc_hdr, FTDCData = ftdc_data ,Data = ftdc_data});

                offset += Constanst.FTDC_HDRLEN + ftdc_hdr.wFiLen;
            }
            return list;
        }
          

    }
}
