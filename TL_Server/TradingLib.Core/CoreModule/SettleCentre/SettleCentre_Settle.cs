using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    internal class ExchangeSettleInfo:IComparable
    {
        /// <summary>
        /// 默认 按时间升序排列，第一个元素是最近的一个时间
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            ExchangeSettleInfo info = obj as ExchangeSettleInfo;
            if (info.LocalSysSettleTime > this.LocalSysSettleTime)
            {
                return -1;
            }
            else if (info.LocalSysSettleTime == this.LocalSysSettleTime)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        /// <summary>
        /// 交易所
        /// </summary>
        public IExchange Exchange {get;set;}

        /// <summary>
        /// 该交易所下一个结算时间
        /// </summary>
        public DateTime NextExchangeSettleTime {get;set;}

        /// <summary>
        /// 结算时间对应的本地时间
        /// </summary>
        public DateTime LocalSysSettleTime {get;set;}

        /// <summary>
        /// 对应的交易所结算日
        /// </summary>
        public int Settleday {get;set;}


        public override string ToString()
        {
            return string.Format("{0} Now:{1} NextSettleTime:{2} SysTime:{3} Settleday:{4} ", Exchange.EXCode,Exchange.GetExchangeTime().ToString("yyyyMMdd HH:mm:ss") ,NextExchangeSettleTime.ToString("yyyyMMdd HH:mm:ss"), LocalSysSettleTime.ToString("yyyyMMdd HH:mm:ss"), Settleday);
        }

    }

    
    public partial class SettleCentre
    {

        /// <summary>
        /// 获得交易所下一个结算时间
        /// 这个结算时间 不排除周末和节假日，具体交易所是否结算 需要排除周末和节假日，
        /// 这里结算时间的获取 只是用于获得当前系统的结算日 以及对应的结算时间
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="systime"></param>
        /// <returns></returns>
        DateTime GetNextSettleTime(IExchange exchange)
        {
            //获得交易所当前时间
            DateTime exchangetime = exchange.GetExchangeTime();
            //如果交易所当前时间小于结算时间，则下一个结算时间就是当前日期对应的结算时间 
            if (exchangetime.ToTLTime() < exchange.CloseTime)
            {
                return Util.ToDateTime(exchangetime.ToTLDate(), exchange.CloseTime);
            }
            else //当前时间大于结算时间 则取下一日的结算时间
            {
                return Util.ToDateTime(exchangetime.AddDays(1).ToTLDate(), exchange.CloseTime);
            }
        }

        ExchangeSettleInfo GetExchangeSettleInfo(IExchange exchange)
        {
            ExchangeSettleInfo info = new ExchangeSettleInfo();
            info.Exchange = exchange;
            info.NextExchangeSettleTime = GetNextSettleTime(exchange);
            info.LocalSysSettleTime = exchange.ConvertToSystemTime(info.NextExchangeSettleTime);
            info.Settleday = info.NextExchangeSettleTime.ToTLDate();
            return info;

        }


        /// <summary>
        /// 初始化结算任务
        /// 同时按照系统设定的交易所结算时间自动获得对应的计算时间
        /// </summary>
        void InitSettleTask()
        {
            logger.Info("初始化结算任务");
            Dictionary<DateTime, List<IExchange>> exchangesettlemap = new Dictionary<DateTime, List<IExchange>>();

            List<ExchangeSettleInfo> infolsit = new List<ExchangeSettleInfo>();

            foreach (var ex in BasicTracker.ExchagneTracker.Exchanges)
            {
                ExchangeSettleInfo exinfo = GetExchangeSettleInfo(ex);
                infolsit.Add(exinfo);

                if (!exchangesettlemap.Keys.Contains(exinfo.LocalSysSettleTime))
                {
                    exchangesettlemap.Add(exinfo.LocalSysSettleTime, new List<IExchange>());
                }

                exchangesettlemap[exinfo.LocalSysSettleTime].Add(ex);
            }

            //1.注册交易所结算定时任务
            foreach (var ky in exchangesettlemap.Keys)
            {
                RegisterExchangeSettleTask(ky, exchangesettlemap[ky]);
            }

            infolsit.Sort();
            foreach (var info in infolsit)
            {
                logger.Info(info);
            }
            ExchangeSettleInfo firstex = infolsit.First();
            _tradingday = firstex.Settleday;//最早结算交易所对应的日期就是 结算日

            List<ExchangeSettleInfo> tmp = infolsit.Where(e => e.Settleday == _tradingday).ToList();
            tmp.Sort();
            ExchangeSettleInfo lastex = tmp.Last();
            _netxSettleTime = lastex.LocalSysSettleTime.AddMinutes(5);//应该结算日最晚的一个交易所结算时间 为系统柜台结算时间
            //2.注册交易帐户结算定时任务
            RegisterAccountSettleTask(_netxSettleTime);
            
            //周期定时结算时间
            _settleTime = _netxSettleTime.ToTLTime();
            _resetTime = _netxSettleTime.AddMinutes(5).ToTLTime();

            DateTime _nextSettResetTime = _netxSettleTime.AddMinutes(5);//结算后5分钟为重置时间
            RegisterSettleResetTask(_nextSettResetTime);

            //当前时间是否大于最后交易所结算时间 且在柜台结算时间之前，如果在这个时间段内，则当前tradingday为 判定出来的交易日的上一个交易日
            int now = Util.ToTLTime();
            if (now > lastex.LocalSysSettleTime.ToTLTime() && now < _netxSettleTime.ToTLTime())
            {
                _tradingday = Util.ToDateTime(_tradingday, 0).AddDays(-1).ToTLDate();
            }

            //3.注册定时转储任务 交易帐户结算后随机的5-10分钟之内执行数据转储
            DateTime storetime = _netxSettleTime.AddMinutes(new Random().Next(5,10));
            RegisterDataStoreTask(storetime);
            logger.Info(string.Format("判定当前交易日:{0} 柜台结算时间:{1}", _tradingday, _netxSettleTime.ToString("yyyyMMdd HH:mm:ss")));
        }

        /// <summary>
        /// 注册交易所结算任务
        /// </summary>
        /// <param name="settletime"></param>
        /// <param name="list"></param>
        void RegisterExchangeSettleTask(DateTime settletime, List<IExchange> list)
        {
            logger.Info("注册交易所结算任务,结算时间:" + settletime.ToString("HH:mm:ss"));
            //DateTime flattime = settletime.AddMinutes(-5);//提前5分钟强平
            TaskProc task = new TaskProc(this.UUID, "交易所结算-" + settletime.ToString("HH:mm:ss"), settletime.Hour, settletime.Minute, settletime.Second, delegate() { SettleExchange(list); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        /// <summary>
        /// 注册交易账户结算任务
        /// </summary>
        /// <param name="settletime"></param>
        void RegisterAccountSettleTask(DateTime settletime)
        {
            logger.Info("注册交易帐户结算任务,结算时间:" + settletime.ToString("HH:mm:ss"));
            TaskProc task = new TaskProc(this.UUID, "交易帐户结算-" + settletime.ToString("HH:mm:ss"), settletime.Hour, settletime.Minute, settletime.Second, delegate() { SettleAccount(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        void RegisterSettleResetTask(DateTime resettime)
        {
            logger.Info("注册系统重置任务,重置时间:" + resettime.ToString("HH:mm:ss"));
            TaskProc task = new TaskProc(this.UUID, "交易系统重置-" + resettime.ToString("HH:mm:ss"), resettime.Hour, resettime.Minute, resettime.Second, delegate() { SetteReset(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        void RegisterDataStoreTask(DateTime storetime)
        {
            logger.Info("注册交易记录转储任务,转储时间:" + storetime.ToString("HH:mm:ss"));
            TaskProc task = new TaskProc(this.UUID, "交易数据转储-" + storetime.ToString("HH:mm:ss"), storetime.Hour, storetime.Minute, storetime.Second, delegate() { Dump2Log(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }


        /// <summary>
        /// 交易帐户结算
        /// 系统执行每天定时结算包含周末与节假日,多交易所情况下 交易所按交易所的结算规则进行结算，系统进行每日结算
        /// </summary>
        void SettleAccount()
        {
            logger.Info(string.Format("#####SettleAccount: Start Settele Account,Current Tradingday:{0}", Tradingday));
            
            this.IsInSettle = true;//标识结算中心处于结算状态

            //触发结算前事件
            TLCtxHelper.EventSystem.FireBeforeSettleEvent(this, new SystemEventArgs());

            logger.Info("系统执行帐户结算,结算日:" + this.Tradingday);
            DateTime now = DateTime.Now;
            foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                if (acc.CreatedTime < now)//历史结算中 帐户创建时间之前的交易日不执行结算
                {
                    acc.SettleAccount(this.Tradingday);
                }
            }

            //滚动交易日
            RollTradingDay();

            logger.Info(string.Format("Settle finished,entry tradingday:{0}", this.Tradingday));

            //触发结算后事件
            TLCtxHelper.EventSystem.FireAfterSettleEvent(this, new SystemEventArgs());
            
        }

        /// <summary>
        /// 单独触发结算事件
        /// </summary>
        void SetteReset()
        {
            //TODO:结算后 如果立刻执行重置 加载交易账户交易记录时会导致重复加载
            logger.Info("结算后系统重置");
            //触发 系统重置操作事件
            TLCtxHelper.EventSystem.FireSettleResetEvet(this, new SystemEventArgs());

            this.IsInSettle = false;//标识系统结算完毕
        }
        /// <summary>
        /// 结算重置
        /// </summary>
        //void SettleReset()
        //{
        //    logger.Info("结算后系统重置");
        //    //触发 系统重置操作事件
        //    TLCtxHelper.EventSystem.FireSettleResetEvet(this, new SystemEventArgs());
        //}

        /// <summary>
        /// 获得某个行情的结算价信息
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        decimal GetAvabileSettlementPrice(Tick k)
        {
            //如果结算价不为0则返回结算价否则返回最后一个成交价 即收盘价
            if (k.Settlement != 0) return k.Settlement;
            return k.Trade;
        }

        /// <summary>
        /// 保存交易所的结算价格
        /// </summary>
        /// <param name="exchange"></param>
        void SaveSettlementPrice(IExchange exchange,int settleday)
        {
            //获得交易所所有合约结算价格
            foreach (var sym in BasicTracker.DomainTracker.SuperDomain.GetSymbols().Where(s=>s.SecurityFamily.Exchange.EXCode == exchange.EXCode))
            {
                MarketData data = new MarketData();
                data.Symbol = sym.Symbol;
                data.Exchange = exchange.EXCode;
                data.SettleDay = settleday;

                Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(sym.Symbol);
                if (k != null)
                {
                    data.AskPrice = k.AskPrice;
                    data.AskSize = k.AskSize;
                    data.BidPrice = k.BidPrice;
                    data.BidSize = k.BidSize;
                    data.Close = k.Trade;
                    data.High = k.High;
                    data.Low = k.Low;
                    data.LowerLimit = k.LowerLimit;
                    data.OI = k.OpenInterest;
                    data.Open = k.Open;
                    data.PreOI = k.PreOpenInterest;
                    data.PreSettlement = k.PreSettlement;
                    data.Settlement = GetAvabileSettlementPrice(k);
                    data.UpperLimit = k.UpperLimit;
                    data.Vol = k.Vol;
                }
                _settlementPriceTracker.UpdateSettlementPrice(data);
            }
        }
        /// <summary>
        /// 交易所结算
        /// 交易所结算规则
        /// 1.周六周日 对应的时间节点上不执行结算
        /// 2.节假日不执行结算
        /// 3.结算时 当前日期就是结算日 只是上个交易日的T+1时段的交易 并入当前交易日进行结算
        /// 
        /// 1.分账户结算
        /// 2.Broker结算
        /// </summary>
        /// <param name="list"></param>
        void SettleExchange(List<IExchange> list,int tradingday=0)
        {
            try
            {
                bool histmode = _settlemode == QSEnumSettleMode.HistMode;
                //历史结算需要制定交易日
                if (histmode && tradingday == 0)
                {
                    throw new ArgumentException("hist settle need tradingday");
                }

                foreach (var exchange in list)
                {
                    //历史结算使用制定交易日否则使用当前交易所时间
                    DateTime extime = histmode ? Util.ToDateTime(tradingday, 0) : exchange.GetExchangeTime(); //获得交易所当前时间

                    //非工作日不结算
                    if (!extime.IsWorkDay())
                    {
                        continue;
                    }
                    //节假日不结算
                    if (exchange.IsInHoliday(extime))
                    {
                        continue;
                    }
                    //结算日为交易所当前日期
                    int settleday = extime.ToTLDate();

                    logger.Info(string.Format("交易所:{0} 执行结算 结算日:{1}", exchange.EXCode, settleday));
                    if (!histmode) //正常结算模式 需要保存结算价
                    {
                        //保存交易所对应的结算价格
                        SaveSettlementPrice(exchange, settleday);
                    }
                    DateTime now = DateTime.Now;
                    //执行交易帐户的交易所结算
                    foreach (var account in TLCtxHelper.ModuleAccountManager.Accounts)
                    {
                        if (account.CreatedTime < now)//历史结算中 帐户创建时间之前的交易日不执行结算
                        {
                            account.SettleExchange(exchange, settleday);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("交易所结算失败:" + ex.ToString());
            }
        }



        /// <summary>
        /// 获得某个交易日某个合约的结算价格
        /// </summary>
        /// <param name="settleday"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public SettlementPrice GetSettlementPrice(int settleday, string symbol)
        {
            return _settlementPriceTracker[settleday, symbol];
        }


        public Tick GetLastTickSnapshot(string symbol)
        {
            return _settlementPriceTracker.GetLastTickSnapshot(symbol);
        }



        /// <summary>
        /// 将已结算的交易记录转储到历史交易记录表
        /// 保存交易记录发生在交易帐户结算之后
        /// </summary>
        public void Dump2Log(bool dumpall =false)
        {
            logger.Info("Dump TradingInfo(Order,Trade,OrderAction)");
            int onum, tnum, cnum;//, prnum;
            int tradingday = this.LastSettleday;//保存上一个交易日的结算数据
            ORM.MTradingInfo.DumpSettledOrders(out onum, tradingday,dumpall);
            ORM.MTradingInfo.DumpSettledTrades(out tnum, tradingday, dumpall);
            ORM.MTradingInfo.DumpSettledOrderActions(out cnum, tradingday, dumpall);
           // ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

            logger.Info("Order       Saved:" + onum.ToString());
            logger.Info("Trade       Saved:" + tnum.ToString());
            logger.Info("OrderAction Saved:" + cnum.ToString());
            //logger.Info("PosTrans    Saved:" + prnum.ToString());
        }



        ///////////////////////////
        /// <summary>
        /// 保存历史持仓记录
        /// </summary>
        //void SaveHistPositionDetail(IExchange exchange)
        //{
        //    int i = 0;
        //    //遍历所有交易帐户
        //    foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts)
        //    {
        //        //遍历交易帐户下所有未平仓持仓对象
        //        foreach (Position pos in account.GetPositions(exchange).Where(p=>!p.isFlat))
        //        {
        //            //遍历该未平仓持仓对象下的所有持仓明细
        //            foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
        //            {
        //                //保存结算持仓明细时要将结算日更新为当前
        //                pd.Settleday = TLCtxHelper.ModuleSettleCentre.NextTradingday;
        //                //保存持仓明细到数据库
        //                ORM.MSettlement.InsertPositionDetail(pd);
        //                i++;
        //            }
        //        }
        //    }
        //    logger.Info(string.Format("Saved {0} Account PositionDetails Successfull", i));

        //    i = 0;
        //    //遍历所有成交接口
        //    foreach (IBroker broker in TLCtxHelper.ServiceRouterManager.Brokers)
        //    {
        //        //接口没有启动 则没有交易数据
        //        if (!broker.IsLive)
        //            continue;

        //        //遍历成交接口有持仓的 持仓，将该持仓的持仓明细保存到数据库
        //        foreach (Position pos in broker.GetPositions(exchange).Where(p => !p.isFlat))
        //        {
        //            foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
        //            {
        //                //保存结算持仓明细时要将结算日更新为当前
        //                pd.Settleday = TLCtxHelper.ModuleSettleCentre.NextTradingday;
        //                //设定标识
        //                pd.Broker = broker.Token;
        //                pd.Breed = QSEnumOrderBreedType.BROKER;

        //                //保存持仓明细到数据库
        //                ORM.MSettlement.InsertPositionDetail(pd);
        //                i++;
        //            }
        //        }
        //    }
        //    logger.Info(string.Format("Saved {0} Broker PositionDetails Successfull", i));
        //}
    }
}
