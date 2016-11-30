using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using CTPService.Struct;

namespace CTPService.Struct.V12
{
    public class StructHelperV12
    {

        public static IFieldId BytesToStruct(byte[] data, int offset, EnumFiledID fieldID)
        {
            switch (fieldID)
            {
                    //设定数据流业务数据结构体
                case EnumFiledID.F_REQ_SEQSN:
                    {
                        return ByteSwapHelp.BytesToStruct<TopicData>(data, offset);
                    }
                    //请求登入业务结构体提
                case EnumFiledID.F_REQ_LOGIN:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcReqUserLoginField>(data, offset);
                    }
                default:
                    throw new Exception(string.Format("FieldID:{0} pkt not handled", fieldID));
            }
        }

        static void InitFTDCHeader(ref ftdc_hdr hdr,int size,EnumFiledID fieldID)
        {
            hdr.wFiId = (ushort)fieldID;
            hdr.wFiLen = (ushort)size;
        }

        public static void FillRspHeader(ref proto_hdr proto_hdr, ref ftd_hdr ftd_hdr, ushort pktLen, EnumSeqType seqType, EnumTransactionID transId, ushort fieldCount, uint reqId)
        {
            proto_hdr.bFtdtype = (byte)EnumFTDType.FTDTypeFTDC;
            proto_hdr.bExLen = 0;
            proto_hdr.wPktLen = (ushort)(pktLen - 4);

            ftd_hdr.bHead = Constanst.FTDC_HEAD;
            ftd_hdr.bVersion = Constanst.FTDC_VER;
            ftd_hdr.bEnctype = Constanst.THOST_ENC_NONE;
            ftd_hdr.bChain = (byte)'L';
            ftd_hdr.wSeqSn = (ushort)seqType;
            ftd_hdr.dTransId = (uint)transId;
            ftd_hdr.dSeqNo = 0;//这里应该是递增的变量 需改动 
            ftd_hdr.wFiCount = fieldCount;
            ftd_hdr.wFtdcLen = (ushort)(pktLen - 4 - Constanst.FTD_HDRLEN);
            ftd_hdr.dReqId = reqId;
        }

        public static byte[] FillRsp<T>(ref LCThostFtdcRspInfoField rsp, ref T field, EnumSeqType seqType, EnumTransactionID transId, int fieldCount, int reqId)
            where T:IByteSwap
        {
            //proftd_hdr proftdHeader = new proftd_hdr();
            proto_hdr protoHeader = new proto_hdr();
            ftd_hdr ftdHeader = new ftd_hdr();

            ftdc_hdr fieldHeader = new ftdc_hdr();
            ftdc_hdr rspHeader = new ftdc_hdr();

            IFieldId tmp = field as IFieldId;
            Type type = typeof(T);
            int fieldSize = Marshal.SizeOf(type);
            Type rspType = typeof(LCThostFtdcRspInfoField);
            int rspSize = Marshal.SizeOf(rspType);

            //初始化ftdc_hdr
            InitFTDCHeader(ref fieldHeader, fieldSize, (EnumFiledID)tmp.FieldId);

            InitFTDCHeader(ref rspHeader, rspSize, (EnumFiledID)rsp.FieldId);

            //初始化proftd_hdr
            int ftdcLen = Constanst.FTDC_HDRLEN + fieldSize + Constanst.FTDC_HDRLEN + rspSize; //FTDC正文长度 = 报头长度 + 结构体长度
            int pktLen = Constanst.PROFTD_HDRLEN + ftdcLen;//数据包总长度
            FillRspHeader(ref protoHeader,ref ftdHeader,(ushort)pktLen, seqType, transId, (ushort)fieldCount, (uint)reqId);

            //打包数据
            //IntPtr bufferProtoHeader = Marshal.AllocHGlobal(Constanst.PROTO_HDRLEN);
            //IntPtr bufferFtdHeader = Marshal.AllocHGlobal(Constanst.FTD_HDRLEN);

            //IntPtr bufferInfoHeader = Marshal.AllocHGlobal(Constanst.FTDC_HDRLEN);
            //IntPtr bufferInfo = Marshal.AllocHGlobal(rspSize);

            //IntPtr bufferFieldHeader = Marshal.AllocHGlobal(Constanst.FTDC_HDRLEN);
            //IntPtr bufferField = Marshal.AllocHGlobal(fieldSize);

            int offset = 0;
            try
            {
                Byte[] bytes = new Byte[pktLen];
                Array.Copy(ByteSwapHelp.StructToBytes<proto_hdr>(protoHeader), 0,bytes, 0, Constanst.PROTO_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<ftd_hdr>(ftdHeader), 0, bytes, Constanst.PROTO_HDRLEN, Constanst.FTD_HDRLEN);
                proto_hdr tmp0 = ByteSwapHelp.BytesToStruct<Struct.proto_hdr>(bytes, 0);
                ftd_hdr tmp1 = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(bytes, 4);

                //Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(rspHeader), 0, bytes, Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                //Array.Copy(ByteSwapHelp.StructToBytes<LCThostFtdcRspInfoField>(rsp), 0, bytes, Constanst.PROFTD_HDRLEN + , rspSize);

                offset = 0;
                Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(rspHeader), 0, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<LCThostFtdcRspInfoField>(rsp), 0, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, rspSize);

                ftdc_hdr tmp2 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(bytes, offset + Constanst.PROFTD_HDRLEN);
                LCThostFtdcRspInfoField tmp3 = ByteSwapHelp.BytesToStruct<LCThostFtdcRspInfoField>(bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                offset += Constanst.FTDC_HDRLEN + rspSize;
                Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(fieldHeader), 0, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<T>(field), 0, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, fieldSize);

                ftdc_hdr tmp4 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(bytes, offset + Constanst.PROFTD_HDRLEN);
                LCThostFtdcRspUserLoginField tmp5 = ByteSwapHelp.BytesToStruct<LCThostFtdcRspUserLoginField>(bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);


                //Marshal.StructureToPtr(protoHeader, bufferProtoHeader, false);
                //Marshal.StructureToPtr(ftdHeader, bufferFtdHeader, false);

                //Marshal.StructureToPtr(rspHeader, bufferInfoHeader, false);
                //Marshal.StructureToPtr(rsp, bufferInfo, false);

                //Marshal.StructureToPtr(fieldHeader, bufferFieldHeader, false);
                //Marshal.StructureToPtr(field, bufferField, false);

                //Byte[] bytes = new Byte[pktLen];

                //Marshal.Copy(bufferProtoHeader, bytes, 0, Constanst.PROTO_HDRLEN);
                //proto_hdr tmp0 = ByteSwapHelp.BytesToStruct<Struct.proto_hdr>(bytes, 0);

                //Marshal.Copy(bufferFtdHeader, bytes,0,Constanst.FTD_HDRLEN);
                //ftd_hdr tmp1 = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(bytes, 4);

                //offset = 0;
                //Marshal.Copy(bufferInfoHeader, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                //Marshal.Copy(bufferInfo, bytes,offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, rspSize);
                //offset += Constanst.FTDC_HDRLEN + rspSize;
                //Marshal.Copy(bufferFieldHeader, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                //Marshal.Copy(bufferField, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, fieldSize);

                return bytes;
            }
            finally
            {
                //Marshal.FreeHGlobal(bufferProtoHeader);
                //Marshal.FreeHGlobal(bufferFtdHeader);

                //Marshal.FreeHGlobal(bufferInfoHeader);
                //Marshal.FreeHGlobal(bufferInfo);

                //Marshal.FreeHGlobal(bufferFieldHeader);
                //Marshal.FreeHGlobal(bufferField);
            }
        }

        /// <summary>
        /// 数据包加密
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static byte[] EncPkt(byte[] srcdata,out int pktLen)
        {

            int DstLen = srcdata.Length;
            int SrcLen = srcdata.Length;// DstLen - 8;//前8个字节不压缩
            pktLen = DstLen;
            byte[] dstData = null;
            if (SrcLen > 8)
            {
                DstLen *= 2;  //保证足够存储空间
                dstData = new byte[DstLen];
                Array.Copy(srcdata, 0, dstData, 0, 8);//复制前面8个字节
                LZ_Compress(ref dstData, ref DstLen, srcdata, SrcLen);
                dstData[5] = Constanst.THOST_ENC_LZ;
                byte[] sb = BitConverter.GetBytes(ByteSwapHelp.ReverseBytes((ushort)(DstLen - 4)));
                dstData[2] = sb[0];
                dstData[3] = sb[1];
                pktLen = DstLen;
            }
            return dstData;
        }


        public static void LZ_Uncompress(ref byte[] dstBuf, ref int dstLen, byte[] srcBuf, int startIdx, int srcLen)
        {

            int srcInd = 8;
            int dstInd = 8;
            startIdx = 0;
            while (srcInd < srcLen && dstInd < dstLen)
            {
                if (srcBuf[startIdx + srcInd] == 0xe0) // 转义字节
                {
                    dstBuf[dstInd++] = srcBuf[startIdx + ++srcInd];
                    srcInd++;
                }
                else if ((srcBuf[srcInd] & 0xf0) == 0xe0) // 压缩的空字节
                {
                    if (dstInd + (srcBuf[startIdx + srcInd] & 0x0f) > dstLen) { break; }

                    dstInd += (srcBuf[startIdx + srcInd] & 0x0f);
                    srcInd++;
                }
                else // 普通字节
                {
                    dstBuf[dstInd++] = srcBuf[startIdx + srcInd++]; 
                }
            }
        }

        /// <summary>
        /// LZ压缩
        /// </summary>
        /// <param name="dstBuf"></param>
        /// <param name="dstLen"></param>
        /// <param name="srcBuf"></param>
        /// <param name="srcLen"></param>
        public static void LZ_Compress(ref byte[] dstBuf, ref int dstLen,byte[] srcBuf,int srcLen)
        {
            int srcInd=8;
            int dstInd=8;
            int i=0;
            int iNum=0;
            //int offset = 0;
            while(srcInd < srcLen)
            {
                if (srcBuf[srcInd] != 0)
                {
                    if ((srcBuf[srcInd] & 0xf0) == 0xe0) // 0xe0-0xef
                    {
                        dstBuf[dstInd++] = 0xe0;
                        dstBuf[dstInd++] = srcBuf[srcInd];
                    }
                    else //普通字节
                    {
                        dstBuf[dstInd++] = srcBuf[srcInd];
                    }
                }
                else
                {
                    iNum ++;
                    //下一个字节不为空 则将当前记录到的空的个数进行处理
                    if ((srcInd < srcLen - 1 &&srcBuf[srcInd + 1] != 0 ) || srcInd == srcLen - 1)
                    {
                        for(i=0;i<(iNum/15);i++)//最多防止15个空
                        {
                            dstBuf[dstInd++] = 0xef; //0xef = 239
                        }
                        if(iNum%15!=0)//对应字节 存放空字节个数
                        {
                            //dstBuf[dstInd++] = 0xe0 + iNum%15;
                            dstBuf[dstInd++] = (byte)(224 + iNum % 15);
                        }
                        iNum = 0;
                    }
                }
                srcInd ++;
            }
            dstLen = dstInd;
        }
    }
}
