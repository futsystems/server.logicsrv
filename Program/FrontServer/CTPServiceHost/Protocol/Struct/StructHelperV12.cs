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
                    //请求查询投资者
                case EnumFiledID.F_QRY_USRINF:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryInvestorField>(data, offset);
                    }
                    //请求查询投资者结算确认
                case EnumFiledID.F_QRY_SETCONFIRM:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQrySettlementInfoConfirmField>(data, offset);
                    }
                    //请求查询保证金监管系统经纪公司资金账户密钥
                case EnumFiledID.F_QRY_CFMMCKEY:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryCFMMCTradingAccountKeyField>(data, offset);
                    }
                    //请求查询监控中心用户令牌
                case EnumFiledID.F_QRY_TDTOK:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQueryCFMMCTradingAccountTokenField>(data, offset);
                    }
                    //请求查询客户通知
                case EnumFiledID.F_QRY_NOTICE:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryNoticeField>(data, offset);
                    }
                    //请求查询交易通知
                case EnumFiledID.F_QRY_TDNOTICE:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryTradingNoticeField>(data, offset);
                    }
                    //请求查询投资者结算结果
                case EnumFiledID.F_QRY_SMI:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQrySettlementInfoField>(data, offset);
                    }
                    //投资者结算结果确认
                case EnumFiledID.F_REQ_SETCONFIRM:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcSettlementInfoConfirmField>(data, offset);
                    }
                    //请求查询合约 
                case EnumFiledID.F_QRY_INST:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryInstrumentField>(data, offset);
                    }
                    //请求查询报单
                case EnumFiledID.F_QRY_ORDER:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryOrderField>(data, offset); 
                    }
                    //查询成交
                case EnumFiledID.F_QRY_TRADE:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryTradeField>(data, offset); 
                    }
                    //请求查询投资者持仓
                case EnumFiledID.F_QRY_INVPOS:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryInvestorPositionField>(data, offset); 
                    }
                    //请求查询资金账户
                case EnumFiledID.F_QRY_TDACC:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryTradingAccountField>(data, offset); 
                    }
                    //查询签约银行
                case EnumFiledID.F_QRY_CONTBK:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryContractBankField>(data, offset);
                    }
                    //查询银行签约关系
                case EnumFiledID.F_QRY_ACCREG:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryAccountregisterField>(data, offset);
                    }
                    //查询最大报单数量请求
                case EnumFiledID.F_QRY_MAXORDVOL:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQueryMaxOrderVolumeField>(data, offset);
                    }
                    //报单录入请求
                case EnumFiledID.F_REQ_ORDINSERT:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcInputOrderField>(data, offset);
                    }
                    //委托操作 撤单
                case EnumFiledID.F_REQ_CANCEL:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcInputOrderActionField>(data, offset);
                    }
                    //用户口令变更
                case EnumFiledID.F_REQ_MODPASS:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcUserPasswordUpdateField>(data, offset);
                    }
                    //资金账户口令更新请求
                case EnumFiledID.F_REQ_MODACCPASS:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcTradingAccountPasswordUpdateField>(data, offset);
                    }
                    //查询银行流水
                case EnumFiledID.F_QRY_TFSN:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryTransferSerialField>(data, offset);
                    }
                    //查询持仓明细
                case EnumFiledID.F_QRY_POSDETAIL:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryInvestorPositionDetailField>(data, offset);
                    }
                    //客户端认证请求 ReqAuthenticate
                case EnumFiledID.F_REQ_AUTHINF:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcAuthenticationInfoField>(data, offset);
                    }
                //请求查询执行宣告
                case EnumFiledID.F_QRY_EXECORD:
                    {
                        return ByteSwapHelp.BytesToStruct<LCThostFtdcQryExecOrderField>(data, offset);
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

        public static void FillRspHeader(ref proto_hdr proto_hdr, ref ftd_hdr ftd_hdr, ushort pktLen, EnumSeqType seqType, EnumTransactionID transId, ushort fieldCount, uint reqId,uint seqId,bool isLast)
        {
            proto_hdr.bFtdtype = (byte)EnumFTDType.FTDTypeFTDC;
            proto_hdr.bExLen = 0;
            proto_hdr.wPktLen = (ushort)(pktLen - 4);

            ftd_hdr.bHead = Constanst.FTDC_HEAD;
            ftd_hdr.bVersion = Constanst.FTDC_VER;
            ftd_hdr.bEnctype = Constanst.THOST_ENC_NONE;
            ftd_hdr.bChain = isLast ? (byte)'L' : (byte)'C';
            ftd_hdr.wSeqSn = (ushort)seqType;
            ftd_hdr.dTransId = (uint)transId;
            ftd_hdr.dSeqNo = seqId;//这里应该是递增的变量 需改动 
            ftd_hdr.wFiCount = fieldCount;
            ftd_hdr.wFtdcLen = (ushort)(pktLen - 4 - Constanst.FTD_HDRLEN);
            ftd_hdr.dReqId = reqId;
        }

        /// <summary>
        /// 打包查询回报
        /// 该查询不包含任何有效回报字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="seqType"></param>
        /// <param name="transId"></param>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public static byte[] PackRsp(EnumSeqType seqType, EnumTransactionID transId, int reqId, int seqId,bool isLast=true)
        {
            proto_hdr protoHeader = new proto_hdr();
            ftd_hdr ftdHeader = new ftd_hdr();

            //初始化proftd_hdr
            int ftdcLen = 0;
            int pktLen = Constanst.PROFTD_HDRLEN + ftdcLen;//数据包总长度
            FillRspHeader(ref protoHeader, ref ftdHeader, (ushort)pktLen, seqType, transId, 0, (uint)reqId, (uint)seqId,isLast);
            try
            {
                Byte[] bytes = new Byte[pktLen];

                Array.Copy(ByteSwapHelp.StructToBytes<proto_hdr>(protoHeader), 0, bytes, 0, Constanst.PROTO_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<ftd_hdr>(ftdHeader), 0, bytes, Constanst.PROTO_HDRLEN, Constanst.FTD_HDRLEN);
                return bytes;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 打包查询回报
        /// 只包含查询结果域 不包含RspInfo域
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="field"></param>
        /// <param name="seqType"></param>
        /// <param name="transId"></param>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public static byte[] PackRsp<T>(ref T field, EnumSeqType seqType, EnumTransactionID transId, int reqId,int seqId,bool isLast=true)
            where T : IByteSwap
        {
            proto_hdr protoHeader = new proto_hdr();
            ftd_hdr ftdHeader = new ftd_hdr();

            ftdc_hdr fieldHeader = new ftdc_hdr();
            IFieldId tmp = field as IFieldId;
            Type type = typeof(T);
            int fieldSize = Marshal.SizeOf(type);
            InitFTDCHeader(ref fieldHeader, fieldSize, (EnumFiledID)tmp.FieldId);

            //初始化proftd_hdr
            int ftdcLen = Constanst.FTDC_HDRLEN + fieldSize;
            int pktLen = Constanst.PROFTD_HDRLEN + ftdcLen;//数据包总长度
            FillRspHeader(ref protoHeader, ref ftdHeader, (ushort)pktLen, seqType, transId,(ushort)1, (uint)reqId,(uint)seqId,isLast);

            int offset = 0;
            try
            {
                Byte[] bytes = new Byte[pktLen];

                Array.Copy(ByteSwapHelp.StructToBytes<proto_hdr>(protoHeader), 0, bytes, 0, Constanst.PROTO_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<ftd_hdr>(ftdHeader), 0, bytes, Constanst.PROTO_HDRLEN, Constanst.FTD_HDRLEN);

                offset = 0;
                Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(fieldHeader), 0, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<T>(field), 0, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, fieldSize);
                return bytes;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 将一组数据打包到数据结构体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fields"></param>
        /// <param name="seqType"></param>
        /// <param name="transId"></param>
        /// <param name="reqId"></param>
        /// <param name="seqId"></param>
        /// <param name="isLast"></param>
        /// <returns></returns>
        public static byte[] PackRsp<T>(ref List<T> fields, EnumSeqType seqType, EnumTransactionID transId, int reqId, int seqId, bool isLast = true)
            where T : IByteSwap
        {
            proto_hdr protoHeader = new proto_hdr();
            ftd_hdr ftdHeader = new ftd_hdr();

            ftdc_hdr fieldHeader = new ftdc_hdr();
            IFieldId tmp = fields[0] as IFieldId;
            Type type = typeof(T);
            int fieldSize = Marshal.SizeOf(type);
            InitFTDCHeader(ref fieldHeader, fieldSize, (EnumFiledID)tmp.FieldId);

            //初始化proftd_hdr
            int ftdcLen = (Constanst.FTDC_HDRLEN + fieldSize) * fields.Count;//数据域长度 每个数据与包含数据头和数据结构体
            int pktLen = Constanst.PROFTD_HDRLEN + ftdcLen;//数据包总长度
            FillRspHeader(ref protoHeader, ref ftdHeader, (ushort)pktLen, seqType, transId, (ushort)1, (uint)reqId, (uint)seqId, isLast);

            int offset = 0;
            try
            {
                Byte[] bytes = new Byte[pktLen];

                Array.Copy(ByteSwapHelp.StructToBytes<proto_hdr>(protoHeader), 0, bytes, 0, Constanst.PROTO_HDRLEN);
                offset += Constanst.PROTO_HDRLEN;
                Array.Copy(ByteSwapHelp.StructToBytes<ftd_hdr>(ftdHeader), 0, bytes,offset, Constanst.FTD_HDRLEN);
                offset += Constanst.FTD_HDRLEN;
                
                for (int i = 0; i < fields.Count;i++ )
                {
                    Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(fieldHeader), 0, bytes, offset, Constanst.FTDC_HDRLEN);
                    offset += Constanst.FTDC_HDRLEN;
                    Array.Copy(ByteSwapHelp.StructToBytes<T>(fields[i]), 0, bytes, offset, fieldSize);
                    offset += fieldSize;
                }
                return bytes;
            }
            finally
            {
            }
        }


        public static byte[] PackNotify<T>(ref T field, EnumSeqType seqType, EnumTransactionID transId, int seqId)
            where T : IByteSwap
        {
            proto_hdr protoHeader = new proto_hdr();
            ftd_hdr ftdHeader = new ftd_hdr();

            ftdc_hdr fieldHeader = new ftdc_hdr();
            IFieldId tmp = field as IFieldId;
            Type type = typeof(T);
            int fieldSize = Marshal.SizeOf(type);
            InitFTDCHeader(ref fieldHeader, fieldSize, (EnumFiledID)tmp.FieldId);

            //初始化proftd_hdr
            int ftdcLen = Constanst.FTDC_HDRLEN + fieldSize;
            int pktLen = Constanst.PROFTD_HDRLEN + ftdcLen;//数据包总长度
            FillRspHeader(ref protoHeader, ref ftdHeader, (ushort)pktLen, seqType, transId, (ushort)1, (uint)0, (uint)seqId, true);

            int offset = 0;
            try
            {
                Byte[] bytes = new Byte[pktLen];

                Array.Copy(ByteSwapHelp.StructToBytes<proto_hdr>(protoHeader), 0, bytes, 0, Constanst.PROTO_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<ftd_hdr>(ftdHeader), 0, bytes, Constanst.PROTO_HDRLEN, Constanst.FTD_HDRLEN);

                offset = 0;
                Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(fieldHeader), 0, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<T>(field), 0, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, fieldSize);
                return bytes;
            }
            finally
            {
            }
        }

        /// <summary>
        /// 打包查询回报
        /// 包含查询结果域 与 RspInfo 域
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rsp"></param>
        /// <param name="field"></param>
        /// <param name="seqType"></param>
        /// <param name="transId"></param>
        /// <param name="fieldCount"></param>
        /// <param name="reqId"></param>
        /// <returns></returns>
        public static byte[] PackRsp<T>(ref LCThostFtdcRspInfoField rsp, ref T field, EnumSeqType seqType, EnumTransactionID transId,int reqId,int seqId,bool isLast=true)
            where T:IByteSwap
        {
            proto_hdr protoHeader = new proto_hdr();
            ftd_hdr ftdHeader = new ftd_hdr();

            ftdc_hdr rspHeader = new ftdc_hdr();
            Type rspType = typeof(LCThostFtdcRspInfoField);
            int rspSize = Marshal.SizeOf(rspType);
            InitFTDCHeader(ref rspHeader, rspSize, (EnumFiledID)rsp.FieldId);

            ftdc_hdr fieldHeader = new ftdc_hdr();
            IFieldId tmp = field as IFieldId;
            Type type = typeof(T);
            int fieldSize = Marshal.SizeOf(type);
            InitFTDCHeader(ref fieldHeader, fieldSize, (EnumFiledID)tmp.FieldId);

            //初始化proftd_hdr
            int ftdcLen = Constanst.FTDC_HDRLEN + fieldSize + Constanst.FTDC_HDRLEN + rspSize; //FTDC正文长度 = 报头长度 + 结构体长度
            int pktLen = Constanst.PROFTD_HDRLEN + ftdcLen;//数据包总长度
            FillRspHeader(ref protoHeader, ref ftdHeader, (ushort)pktLen, seqType, transId, (ushort)2, (uint)reqId, (uint)seqId, isLast);

            int offset = 0;
            try
            {
                Byte[] bytes = new Byte[pktLen];

                Array.Copy(ByteSwapHelp.StructToBytes<proto_hdr>(protoHeader), 0,bytes, 0, Constanst.PROTO_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<ftd_hdr>(ftdHeader), 0, bytes, Constanst.PROTO_HDRLEN, Constanst.FTD_HDRLEN);
                //proto_hdr tmp0 = ByteSwapHelp.BytesToStruct<Struct.proto_hdr>(bytes, 0);
                //ftd_hdr tmp1 = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(bytes, 4);

                offset = 0;
                Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(rspHeader), 0, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<LCThostFtdcRspInfoField>(rsp), 0, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, rspSize);
                //ftdc_hdr tmp2 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(bytes, offset + Constanst.PROFTD_HDRLEN);
                //LCThostFtdcRspInfoField tmp3 = ByteSwapHelp.BytesToStruct<LCThostFtdcRspInfoField>(bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                offset += Constanst.FTDC_HDRLEN + rspSize;
                Array.Copy(ByteSwapHelp.StructToBytes<ftdc_hdr>(fieldHeader), 0, bytes, offset + Constanst.PROFTD_HDRLEN, Constanst.FTDC_HDRLEN);
                Array.Copy(ByteSwapHelp.StructToBytes<T>(field), 0, bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN, fieldSize);
                //ftdc_hdr tmp4 = ByteSwapHelp.BytesToStruct<ftdc_hdr>(bytes, offset + Constanst.PROFTD_HDRLEN);
                //LCThostFtdcRspUserLoginField tmp5 = ByteSwapHelp.BytesToStruct<LCThostFtdcRspUserLoginField>(bytes, offset + Constanst.PROFTD_HDRLEN + Constanst.FTDC_HDRLEN);

                return bytes;
            }
            finally
            {
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
                //Array.Copy(srcdata, 0, dstData, 0, 8);//复制前面8个字节
                LZ_Compress(ref dstData, ref DstLen, srcdata, SrcLen);
                dstData[5] = Constanst.THOST_ENC_LZ;
                byte[] sb = BitConverter.GetBytes(ByteSwapHelp.ReverseBytes((ushort)(DstLen - 4)));
                dstData[2] = sb[0];
                dstData[3] = sb[1];
                pktLen = DstLen;
            }
            return dstData;
        }

        /// <summary>
        /// 将某个数据包进行解码
        /// </summary>
        /// <param name="dstBuf"></param>
        /// <param name="dstLen"></param>
        /// <param name="srcBuf"></param>
        /// <param name="srcLen"></param>
        public static void LZ_Uncompress(ref byte[] dstBuf, ref int dstLen, byte[] srcBuf,int srcLen)
        {

            int srcInd = 8;
            int dstInd = 8;
            int startIdx = 0;
            for (int i = 0; i < 8; i++)
            {
                dstBuf[i] = srcBuf[i];
            }
            while (srcInd < srcLen && dstInd < dstLen)
            {
                if (srcBuf[startIdx + srcInd] == 0xe0) // 转义字节
                {
                    dstBuf[dstInd++] = srcBuf[startIdx + ++srcInd];
                    srcInd++;
                }
                else if ((srcBuf[srcInd] & 0xf0) == 0xe0) // 压缩的空字节
                {
                    if (dstInd + (srcBuf[startIdx + srcInd] & 0x0f) > dstLen) 
                    {
                        break; 
                    }
                    if (srcInd == 136)
                    {
                        int i = 0;
                    }
                    dstInd += (srcBuf[startIdx + srcInd] & 0x0f);
                    srcInd++;
                }
                else // 普通字节
                {
                    dstBuf[dstInd++] = srcBuf[startIdx + srcInd++];
                }
            }

            dstLen = dstInd;
            
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
            //复制前面8个字节
            for (i = 0; i < 8; i++)
            {
                dstBuf[i] = srcBuf[i];
            }

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
