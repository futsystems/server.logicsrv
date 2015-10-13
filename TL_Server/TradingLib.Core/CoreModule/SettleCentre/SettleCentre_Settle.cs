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
        /// 初始化结算任务
        /// </summary>
        void InitSettleTask()
        {
            logger.Info("初始化结算任务");
            Dictionary<DateTime, List<IExchange>> exchangesettlemap = new Dictionary<DateTime, List<IExchange>>();

            //int utcoffset = 12;//按utc + 从最高降低
            //获得当前系统时间与UTC时间的Offset
            //TimeSpan offset = TimeZoneInfo.FindSystemTimeZoneById(TimeZone.CurrentTimeZone.s).BaseUtcOffset;
            //DateTime lastsettletime = DateTime.Now;
            IExchange latestexchange = null;
            DateTime latestsettlesystime = DateTime.Now;

            foreach (var ex in BasicTracker.ExchagneTracker.Exchanges)
            {

                DateTime settleextime = Util.ToDateTime(ex.GetExchangeTime().ToTLDate(), ex.CloseTime);//获得交易所结算时间对应交易所时间

                DateTime settlesystime = ex.GetSystemTime(settleextime);//转换成系统时间
                //logger.Info("exch:" + ex.EXCode + " extime:" + settleextime.ToString() + " systime:" + settlesystime.ToString());
                if (latestexchange == null)
                {
                    latestexchange = ex;
                    latestsettlesystime = settlesystime;
                }

                if (settlesystime > latestsettlesystime)
                {
                    latestsettlesystime = settlesystime;
                    latestexchange = ex;
                }

                if (!exchangesettlemap.Keys.Contains(settlesystime))
                {
                    exchangesettlemap.Add(settlesystime, new List<IExchange>());
                }

                exchangesettlemap[settlesystime].Add(ex);
            }
                       

            foreach (var ky in exchangesettlemap.Keys)
            {
                RegisterExchangeSettleTask(ky, exchangesettlemap[ky]);
            }

            //最后一个结算的交易所 结算完成5分钟后执行交易帐户结算
            logger.Info(string.Format("最后结算交易所:{0} 时间:{1}",latestexchange.EXCode,latestsettlesystime.ToString("HH:mm:ss")));
            RegisterAccountSettleTask(latestsettlesystime.AddMinutes(5));
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

        /// <summary>
        /// 交易帐户结算
        /// 系统执行每天定时结算包含周末与节假日,多交易所情况下 交易所按交易所的结算规则进行结算，系统进行每日结算
        /// </summary>
        void SettleAccounts()
        {
            this.IsInSettle = true;//标识结算中心处于结算状态
            logger.Info("系统执行帐户结算,结算日:" + this.CurrentTradingday);
            foreach (IAccount acc in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                acc.SettleAccount(this.CurrentTradingday);
            }
            this.IsInSettle = false;//标识系统结算完毕

        }

        /// <summary>
        /// 获得某个行情的结算价信息
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        decimal GetAvabileSettlementPrice(Tick k)
        {
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
        void SettleExchange(List<IExchange> list)
        {
            try
            {
                foreach (var exchange in list)
                {
                    DateTime extime = exchange.GetExchangeTime();//获得交易所当前时间

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
                    //保存交易所对应的结算价格
                    SaveSettlementPrice(exchange, settleday);
                    //执行交易帐户的交易所结算
                    foreach (var account in TLCtxHelper.ModuleAccountManager.Accounts)
                    {
                        account.SettleExchange(exchange, settleday);
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








        ///////////////////////////
        /// <summary>
        /// 保存历史持仓记录
        /// </summary>
        void SaveHistPositionDetail(IExchange exchange)
        {
            int i = 0;
            //遍历所有交易帐户
            foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                //遍历交易帐户下所有未平仓持仓对象
                foreach (Position pos in account.GetPositions(exchange).Where(p=>!p.isFlat))
                {
                    //遍历该未平仓持仓对象下的所有持仓明细
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = TLCtxHelper.ModuleSettleCentre.NextTradingday;
                        //保存持仓明细到数据库
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }
                }
            }
            logger.Info(string.Format("Saved {0} Account PositionDetails Successfull", i));

            i = 0;
            //遍历所有成交接口
            foreach (IBroker broker in TLCtxHelper.ServiceRouterManager.Brokers)
            {
                //接口没有启动 则没有交易数据
                if (!broker.IsLive)
                    continue;

                //遍历成交接口有持仓的 持仓，将该持仓的持仓明细保存到数据库
                foreach (Position pos in broker.GetPositions(exchange).Where(p => !p.isFlat))
                {
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = TLCtxHelper.ModuleSettleCentre.NextTradingday;
                        //设定标识
                        pd.Broker = broker.Token;
                        pd.Breed = QSEnumOrderBreedType.BROKER;

                        //保存持仓明细到数据库
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }
                }
            }
            logger.Info(string.Format("Saved {0} Broker PositionDetails Successfull", i));
        }
    }
}
