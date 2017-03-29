using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using TradingLib.XLProtocol;
using TradingLib.XLProtocol.V1;

using Common.Logging;
namespace TradingLib.DataFarm.Common
{
    public partial class DataServer
    {
        void OnXLRequestEvent(IServiceHost host, IConnection conn, object xldata, int requestId)
        {
            var reqPkt = xldata as XLProtocol.XLPacketData;
            if (reqPkt == null)
            {
                logger.Error("XLRequest Data null");
                return;
            }
            //更新客户端连接心跳
            SrvUpdateHeartBeat(conn);

            
            switch (reqPkt.MessageType)
            {
                case XLMessageType.T_HEARTBEEAT:
                    {

                        XLPacketData pkt = new XLPacketData(XLMessageType.T_HEARTBEEAT);
                        byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, true);
                        SendData(conn, ret);
                        break;
                    }
                case XLMessageType.T_REQ_LOGIN:
                    {
                        var data = reqPkt.FieldList[0];
                        if (data is XLReqLoginField)
                        {
                            XLReqLoginField request = (XLReqLoginField)data;

                            XLRspLoginField field = new XLRspLoginField();
                            field.TradingDay = 1;
                            field.UserID = request.UserID;
                            field.Name = "";

                            ErrorField rsp = new ErrorField();

                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_LOGIN);
                            pkt.AddField(rsp);
                            pkt.AddField(field);

                            if (conn.ProtocolType == EnumConnProtocolType.XL)
                            {
                                byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, true);
                                SendData(conn, ret);
                            }
                            else
                            {
                                string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, true);
                                SendData(conn, json);
                            }
                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", reqPkt.MessageType));
                        }
                        break;
                    }
                case XLMessageType.T_REQ_MARKETDATA:
                    {
                        foreach (var data in reqPkt.FieldList)
                        {
                            if (data is XLSpecificSymbolField)
                            {
                                XLSpecificSymbolField req = (XLSpecificSymbolField)data;
                                OnXLRegisterSymbol(conn, req);
                            }
                        }
                        break;
                    }
                case XLProtocol.XLMessageType.T_REQ_UPDATEPASS:
                    {

                    }
                    break;
                case XLProtocol.XLMessageType.T_QRY_SYMBOL:
                    {
                        var data = reqPkt.FieldList[0];
                        if (data is XLQrySymbolField)
                        {
                            logger.Info("Request Symbol data");
                            int i=0;
                            int length = MDBasicTracker.SymbolTracker.Symbols.Count();
                            foreach (var sym in MDBasicTracker.SymbolTracker.Symbols)
                            {
                                i++;
                                XLSymbolField field = new XLSymbolField();
                                field.SymbolID = sym.Symbol;
                                field.ExchangeID = sym.Exchange;
                                field.SymbolName = sym.GetName(true);
                                field.SecurityID = sym.SecurityFamily.Code;
                                field.SecurityType = XLSecurityType.Future;
                                field.Multiple = sym.SecurityFamily.Multiple;
                                field.PriceTick = (double)sym.SecurityFamily.PriceTick;
                                field.ExpireDate = sym.ExpireDate.ToString();
                                field.Currency = ConvCurrencyType(sym.Currency);


                                XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SYMBOL);
                                pkt.AddField(field);

                                if (conn.ProtocolType == EnumConnProtocolType.XL)
                                {
                                    byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, i == length);
                                    SendData(conn, ret);
                                }
                                if (conn.ProtocolType == EnumConnProtocolType.Json)
                                {
                                    string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, i == length);
                                    SendData(conn, json);
                                }
                            }

                        }
                    }
                    break;
                default:
                    logger.Warn(string.Format("XLMessage Type:{0} not handled", reqPkt.MessageType));
                    break;
            }


        }

        /// <summary>
        /// 转换货币
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        XLCurrencyType ConvCurrencyType(CurrencyType type)
        {
            switch (type)
            {
                case CurrencyType.RMB: return XLCurrencyType.RMB;
                case CurrencyType.USD: return XLCurrencyType.USD;
                case CurrencyType.HKD: return XLCurrencyType.HKD;
                case CurrencyType.EUR: return XLCurrencyType.EUR;
                default:
                    return XLCurrencyType.RMB;
            }
        }



    }

}
