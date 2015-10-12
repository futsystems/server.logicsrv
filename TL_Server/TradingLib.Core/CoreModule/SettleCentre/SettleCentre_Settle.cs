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
        /// 结算某个交易账户 指定交易所和结算日
        /// 交易所结算相当于是将帐户中的记录进行结算汇总，
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        ExchangeSettlement SettleAccount(IAccount account,IExchange exchange,int settleday)
        {
            ExchangeSettlement settlement = new ExchangeSettlementImpl();
            settlement.Account = account.ID;
            settlement.Exchange = exchange.EXCode;
            settlement.Settleday = settleday;
            
            //手续费 手续费为所有成交手续费累加
            settlement.Commission = account.GetTrades(exchange).Sum(f => f.Commission);
            //平仓盈亏 为所有持仓对象下面的平仓明细的平仓盈亏累加
            settlement.CloseProfitByDate = account.GetPositions(exchange).Sum(pos => pos.PositionCloseDetail.Sum(pcd => pcd.CloseProfitByDate));
            //浮动盈亏
            settlement.PositionProfitByDate = account.GetPositions(exchange).Sum(pos => pos.PositionDetailTotal.Sum(pd => pd.PositionProfitByDate));

            return settlement;
        }
        /// <summary>
        /// 保存结算价格
        /// </summary>
        void SaveSettlePrice()
        { 
        
        }
        
        /// <summary>
        /// 设定持仓结算价格
        /// </summary>
        void BindSetlePrice()
        { 
        
        }

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
