using System;
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
    public partial class DataServerBase
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
        protected virtual void SrvOnVersionRequest(IServiceHost host, IConnection conn, VersionRequest request)
        {

            VersionResponse response = ResponseTemplate<VersionResponse>.SrvSendRspResponse(request);
            TLVersion v = new TLVersion();
            v.ProductType = QSEnumProductType.CounterSystem;
            v.Platfrom = PlatformID.Unix;
            v.Major = 1;
            v.Minor = 0;
            v.Fix = 0;
            v.DeployID = "demo";
            response.Version = v;

            conn.Send(response);

        }

        /// <summary>
        /// 服务端功能查询
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        protected virtual void SrvOnFeatureRequest(IServiceHost host, IConnection conn, FeatureRequest request)
        {
            FeatureResponse response = ResponseTemplate<FeatureResponse>.SrvSendRspResponse(request);
            response.Add(MessageTypes.XQRYMARKETTIME);
            response.Add(MessageTypes.XQRYEXCHANGE);
            response.Add(MessageTypes.XQRYSECURITY);
            response.Add(MessageTypes.XQRYSYMBOL);

            conn.Send(response);
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
            conn.Send(response);
        }

        //为什么超过一定数量的Bar一起发送 客户端就无法收到数据 socket缓存?
        int _barbatchsize = 1;
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
                logger.Info("Got Qry Bar Request:" + request.ToString());
                //查询Bar数据
                IHistDataStore store = this.GetHistDataSotre();
                if (store == null)
                {
                    logger.Warn("HistDataSotre is null, can not provider QryBar service");
                    throw new Exception("DataStore not inited");
                }

                //合约频率数据没有注册直接返回
                //if (!store.IsRegisted(request.Symbol, request.IntervalType, request.Interval))
                //{
                //    logger.Warn(string.Format("SymbolFreq:{0}-{1}-{2} is not registed", request.Symbol, request.IntervalType, request.Interval));
                //    throw new Exception("SymbolFreq not registed");
                //}
                Symbol symbol = MDBasicTracker.SymbolTracker[request.Symbol];
                if (symbol == null)
                {
                    logger.Warn(string.Format("Symbol:{0} do not exist", request.Symbol));
                    return;
                }

                //如果数据已缓存则直接查询
                if (store.IsCached(request.Symbol, request.IntervalType, request.Interval))
                {
                    Profiler pf = new Profiler();
                    pf.EnterSection("QRY  BAR");
                    List<BarImpl> bars = store.QryBar(symbol, request.IntervalType, request.Interval, request.Start, request.End, (int)request.MaxCount, request.FromEnd).ToList();
                    pf.LeaveSection();

                    Bar partial = freqService.GetPartialBar(symbol, new BarFrequency(request.IntervalType, request.Interval));
                    if (partial != null)
                    {
                        bars.Add(new BarImpl(partial));
                    }
                    //logger.Info("got bar cnt:" + bars.Count());
                    
                    pf.EnterSection("SEND BAR");

                    switch(request.BarResponseType)
                    {
                        case EnumBarResponseType.PLAINTEXT:
                            {
                                for (int i = 0; i < bars.Count; i++)
                                {
                                    RspQryBarResponse response = ResponseTemplate<RspQryBarResponse>.SrvSendRspResponse(request);
                                    response.Bar = bars[i];
                                    conn.SendResponse(response, i == bars.Count - 1);
                                }
                                break;
                            }
                        case EnumBarResponseType.BINARY:
                            {
                                //MemoryStream ms = new MemoryStream();
                                //BinaryWriter b = new BinaryWriter(ms);
                                //byte[] sizebyte = BitConverter.GetBytes(8 + 1 + bars.Length * 88);
                                //byte[] typebyte = BitConverter.GetBytes((int)MessageTypes.BARRESPONSE);

                                //b.Write(sizebyte);
                                //b.Write(typebyte);
                                //b.Write(true);
                                //for (int i = 0; i < bars.Length; i++)
                                //{
                                //    BarImpl.Write(b, bars[i]);
                                //    //RspQryBarResponseBin response = RspQryBarResponseBin.CreateResponse(request);
                                //    //response.Bar = bars[i];
                                //    //conn.Send(response);
                                //}

                                //logger.Info("Size:" + ms.Position);
                                //break;

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
                                        conn.Send(response);
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
                                    conn.Send(response);
                                }
                                break;
                            }
                        default:
                            break;
                    }
                    
                    pf.LeaveSection();
                    logger.Info(string.Format("----BarRequest Statistics QTY:{0}---- \n{1}", bars.Count, pf.GetStatsString()));
                    //logger.Info("send bar finished");

                    logger.Info("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~``");
                    logger.Info("----FrequencyManager Statistics---- \n" + FrequencyManager.pf.GetStatsString());
                }
                else
                {
                    this.BackendQryBar(host, conn, request);
                }
            }
            catch (Exception ex)
            {
                logger.Error("SrvOnBarRequest Error:" + ex.ToString());
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
            logger.Info(string.Format("Conn:{0} qry exchange:{1}", conn.SessionID, request.ToString()));
            IExchange[] exchs =MDBasicTracker.ExchagneTracker.Exchanges;

            int totalnum = exchs.Length;

            for (int i = 0; i < totalnum; i++)
            {
                RspXQryExchangeResponse response = ResponseTemplate<RspXQryExchangeResponse>.SrvSendRspResponse(request);
                response.Exchange = exchs[i] as Exchange;
                conn.SendResponse(response, i == totalnum - 1);
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
            logger.Info(string.Format("Conn:{0} qry market time:{1}", conn.SessionID, request.ToString()));
            MarketTime[] mts = MDBasicTracker.MarketTimeTracker.MarketTimes.ToArray();

            int totalnum = mts.Length;
            for (int i = 0; i < totalnum; i++)
            {
                RspXQryMarketTimeResponse response = ResponseTemplate<RspXQryMarketTimeResponse>.SrvSendRspResponse(request);
                response.MarketTime = mts[i];
                conn.SendResponse(response, i == totalnum - 1);
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
            logger.Info(string.Format("Conn:{0} qry security:{1}", conn.SessionID, request.ToString()));
            SecurityFamilyImpl[] seclist = MDBasicTracker.SecurityTracker.Securities.ToArray();
            int totalnum = seclist.Length;
            int n = 0;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQrySecurityResponse response = ResponseTemplate<RspXQrySecurityResponse>.SrvSendRspResponse(request);
                    response.SecurityFaimly = seclist[i];
                    conn.SendResponse(response, i == totalnum - 1);
                    //n++;
                    //if (n == 20)
                    //{
                    //    Util.sleep(100);
                    //    n = 0;
                    //}
                }
            }
            else
            {
                RspXQrySecurityResponse response = ResponseTemplate<RspXQrySecurityResponse>.SrvSendRspResponse(request);
                conn.SendResponse(response);
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
            logger.Info(string.Format("Conn:{0} qry symbols:{1}", conn.SessionID, request.ToString()));
            SymbolImpl[] symlis = MDBasicTracker.SymbolTracker.Symbols.ToArray();
            int totalnum = symlis.Length;
            int n = 0;
            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    RspXQrySymbolResponse response = ResponseTemplate<RspXQrySymbolResponse>.SrvSendRspResponse(request);
                    response.Symbol = symlis[i];
                    conn.SendResponse(response, i == totalnum - 1);
                    //n++;
                    //if (n == 20)
                    //{
                    //    Util.sleep(100);
                    //    n = 0;
                    //}
                }
            }
            else
            {
                RspXQrySymbolResponse response = ResponseTemplate<RspXQrySymbolResponse>.SrvSendRspResponse(request);
                conn.SendResponse(response);
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
            logger.Info(string.Format("Conn:{0} try to unregister symbol:{1}", conn.SessionID, string.Join(",",request.SymbolList.ToArray())));
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
            logger.Info(string.Format("Conn:{0} try to register symbol:{1}", conn.SessionID, string.Join(",", request.SymbolList.ToArray())));
            OnRegisterSymbol(conn, request);
            
        }


        void SrvOnMDDemoTick(IServiceHost host, IConnection conn, MDDemoTickRequest request)
        {
            logger.Info(string.Format("Conn:{0} send demotick request", conn.SessionID));
            Tick k = new TickImpl("CNH6");
            k.Date = 20160323;
            k.Time = request.Time;
            k.Trade = request.Trade;
            k.Size = 1;

            freqService.ProcessTick(k);

        }
    }
}
