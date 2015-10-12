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

        
        public void SettleExchange(IExchange exchange,int settleday)
        {
            ExchangeSettlement settlement = new ExchangeSettlementImpl();
            settlement.Account = this.ID;
            settlement.Exchange = exchange.EXCode;
            settlement.Settleday = settleday;

            //手续费 手续费为所有成交手续费累加
            settlement.Commission = this.GetTrades(exchange).Sum(f => f.Commission);
            //平仓盈亏 为所有持仓对象下面的平仓明细的平仓盈亏累加
            settlement.CloseProfitByDate = this.GetPositions(exchange).Sum(pos => pos.PositionCloseDetail.Sum(pcd => pcd.CloseProfitByDate));
            //浮动盈亏
            settlement.PositionProfitByDate = this.GetPositions(exchange).Sum(pos => pos.PositionDetailTotal.Sum(pd => pd.PositionProfitByDate));

            List<PositionDetail> positiondetail_settle = new List<PositionDetail>();
            //遍历交易帐户下所有未平仓持仓对象 生成持仓明细
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

            //

            //将结算记录到数据库
            ORM.MSettlement.InsertExchangeSettlement(settlement);

            //加入待结算列表的同时需要将内存中的数据失效 避免重复计算

            //标注对应委托和结算为已结算
            foreach (var o in this.GetOrders(exchange))
            {
                o.Settled = true;
            }
            foreach (var f in this.GetTrades(exchange))
            {
                f.Settled = true;
            }
            foreach (var pos in this.GetPositions(exchange))
            {
                pos.Settled = true;
            }

            //将已经结算的持仓从内存数据对象中屏蔽
            this.TKPosition.DropSettled();

            //将结算持仓重新载入内存
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
                return settlementlist.Where(settle => settle.Settleday > this.LastSettleday);
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
