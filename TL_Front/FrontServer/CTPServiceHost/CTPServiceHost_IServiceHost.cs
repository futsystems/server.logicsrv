using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;
using Common.Logging;


namespace CTPService
{
    public partial class CTPServiceHost
    {
        /// <summary>
        /// 将逻辑服务端的消息进行处理 转换成ServiceHost支持协议内容
        /// 如果不需发送消息到客户端则返回null
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        public void HandleLogicMessage(FrontServer.IConnection connection, IPacket packet)
        {
            try
            {
                string hex = string.Empty;
                CTPConnection conn = GetConnection(connection.SessionID);
                if (conn == null)
                {
                    logger.Warn(string.Format("Session:{0} CTPConnection do not exist", connection.SessionID));
                    return;
                }
                switch (packet.Type)
                {
                    //登入回报
                    case MessageTypes.LOGINRESPONSE:
                        {
                            LoginResponse response = packet as LoginResponse;
                            //将数据转换成CTP业务结构体
                            Struct.V12.LCThostFtdcRspUserLoginField field = new Struct.V12.LCThostFtdcRspUserLoginField();
                            field.TradingDay = response.TradingDay.ToString();
                            field.MaxOrderRef = "1";
                            field.UserID = response.LoginID;
                            field.SystemName = "FutsSystems";
                            field.FrontID = response.FrontIDi;
                            field.SessionID = response.SessionIDi;
                            field.BrokerID = "88888";

                            string time = DateTime.Now.ToString("HH:mm:ss");
                            field.LoginTime = time;
                            field.SHFETime = time;
                            field.DCETime = time;
                            field.CZCETime = time;
                            field.FFEXTime = time;
                            field.INETime = time;

                            Struct.V12.LCThostFtdcRspInfoField rsp = new Struct.V12.LCThostFtdcRspInfoField();
                            rsp.ErrorID = response.RspInfo.ErrorID;
                            rsp.ErrorMsg = string.Format("CTP:{0}", response.RspInfo.ErrorMessage);

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcRspUserLoginField>(ref rsp, ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_LOGIN, response.RequestID,conn.NextSeqId);

                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;

                            //发送数据
                            conn.Send(encData, encPktLen);
                            break;
                        }
                    //投资者查询回报
                    case MessageTypes.INVESTORRESPONSE:
                        {
                            RspQryInvestorResponse response = packet as RspQryInvestorResponse;

                            Struct.V12.LCThostFtdcInvestorField field = new Struct.V12.LCThostFtdcInvestorField();
                            field.BrokerID = "88888";
                            field.InvestorID = response.TradingAccount;
                            field.InvestorName = response.NickName;
                            field.IdentifiedCardType = TThostFtdcIdCardTypeType.IDCard;
                            field.IdentifiedCardNo = "999900130711111111111";
                            field.Telephone = response.Mobile;
                            field.Mobile = response.Mobile;
                            field.Address = "";
                            field.OpenDate = "20150101";
                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcInvestorField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_USRINF, response.RequestID, conn.NextSeqId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200008d01030c4ce104e28009e301e101018ce302e106018838353632303030383032e33838383838efe4c7aeb2a8efefefefefe231333230353832313938333034313934353134efefe6013138303531313336313637efefc9ccb3c7c2b7333431bac5d7cfb9e0e2b4f3cfc332323033cad2efefefefefe13230313230393137e13138303531313336313637efefefeb";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;

                            //发送数据
                            conn.Send(encData, encPktLen);
                            break;
                        }
                        //查询投资者结算确认信息
                    case MessageTypes.SETTLEINFOCONFIRMRESPONSE:
                        {
                            RspQrySettleInfoConfirmResponse response = packet as RspQrySettleInfoConfirmResponse;
                            Struct.V12.LCThostFtdcSettlementInfoConfirmField field = new Struct.V12.LCThostFtdcSettlementInfoConfirmField();
                            field.BrokerID = "88888";
                            field.InvestorID = response.TradingAccount;
                            field.ConfirmDate = response.ConfirmDay.ToString();
                            field.ConfirmTime = Util.ToDateTime(response.ConfirmDay, response.ConfirmTime).ToString("HH:mm:ss");

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_SETCONFIRM, response.RequestID, conn.NextSeqId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200000d01030c4ce104e28057e302e704";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;
                            //发送数据
                            conn.Send(encData, encPktLen);
                            break;
                        }
                        //请求查询客户通知
                    case MessageTypes.NOTICERESPONSE:
                        {
                            RspQryNoticeResponse response = packet as RspQryNoticeResponse;
                            Struct.V12.LCThostFtdcNoticeField field = new Struct.V12.LCThostFtdcNoticeField();
                            field.BrokerID = "888888";
                            field.Content = response.NoticeContent;

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcNoticeField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_NOTICE, response.RequestID, conn.NextSeqId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            break;
                        }
                        //查询结算信息回报
                    case MessageTypes.XSETTLEINFORESPONSE:
                        {
                            RspXQrySettleInfoResponse response = packet as RspXQrySettleInfoResponse;
                            Struct.V12.LCThostFtdcSettlementInfoField field = new Struct.V12.LCThostFtdcSettlementInfoField();
                            field.Content = response.SettlementContent;
                            field.TradingDay = response.Tradingday.ToString();
                            field.InvestorID = response.TradingAccount;
                            field.SettlementID = response.SettlementID;
                            field.SequenceNo = response.SequenceNo;

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcSettlementInfoField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_SMI, response.RequestID, conn.NextSeqId,response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            break;

                        }

                    default:
                        logger.Warn(string.Format("Logic Packet:{0} not handled", packet.Type));
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.Error(string.Format("Handler Logic Packet:{0} Error:{1}", packet.ToString(), ex.ToString()));
            }
        }
    }
}
