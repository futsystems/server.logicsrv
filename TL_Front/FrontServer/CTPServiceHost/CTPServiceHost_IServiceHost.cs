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
                        //结算单确认回报
                    case MessageTypes.CONFIRMSETTLEMENTRESPONSE:
                        {
                            RspConfirmSettlementResponse response = packet as RspConfirmSettlementResponse;
                            Struct.V12.LCThostFtdcSettlementInfoConfirmField field = new Struct.V12.LCThostFtdcSettlementInfoConfirmField();
                            field.ConfirmDate = response.ConfirmDay.ToString();
                            field.ConfirmTime = Util.ToDateTime(response.ConfirmDay, response.ConfirmTime).ToString("HH:mm:ss");

                            //打包数据 SeqType 为SeqQry 否则博弈会开在查询合约
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcSettlementInfoConfirmField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_CONFIRMSET, response.RequestID, conn.NextSeqId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            break;
                        }
                        //查询合约回报
                    case MessageTypes.SYMBOLRESPONSE:
                        {
                            RspQrySymbolResponse response = packet as RspQrySymbolResponse;
                            Struct.V12.LCThostFtdcInstrumentField field = new Struct.V12.LCThostFtdcInstrumentField();
                            field.InstrumentID = response.InstrumentToSend.Symbol;
                            field.ExchangeInstID = response.InstrumentToSend.Symbol;
                            field.InstrumentName = response.InstrumentToSend.Name;
                            field.ProductID = response.InstrumentToSend.Security;
                            field.ExchangeID = response.InstrumentToSend.ExchangeID;
                            field.ProductClass = TThostFtdcProductClassType.Futures;

                            field.MaxMarketOrderVolume = 1000;
                            field.MinMarketOrderVolume = 1;
                            field.MaxLimitOrderVolume = 1000;
                            field.MinLimitOrderVolume = 1;

                            field.VolumeMultiple = response.InstrumentToSend.Multiple;
                            field.PriceTick = (double)response.InstrumentToSend.PriceTick;
                            
                            //field.CreateDate = response.InstrumentToSend.ExpireDate.ToString();
                            //field.DeliveryYear = response.InstrumentToSend.ExpireDate / 10000;
                            //field.DeliveryMonth = response.InstrumentToSend.ExpireDate / 100 - (response.InstrumentToSend.ExpireDate / 10000) * 100;
                            
                            //field.EndDelivDate = response.InstrumentToSend.ExpireDate.ToString();
                            //field.ExpireDate = response.InstrumentToSend.ExpireDate.ToString();
                            //field.OpenDate = response.InstrumentToSend.ExpireDate.ToString();
                            //field.StartDelivDate = response.InstrumentToSend.ExpireDate.ToString();

                            field.IsTrading = 1;
                            field.InstLifePhase = TThostFtdcInstLifePhaseType.Started;

                            field.PositionDateType = TThostFtdcPositionDateTypeType.NoUseHistory;
                            field.PositionType = TThostFtdcPositionTypeType.Gross;

                            field.LongMarginRatio = 0.1;
                            field.ShortMarginRatio = 0.1;

                            //field.MaxMarginSideAlgorithm = TThostFtdcMaxMarginSideAlgorithmType.NO;
                            //field.UnderlyingInstrID = "";
                            //field.StrikePrice = 0;
                            //field.OptionsType = (TThostFtdcOptionsTypeType)(byte)'0';// TThostFtdcOptionsTypeType.CallOptions;
                            //field.UnderlyingMultiple = 0;
                            //field.CombinationType = TThostFtdcCombinationTypeType.Future;
                            //if (!response.IsLast) return;
                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcInstrumentField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_INST, response.RequestID, conn.NextSeqId,response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200028201030c43e104e2802fe30de1040468e319e10301165a43363132efeb435a4345e5b6afc1a6c3ba363132ec5a43363132efeb5a43efee31e207e0e0e30ce3c8e301e203e0e8e301e3643fc999999999999a3230313531313235e13230313531323038e13230313631323037e13230313631323037e13230313631323037e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe13ff0e630e10301164647373031efeb435a4345e5b2a3c1a7373031ee4647373031efeb4647efee31e207e0e1e301e3c8e301e203e0e8e301e3143ff0e63230313531323233e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116637331373031efea444345e6d3f1c3d7b5e0edb7db31373031e9637331373031efea6373efee31e207e0e1e301e203e0e8e301e203e0e8e301e30a3ff0e63230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116666231373031efea444345e6d6d0c3dcb6c8cfcbceacb0e0e531373031e5666231373031efea6662efee31e207e0e1e301e203e0e8e301e203e0e8e301e201f43fa999999999999a3230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe17fe0efffffffffffff30";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;
                            //encData[7] = (byte)'L';
                            conn.Send(encData, encPktLen);
                            break;
                        }
                        //委托查询回报
                    case MessageTypes.ORDERRESPONSE:
                        {
                            RspQryOrderResponse response = packet as RspQryOrderResponse;
                            Struct.V12.LCThostFtdcOrderField field = new Struct.V12.LCThostFtdcOrderField();

                            byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_QRYORD, response.RequestID, conn.NextSeqId, response.IsLast);
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
