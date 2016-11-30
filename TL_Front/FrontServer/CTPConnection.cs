using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;

using CTPService.Struct;
using CTPService.Struct.V12;


namespace CTPService
{
    public class CTPConnection:IConnection
    {
        TLSessionBase _session = null;
        ILog logger = LogManager.GetLogger("conn");

        public CTPConnection(TLSessionBase session)
        { 
            _session = session;

            this.SessionID = _session.SessionID;
        }

        

        /// <summary>
        /// 回话编号
        /// </summary>
        public string SessionID { get; set; }

        /// <summary>
        /// 向客户端发送数据包
        /// 逻辑服务器返回的数据包转换成客户端协议支持的数据包对外发送
        /// </summary>
        /// <param name="packet"></param>
        public void SendToClient(IPacket packet)
        {
            logger.Info("SendToClient:" + packet.ToString());
            switch (packet.Type)
            {
                case MessageTypes.LOGINRESPONSE:
                    {
                        LoginResponse response = packet as LoginResponse;

                        LCThostFtdcRspUserLoginField field = new LCThostFtdcRspUserLoginField();
                        field.TradingDay = response.TradingDay.ToString();
                        field.MaxOrderRef = "1";
                        field.UserID = response.LoginID;
                        field.SystemName = "FutsSystems";
                        field.FrontID = response.FrontIDi;
                        field.SessionID = response.SessionIDi;
                        field.BrokerID = "88888";

                        string time = DateTime.Now.ToString("HH:mm:ss");
                        field.LoginTime = time;
                        field.SHFETime =time;
                        field.DCETime = time;
                        field.CZCETime = time;
                        field.FFEXTime = time;
                        field.INETime = time;

                        LCThostFtdcRspInfoField rsp = new LCThostFtdcRspInfoField();
                        rsp.ErrorID = response.RspInfo.ErrorID;
                        rsp.ErrorMsg = response.RspInfo.ErrorMessage;

                        byte[] data = StructHelperV12.FillRsp<LCThostFtdcRspUserLoginField>(ref rsp, ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_LOGIN, 1, response.RequestID);

                        Send(data);
                        return;

                        /*
                        // 服务端数据不压缩 直接发送给客户端接口 客户端接口可以直接识别
                        int pktLen = 0;
                        byte[] encData = StructHelperV12.EncPkt(data,out pktLen);

                        logger.Info("RawBytes:" + ByteUtil.ByteToHex(data, ' '));
                        logger.Info("EncBytes:" + ByteUtil.ByteToHex(encData, ' ',pktLen));
                        //Send(encData,pktLen);


                        EnumFTDType bFtdtype = (EnumFTDType)encData[0];
                        int bExLen = encData[1];
                        int wPktLen = Endian.SwapInt16((BitConverter.ToInt16(encData, 2)));//字节序变化

                        byte[] outdata = new byte[pktLen];
                        Array.Copy(encData, 0, outdata, 0, pktLen);

                        Type type = typeof(ftd_hdr);
                        byte[] ftdhdrData = new byte[Constanst.FTD_HDRLEN];
                        int ftdSize = Constanst.FTD_HDRLEN;

                        byte[] pdata = new byte[data.Length];
                        int pSize = data.Length;
                        StructHelperV12.LZ_Uncompress(ref pdata, ref pSize, outdata, 4, outdata.Length);
                        Array.Copy(encData, 0, pdata, 0, 8);
                        logger.Info("PPPData:" + ByteUtil.ByteToHex(pdata, ' '));

                        if (bFtdtype == EnumFTDType.FTDTypeFTDC)
                        {
                            try
                            {
                                proto_hdr protohdr = ByteSwapHelp.BytesToStruct<proto_hdr>(data);
                                proto_hdr protohdr2 = ByteSwapHelp.BytesToStruct<proto_hdr>(encData);

                                //解析FTD报头
                                Struct.ftd_hdr ftdhdr_old = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(data,4);

                                //解析FTD报头
                                Struct.ftd_hdr ftdhdr = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(pdata,4);
                                //根据tid进行数据解析
                                switch ((EnumTransactionID)ftdhdr.dTransId)
                                {

                                    //请求登入
                                    case EnumTransactionID.T_REQ_LOGIN:
                                        {
                                            //fieldList = ParsePktDataV12(data, ftdhdr.wFtdcLen, ftdhdr.wFiCount);
                                            break;
                                        }
                                    default:
                                        throw new Exception(string.Format("TransactionID:{0} pkt not handled", (EnumTransactionID)ftdhdr.dTransId));
                                }
                                //解析FTDC报头
                                //Struct.ftdc_hdr ftdc_hdr = ByteSwapHelp.BytesToStruct<Struct.ftdc_hdr>(data, Constanst.FTD_HDRLEN);//通过fid 获得对应的结构体数据类别

                                //key = string.Format("FTD Data Ver:{0} Tid:{1} SeqType:{2} SeqNo:{3} Enc:{4} ReqID:{5} Chain:{6} FieldCnt:{7} FtdcLen:{8}", ftdhdr.bVersion, ftdhdr.dTransId, (EnumSeqType)ftdhdr.wSeqSn, ftdhdr.dSeqNo, (EnumEncType)ftdhdr.bEnctype, ftdhdr.dReqId, (EnumChainType)ftdhdr.bChain, ftdhdr.wFiCount, ftdhdr.wFtdcLen);
                            }
                            catch (Exception ex)
                            {
                                //key = ex.ToString();
                            }
                        }
                        **/

                        break;
                    }
                default:
                    logger.Warn(string.Format("Packet:{0} not handled", packet.ToString()));
                    break;
            }
        }

        void Send(byte[] data)
        {
            _session.Send(data, 0, data.Length);
        }

        void Send(byte[] data, int len)
        {
            _session.Send(data, 0, len);
        }
        /// <summary>
        /// 向逻辑服务器发送数据包
        /// 客户端提交上来的请求转换成内部数据格式 向逻辑服务器发送
        /// </summary>
        /// <param name="packet"></param>
        public void ForwardToLogic(IPacket packet)
        {
            logger.Info("ForwardToLogic:" + packet.ToString());
            //
        }

        /// <summary>
        /// 关闭会话
        /// </summary>
        public void Close()
        {
            _session.Close();
        }
    }
}
