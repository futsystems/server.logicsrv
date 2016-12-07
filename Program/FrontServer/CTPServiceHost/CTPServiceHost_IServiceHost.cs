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
                            field.BrokerID = conn.State.BrokerID;

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

                            if (response.RspInfo.ErrorID == 0)
                            {
                                conn.State.Authorized = true;
                                conn.State.FrontID = field.FrontID;
                                conn.State.SessionID = field.SessionID;
                            }

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcRspUserLoginField>(ref rsp, ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_LOGIN, response.RequestID,0);//登入回报 默认Seq为0

                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;

                            //发送数据
                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> LoginResponse", conn.SessionID));

                            //topicData回报
                            hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                            encData = ByteUtil.HexToByte(hex);
                            encPktLen = encData.Length;

                            conn.Send(encData, encPktLen);


                            break;
                        }
                    //投资者查询回报
                    case MessageTypes.INVESTORRESPONSE:
                        {
                            RspQryInvestorResponse response = packet as RspQryInvestorResponse;

                            Struct.V12.LCThostFtdcInvestorField field = new Struct.V12.LCThostFtdcInvestorField();
                            field.BrokerID = conn.State.BrokerID;
                            field.InvestorID = response.TradingAccount;
                            field.InvestorName = response.NickName;
                            field.IdentifiedCardType = TThostFtdcIdCardTypeType.IDCard;
                            field.IdentifiedCardNo = "88888888";
                            field.Telephone = response.Mobile;
                            field.Mobile = response.Mobile;
                            field.Address = "";
                            field.OpenDate = "20150101";
                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcInvestorField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_USRINF, response.RequestID, conn.NextSeqQryId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200008d01030c4ce104e28009e301e101018ce302e106018838353632303030383032e33838383838efe4c7aeb2a8efefefefefe231333230353832313938333034313934353134efefe6013138303531313336313637efefc9ccb3c7c2b7333431bac5d7cfb9e0e2b4f3cfc332323033cad2efefefefefe13230313230393137e13138303531313336313637efefefeb";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;

                            //发送数据
                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryInvestorResponse", conn.SessionID));
                            break;
                        }
                        //查询投资者结算确认信息
                    case MessageTypes.SETTLEINFOCONFIRMRESPONSE:
                        {
                            RspQrySettleInfoConfirmResponse response = packet as RspQrySettleInfoConfirmResponse;
                            Struct.V12.LCThostFtdcSettlementInfoConfirmField field = new Struct.V12.LCThostFtdcSettlementInfoConfirmField();
                            field.BrokerID = conn.State.BrokerID;
                            field.InvestorID = conn.State.LoginID; ;
                            field.ConfirmDate = response.ConfirmDay.ToString();
                            field.ConfirmTime = Util.ToDateTime(response.ConfirmDay, response.ConfirmTime).ToString("HH:mm:ss");

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_SETCONFIRM, response.RequestID, conn.NextSeqQryId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200000d01030c4ce104e28057e302e704";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;
                            //发送数据
                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQrySettleInfoConfirmResponse", conn.SessionID));
                            break;
                        }
                        //请求查询客户通知
                    case MessageTypes.NOTICERESPONSE:
                        {
                            RspQryNoticeResponse response = packet as RspQryNoticeResponse;
                            Struct.V12.LCThostFtdcNoticeField field = new Struct.V12.LCThostFtdcNoticeField();
                            field.BrokerID = conn.State.BrokerID;
                            field.Content = "abc";// CTPConvert.ConvUTF82GB2312(response.NoticeContent);

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcNoticeField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_NOTICE, response.RequestID, conn.NextSeqQryId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryNoticeResponse", conn.SessionID));
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
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcSettlementInfoField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_SMI, response.RequestID, conn.NextSeqQryId, response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQrySettleInfoResponse", conn.SessionID));
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
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcSettlementInfoConfirmField>(ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_CONFIRMSET, response.RequestID, conn.NextSeqReqId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspConfirmSettlementResponse", conn.SessionID));
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
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcInstrumentField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_INST, response.RequestID, conn.NextSeqQryId, response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200028201030c43e104e2802fe30de1040468e319e10301165a43363132efeb435a4345e5b6afc1a6c3ba363132ec5a43363132efeb5a43efee31e207e0e0e30ce3c8e301e203e0e8e301e3643fc999999999999a3230313531313235e13230313531323038e13230313631323037e13230313631323037e13230313631323037e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe13ff0e630e10301164647373031efeb435a4345e5b2a3c1a7373031ee4647373031efeb4647efee31e207e0e1e301e3c8e301e203e0e8e301e3143ff0e63230313531323233e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116637331373031efea444345e6d3f1c3d7b5e0edb7db31373031e9637331373031efea6373efee31e207e0e1e301e203e0e8e301e203e0e8e301e30a3ff0e63230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fb1e0eb851eb851e0eb3fb1e0eb851eb851e0eb30efefe17fe0efffffffffffffe17fe0efffffffffffff30e1030116666231373031efea444345e6d6d0c3dcb6c8cfcbceacb0e0e531373031e5666231373031efea6662efee31e207e0e1e301e203e0e8e301e203e0e8e301e201f43fa999999999999a3230313531323234e13230313630313138e13230313730313136e13230313730313136e13230313730313136e131e30132323fc999999999999a3fc999999999999a30efefe17fe0efffffffffffffe17fe0efffffffffffff30";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;
                            //encData[7] = (byte)'L';
                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQrySymbolResponse", conn.SessionID));
                            break;
                        }
                        //委托查询回报
                    case MessageTypes.ORDERRESPONSE:
                        {
                            RspQryOrderResponse response = packet as RspQryOrderResponse;
                            int encPktLen = 0;
                            byte[] encData = null;
                            if (response.OrderToSend == null)
                            {
                                byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_QRYORD, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            else
                            {
                                Struct.V12.LCThostFtdcOrderField field = new Struct.V12.LCThostFtdcOrderField();

                                CTPConvert.ConvOrder(response.OrderToSend, ref field);
                                field.BrokerID = conn.State.BrokerID;
                                field.FrontID = response.OrderToSend.FrontIDi;
                                field.SessionID = response.OrderToSend.SessionIDi;
                                field.ParticipantID = conn.State.BrokerID;

                                byte[] data = Struct.V12.StructHelperV12.PackRsp(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_QRYORD, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryOrderResponse", conn.SessionID));
                            break;

                        }
                        //查询成交回报
                    case MessageTypes.TRADERESPONSE:
                        {
                            RspQryTradeResponse response = packet as RspQryTradeResponse;
                            int encPktLen = 0;
                            byte[] encData = null;
                            if (response.TradeToSend == null)
                            {
                                byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_QRYTD, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            else
                            {
                                Struct.V12.LCThostFtdcTradeField field = new Struct.V12.LCThostFtdcTradeField();

                                CTPConvert.ConvTrade(response.TradeToSend, ref field);
                                field.BrokerID = conn.State.BrokerID;
                                field.ClearingPartID = conn.State.BrokerID;
                                field.ParticipantID = conn.State.BrokerID;

                                byte[] data = Struct.V12.StructHelperV12.PackRsp(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_QRYTD, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryTradeResponse", conn.SessionID));
                            break;
                        }
                        //查询投资者持仓
                    case MessageTypes.POSITIONRESPONSE:
                        {
                            RspQryPositionResponse response = packet as RspQryPositionResponse;
                            int encPktLen = 0;
                            byte[] encData = null;
                            if (response.PositionToSend == null)
                            {
                                byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_INVPOS, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            else
                            {
                                Struct.V12.LCThostFtdcInvestorPositionField field = new Struct.V12.LCThostFtdcInvestorPositionField();
                                CTPConvert.ConvPosition(response.PositionToSend, ref field);
                                field.BrokerID = conn.State.BrokerID;


                                byte[] data = Struct.V12.StructHelperV12.PackRsp(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_INVPOS, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryPositionResponse", conn.SessionID));
                            break;
                        }
                        //查询交易账户回报
                    case MessageTypes.ACCOUNTINFORESPONSE:
                        {
                            RspQryAccountInfoResponse response = packet as RspQryAccountInfoResponse;
                            Struct.V12.LCThostFtdcTradingAccountField field = new Struct.V12.LCThostFtdcTradingAccountField();

                            TradingLib.Common.AccountInfo info = response.AccInfo;
                            field.AccountID = info.Account;
                            field.BrokerID = "88888";
                            field.CloseProfit = (double)info.RealizedPL;
                            field.Commission = (double)info.Commission;
                            field.PositionProfit = (double)info.UnRealizedPL;
                            field.PreBalance = (double)info.LastEquity;
                            field.Balance = (double)info.NowEquity;

                            field.Deposit = (double)info.CashIn;
                            field.Withdraw = (double)info.CashOut;
                            field.CurrMargin = (double)info.FutMarginUsed;
                            field.FrozenMargin = (double)info.FutMarginFrozen;
                            field.Credit = (double)info.Credit;
                            field.PreCredit = (double)info.LastCredit;

                            field.Available = (double)info.AvabileFunds;
                            field.TradingDay = "";

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcTradingAccountField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_TDACC, response.RequestID, conn.NextSeqQryId);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryAccountInfoResponse", conn.SessionID));
                            break;
                        }
                        //银行回报
                    case MessageTypes.CONTRACTBANKRESPONSE:
                        {
                            RspQryContractBankResponse response = packet as RspQryContractBankResponse;
                            Struct.V12.LCThostFtdcContractBankField field = new Struct.V12.LCThostFtdcContractBankField();

                            field.BrokerID = "888888";
                            field.BankID = response.BankID;
                            field.BankBrchID = response.BankBrchID;
                            field.BankName = response.BankName;

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcContractBankField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_CONTBK, response.RequestID, conn.NextSeqQryId, response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200007501030c4ce104e28104e2017de1030177e33a2470e1793838383838e637e330303030e1d0cbd2b5d2f8d0d0efefefefefefe32470e1793838383838e638e330303030e1bbe0e3b7e0e1d2f8d0d0efefefefefefe32470e1793838383838e639e330303030e1b9e0e2b4f3d2f8d0d0efefefefefefe3";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryContractBankResponse", conn.SessionID));
                            break;
                        }
                        //签约关系回报
                    case MessageTypes.REGISTERBANKACCOUNTRESPONSE:
                        {
                            RspQryRegisterBankAccountResponse response = packet as RspQryRegisterBankAccountResponse;
                            Struct.V12.LCThostFtdcAccountregisterField field = new Struct.V12.LCThostFtdcAccountregisterField();

                            field.TradeDay = Util.ToTLDate().ToString();
                            field.BankID = response.BankID;
                            field.BankBranchID = "0000";
                            field.BankAccount = response.BankAC;
                            field.BrokerID = "88888";
                            field.BrokerBranchID = "0000";
                            field.AccountID = response.TradingAccount;
                            field.IdCardType = TThostFtdcIdCardTypeType.IDCard;
                            field.IdentifiedCardNo = "88888888";
                            field.CustomerName = "";
                            field.CurrencyID = "RMB";
                            field.OpenOrDestroy = TThostFtdcOpenOrDestroyType.Open;
                            field.RegDate = "20150101";
                            field.OutDate = "0";
                            field.TID = 1;
                            field.CustType = TThostFtdcCustTypeType.Person;
                            field.BankAccType = TThostFtdcBankAccTypeType.SavingCard;

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcAccountregisterField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_ACCREG, response.RequestID, conn.NextSeqQryId, response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryRegisterBankAccountResponse", conn.SessionID));
                            break;
                        }
                        //查询可开
                    case MessageTypes.MAXORDERVOLRESPONSE:
                        {
                            RspQryMaxOrderVolResponse response = packet as RspQryMaxOrderVolResponse;
                            Struct.V12.LCThostFtdcQueryMaxOrderVolumeField field = new Struct.V12.LCThostFtdcQueryMaxOrderVolumeField();
                            field.BrokerID = conn.State.BrokerID;
                            field.InvestorID = conn.State.LoginID;
                            field.Direction = response.Side ? TThostFtdcDirectionType.Buy : TThostFtdcDirectionType.Sell;
                            field.HedgeFlag = TThostFtdcHedgeFlagType.Speculation;
                            field.InstrumentID = response.Symbol;
                            field.MaxVolume = response.MaxVol;
                            field.OffsetFlag = CTPConvert.ConvOffsetFlag(response.OffsetFlag);

                            Struct.V12.LCThostFtdcRspInfoField rsp = new Struct.V12.LCThostFtdcRspInfoField();
                            rsp.ErrorID = 0;
                            rsp.ErrorMsg = "正确";

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcQueryMaxOrderVolumeField>(ref rsp, ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_MAXORDVOL, response.RequestID, conn.NextSeqQryId, response.IsLast);
                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            if (response.IsLast) logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryRegisterBankAccountResponse", conn.SessionID));
                            break;
                        }
                        //委托回报
                    case MessageTypes.ORDERNOTIFY:
                        {
                            OrderNotify notify = packet as OrderNotify;
                            Struct.V12.LCThostFtdcOrderField field = new Struct.V12.LCThostFtdcOrderField();

                            CTPConvert.ConvOrder(notify.Order, ref field);
                            field.BrokerID = conn.State.BrokerID;
                            field.FrontID = notify.Order.FrontIDi;
                            field.SessionID = notify.Order.SessionIDi;
                            field.ParticipantID = conn.State.BrokerID;

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackNotify<Struct.V12.LCThostFtdcOrderField>(ref field, EnumSeqType.SeqRtn, EnumTransactionID.T_RTN_ORDER, conn.NextSeqRtnId);

                            //从数据包加载数据
                            var tmp0 = ByteSwapHelp.BytesToStruct<Struct.proto_hdr>(data, 0);
                            var tmp1 = ByteSwapHelp.BytesToStruct<Struct.ftd_hdr>(data, 4);
                            EnumTransactionID transid = (EnumTransactionID)tmp1.dTransId;

                            var tmp2 = ByteSwapHelp.BytesToStruct<Struct.ftdc_hdr>(data, 0 + Constanst.PROFTD_HDRLEN);
                            EnumFiledID fieldid = (EnumFiledID)tmp2.wFiId;


                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> OrderNotify", conn.SessionID));
                            break;
                        }
                        //委托异常通知
                    case MessageTypes.ERRORORDERNOTIFY:
                        {
                            ErrorOrderNotify notify = packet as ErrorOrderNotify;
                            Struct.V12.LCThostFtdcInputOrderField field = new Struct.V12.LCThostFtdcInputOrderField();

                            field.BrokerID = conn.State.BrokerID;
                            field.BusinessUnit = "";
                            field.CombHedgeFlag_0 = TThostFtdcHedgeFlagType.Speculation;
                            field.CombOffsetFlag_0 = TThostFtdcOffsetFlagType.Open;
                            field.Direction =  notify.Order.Side?TThostFtdcDirectionType.Buy:TThostFtdcDirectionType.Sell;
                            field.ContingentCondition = TThostFtdcContingentConditionType.Immediately;
                            field.UserForceClose = 0;
                            field.ForceCloseReason = TThostFtdcForceCloseReasonType.NotForceClose;

                            field.InstrumentID = notify.Order.Symbol;
                            field.InvestorID = notify.Order.Symbol;
                            field.UserID = conn.State.LoginID;
                            field.OrderRef = notify.Order.OrderRef;
                            field.IsAutoSuspend = 0;
                            field.MinVolume = 0;
                            field.OrderPriceType = notify.Order.isLimit? TThostFtdcOrderPriceTypeType.LimitPrice: TThostFtdcOrderPriceTypeType.AnyPrice;
                            field.LimitPrice = (double)notify.Order.LimitPrice;
                            field.StopPrice = (double)notify.Order.StopPrice;
                            field.VolumeTotalOriginal = Math.Abs(notify.Order.TotalSize);
                            field.RequestID = notify.Order.RequestID;

                            Struct.V12.LCThostFtdcRspInfoField rsp = new Struct.V12.LCThostFtdcRspInfoField();
                            rsp.ErrorID = notify.RspInfo.ErrorID;
                            rsp.ErrorMsg = string.Format("CTP:{0}", notify.RspInfo.ErrorMessage);

                            if (notify.RspInfo.ErrorID == 0)
                            {
                                conn.State.Authorized = true;
                            }

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcInputOrderField>(ref rsp, ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_ORDINSERT, notify.Order.RequestID,conn.NextSeqReqId);

                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            //hex = "0200008501030c4ce43001e502e1f5e301e355e4d5fdc8b7efefefefefe21003e1983230313631323031e131323a32303a3337e13838383838e638353632303030383032e654726164696e67486f7374696e67efef04a515665831ec31323a32303a3337e131323a32303a3338e131323a32303a3338e131323a32303a3337e12d2d3a2d2d3a2d2de1";
                            //encData = ByteUtil.HexToByte(hex);
                            //encPktLen = encData.Length;

                            //发送数据
                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> ErrorOrderNotify", conn.SessionID));



                            break;
                        }
                        //成交回报
                    case MessageTypes.EXECUTENOTIFY:
                        {
                            TradeNotify notify = packet as TradeNotify;
                            Struct.V12.LCThostFtdcTradeField field = new Struct.V12.LCThostFtdcTradeField();

                            CTPConvert.ConvTrade(notify.Trade, ref field);
                            
                            field.BrokerID = conn.State.BrokerID;
                            field.InvestorID = conn.State.LoginID;
                            field.ClearingPartID = conn.State.BrokerID;
                            field.ParticipantID = conn.State.BrokerID;

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcTradeField>(ref field, EnumSeqType.SeqRtn, EnumTransactionID.T_RTN_TRADE, 0, conn.NextSeqRtnId);

                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> TradeNotify", conn.SessionID));
                            break;
                        }
                        //修改密码回报
                    case MessageTypes.CHANGEPASSRESPONSE:
                        {
                            RspReqChangePasswordResponse response = packet as RspReqChangePasswordResponse;

                            Struct.V12.LCThostFtdcUserPasswordUpdateField field = new Struct.V12.LCThostFtdcUserPasswordUpdateField();

                            field.BrokerID = conn.State.BrokerID;
                            field.UserID = conn.State.LoginID;

                            Struct.V12.LCThostFtdcRspInfoField rsp = new Struct.V12.LCThostFtdcRspInfoField();
                            rsp.ErrorID = response.RspInfo.ErrorID;
                            rsp.ErrorMsg = string.Format("CTP:{0}", response.RspInfo.ErrorMessage);

                            //打包数据
                            byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcUserPasswordUpdateField>(ref rsp, ref field, EnumSeqType.SeqReq, EnumTransactionID.T_RSP_MODPASS, response.RequestID, conn.NextSeqReqId);

                            int encPktLen = 0;
                            byte[] encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);

                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspReqChangePasswordResponse", conn.SessionID));

                            break;
                        }
                        //查询出入金记录
                    case MessageTypes.TRANSFERSERIALRESPONSE:
                        {
                            RspQryTransferSerialResponse response = packet as RspQryTransferSerialResponse;
                            byte[] encData = null;
                            int encPktLen = 0;

                            if (response.CashTransaction == null)
                            {
                                byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_TFSN, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            else
                            {
                                Struct.V12.LCThostFtdcTransferSerialField field = new Struct.V12.LCThostFtdcTransferSerialField();

                                DateTime dt = Util.ToDateTime(response.CashTransaction.DateTime);
                                field.TradeDate = Util.ToTLDate(dt).ToString();
                                field.TradeTime = dt.ToString("HH:mm:ss");
                                field.InvestorID = conn.State.LoginID;
                                field.AccountID = conn.State.LoginID;
                                field.BankAccount = response.CashTransaction.BankAccount;
                                field.TradeAmount = (double)response.CashTransaction.Amount;
                                field.CurrencyID = "CNY";
                                field.AvailabilityFlag = TThostFtdcAvailabilityFlagType.Valid;
                                field.BankAccType = TThostFtdcBankAccTypeType.SavingCard;
                                field.BankSerial = response.CashTransaction.TxnRef;
                                field.FutureSerial = 0;
                                field.ErrorID = 0;
                                field.ErrorMsg = "交易成功";
                                field.TradeCode = response.CashTransaction.TxnType == QSEnumCashOperation.Deposit ? "202001" : "202002";//根据此编号区别银行到期货 还是期货到银行



                                //打包数据
                                byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcTransferSerialField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_TFSN, response.RequestID, conn.NextSeqQryId, response.IsLast);

                                encPktLen = 0;
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspQryTransferSerialResponse", conn.SessionID));

                            break;
                        }
                        //查询持仓明细回报
                    case MessageTypes.XPOSITIONDETAILRESPONSE:
                        {
                            RspXQryPositionDetailResponse response = packet as RspXQryPositionDetailResponse;

                            byte[] encData = null;
                            int encPktLen = 0;
                            if (response.PositionDetail == null)
                            {
                                byte[] data = Struct.V12.StructHelperV12.PackRsp(EnumSeqType.SeqQry, EnumTransactionID.T_RSP_POSDETAIL, response.RequestID, conn.NextSeqQryId, response.IsLast);
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            else
                            {
                                Struct.V12.LCThostFtdcInvestorPositionDetailField field = new Struct.V12.LCThostFtdcInvestorPositionDetailField();

                                field.BrokerID = conn.State.BrokerID;
                                field.InstrumentID = response.PositionDetail.Symbol;
                                field.InvestorID = response.PositionDetail.Account;
                                field.ExchangeID = response.PositionDetail.Exchange;
                                field.TradeID = response.PositionDetail.TradeID;

                                field.CloseAmount = (double)response.PositionDetail.CloseAmount;
                                field.CloseVolume = response.PositionDetail.CloseVolume;
                                field.Volume = response.PositionDetail.Volume;

                                field.HedgeFlag = TThostFtdcHedgeFlagType.Speculation;
                                field.Direction = response.PositionDetail.Side ? TThostFtdcDirectionType.Buy : TThostFtdcDirectionType.Sell;
                                field.TradeType = TThostFtdcTradeTypeType.Common;

                                field.LastSettlementPrice = (double)response.PositionDetail.LastSettlementPrice;
                                field.SettlementPrice = (double)response.PositionDetail.SettlementPrice;
                                field.ExchMargin = 0;
                                field.Margin = (double)response.PositionDetail.Margin;
                                field.MarginRateByMoney = 0;
                                field.MarginRateByVolume = 0;
                                field.OpenDate = response.PositionDetail.OpenDate.ToString();
                                field.OpenPrice = (double)response.PositionDetail.OpenPrice;

                                field.CloseProfitByDate = (double)response.PositionDetail.CloseProfitByDate;
                                field.CloseProfitByTrade = (double)response.PositionDetail.CloseProfitByTrade;
                                field.PositionProfitByDate = 0;
                                field.PositionProfitByTrade = 0;

                                field.SettlementID = 1;

                                //field.TradingDay = 0;
                                //打包数据
                                byte[] data = Struct.V12.StructHelperV12.PackRsp<Struct.V12.LCThostFtdcInvestorPositionDetailField>(ref field, EnumSeqType.SeqQry, EnumTransactionID.T_RSP_POSDETAIL, response.RequestID, conn.NextSeqQryId, response.IsLast);

                                encPktLen = 0;
                                encData = Struct.V12.StructHelperV12.EncPkt(data, out encPktLen);
                            }
                            conn.Send(encData, encPktLen);
                            logger.Info(string.Format("LogicSrv Reply Session:{0} -> RspXQryPositionDetailResponse", conn.SessionID));
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
