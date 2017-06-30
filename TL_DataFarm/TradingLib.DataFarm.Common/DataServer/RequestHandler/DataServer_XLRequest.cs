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
                        SendXLData(conn, pkt, XLEnumSeqType.SeqReq, (uint)0, (uint)requestId, true);
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

                            SendXLData(conn, pkt, XLEnumSeqType.SeqQry, (uint)0, (uint)requestId, true);
                        }
                        else
                        {
                            logger.Warn(string.Format("Request:{0} Data Field do not macth", reqPkt.MessageType));
                        }
                        break;
                    }
                case XLMessageType.T_REQ_SUB_MARKETDATA:
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
                case XLMessageType.T_REQ_UNSUB_MARKETDATA:
                    {
                        foreach (var data in reqPkt.FieldList)
                        {
                            if (data is XLSpecificSymbolField)
                            {
                                XLSpecificSymbolField req = (XLSpecificSymbolField)data;
                                OnXLUnRegisterSymbol(conn, req);
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
                                field.SymbolName = sym.GetTitleName(true);
                                field.SecurityID = sym.SecurityFamily.Code;
                                field.SecurityType = XLSecurityType.Future;
                                field.Multiple = sym.SecurityFamily.Multiple;
                                field.PriceTick = (double)sym.SecurityFamily.PriceTick;
                                field.ExpireDate = sym.ExpireDate.ToString();
                                field.Currency = ConvCurrencyType(sym.Currency);
                                field.TradingSession = sym.TradingSession;

                                XLPacketData pkt = new XLPacketData(XLMessageType.T_RSP_SYMBOL);
                                pkt.AddField(field);

                                SendXLData(conn, pkt, XLEnumSeqType.SeqQry, (uint)0, (uint)requestId, i == length);
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
                            
                            int tradingday = request.TradingDay;
                            if (tradingday == 0)
                            {
                                tradingday = md.TradingDay;
                            }

                            List<MinuteData> mdlist = eodservice.QryMinuteData(symbol, tradingday, request.Start.ToDateTimeEx(DateTime.MinValue));

                            if (_verbose)
                            {
                                logger.Info(string.Format("Qry MinuteData Symbol:{0} Tradingday:{1} Sec:{2} marketday:{3} Start:{4} StartTime:{5} Cnt:{6}",
                                    symbol.Symbol,
                                    tradingday,
                                    symbol.SecurityFamily.Code,
                                    md.ToSessionString(),
                                    request.Start,
                                    request.Start.ToDateTimeEx(DateTime.MinValue),
                                    mdlist.Count
                                    ));
                            }

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
                                    SendXLData(conn, pkt, XLEnumSeqType.SeqQry, (uint)0, (uint)requestId, islast);
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
                                SendXLData(conn, pkt, XLEnumSeqType.SeqQry, (uint)0, (uint)requestId, islast);
                            }
                        }
                    }
                    break;
                #endregion

                #region 查询Bar数据
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

                            List<BarImpl> bars = store.QryBar(symbol, request.IsEOD ? BarInterval.Day : BarInterval.CustomTime, request.Interval, request.Start.ToDateTimeEx(DateTime.MinValue), request.End.ToDateTimeEx(DateTime.MaxValue), request.StartIndex, request.MaxCount, request.HavePartial);

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
                                    SendXLData(conn, pkt, XLEnumSeqType.SeqQry, (uint)0, (uint)requestId, islast);
                                    if (!islast)
                                    {
                                        pkt = new XLPacketData(XLMessageType.T_RSP_BARDATA);
                                    }
                                    j = 0;
                                }
                            }
                            //如果不为最后一条 则标记为最后一条并发送
                            if (!islast)
                            {
                                islast = true;
                                SendXLData(conn, pkt, XLEnumSeqType.SeqQry, (uint)0, (uint)requestId, islast);
                            }

                        }
                        else
                        {
                            logger.Error("Erro request filed for QryBar");
                        }
                    }
                    break;
                #endregion

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
