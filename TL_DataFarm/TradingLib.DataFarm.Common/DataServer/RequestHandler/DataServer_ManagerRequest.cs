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
    public partial class DataServer
    {
        [DataCommandAttr("Demo", "Demo -  demo function", "测试操作", QSEnumArgParseType.Json)]
        public void CTE_Demo(IServiceHost host, IConnection conn, string args)
        {
            logger.Info("Demo Function");
            

        }

        [DataCommandAttr("UpdateBar", "UpdateBar -  update bar data", "更新某个bar的相关数据",QSEnumArgParseType.Json)]
        public void CTE_UpdateBar(IServiceHost host, IConnection conn,string args)
        {
            if (!_syncdb)
            {
                logger.Warn("Update Bar Not Supported");
                return;
            }

            logger.Info("UpdateBar data:" + args);
            BarImpl bar = args.DeserializeObject<BarImpl>();
            if (bar != null)
            {
                Symbol symbol = MDBasicTracker.SymbolTracker[bar.Exchange, bar.Symbol];
                if (symbol != null)
                {
                    UpdateBar(symbol, bar);
                }
            }
        }

        [DataCommandAttr("DeleteBar", "DeleteBar -  delete bar data", "删除某个Bar数据", QSEnumArgParseType.Json)]
        public void CTE_Delete(IServiceHost host, IConnection conn, string args)
        {
            if (!_syncdb)
            {
                logger.Warn("DeleteBar Not Supported");
                return;
            }


            logger.Info("Delete data:" + args);

            var data = args.DeserializeObject();
            string exchange = data["Exchange"].ToString();
            string symbol = data["Symbol"].ToString();
            int interval = int.Parse(data["Interval"].ToString());
            BarInterval intervalType = (BarInterval)int.Parse(data["IntervalType"].ToString());
            int[] ids = data["ID"].ToObject<int[]>();////TradingLib.Mixins.Json.JsonMapper.ToObject<int[]>(data["ID"].ToJson());
            Symbol sym = MDBasicTracker.SymbolTracker[exchange,symbol];
            if (sym != null)
            {
                DeleteBar(sym, intervalType, interval, ids);
            }
        }

        /// <summary>
        /// 系统启动后 默认不接受Tick数据
        /// 比如需要执行相关维护 上传数据等 需要等数据操作完毕后再接受Tick数据
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        [DataCommandAttr("AcceptFeedTick", "AcceptFeedTick -  start feed tick", "系统开始接受实时Tick数据")]
        public void CTE_StartFeedTick(IServiceHost host, IConnection conn)
        {
            logger.Info("AcceptFeedTick");
            _acceptTick = true;
        }

        [DataCommandAttr("RejectFeedTick", "RejectFeedTick -  stop feed tick", "系统停止接受实时Tick数据")]
        public void CTE_StopFeedTick(IServiceHost host, IConnection conn)
        {
            logger.Info("RejectFeedTick");
            _acceptTick = false;
        }

        [DataCommandAttr("ResetSnapshot", "ResetSnapshot -  reset snapshot", "重置快照数据")]
        public void CTE_ResetSnapshot(IServiceHost host, IConnection conn)
        {
            logger.Info("ResetSnapshot");
            this.ResetAllSnapshot();
        }

        [DataCommandAttr("QryRestoreTask", "QryRestoreTask -  qry restore task", "查询恢复任务状态")]
        public void CTE_QryRestoreTask(IServiceHost host, IConnection conn)
        {
            logger.Info("QryRestoreTask");
            RestoreTask[] taskarray = restoresrv.RestoreTasks.ToArray();
            SendContribResponse(conn, taskarray);
        }

        [DataCommandAttr("ResetRestoreTask", "ResetRestoreTask -  reset restore task", "重置恢复任务状态",QSEnumArgParseType.Json)]
        public void CTE_ResetRestoreTask(IServiceHost host, IConnection conn,string args)
        {
            logger.Info("ResetRestoreTask:"+args);
            var data = args.DeserializeObject();
            string exchange = data["exchange"].ToString();
            string symbol = data["symbol"].ToString();
            restoresrv.ResetTask(exchange,symbol);
        }


        //查询日历列表
        [DataCommandAttr("QryCalendarList", "QryCalendarList -  qry calendar list", "查询日历对象列表")]
        public void CTE_QryCalendarList(IServiceHost host, IConnection conn)
        {
            CalendarItem[] items = MDBasicTracker.CalendarTracker.Calendars.Select(c => new CalendarItem() { Code = c.Code, Name = c.Name }).ToArray();
            int totalnum = items.Length;

            if (totalnum > 0)
            {
                for (int i = 0; i < totalnum; i++)
                {
                    this.SendContribResponse(conn,items[i], i == totalnum - 1);
                }
            }
            else
            {
                this.SendContribResponse(conn,null);
            }
            
        }



        //查询日历列表
        [DataCommandAttr("SwitchTickSrv", "SwitchTickSrv - switch tickpubsrv of FastTickFeed", "切换实时行情服务器")]
        public void CTE_SwitchTickSrv(IServiceHost host, IConnection conn)
        {
            SwitchTickSrv("Manager Swtich TickSrv Manualy");
        }

        //查询日历列表
        [DataCommandAttr("QryTickSrv", "QryTickSrv - qry tickpubsrv of FastTickFeed", "查询当前所用实时行情服务器")]
        public void CTE_QryTickSrv(IServiceHost host, IConnection conn)
        {
            if (_tickFeeds.Count > 0)
            {
                ITickFeed feed = _tickFeeds[0];
                this.SendContribResponse(conn, new { Server = feed.CurrentServer });
            }
            else
            {
                logger.Warn("TickFeed not loaded");
            }
        }


        


        /// <summary>
        /// 更新交易小节
        /// </summary>
        /// <param name="request"></param>
        /// <param name="session"></param>
        /// <param name="manager"></param>
        [DataCommandAttr("UpdateMarketTime", "UpdateMarketTime -  update marketTime", "更新交易所", QSEnumArgParseType.Json)]
        public void CTE_UpdateMarketTime(IServiceHost host, IConnection conn, string json)
        {
            //if (!_syncdb)
            //{
            //    logger.Warn("Update MarketTime Not Supported");
            //    return;
            //}

            logger.Info(string.Format("Conn:{0} 请求更新交易时间段:{1}", conn.SessionID, json));
            string message = json.DeserializeObject<string>();
            MarketTimeImpl mt = MarketTimeImpl.Deserialize(message);
            MDBasicTracker.MarketTimeTracker.UpdateMarketTime(mt);
            MarketTimeImpl localamt = MDBasicTracker.MarketTimeTracker[mt.ID];
            SendContribResponse(conn, "UpdateMarketTime", MarketTimeImpl.Serialize(localamt));
            //RspMGRUpdateMarketTimeResponse response = ResponseTemplate<RspMGRUpdateMarketTimeResponse>.SrvSendRspResponse(request);
            //response.MarketTime = MDBasicTracker.MarketTimeTracker[request.MarketTime.ID];

            //this.SendData(conn, response);

        }


        /// <summary>
        /// 更新交易所
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        [DataCommandAttr("UpdateExchange", "UpdateExchange -  update exchange", "更新交易所", QSEnumArgParseType.Json)]
        public void CTE_UpdateExchange(IServiceHost host, IConnection conn, string json)
        {
            //if (!_syncdb)
            //{
            //    logger.Warn("Update Exchange Not Supported");
            //    return;
            //}


            logger.Info(string.Format("Conn:{0} 请求更新交易所信息:{1}", conn.SessionID, json));

            string message = json.DeserializeObject<string>();
            ExchangeImpl ex = ExchangeImpl.Deserialize(message);
            MDBasicTracker.ExchagneTracker.UpdateExchange(ex);
            
            ExchangeImpl localex = MDBasicTracker.ExchagneTracker[ex.ID];
            SendContribResponse(conn, "UpdateExchange", ExchangeImpl.Serialize(localex));
                //RspMGRUpdateExchangeResponse response = ResponseTemplate<RspMGRUpdateExchangeResponse>.SrvSendRspResponse(request);
                //response.Exchange = MDBasicTracker.ExchagneTracker[request.Exchange.ID];
                //this.SendData(conn, response);
            
            
        }



        /// <summary>
        /// 更新品种
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        [DataCommandAttr("UpdateSecurity", "UpdateSecurity -  update security", "更新品种", QSEnumArgParseType.Json)]
        public void CTE_UpdateSecurity(IServiceHost host, IConnection conn, string json)
        {
            //if (!_syncdb)
            //{
            //    logger.Warn("UpdateSecurity Not Supported");
            //    return;
            //}


            logger.Info(string.Format("Conn:{0} 请求更新品种:{1}", conn.SessionID, json));

            string message = json.DeserializeObject<string>();
            SecurityFamilyImpl sec = SecurityFamilyImpl.Deserialize(message);
            //如果已经存在该品种则不执行添加操作
            if (sec.ID == 0 && MDBasicTracker.SecurityTracker[sec.Code] != null)
            {
                throw new FutsRspError("已经存在品种:" + sec.Code);
            }
            //通过对应的域更新品种对象
            MDBasicTracker.SecurityTracker.UpdateSecurity(sec);
            int secidupdate = sec.ID;

            SecurityFamilyImpl localsec = MDBasicTracker.SecurityTracker[sec.ID];
            SendContribResponse(conn, "UpdateSecurity", SecurityFamilyImpl.Serialize(localsec));
            //需要通过第一次更新获得sec_id来获得对象进行回报 否则在更新其他域的品种对象时id会发生同步变化
            //RspMGRUpdateSecurityResponse response = ResponseTemplate<RspMGRUpdateSecurityResponse>.SrvSendRspResponse(request);
            //response.SecurityFaimly = MDBasicTracker.SecurityTracker[sec.ID];

            //this.SendData(conn, response);

        }

        /// <summary>
        /// 更新合约
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        [DataCommandAttr("UpdateSymbol", "UpdateSymbol -  update symbol", "更新合约", QSEnumArgParseType.Json)]
        public void CTE_UpdateSymbol(IServiceHost host, IConnection conn, string json)
        {
            //if (!_syncdb)
            //{
            //    logger.Warn("UpdateSymbol Not Supported");
            //    return;
            //}


            logger.Info(string.Format("Conn:{0} 请求更新合约:{1}", conn.SessionID, json));
            string message = json.DeserializeObject<string>();
            SymbolImpl symbol = SymbolImpl.Deserialize(message);
            symbol.Domain_ID = 1;

            //检查品种
            SecurityFamilyImpl sec = MDBasicTracker.SecurityTracker[symbol.security_fk];
            if (sec == null)
            {
                throw new FutsRspError("品种数据异常");
            }
            symbol.SecurityFamily = sec;

            //如果是添加合约 检查合约是否存在
            if (symbol.ID == 0)
            {
                if (MDBasicTracker.SymbolTracker[symbol.Exchange, symbol.Symbol] != null)
                {
                    throw new FutsRspError("已经存在合约:" + symbol.Symbol);
                }
                string code; int year, month;
                symbol.ParseFututureContract(out code, out year, out month);
                if (string.Format("{0:D2}", month) != symbol.Month)
                {
                    throw new FutsRspError(string.Format("Symbol:{0} set error month:{1}", symbol.Symbol, symbol.Month));
                }
            }


            //调用该域更新该合约
            MDBasicTracker.SymbolTracker.UpdateSymbol(symbol, true);

            //RspMGRUpdateSymbolResponse response = ResponseTemplate<RspMGRUpdateSymbolResponse>.SrvSendRspResponse(request);
            SymbolImpl localsymbol = MDBasicTracker.SymbolTracker[symbol.ID];

            SendContribResponse(conn, "UpdateSymbol", SymbolImpl.Serialize(localsymbol));
            //response.Symbol = localsymbol;

            //this.SendData(conn, response);
        }
        /// <summary>
        /// 上传Bar数据
        /// Header指定交易所,Bar合约 CL11 表示CL 11月份合约
        /// 所有合约都是品种+2位月份数字表示
        /// </summary>
        /// <param name="host"></param>
        /// <param name="conn"></param>
        /// <param name="request"></param>
        void SrvOnMGRUploadBarData(IServiceHost host, IConnection conn, UploadBarDataRequest request)
        {
            if (!_syncdb)
            {
                logger.Warn("Upload Bar Not Supported");
                return;
            }


            logger.Info(string.Format("Conn:{0} Upload  {1} Bars", conn.SessionID,request.Header.BarCount));

            string key = string.Format("{0}-{1}-{2}-{3}", request.Header.Exchange, request.Header.Symbol, request.Header.IntervalType, request.Header.Interval);


            SecurityFamily sec = MDBasicTracker.SecurityTracker.GetSecurityOfContinuousSymbol(request.Header.Symbol);
            if (sec == null)
            {
                logger.Error("Symbol:{0} have no security avabile".Put(request.Header.Symbol));
            }
            request.Bars.ForEach(bar => 
            { 
                bar.Exchange = request.Header.Exchange; 
                bar.Symbol = request.Header.Symbol; 
                bar.IntervalType = request.Header.IntervalType; 
                bar.Interval = request.Header.Interval;

                //设定交易日
                if (bar.TradingDay == 0)
                {
                    bar.TradingDay =BarGenerator.GetTradingDay(sec, bar.EndTime);
                }
            
            });
            
            this.UploadBars(key, request.Bars);
        }


    }
}
