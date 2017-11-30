using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{

    /// <summary>
    /// 对帐户部分品种进行结算后 如何反应到内存数据
    /// 让交易账户的统计和计算可以反应出当前最新变化
    /// 交易所结算完毕后，账户等待所有交易所交易日结转后 生成帐户的结算记录 账户进入下一个交易日
    /// </summary>
    public partial class AccountImpl
    {
        ThreadSafeList<ExchangeSettlement> settlementlist = new ThreadSafeList<ExchangeSettlement>();//交易所结算记录
        ThreadSafeList<CashTransaction> cashtranslsit = new ThreadSafeList<CashTransaction>();//当日出入金记录


        public void SettleAccount(int settleday)
        { 
            //所有交易所对应的settleday交易日结算完毕后，我们执行汇总结算
            AccountSettlement settlement = new AccountSettlementImpl();
            settlement.Account = this.ID;
            settlement.Settleday = settleday;

            settlement.LastEquity = this.LastEquity;
            settlement.LastCredit = this.LastCredit;
            settlement.CashIn = this.CashIn;
            settlement.CashOut = this.CashOut;
            settlement.CreditCashIn = this.CreditCashIn;
            settlement.CreditCashOut = this.CreditCashOut;

            //对交易所结算进行汇总 账户结算数据是汇总所有交易所结算数据 交易所结算数据异常会导致账户总结算异常
            settlement.CloseProfitByDate = this.PendingSettleCloseProfitByDate;
            settlement.PositionProfitByDate = this.PendingSettlePositionProfitByDate;
            settlement.AssetBuyAmount = this.PendingSettleAssetBuyAmount;
            settlement.AssetSellAmount = this.PendingSettleAssetSellAmount;
            settlement.Commission = PendingSettleCommission;

            //结算权益 = 昨日权益 + 入金 - 出金 + 平仓盈亏 - 浮动盈亏 - 资产买入额 + 资产卖出额 - 手续费
            settlement.EquitySettled = settlement.LastEquity + settlement.CashIn - settlement.CashOut + settlement.CloseProfitByDate + settlement.PositionProfitByDate - settlement.AssetBuyAmount + settlement.AssetSellAmount - settlement.Commission;
            settlement.CreditSettled = settlement.LastCredit + settlement.CreditCashIn - settlement.CreditCashOut;


            //保存结算记录
            ORM.MSettlement.InsertAccountSettlement(settlement);
            TLCtxHelper.ModuleSettleCentre.InvestAccountSettled(settlement);

            //标注交易所结算记录
            foreach (var settle in settlementlist)
            {
                settle.Settled = true;
                TLCtxHelper.ModuleDataRepository.MarkExchangeSettlementSettled(settle);
            }

            //标注出入金记录 已结算
            foreach (var txn in cashtranslsit)
            {
                txn.Settled = true;//标注已结算
                TLCtxHelper.ModuleDataRepository.MarkCashTransactionSettled(txn);
            }
            //设置昨日权益等信息
            this.LastEquity = settlement.EquitySettled;
            this.LastCredit = settlement.CreditSettled;
            //this.LastSettleday = settlement.Settleday;


        
        }
        /// <summary>
        /// 交易所 某个交易日 执行结算
        /// 交易所结算目的是按照交易所的结算时间进行 结算
        /// 结算生成 交易所结算记录以及对应的隔夜持仓
        /// 帐户加载恢复数据时 加载交易所结算数据，隔夜持仓数据 以及对应的未结算委托与成交
        /// 帐户在多交易所交易时 需要按交易所进行分别结算，
        /// 
        /// 交易所结算过程
        /// 1.计算交易所结算数据 手续费，平仓盈亏，盯市结算盈亏
        /// 2.保存隔夜持仓明细
        /// 3.保存结算记录
        /// 4.标注结算标识
        /// 
        /// 执行帐户某个交易所结算时 按交易所过滤持仓后
        /// 品种不同 结算方式不同
        /// 期货 逐日结算
        /// 股票 逐笔结算
        /// 
        /// CloseProfitByDate
        /// PositionProfitByDate
        /// CloseProfitByTrade
        /// PositionProfitByTrade
        /// 
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="settleday"></param>
        public void SettleExchange(Exchange exchange, int settleday)
        {
            try
            {
                if (this.ID == "8100007")
                {
                    int x = 1;
                }
                ///0.检查对应交易所是否有结算记录
                if (settlementlist.Any(settle => settle.Settleday == settleday && settle.Exchange == exchange.EXCode))
                {
                    logger.Warn(string.Format("Account:{0} have setteld in Exchange:{1} for date:{2}", this.ID, exchange.EXCode, settleday));
                    return;
                }

                ExchangeSettlement settlement = new ExchangeSettlementImpl();
                settlement.Account = this.ID;
                settlement.Exchange = exchange.EXCode;
                settlement.Settleday = settleday;


                ///1.设定持仓结算价格 生成持仓明细(插入持仓明细时会检查是否已经存在相同的持仓明晰)
                List<PositionDetail> positiondetail_settle = new List<PositionDetail>();
                foreach (Position pos in this.GetPositions(exchange).Where(p => !p.isFlat))
                {
                    //设定持仓结算价格
                    var target = BasicTracker.SettlementPriceTracker[settleday, pos.Symbol];// TLCtxHelper.ModuleSettleCentre.GetSettlementPrice(settleday, pos.Symbol);
                    if (target != null && target.Settlement > 0)
                    {
                        pos.SettlementPrice = target.Settlement;
                    }

                    //如果没有正常获得结算价格 持仓结算价按对应的最新价进行结算 系统加载结算时候 会将价格以tick fill到系统，因此分帐户侧持仓结算价为-1
                    if (pos.SettlementPrice == null || pos.SettlementPrice<0)
                    {
                        pos.SettlementPrice = pos.LastPrice;
                    }

                    //遍历该未平仓持仓对象下的所有持仓明细 获得持仓的持仓明细 期货持仓明细分别保存，股票类持仓进行合并
                    foreach (PositionDetail pd in pos.GetSettlePositionDetals())
                    {
                        if (pd == null) continue;
                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = settleday;
                        //保存持仓明细到数据库
                        TLCtxHelper.ModuleDataRepository.NewPositionDetail(pd);
                        positiondetail_settle.Add(pd);
                    }
                }
                ///2.统计手续费 平仓盈亏 盯市持仓盈亏
                //手续费 手续费为所有成交手续费累加
                settlement.Commission = this.GetTrades(exchange, settleday).Sum(f => f.GetCommission() * this.GetExchangeRate(f.oSymbol.SecurityFamily));

                //平仓盈亏 为所有持仓对象下面的平仓明细的平仓盈亏累加
                //settlement.CloseProfitByDate = this.GetPositions(exchange).Sum(pos => pos.PositionCloseDetail.Sum(pcd => pcd.CloseProfitByDate));
                //平仓盈亏核查 理论上成交累加的平仓盈亏和持仓明细累加的平仓盈亏应该一致
                //decimal closeprofit_commission = this.GetTrades(exchange, settleday).Sum(f => f.Profit);//累加某个交易所的所有成交平仓盈亏
                //根据交易所结算模式返回逐日或逐笔平仓盈亏
                settlement.CloseProfitByDate = exchange.SettleType == QSEnumSettleType.ByDate ? this.GetPositions(exchange).Where(pos => pos.IsMarginTrading()).Sum(pos => pos.CalCloseProfitByDate()*this.GetExchangeRate(pos.oSymbol.SecurityFamily)) : this.GetPositions(exchange).Where(pos => pos.IsMarginTrading()).Sum(pos => pos.CalCloseProfitByTrade()*this.GetExchangeRate(pos.oSymbol.SecurityFamily));
                
                
                //bool same = closeprofit_commission - closeprofit_posdetail < 1;
                ////两种计算方式不一致
                //if (!same)
                //{
                //    Util.Info("XXXXXXXXXXXXXXXXXXXX EXCHANGE SETTLE ERROR XXXXXXXXXXXXXXXXXXXXXXXXX");
                //    //输出信息
                //    foreach (var pos in this.GetPositions(exchange))
                //    {
                //        Util.Info(pos.ToString());
                //        //输出持仓明细
                //        foreach (var pd in pos.PositionDetailTotal)
                //        {
                //            Util.Info(pd.ToString());
                //        }
                //        //输出平仓明细
                //        foreach (var close in pos.PositionCloseDetail)
                //        {
                //            Util.Info(close.ToString());
                //        }
                //    }
                //}
                //settlement.CloseProfitByDate = closeprofit_posdetail;

                //浮动盈亏
                //根据交易所结算规则返回逐日浮动盈亏或0 逐笔结算不将浮动盈亏结算进入当日权益
                settlement.PositionProfitByDate = exchange.SettleType == QSEnumSettleType.ByDate ? this.GetPositions(exchange).Where(pos=>pos.IsMarginTrading()).Sum(pos => pos.CalPositionProfitByDate()) : 0;

                settlement.AssetBuyAmount = this.GetPositions(exchange).Where(pos => !pos.IsMarginTrading()).Sum(pos => pos.CalcBuyAmount()*this.GetExchangeRate(pos.oSymbol.SecurityFamily));

                settlement.AssetSellAmount = this.GetPositions(exchange).Where(pos => !pos.IsMarginTrading()).Sum(pos => pos.CalcSellAmount()*this.GetExchangeRate(pos.oSymbol.SecurityFamily));
                
                ///3.保存结算记录到数据库
                TLCtxHelper.ModuleDataRepository.NewExchangeSettlement(settlement);

                ///4.标注已结算数据 委托 成交 持仓
                foreach (var o in  this.GetOrders(exchange, settleday))
                {
                    o.Settled = true;
                    //TLCtxHelper.ModuleDataRepository.MarkOrderSettled(o);
                }
                foreach (var f in this.GetTrades(exchange, settleday))
                {
                    f.Settled = true;
                    //TLCtxHelper.ModuleDataRepository.MarkTradeSettled(f);
                }
                foreach (var pos in this.GetPositions(exchange))
                {
                    pos.Settled = true;
                    //如果持仓有隔夜持仓 将对应的隔夜持仓标注成已结算否则会对隔夜持仓重复加载
                    foreach (var pd in pos.PositionDetailYdRef)
                    {
                        TLCtxHelper.ModuleDataRepository.MarkPositionDetailSettled(pd);
                    }
                }
                //将已经结算的持仓从内存数据对象中屏蔽 持仓数据是一个状态数据,因此我们这里将上个周期的持仓对象进行屏蔽
                this.TKPosition.DropSettled();

                //处理所有持仓生成除权操作数据集
                List<PowerTransaction> powertrans = new List<PowerTransaction>();
                //执行除权操作 遍历所有持仓
                foreach (var pos in positiondetail_settle)
                {
                    PowerData pd = BasicTracker.PowerDataTracker[pos.Symbol];
                    if (pd == null) continue;
                    //执行分红操作
                    if (pd.Dividend != 0)
                    {
                        PowerTransaction txn = new PowerTransactionImpl();
                        txn.Settleday = settleday;
                        txn.Account = this.ID;
                        txn.Symbol = pos.Symbol;
                        txn.Size = pos.Volume;

                        txn.Dividend = txn.Size * pd.Dividend;//计算分红金额
                        powertrans.Add(txn);
                    }
                    //执行送股操作
                    if (pd.DonateShares != 0)
                    {
                        PowerTransaction txn = new PowerTransactionImpl();
                        txn.Settleday = settleday;
                        txn.Account = this.ID;
                        txn.Symbol = pos.Symbol;
                        txn.Size = pos.Volume;

                        txn.Shares = (int)(pos.Volume * pd.DonateShares);

                        if (txn.Shares > 0)
                        {
                            powertrans.Add(txn);
                        }

                    }
                    //执行配股操作
                    if (pd.RationeShares != 0 && pd.RationePrice != 0)
                    {
                        PowerTransaction txn = new PowerTransactionImpl();
                        txn.Settleday = settleday;
                        txn.Account = this.ID;
                        txn.Symbol = pos.Symbol;
                        txn.Size = pos.Volume;

                        txn.Shares = (int)(pos.Volume * pd.RationeShares);

                        if (txn.Shares > 0)
                        {
                            txn.Amount = txn.Shares * pd.RationePrice;
                            powertrans.Add(txn);
                        }
                    }
                }

                //执行除权操作
                foreach (var ptxn in powertrans)
                {
                    //根据除权操作执行交易账户出入金操作
                    TLCtxHelper.ModuleAccountManager.PowerOperation(ptxn);

                    //根据出去操作调整持仓
                    if (ptxn.Shares > 0)
                    { 
                        
                    }
                }

                ///5.加载持仓明晰和交易所结算记录
                foreach (var pd in positiondetail_settle)
                {
                    this.TKPosition.GotPosition(pd);
                }
                settlementlist.Add(settlement);//将交易所结算记录加入列表
            }
            catch (Exception ex) //增加单个帐户交易所结算报错
            {
                logger.Error(string.Format("Account:{0} 交易所结算失败:{1}", this.ID, ex.ToString()));
            }

        }


        /// <summary>
        /// 待结算记录
        /// 交易所结算后记录到待结算记录，所有交易所完成某个交易日的结算后一并进行帐户结算
        /// </summary>
        IEnumerable<ExchangeSettlement> PendingSettlement
        {
            get
            {
                return settlementlist.Where(settle => !settle.Settled);
            }
        }

        /// <summary>
        /// 待结算手续费
        /// </summary>
        /// <returns></returns>
        decimal PendingSettleCommission
        {
            get
            {
                return PendingSettlement.Sum(settle => settle.Commission);
            }
        }
        /// <summary>
        /// 待结算平仓盈亏
        /// </summary>
        decimal PendingSettleCloseProfitByDate
        {
            get
            {
                return PendingSettlement.Sum(settle => settle.CloseProfitByDate);
            }
        }

        /// <summary>
        /// 待结算浮动盈亏
        /// </summary>
        decimal PendingSettlePositionProfitByDate
        {
            get
            {
                return PendingSettlement.Sum(settle => settle.PositionProfitByDate);
            }
        }

        /// <summary>
        /// 待结算资产买入金额
        /// </summary>
        decimal PendingSettleAssetBuyAmount
        {
            get
            {
                return PendingSettlement.Sum(settle => settle.AssetBuyAmount);
            }
        }

        /// <summary>
        /// 待结算卖出金额
        /// </summary>
        decimal PendingSettleAssetSellAmount
        {
            get
            {
                return PendingSettlement.Sum(settle => settle.AssetSellAmount);
            }
        }

        /// <summary>
        /// 加载交易所结算数据
        /// </summary>
        /// <param name="settle"></param>
        public void LoadExchangeSettlement(ExchangeSettlement settle)
        {
            settlementlist.Add(settle);
        }

        /// <summary>
        /// 上次结算日
        /// </summary>
        public DateTime SettleDateTime { get; set; }

        /// <summary>
        /// 最近结算确认日期
        /// </summary>
        public long SettlementConfirmTimeStamp { get; set; }


        /// <summary>
        /// 帐户资金操作
        /// </summary>
        /// <param name="txn"></param>
        public void LoadCashTrans(CashTransaction txn)
        {
            cashtranslsit.Add(txn);
        }

        

    }
}
