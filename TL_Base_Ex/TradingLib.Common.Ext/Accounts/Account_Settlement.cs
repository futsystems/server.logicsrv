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
            settlement.CashIn = 0;
            settlement.CashOut = 0;
            settlement.CreditCashIn = 0;
            settlement.CreditCashOut = 0;
            settlement.CloseProfitByDate = PendingSettleCloseProfitByDate;
            settlement.PositionProfitByDate = PendingSettlePositionProfitByDate;
            settlement.Commission = PendingSettleCommission;
            settlement.EquitySettled = settlement.LastEquity + settlement.CashIn - settlement.CashOut + settlement.CloseProfitByDate + settlement.PositionProfitByDate - settlement.Commission;
            settlement.CreditSettled = settlement.LastCredit + settlement.CreditCashIn - settlement.CreditCashOut;

            //保存结算记录
            ORM.MSettlement.InsertAccountSettlement(settlement);

            foreach (var settle in settlementlist)
            {
                settle.Settled = true;
                TLCtxHelper.ModuleDataRepository.MarkExchangeSettlementSettled(settle);
            }
            foreach (var txn in cashtranslsit)
            {
                txn.Settled = true;//标注已结算
            }
            //TODO:出入金也记录入列表 然后通过Settled来标注
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
        /// </summary>
        /// <param name="exchange"></param>
        /// <param name="settleday"></param>
        public void SettleExchange(IExchange exchange,int settleday)
        {
            ///0.检查对应交易所是否有结算记录
            if (settlementlist.Any(settle => settle.Settleday == settleday && settle.Exchange==exchange.EXCode))
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
                //遍历该未平仓持仓对象下的所有持仓明细
                foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                {
                    //保存结算持仓明细时要将结算日更新为当前
                    pd.Settleday = settleday;
                    //保存持仓明细到数据库
                    ORM.MSettlement.InsertPositionDetail(pd);
                    positiondetail_settle.Add(pd);
                }
            }

            ///2.统计手续费 平仓盈亏 盯市持仓盈亏
            //手续费 手续费为所有成交手续费累加
            settlement.Commission = this.GetTrades(exchange).Sum(f => f.Commission);
            //平仓盈亏 为所有持仓对象下面的平仓明细的平仓盈亏累加
            settlement.CloseProfitByDate = this.GetPositions(exchange).Sum(pos => pos.PositionCloseDetail.Sum(pcd => pcd.CloseProfitByDate));
            //浮动盈亏
            settlement.PositionProfitByDate = this.GetPositions(exchange).Sum(pos => pos.PositionDetailTotal.Sum(pd => pd.PositionProfitByDate));

            ///3.保存结算记录到数据库
            ORM.MSettlement.InsertExchangeSettlement(settlement);

            ///4.标注已结算数据 委托 成交 持仓
            foreach (var o in this.GetOrders(exchange))
            {
                o.Settled = true;
                TLCtxHelper.ModuleDataRepository.MarkOrderSettled(o);
            }
            foreach (var f in this.GetTrades(exchange))
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
            //将已经结算的持仓从内存数据对象中屏蔽
            this.TKPosition.DropSettled();


            ///5.加载持仓明晰和交易所结算记录
            foreach (var pd in positiondetail_settle)
            {
                this.TKPosition.GotPosition(pd);
            }
            settlementlist.Add(settlement);//将交易所结算记录加入列表

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
