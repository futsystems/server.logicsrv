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
                #region 查询分时数据
                case XLProtocol.XLMessageType.T_QRY_MINUTEDATA:
                    {
                        var data = reqPkt.FieldList[0];
                        if (data is XLQryMinuteDataField)
                        {
                            //logger.Info("Qry minute data");
                            var request =(XLQryMinuteDataField)data;

                            Symbol symbol = MDBasicTracker.SymbolTracker[request.ExchangeID, request.SymbolID];
                            if (symbol == null)
                            {
                                logger.Warn(string.Format("Symbol:{0} do not exist", request.SymbolID));
                                return;
                            }
                            MarketDay md = eodservice.GetCurrentMarketDay(symbol.SecurityFamily);
                            if (md == null)
                            {
                                logger.Warn(string.Format("Sec:{0} have no marketday", symbol.SecurityFamily.Code));
                                return;
                            }
                            logger.Info(string.Format("Sec:{0} marketday:{1}", symbol.SecurityFamily.Code, md.ToSessionString()));

                            int tradingday = request.TradingDay;
                            if (tradingday == 0)
                            {
                                tradingday = md.TradingDay;
                            }

                            List<MinuteData> mdlist = eodservice.QryMinuteData(symbol, tradingday, request.Start.ToDateTimeEx(DateTime.MinValue));////GetHistDataSotre().QryMinuteData(symbol, tradingday);


                            int j = 0;
                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_MINUTEDATA);
                            bool islast = false;
                            for (int i = 0; i < mdlist.Count; i++)
                            {
                                var item = mdlist[i];
                                XLMinuteDataField t = new XLMinuteDataField();
                                t.Close = item.Close;
                                t.Vol = item.Vol;
                                t.Date = item.Date;
                                t.Time = item.Time;

                                pkt.AddField(t);

                                j++;
                                if (j == _minutedatabatchsize)
                                {
                                    islast = (i == mdlist.Count - 1);

                                    if (conn.ProtocolType == EnumConnProtocolType.XL)
                                    {
                                        byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, islast);
                                        SendData(conn, ret);
                                    }
                                    if (conn.ProtocolType == EnumConnProtocolType.Json)
                                    {
                                        string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, islast);
                                        SendData(conn, json);
                                    }

                                    if (!islast)
                                    {
                                        pkt = new XLPacketData(XLMessageType.T_RSP_MINUTEDATA);
                                    }
                                    j = 0;
                                }
                            }

                            if (!islast)
                            {
                                islast = true;
                                if (conn.ProtocolType == EnumConnProtocolType.XL)
                                {
                                    byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, islast);
                                    SendData(conn, ret);
                                }
                                if (conn.ProtocolType == EnumConnProtocolType.Json)
                                {
                                    string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, islast);
                                    SendData(conn, json);
                                }
                            }
                        }
                    }
                    break;
                #endregion
                case XLMessageType.T_QRY_BARDATA:
                    { 
                        var data = reqPkt.FieldList[0];
                        if (data is XLQryBarDataField)
                        {
                            //logger.Info("Qry minute data");
                            var request = (XLQryBarDataField)data;
                            IHistDataStore store = this.GetHistDataSotre();
                            if (store == null)
                            {
                                logger.Warn("HistDataSotre is null, can not provider QryBar service");
                                throw new Exception("DataStore not inited");
                            }
                            Symbol symbol = MDBasicTracker.SymbolTracker[request.ExchangeID, request.SymbolID];
                            if (symbol == null)
                            {
                                logger.Warn(string.Format("Symbol:{0} do not exist", request.SymbolID));
                                return;
                            }

                            List<BarImpl> bars = store.QryBar(symbol,BarInterval.CustomTime, request.Interval, request.Start.ToDateTimeEx(DateTime.MinValue), request.End.ToDateTimeEx(DateTime.MaxValue), request.StartIndex, request.MaxCount, request.HavePartial);

                            int j = 0;
                            XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_BARDATA);
                            bool islast = false;
                            for (int i = 0; i < bars.Count; i++)
                            {
                                pkt.AddField(bars[i].ToXLBarDataField());
                                j++;
                                if (j == _barbatchsize)
                                {
                                    //一定数目的Bar之后 发送数据 同时判断是否是最后一条
                                    islast = (i == bars.Count - 1);
                                    if (conn.ProtocolType == EnumConnProtocolType.XL)
                                    {
                                        byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, islast);
                                        SendData(conn, ret);
                                    }
                                    if (conn.ProtocolType == EnumConnProtocolType.Json)
                                    {
                                        string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, islast);
                                        SendData(conn, json);
                                    }

                                    if (!islast)
                                    {
                                        pkt = new XLPacketData(XLMessageType.T_RSP_MINUTEDATA);
                                    }
                                    j = 0;
                                }
                            }
                            //如果不为最后一条 则标记为最后一条并发送
                            if (!islast)
                            {
                                islast = true;
                                if (conn.ProtocolType == EnumConnProtocolType.XL)
                                {
                                    byte[] ret = XLPacketData.PackToBytes(pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, islast);
                                    SendData(conn, ret);
                                }
                                if (conn.ProtocolType == EnumConnProtocolType.Json)
                                {
                                    string json = XLPacketData.PackJsonResponse(pkt, (int)requestId, islast);
                                    SendData(conn, json);
                                }
                            }

                        }
                        else
                        {
                            logger.Error("Erro request filed for QryBar");
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
