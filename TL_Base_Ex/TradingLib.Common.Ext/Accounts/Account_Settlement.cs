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
    public partial class AccountBase
    {
        ThreadSafeList<ExchangeSettlement> settlementlist = new ThreadSafeList<ExchangeSettlement>();

        /// <summary>
        /// 加载交易所结算数据
        /// </summary>
        /// <param name="settle"></param>
        public void LoadExchangeSettlement(ExchangeSettlement settle)
        {
            settlementlist.Add(settle);
        }


        public void SettleAccount(int settleday)
        { 
            //所有交易所对应的settleday交易日结算完毕后，我们执行汇总结算
            AccountSettlement settlement = new AccountSettlement();
            settlement.Account = this.ID;
            settlement.Settleday = settleday;

            settlement.LastEquity = this.LastEquity;
            settlement.LastCredit = this.LastCredit;
            settlement.CashIn = this.CashIn;
            settlement.CashOut = this.CashOut;
            settlement.CreditCashIn = this.CreditCashIn;
            settlement.CreditCashOut = this.CreditCashOut;

            settlement.CloseProfitByDate = PendingSettleCloseProfitByDate;
            settlement.PositionProfitByDate = PendingSettlePositionProfitByDate;
            settlement.Commission = PendingSettleCommission;

            settlement.EquitySettled = settlement.LastEquity + settlement.CashIn - settlement.CashOut + settlement.CloseProfitByDate + settlement.PositionProfitByDate - settlement.Commission;
            settlement.CreditSettled = settlement.LastCredit + settlement.CreditCashIn - settlement.CreditCashOut;

            //保存结算记录
            ORM.MSettlement.InsertAccountSettlement(settlement);

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
            this.LastSettleday = settlement.Settleday;


        
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
        public void SettleExchange(IExchange exchange,int settleday)
        {
            try
            {
                ///0.检查对应交易所是否有结算记录
                if (settlementlist.Any(settle => settle.Settleday == settleday && settle.Exchange == exchange.EXCode))
                {
                    Util.Warn(string.Format("Account:{0} have setteld in Exchange:{1} for date:{2}", this.ID, exchange.EXCode, settleday));
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
                    SettlementPrice target = TLCtxHelper.ModuleSettleCentre.GetSettlementPrice(settleday, pos.Symbol);
                    if (target != null && target.Settlement > 0)
                    {
                        pos.SettlementPrice = target.Settlement;
                    }

                    //如果没有正常获得结算价格 持仓结算价按对应的最新价进行结算
                    if (pos.SettlementPrice == null)
                    {
                        pos.SettlementPrice = pos.LastPrice;
                    }

                    //遍历该未平仓持仓对象下的所有持仓明细
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = settleday;
                        //保存持仓明细到数据库
                        TLCtxHelper.ModuleDataRepository.NewPositionDetail(pd);
                        positiondetail_settle.Add(pd);
                    }
                }
                ///2.统计手续费 平仓盈亏 盯市持仓盈亏
                //手续费 手续费为所有成交手续费累加
                settlement.Commission = this.GetTrades(exchange, settleday).Sum(f => f.Commission);

                //平仓盈亏 为所有持仓对象下面的平仓明细的平仓盈亏累加
                //settlement.CloseProfitByDate = this.GetPositions(exchange).Sum(pos => pos.PositionCloseDetail.Sum(pcd => pcd.CloseProfitByDate));
                //平仓盈亏核查 理论上成交累加的平仓盈亏和持仓明细累加的平仓盈亏应该一致
                decimal closeprofit_commission = this.GetTrades(exchange, settleday).Sum(f => f.Profit);//累加某个交易所的所有成交平仓盈亏
                //根据交易所结算模式返回逐日或逐笔平仓盈亏
                decimal closeprofit_posdetail = exchange.SettleType == QSEnumSettleType.ByDate ? this.GetPositions(exchange).Sum(pos => pos.CalCloseProfitByDate()) : this.GetPositions(exchange).Sum(pos => pos.CalCloseProfitByTrade());
                
                
                bool same = closeprofit_commission - closeprofit_posdetail < 1;
                //两种计算方式不一致
                if (!same)
                {
                    Util.Info("XXXXXXXXXXXXXXXXXXXX EXCHANGE SETTLE ERROR XXXXXXXXXXXXXXXXXXXXXXXXX");
                    //输出信息
                    foreach (var pos in this.GetPositions(exchange))
                    {
                        Util.Info(pos.ToString());
                        //输出持仓明细
                        foreach (var pd in pos.PositionDetailTotal)
                        {
                            Util.Info(pd.ToString());
                        }
                        //输出平仓明细
                        foreach (var close in pos.PositionCloseDetail)
                        {
                            Util.Info(close.ToString());
                        }
                    }
                }
                settlement.CloseProfitByDate = closeprofit_posdetail;

                //浮动盈亏
                //根据交易所结算规则返回逐日浮动盈亏或0 逐笔结算不将浮动盈亏结算进入当日权益
                settlement.PositionProfitByDate = exchange.SettleType == QSEnumSettleType.ByDate ? this.GetPositions(exchange).Sum(pos => pos.CalPositionProfitByDate()) : 0;
                
                
                ///3.保存结算记录到数据库
                TLCtxHelper.ModuleDataRepository.NewExchangeSettlement(settlement);

                ///4.标注已结算数据 委托 成交 持仓
                foreach (var o in  this.GetOrders(exchange, settleday))
                {
                    o.Settled = true;
                    TLCtxHelper.ModuleDataRepository.MarkOrderSettled(o);
                }
                foreach (var f in this.GetTrades(exchange, settleday))
                {
                    f.Settled = true;
                    TLCtxHelper.ModuleDataRepository.MarkTradeSettled(f);
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


                ///5.加载持仓明晰和交易所结算记录
                foreach (var pd in positiondetail_settle)
                {
                    this.TKPosition.GotPosition(pd);
                }
                settlementlist.Add(settlement);//将交易所结算记录加入列表
            }
            catch (Exception ex) //增加单个帐户交易所结算报错
            {
                Util.Error(string.Format("Account:{0} 交易所结算失败:{1}", this.ID, ex.ToString()));
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
        /// 待结算盯市浮动盈亏
        /// </summary>
        decimal PendingSettlePositionProfitByDate
        {
            get
            {
                return PendingSettlement.Sum(settle => settle.PositionProfitByDate);
            }
        }
    }
}
