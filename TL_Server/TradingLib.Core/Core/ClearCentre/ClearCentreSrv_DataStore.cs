using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class ClearCentre
    {
        #region 数据保存

        string datacleanheader = "#####DataClean:";
        /// <summary>
        /// 结算后清空日内临时记录表
        /// </summary>
        public void CleanTempTable()
        {
            debug(datacleanheader + "Clean Tmp_XXX Tables", QSEnumDebugLevel.INFO);
            ORM.MTradingInfo.ClearIntradayOrders(TLCtxHelper.CmdSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayTrades(TLCtxHelper.CmdSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayOrderActions(TLCtxHelper.CmdSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayPosTransactions(TLCtxHelper.CmdSettleCentre.NextTradingday);
            debug("Cleaned Success", QSEnumDebugLevel.INFO);
        }



        public void SaveHoldInfo()
        {
            debug(datastoreheader + "Save PositionRound Open Into DataBase", QSEnumDebugLevel.INFO);
            foreach (PositionRound pr in this.PositionRoundTracker.RoundOpened)
            {
                ORM.MSettlement.InsertHoldPositionRound(pr, TLCtxHelper.CmdSettleCentre.NextTradingday);
            }
            debug(datastoreheader + "Save Positionround Open Successfull", QSEnumDebugLevel.INFO);
        }

        string datastoreheader = "#####DataStore:";
        /// <summary>
        /// 1.保存持仓明细
        /// 将所有隔夜持仓明细保存到历史持仓明细表 用于下一个交易日生成对应的持仓状态
        /// 这里包含了持仓明细的盯市盈亏计算，该计算需要当日结算价
        /// </summary>
        public void SavePositionDetails()
        {
            debug(datastoreheader + "Save PositionDetails....", QSEnumDebugLevel.MUST);

            //检查所有系统持仓按照一定的逻辑获得 结算价 目前如果结算价不存在则取持仓最新价来替代(持仓最新价 当没有tick时是以持仓成本作价)
            foreach (Position pos in this.TotalPositions)
            {
                if (_settleWithLatestPrice)//如果以最新价进行结算
                {
                    pos.SettlementPrice = pos.LastPrice;//将最新价设定到持仓的结算价
                }
                else
                {
                    if (pos.SettlementPrice == null)
                        pos.SettlementPrice = pos.LastPrice;
                }
            }


            int i = 0;
            //遍历所有交易帐户
            foreach (IAccount account in this.Accounts)
            {
                //遍历交易帐户下所有未平仓持仓对象
                foreach (Position pos in account.GetPositionsHold())
                {
                    //遍历该未平仓持仓对象下的所有持仓明细
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                        //保存持仓明细到数据库
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }
                }
            }
            debug(string.Format("Saved {0} Account PositionDetails Successfull", i), QSEnumDebugLevel.INFO);

            i = 0;
            //遍历所有成交接口
            foreach (IBroker broker in TLCtxHelper.Ctx.RouterManager.Brokers)
            {
                //接口没有启动 则没有交易数据
                if (!broker.IsLive)
                    continue;

                //遍历成交接口有持仓的 持仓，将该持仓的持仓明细保存到数据库
                foreach (Position pos in broker.Positions.Where(p => !p.isFlat))
                {
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                        //设定标识
                        pd.Broker = broker.Token;
                        pd.Breed = QSEnumOrderBreedType.BROKER;

                        //保存持仓明细到数据库
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }
                }
            }
            debug(string.Format("Saved {0} Broker PositionDetails Successfull", i), QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 2.将日内交易数据传储到历史交易记录表
        /// </summary>
        public void Dump2Log()
        {
            debug(datastoreheader + "Dump TradingInfo(Order,Trade,OrderAction)", QSEnumDebugLevel.INFO);
            int onum, tnum, cnum, prnum;

            ORM.MTradingInfo.DumpIntradayOrders(out onum);
            ORM.MTradingInfo.DumpIntradayTrades(out tnum);
            ORM.MTradingInfo.DumpIntradayOrderActions(out cnum);
            ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

            debug("Order       Saved:" + onum.ToString(), QSEnumDebugLevel.INFO);
            debug("Trade       Saved:" + tnum.ToString(), QSEnumDebugLevel.INFO);
            debug("OrderAction Saved:" + cnum.ToString(), QSEnumDebugLevel.INFO);
            debug("PosTrans    Saved:" + prnum.ToString(), QSEnumDebugLevel.INFO);
        }




        #endregion

        /// <summary>
        /// 结算所有交易账户
        /// 结算分析
        /// 1.单日lastequity + realizedpl + unrealizedpl - commission + cashin - cashout = now equity 数据库检验通过
        /// 2.当日结算完毕后的nowequity即为账户表中的lastequity 数据库检验通过
        /// 3.产生错误就是在某些结算记录中上日权益不等于该账户的nowequity
        /// 4.账户结算时需要检查 账户的上日权益是否是数据库记录的上日权益
        /// </summary>
        void SettleAccount()
        {
            debug(string.Format("#####SettleAccount: Start Settele Account,Current Tradingday:{0}", TLCtxHelper.CmdSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            foreach (IAccount acc in this.Accounts)
            {
                try
                {
                    ORM.MSettlement.SettleAccount(acc);
                }
                catch (Exception ex)
                {
                    debug(string.Format("SettleError,Account:{0} errors:{1}", acc.ID, ex.ToString()), QSEnumDebugLevel.ERROR);
                }
            }

            //更新最近结算日
            debug(string.Format("Update lastsettleday as:{0}",TLCtxHelper.CmdSettleCentre.CurrentTradingday), QSEnumDebugLevel.INFO);
            ORM.MSettlement.UpdateSettleday(TLCtxHelper.CmdSettleCentre.CurrentTradingday);
            debug("Settlement Done", QSEnumDebugLevel.INFO);
        }

    }
}
