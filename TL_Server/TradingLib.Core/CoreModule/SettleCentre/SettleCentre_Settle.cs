using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    

    
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
        DateTime GetNextSettleTime(Exchange exchange)
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

        /// <summary>
        /// 获得某个交易所的结算信息
        /// </summary>
        /// <param name="exchange"></param>
        /// <returns></returns>
        ExchangeSettleInfo GetExchangeSettleInfo(Exchange exchange)
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
            logger.Info("Init Settle Task");
            Dictionary<DateTime, List<Exchange>> exchangesettlemap = new Dictionary<DateTime, List<Exchange>>();

            List<ExchangeSettleInfo> infolsit = new List<ExchangeSettleInfo>();

            foreach (var ex in BasicTracker.ExchagneTracker.Exchanges)
            {
                ExchangeSettleInfo exinfo = GetExchangeSettleInfo(ex);
                infolsit.Add(exinfo);

                if (!exchangesettlemap.Keys.Contains(exinfo.LocalSysSettleTime))
                {
                    exchangesettlemap.Add(exinfo.LocalSysSettleTime, new List<Exchange>());
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
                //输出每个交易所结算信息
                logger.Debug(info);
            }

            //最早结算交易所对应的日期就是 结算日
            ExchangeSettleInfo firstex = infolsit.First();
            _tradingday = firstex.Settleday;

            //该结算日最晚的一个交易所结算时间 为系统柜台结算时间(顺延5分钟)
            List<ExchangeSettleInfo> tmp = infolsit.Where(e => e.Settleday == _tradingday).ToList();
            tmp.Sort();
            ExchangeSettleInfo lastex = tmp.Last();

            //2.注册交易帐户结算定时任务
            _nextSettleTime = lastex.LocalSysSettleTime.AddMinutes(5);
            _settleTime = _nextSettleTime.ToTLTime();
            RegisterAccountSettleTask(_nextSettleTime);

            //3.注册结算重置定时任务
            _nextSettleResetTime = _nextSettleTime.AddMinutes(5);//结算后5分钟为重置时间
            _resetTime = _nextSettleResetTime.ToTLTime();
            RegisterSettleResetTask(_nextSettleResetTime);

            //4.注册清算中心开启与关闭时间
            RegisterCloseTask(_nextSettleTime.AddMinutes(-5));//交易账户结算前5分钟关闭系统
            RegisterOpenTask(_nextSettleResetTime.AddMinutes(5));//结算重置后5分钟打开系统

            //当前时间是否大于最后交易所结算时间 且在柜台结算时间之前，如果在这个时间段内，则当前tradingday为 判定出来的交易日的上一个交易日
            int now = Util.ToTLTime();
            if (now > lastex.LocalSysSettleTime.ToTLTime() && now < _nextSettleTime.ToTLTime())
            {
                _tradingday = Util.ToDateTime(_tradingday, 0).AddDays(-1).ToTLDate();
            }

            //4.注册定时转储任务 交易帐户结算后随机的5-10分钟之内执行数据转储
            DateTime storetime = _nextSettleResetTime.AddMinutes(new Random().Next(5, 10));
            RegisterDataStoreTask(storetime);
            //logger.Info(string.Format("Current Tradyingday:{0} Counter Settle Time:{1}", _tradingday, _nextSettleTime.ToString("yyyyMMdd HH:mm:ss")));
        }

        /// <summary>
        /// 注册交易所结算任务
        /// </summary>
        /// <param name="settletime"></param>
        /// <param name="list"></param>
        void RegisterExchangeSettleTask(DateTime settletime, List<Exchange> list)
        {
            logger.Info(string.Format("Register Exchange Settle Task,Time:{0} Ex:{1}", settletime.ToString("HH:mm:ss"), string.Join(" ", list.Select(e => e.EXCode).ToArray())));
            TaskProc task = new TaskProc(this.UUID, "交易所结算-" + settletime.ToString("HH:mm:ss"), settletime.Hour, settletime.Minute, settletime.Second, delegate() { SettleExchange(list); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        /// <summary>
        /// 注册交易账户结算任务
        /// </summary>
        /// <param name="settletime"></param>
        void RegisterAccountSettleTask(DateTime settletime)
        {
            logger.Info(string.Format("Register Account Settle Task,Time:{0}", settletime.ToString("HH:mm:ss")));
            TaskProc task = new TaskProc(this.UUID, "交易帐户结算-" + settletime.ToString("HH:mm:ss"), settletime.Hour, settletime.Minute, settletime.Second, delegate() { SettleAccount(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        /// <summary>
        /// 注册结算重置任务
        /// </summary>
        /// <param name="resettime"></param>
        void RegisterSettleResetTask(DateTime resettime)
        {
            logger.Info(string.Format("Register Settle Reset Task,Time:{0}", resettime.ToString("HH:mm:ss")));
            TaskProc task = new TaskProc(this.UUID, "交易系统重置-" + resettime.ToString("HH:mm:ss"), resettime.Hour, resettime.Minute, resettime.Second, delegate() { SetteReset(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        /// <summary>
        /// 注册数据转储任务
        /// </summary>
        /// <param name="storetime"></param>
        void RegisterDataStoreTask(DateTime storetime)
        {
            logger.Info(string.Format("Register Data Dump Task,Time:{0}", storetime.ToString("HH:mm:ss")));
            TaskProc task = new TaskProc(this.UUID, "交易数据转储-" + storetime.ToString("HH:mm:ss"), storetime.Hour, storetime.Minute, storetime.Second, delegate() { DumpDataToLogTable(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(task);
        }

        /// <summary>
        /// 注册系统关闭任务
        /// </summary>
        /// <param name="closecctime"></param>
        void RegisterCloseTask(DateTime closecctime)
        {
            //注入关闭清算中心任务 结算前5分钟关闭清算中心
            //DateTime closecctime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), _settleTime).AddMinutes(-5);
            logger.Info(string.Format("Register System Close Task,Time:{0}", closecctime.ToString("HH:mm:ss")));
            TaskProc taskclosecc = new TaskProc(this.UUID, "关闭清算中心" + Util.ToTLTime(closecctime).ToString(), closecctime.Hour, closecctime.Minute, closecctime.Second, delegate() { Task_CloseClearCentre(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(taskclosecc);
        }

        /// <summary>
        /// 注册系统开启任务
        /// </summary>
        /// <param name="opencctime"></param>
        void RegisterOpenTask(DateTime opencctime)
        {
            //注入开启清算中心任务 重置后5分钟开启清算中心
            //DateTime opencctime = Util.ToDateTime(Util.ToTLDate(DateTime.Now), _resetTime).AddMinutes(5);
            logger.Info(string.Format("Register System Open Task,Time:{0}", opencctime.ToString("HH:mm:ss")));
            TaskProc taskopencc = new TaskProc(this.UUID, "开启清算中心" + Util.ToTLTime(opencctime).ToString(), opencctime.Hour, opencctime.Minute, opencctime.Second, delegate() { Task_OpenClearCentre(); });
            TLCtxHelper.ModuleTaskCentre.RegisterTask(taskopencc);
        }

        void Task_OpenClearCentre()
        {
            TLCtxHelper.ModuleClearCentre.OpenClearCentre();
        }

        void Task_CloseClearCentre()
        {
            TLCtxHelper.ModuleClearCentre.CloseClearCentre();
        }

        #region 投资者账户结算缓存
        Dictionary<string, AccountSettlement> accountSettlementMap = new Dictionary<string, AccountSettlement>();


        public void InvestAccountSettled(AccountSettlement settlement)
        {
            if (settlement == null || string.IsNullOrEmpty(settlement.Account)) return;
            accountSettlementMap[settlement.Account] = settlement;
        }

        /// <summary>
        /// 获得某个交易账户结算记录
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public AccountSettlement GetInvestSettlement(string account)
        {
            AccountSettlement target = null;
            if (accountSettlementMap.TryGetValue(account, out target))
            {
                return target;
            }
            return null;
        }



        #endregion



        #region 结算过程
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
        void SettleExchange(List<Exchange> list, int tradingday = 0)
        {
            try
            {
                bool histmode = _settlemode == QSEnumSettleMode.HistSettleMode;
                //历史结算需要制定交易日
                if (histmode && tradingday == 0)
                {
                    throw new ArgumentException("hist settle need tradingday");
                }

                foreach (var exchange in list)
                {
                    //历史结算使用指定交易日否则使用当前交易所时间
                    DateTime extime = histmode ? Util.ToDateTime(tradingday, 0) : exchange.GetExchangeTime(); //获得交易所当前时间

                    //非工作日不结算
                    if (!extime.IsWorkDay())
                    {
                        continue;
                    }
                    //避免错误的假日文件导致有交易记录而交易所不执行结算
                    //逐日盯市结算 节假日不执行结算 节假日 结算价如何获取？ 逐笔结算方式 每日结算没有盯市盈亏 且与结算价无关 只与开仓价有关
                    ////节假日不结算
                    if (exchange.IsInHoliday(extime))//非交易日不执行结算
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

                    if (!histmode)//常规结算模式 需要获取除权数据
                    {
                        //保存除权数据
                        SavePowerData(exchange, settleday);
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

                    //执行交易通道结算 注:若此处Broker通道结算异常 会导致其余交易所不执行有效结算 从而导致账户整体结算异常 Broker改进成BrokerTracker在通道初始化设定参数时创建 避免通道未启动导致遍历通道交易数据异常
                    foreach (var broker in TLCtxHelper.ServiceRouterManager.Brokers)
                    {
                        broker.SettleExchange(exchange, settleday);
                    }

                   

                }
            }
            catch (Exception ex)
            {
                logger.Error("交易所结算失败:" + ex.ToString());
            }
        }

        /// <summary>
        /// 交易帐户结算
        /// 执行交易账户结算即结算中心处于结算状态
        /// 系统执行每天定时结算包含周末与节假日,多交易所情况下 交易所按交易所的结算规则进行结算，系统进行每日结算
        /// </summary>
        void SettleAccount()
        {
            logger.Info(string.Format("#####SettleAccount: Start Settele Account,Current Tradingday:{0}", Tradingday));
            this.SettleMode = QSEnumSettleMode.SettleMode;//结算中心进入结算状态
            accountSettlementMap.Clear();

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

            //执行代理账户结算
            foreach (IAgent agent in TLCtxHelper.ModuleAgentManager.Agents)
            {
                if (agent.CreatedTime < Util.ToTLDateTime())
                {
                    agent.Settle(this.Tradingday);
                }
            }

            //数据库中标记结算日的手续费拆分与出入金记录
            ORM.MAgentCashTransaction.MarkeCashTransactionSettled(this.Tradingday);
            ORM.MAgentCommissionSplit.MarkeAgentCommissionSplitSettled(this.Tradingday);

            //TODO:滚动交易日是在结算时执行还是在重置时执行 
            //更新交易日
            ORM.MSettlement.UpdateSettleday(this.Tradingday);
            _lastsettleday = this.Tradingday;
            _tradingday = Util.ToDateTime(this.Tradingday, DateTime.Now.ToTLTime()).AddDays(1).ToTLDate();

            logger.Info(string.Format("Settle Finished,Update Last Settleday:{0} Entry Tradingday:{1}",_lastsettleday, _tradingday));

            //触发结算后事件
            TLCtxHelper.EventSystem.FireAfterSettleEvent(this, new SystemEventArgs());
            
        }

        /// <summary>
        /// 结算后重置
        /// </summary>
        void SetteReset()
        {
            //TODO:结算后 如果立刻执行重置 加载交易账户交易记录时会导致重复加载(结算后需要等待数据库写队列中的数据全部写入数据库 操作结算标记 否则立刻加载会将为标记为结算的数据再次加载)
            logger.Info("结算后系统重置");

            InitData();

            //触发 系统重置操作事件
            TLCtxHelper.EventSystem.FireSettleResetEvet(this, new SystemEventArgs());
            this.SettleMode = QSEnumSettleMode.StandbyMode;
        }

        /// <summary>
        /// 转储所有已结算交易记录
        /// </summary>
        void StoreAllData()
        {
            int onum, tnum, cnum;//, prnum;
            //转储结算完毕的委托 成交数据
            ORM.MTradingInfo.DumpSettledOrders(out onum, int.MaxValue);
            ORM.MTradingInfo.DumpSettledTrades(out tnum, int.MaxValue);
            ORM.MTradingInfo.DumpSettledOrderActions(out cnum, int.MaxValue);
            logger.Info(string.Format("转储所有已结算交易记录结束 Order:{0} Trade:{1} Action:{2}", onum, tnum, cnum));
        }

        /// <summary>
        /// 将已结算的交易记录转储到历史交易记录表
        /// 保存交易记录发生在交易帐户结算之后
        /// </summary>
        public void DumpDataToLogTable()
        {
            logger.Info("Dump TradingInfo(Order,Trade,OrderAction)");
            int tmp_onum, tmp_tnum;
            int tradingday = this.LastSettleday;

            //查询某个交易日的日内委托与成交数量
            tmp_onum = ORM.MTradingInfo.GetInterdayOrderNum(tradingday);
            tmp_tnum = ORM.MTradingInfo.GetInterdayTradeNum(tradingday);

            int onum, tnum, cnum;//, prnum;
            //转储结算完毕的委托 成交数据
            ORM.MTradingInfo.DumpSettledOrders(out onum, tradingday);//注如果有历史数据没有删除则dump后的委托数量是所有数量
            ORM.MTradingInfo.DumpSettledTrades(out tnum, tradingday);
            ORM.MTradingInfo.DumpSettledOrderActions(out cnum, tradingday);
            // ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

            logger.Info("Order       Saved:" + onum.ToString() +" NumQry:"+tmp_onum.ToString());
            logger.Info("Trade       Saved:" + tnum.ToString()+" NumQry:"+tmp_tnum.ToString());
            logger.Info("OrderAction Saved:" + cnum.ToString());
            //logger.Info("PosTrans    Saved:" + prnum.ToString());
            //如果日内数量与转储的数量一致 表面正常结算且已经正常保存到log表 取消删除操作 定时任务中 删除1个月以前的数据
            //if (tmp_onum==onum && tmp_tnum == tnum)
            //{
            //    ORM.MTradingInfo.ClearIntradayOrders(tradingday);
            //    ORM.MTradingInfo.ClearIntradayTrades(tradingday);
            //    ORM.MTradingInfo.ClearIntradayOrderActions(tradingday);
            //}
            
        }

        #endregion


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
        /// 保存某个交易日的除权数据
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="settleday"></param>
        void SavePowerData(Exchange exchange, int settleday)
        { 
            //IEnumerable<PowerData> pds = null;
            //foreach(var pd in pds)
            //{
            //    BasicTracker.PowerDataTracker.UpdatePowerData(pd);
            //}
        }

        /// <summary>
        /// 保存交易所的结算价格
        /// </summary>
        /// <param name="exchange"></param>
        void SaveSettlementPrice(Exchange exchange,int settleday)
        {
            //获得交易所所有合约结算价格
            foreach (var sym in BasicTracker.DomainTracker.SuperDomain.GetSymbols().Where(s=>s.SecurityFamily.Exchange.EXCode == exchange.EXCode))
            {
                SettlementPrice data = new SettlementPrice();
                data.Symbol = sym.Symbol;
                data.Exchange = exchange.EXCode;
                data.SettleDay = settleday;

                Tick k = TLCtxHelper.ModuleDataRouter.GetTickSnapshot(sym.Exchange,sym.Symbol);
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
                BasicTracker.SettlementPriceTracker.UpdateSettlementPrice(data);
            }
        }

    }
}
