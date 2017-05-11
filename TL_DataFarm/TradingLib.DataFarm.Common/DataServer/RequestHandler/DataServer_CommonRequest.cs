﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TradingLib.API;
using TradingLib.Common;
using TradingLib.DataFarm.API;
using Common.Logging;

namespace TradingLib.DataFarm.Common
{
    public partial class DataServer
    {
        /// <summary>
        /// 更新客户端心跳时间戳
        /// </summary>
        /// <param name="conn"></param>
        void SrvUpdateHeartBeat(IConnection conn)
        {
            conn.LastHeartBeat = DateTime.Now;
        }

        

        /// <summary>
        /// 服务端版本查询
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected void SrvOnVersionRequest(IServiceHost host, IConnection conn, VersionRequest request)
        {
            logger.Info(string.Format("Conn:{0} Qry Version:{1}", conn.SessionID, request.Content));

            VersionResponse response = ResponseTemplate<VersionResponse>.SrvSendRspResponse(request);

            string key=string.Empty;
            string strval = string.Empty;
            //TLSocket类别的前置连接 执行客户端与服务端配对检查
            if (conn.FrontType == EnumFrontType.TLSocket)
            {
                key = request.NegotiationKey;
                strval = request.NegotiationString;
                string uuid = StringCipher.Decrypt(request.EncryptUUID, key);
                if (uuid != conn.SessionID)
                {
                    CloseConnection(conn);
                    return;
                }
            }

            TLNegotiation neo = new TLNegotiation();
            neo.DeployID = string.Empty;
            neo.NegoResponse = StringCipher.Encrypt(strval,key);
            neo.PlatformID = System.Environment.OSVersion.Platform;
            neo.Product = "DataSite";
            neo.TLProtoclType = EnumTLProtoclType.TL_Encrypted;
            response.Negotiation = neo;

            this.SendData(conn, response);

        }

        /// <summary>
        /// 服务端功能查询
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected void SrvOnFeatureRequest(IServiceHost host, IConnection conn, FeatureRequest request)
        {
            FeatureResponse response = ResponseTemplate<FeatureResponse>.SrvSendRspResponse(request);
            response.Add(MessageTypes.XQRYMARKETTIME);
            response.Add(MessageTypes.XQRYEXCHANGE);
            response.Add(MessageTypes.XQRYSECURITY);
            response.Add(MessageTypes.XQRYSYMBOL);

            this.SendData(conn, response);
        }

        /// <summary>
        /// 服务端端心跳响应
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void SrvOnHeartbeatRequest(IServiceHost host, IConnection conn, HeartBeatRequest request)
        {
            HeartBeatResponse response = ResponseTemplate<HeartBeatResponse>.SrvSendRspResponse(request);

            this.SendData(conn, response);
        }

        void SrvOnLoginRequest(IServiceHost host, IConnection conn, LoginRequest request)
        {
            LoginResponse response = ResponseTemplate<LoginResponse>.SrvSendRspResponse(request);
            response.Account = "9999";
            response.AccountType = QSEnumAccountCategory.SUBACCOUNT;
            response.Authorized = true;
            response.TradingDay = 20161006;
            response.RspInfo = new RspInfoImpl();

            this.SendData(conn, response);
        }

        //为什么超过一定数量的Bar一起发送 客户端就无法收到数据 socket缓存?
        /// <summary>
        /// 查询Bar数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnBarRequest(IServiceHost host, IConnection conn, QryBarRequest request)
        {
            try
            {
                if (_verbose) logger.Info("Got Qry Bar Request:" + request.ToString());
                IHistDataStore store = this.GetHistDataSotre();
                if (store == null)
                {
                    logger.Warn("HistDataSotre is null, can not provider QryBar service");
                    throw new Exception("DataStore not inited");
                }
                Symbol symbol = MDBasicTracker.SymbolTracker[request.Exchange,request.Symbol];
                if (symbol == null)
                {
                    logger.Warn(string.Format("Symbol:{0} do not exist", request.Symbol));
                    return;
                }

                //pf.EnterSection("QRY  BAR");
                List<BarImpl> bars = store.QryBar(symbol, request.IntervalType, request.Interval, request.Start.ToDateTimeEx(DateTime.MinValue), request.End.ToDateTimeEx(DateTime.MaxValue),request.StartIndex,request.MaxCount,request.HavePartial);
                //pf.LeaveSection();

                //pf.EnterSection("SEND BAR");
                switch(request.BarResponseType)
                {
                    case EnumBarResponseType.PLAINTEXT:
                        {
                            for (int i = 0; i < bars.Count; i++)
                            {
                                RspQryBarResponse response = ResponseTemplate<RspQryBarResponse>.SrvSendRspResponse(request);
                                response.Bar = bars[i];
                                response.IsLast = i == bars.Count - 1;
                                this.SendData(conn, response);
                            }
                            break;
                        }
                    case EnumBarResponseType.BINARY:
                        {
                            int j = 0;
                            RspQryBarResponseBin response = RspQryBarResponseBin.CreateResponse(request);
                            response.IsLast = false;
                            for (int i = 0; i < bars.Count; i++)
                            {
                                response.Add(bars[i]);
                                j++;
                                if (j == _barbatchsize)
                                {
                                    //一定数目的Bar之后 发送数据 同时判断是否是最后一条
                                    response.IsLast = (i == bars.Count-1);
                                    this.SendData(conn, response);
                                    //不是最后一条数据则生成新的Response
                                    if (!response.IsLast)
                                    {
                                        response = RspQryBarResponseBin.CreateResponse(request);
                                        response.IsLast = false;
                                    }
                                    j = 0;
                                }
                            }
                            //如果不为最后一条 则标记为最后一条并发送
                            if (!response.IsLast)
                            {
                                response.IsLast = true;
                                this.SendData(conn, response);
                            }
                            break;
                        }
                    default:
                        break;
                }
                //pf.LeaveSection();
                //logger.Info(string.Format("----BarRequest Statistics QTY:{0}---- \n{1}", bars.Count, pf.GetStatsString()));
                
            }
            catch (Exception ex)
            {
                logger.Error("SrvOnBarRequest Error:" + ex.ToString());
            }
        }
        
        /// <summary>
        /// 查询分笔成交数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected void SrvOnQryTradeSplitRequest(IServiceHost host, IConnection conn, XQryTradeSplitRequest request)
        {
            if (_verbose) logger.Info("Got Qry Trads Request:" + request.ToString());
            Symbol symbol = MDBasicTracker.SymbolTracker[request.Exchange, request.Symbol];
            if (symbol == null)
            {
                logger.Warn(string.Format("Symbol:{0} do not exist", request.Symbol));
                return;
            }

            List<Tick> trades = eodservice.QryTrade(symbol, request.StartIndex, request.MaxCount, request.Tradingday);

            int j = 0;
            RspXQryTradeSplitResponse response = RspXQryTradeSplitResponse.CreateResponse(request);
            response.IsLast = false;
            for (int i = 0; i < trades.Count; i++)
            {
                response.Add(trades[i]);
                j++;
                if (j == _tradebatchsize)
                {
                    //一定数目的Bar之后 发送数据 同时判断是否是最后一条
                    response.IsLast = (i == trades.Count - 1);
                    this.SendData(conn, response);
                    //不是最后一条数据则生成新的Response
                    if (!response.IsLast)
                    {
                        response = RspXQryTradeSplitResponse.CreateResponse(request);
                        response.IsLast = false;
                    }
                    j = 0;
                }
            }
            //如果不为最后一条 则标记为最后一条并发送
            if (!response.IsLast)
            {
                response.IsLast = true;
                this.SendData(conn, response);
            }

        }

        /// <summary>
        /// 查询价格成交量数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected void SrvOnQryPriceVolRequest(IServiceHost host, IConnection conn, XQryPriceVolRequest request)
        {
            if (_verbose) logger.Info("Got Qry PriceVol Request:" + request.ToString());
            Symbol symbol = MDBasicTracker.SymbolTracker[request.Exchange, request.Symbol];
            if (symbol == null)
            {
                logger.Warn(string.Format("Symbol:{0} do not exist", request.Symbol));
                return;
            }

            List<PriceVol> pvlist = eodservice.QryPriceVol(symbol, request.Tradingday);

            int j = 0;
            RspXQryPriceVolResponse response = RspXQryPriceVolResponse.CreateResponse(request);
            response.IsLast = false;
            for (int i = 0; i < pvlist.Count; i++)
            {
                response.Add(pvlist[i]);
                j++;
                if (j == _pricevolbatchsize)
                {
                    //一定数目的Bar之后 发送数据 同时判断是否是最后一条
                    response.IsLast = (i == pvlist.Count - 1);
                    this.SendData(conn, response);
                    //不是最后一条数据则生成新的Response
                    if (!response.IsLast)
                    {
                        response = RspXQryPriceVolResponse.CreateResponse(request);
                        response.IsLast = false;
                    }
                    j = 0;
                }
            }
            //如果不为最后一条 则标记为最后一条并发送
            if (!response.IsLast)
            {
                response.IsLast = true;
                this.SendData(conn, response);
            }
        }

        protected void SrvOnQryMinuteDataRequest(IServiceHost host, IConnection conn, XQryMinuteDataRequest request)
        {
            
            Symbol symbol = MDBasicTracker.SymbolTracker[request.Exchange, request.Symbol];
            if (symbol == null)
            {
                logger.Warn(string.Format("Symbol:{0} do not exist", request.Symbol));
                return;
            }
            MarketDay md = eodservice.GetCurrentMarketDay(symbol.SecurityFamily);
            if (md == null)
            {
                logger.Warn(string.Format("Sec:{0} have no marketday", symbol.SecurityFamily.Code));
                return;
            }
            logger.Info(string.Format("Sec:{0} marketday:{1}", symbol.SecurityFamily.Code, md.ToSessionString()));

            int tradingday = request.Tradingday;
            if (tradingday == 0)
            {
                tradingday = md.TradingDay;
            }

            List<MinuteData> mdlist = eodservice.QryMinuteData(symbol, tradingday,request.Start.ToDateTimeEx(DateTime.MinValue));////GetHistDataSotre().QryMinuteData(symbol, tradingday);

            int j = 0;
            RspXQryMinuteDataResponse response = RspXQryMinuteDataResponse.CreateResponse(request);
            response.IsLast = false;
            for (int i = 0; i < mdlist.Count; i++)
            {
                response.Add(mdlist[i]);
                j++;
                if (j == _minutedatabatchsize)
                {
                    //一定数目的Bar之后 发送数据 同时判断是否是最后一条
                    response.IsLast = (i == mdlist.Count - 1);
                    this.SendData(conn, response);
                    //不是最后一条数据则生成新的Response
                    if (!response.IsLast)
                    {
                        response = RspXQryMinuteDataResponse.CreateResponse(request);
                        response.IsLast = false;
                    }
                    j = 0;
                }
            }
            //如果不为最后一条 则标记为最后一条并发送
            if (!response.IsLast)
            {
                response.IsLast = true;
                this.SendData(conn, response);
                logger.Info("Got Qry MinuteData Request:" + request.ToString() + " res cnt:" + response.MinuteDataList.Count);
            }
        }



        /// <summary>
        /// 查询交易所数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnQryExchangeRequest(IServiceHost host, IConnection conn, XQryExchangeRequuest request)
        {
            if (_verbose) logger.Info(string.Format("Conn:{0} qry exchange:{1}", conn.SessionID, request.ToString()));
            ExchangeImpl[] exchs =MDBasicTracker.ExchagneTracker.Exchanges;

            int totalnum = exchs.Length;

            for (int i = 0; i < totalnum; i++)
            {
                RspXQryExchangeResponse response = ResponseTemplate<RspXQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i];
                response.IsLast = i==totalnum - 1;
                this.SendData(conn, response);
            }
        }

        /// <summary>
        /// 查询交易时间段数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnQryMarketTimeRequest(IServiceHost host, IConnection conn, XQryMarketTimeRequest request)
        {
            if (_verbose) logger.Info(string.Format("Conn:{0} qry market time:{1}", conn.SessionID, request.ToString()));
            MarketTimeImpl[] mts = MDBasicTracker.MarketTimeTracker.MarketTimes.ToArray();

            int totalnum = mts.Length;
            for (int i = 0; i < totalnum; i++)
            {
                RspXQryMarketTimeResponse response = ResponseTemplate<RspXQryMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = mts[i];
                response.IsLast = i == totalnum - 1;
                this.SendData(conn, response);
            }
            
        }
        const int MAXSENTSIZE = 1000;


        /// <summary>
        /// 查询品种数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnQrySecurityRequest(IServiceHost host, IConnection conn, XQrySecurityRequest request)
        {
            if (_verbose) logger.Info(string.Format("Conn:{0} qry security:{1}", conn.SessionID, request.ToString()));
            SecurityFamilyImpl[] seclist = MDBasicTracker.SecurityTracker.Securities.ToArray();
            int totalnum = seclist.Length;
            int n = 0;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQrySecurityResponse response = ResponseTemplate<RspXQrySecurityResponse>.SrvSendRspResponse(request);
                    response.SecurityFaimly = seclist[i];
                    response.IsLast = i == totalnum - 1;
                    this.SendData(conn, response);
                }
            }
            else
            {
                RspXQrySecurityResponse response = ResponseTemplate<RspXQrySecurityResponse>.SrvSendRspResponse(request);
                this.SendData(conn, response);
            }
        }



        /// <summary>
        /// 查询合约数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnQrySymbolRequest(IServiceHost host, IConnection conn, XQrySymbolRequest request)
        {
            if (_verbose) logger.Info(string.Format("Conn:{0} qry symbols:{1}", conn.SessionID, request.ToString()));
            SymbolImpl[] symlis = MDBasicTracker.SymbolTracker.Symbols.ToArray();
            int totalnum = symlis.Length;
            int n = 0;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQrySymbolResponse response = ResponseTemplate<RspXQrySymbolResponse>.SrvSendRspResponse(request);
                    response.Symbol = symlis[i];
                    response.IsLast = i == totalnum - 1;
                    this.SendData(conn, response);
                }
            }
            else
            {
                RspXQrySymbolResponse response = ResponseTemplate<RspXQrySymbolResponse>.SrvSendRspResponse(request);
                this.SendData(conn, response);
            }
        }


        /// <summary>
        /// 注销实时行情
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnUnregisterSymbolTick(IServiceHost host, IConnection conn, UnregisterSymbolTickRequest request)
        {
            if (_verbose) logger.Info(string.Format("Conn:{0} Unregister {1},{2}", conn.SessionID, request.Exchange, string.Join(" ", request.SymbolList.ToArray())));
            OnUngisterSymbol(conn, request);
        }

        /// <summary>
        /// 订阅实时行情
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnRegisterSymbolTick(IServiceHost host, IConnection conn, RegisterSymbolTickRequest request)
        {

            if (_verbose) logger.Info(string.Format("Conn:{0} Register {1},{2}", conn.SessionID, request.Exchange, string.Join(" ", request.SymbolList.ToArray())));
            OnRegisterSymbol(conn, request);
        }

        protected virtual void SrvOnXQryTickSnapshot(IServiceHost host, IConnection conn, XQryTickSnapShotRequest request)
        {
            if (_verbose) logger.Info(string.Format("Conn:{0} Qry TickSnapshot {1} {2}", conn.SessionID, request.Exchange, request.Symbol));

            //查询所有行情快照
            if (string.IsNullOrEmpty(request.Exchange) && string.IsNullOrEmpty(request.Symbol))
            {
                
                Tick[] list = Global.TickTracker.TickSnapshots.ToArray();
                for (int i = 0; i < list.Length; i++)
                {
                    RspXQryTickSnapShotResponse response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
                    response.Tick = list[i];
                    response.IsLast = i == list.Length - 1;
                    this.SendData(conn, response);
                    
                }
            }
            else //查询单个合约的行情快照
            {
                //客户端订阅后发送当前市场快照
                Tick k = Global.TickTracker[request.Exchange, request.Symbol];
                if (k != null)
                {
                    RspXQryTickSnapShotResponse response = ResponseTemplate<RspXQryTickSnapShotResponse>.SrvSendRspResponse(request);
                    response.Tick = k;
                    this.SendData(conn, response);
                }
            }


        
        }


        void SrvOnMDDemoTick(IServiceHost host, IConnection conn, MDDemoTickRequest request)
        {
            //logger.Info(string.Format("Conn:{0} send demotick request", conn.SessionID));
            //Tick k = new TickImpl("CNH6");
            //k.Date = 20160323;
            //k.Time = request.Time;
            //k.Trade = request.Trade;
            //k.Size = 1;

            //freqService.ProcessTick(k);
            //foreach (var bar in MBar.LoadBars("IF04", BarInterval.CustomTime, 60, DateTime.MinValue, DateTime.MaxValue, 1000, true))
            //{
            //    logger.Info("bar:" + bar.ToString());
            //}
            
        }


        
    }
}
