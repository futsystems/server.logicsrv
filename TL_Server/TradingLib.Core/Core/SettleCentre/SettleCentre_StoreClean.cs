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
        #region 数据保存

        string datacleanheader = "#####DataClean:";
        /// <summary>
        /// 结算后清空日内临时记录表
        /// </summary>
        public void CleanTempTable()
        {
            debug(datacleanheader + "Clean Tmp_XXX Tables", QSEnumDebugLevel.INFO);
            ORM.MTradingInfo.ClearIntradayOrders(NextTradingday);
            ORM.MTradingInfo.ClearIntradayTrades(NextTradingday);
            ORM.MTradingInfo.ClearIntradayOrderActions(NextTradingday);
            ORM.MTradingInfo.ClearIntradayPosTransactions(NextTradingday);
            debug("Cleaned Success", QSEnumDebugLevel.INFO);
        }


        /// <summary>
        /// 设定隔夜持仓的结算价格
        /// 如果是历史结算则历史持仓的价格需要获得对应交易日的结算价格 目前这里没有保存历史行情数据
        /// 这里我们取原来的成本价格 将结算浮动盈亏调整到最后一个交易日
        /// </summary>
        //void BindPositionSettlePrice()
        //{
        //    //从清算中心获得所有持仓 如果持仓未关闭则记录到结算持仓表
        //    //foreach (Position pos in _clearcentre.TotalPositions)//总统计中的postion与account中的分帐户统计是不同的postion数据 需要进行同步
        //    //{
        //    //    debug(pos.ToString() + " set settleprice to:" + pos.LastPrice,QSEnumDebugLevel.INFO);
        //    //    if (this.IsNormal)
        //    //    {
        //    //        //1.设定总统计持仓结算价
        //    //        pos.SettlementPrice = pos.LastPrice;
        //    //        //2.设定分帐户持仓结算价
        //    //        IAccount account = _clearcentre[pos.Account];
        //    //        account.GetPosition(pos.Symbol, pos.isLong).SettlementPrice = pos.SettlementPrice;
        //    //    }
        //    //    else
        //    //    {
        //    //        //1.设定总统计持仓结算价
        //    //        pos.SettlementPrice = pos.AvgPrice;
        //    //        //2.设定分帐户持仓结算价
        //    //        IAccount account = _clearcentre[pos.Account];
        //    //        account.GetPosition(pos.Symbol, pos.isLong).SettlementPrice = pos.SettlementPrice;
        //    //    }
        //    //}
        //}

        
        //public void SaveHoldInfo()
        //{
        //    debug(datastoreheader + "保存结算持仓和结算回合记录到相关记录表", QSEnumDebugLevel.INFO);
        //    foreach (PositionRound pr in _clearcentre.PositionRoundTracker.RoundOpened)
        //    {
        //        ORM.MSettlement.InsertHoldPositionRound(pr, NextTradingday);
        //    }

        //    //从清算中心获得所有持仓 如果持仓未关闭则记录到结算持仓表
        //    foreach (Position pos in _clearcentre.TotalPositions.Where(p=>!p.isFlat))
        //    {
        //        ORM.MSettlement.InsertHoldPosition(pos.ToSettlePosition(), NextTradingday);
        //    }
        //    //debug(datastoreheader + "保存PR数据完毕", QSEnumDebugLevel.INFO);
        //}

        string datastoreheader = "#####DataStore:";
        /// <summary>
        /// 1.保存持仓明细
        /// 将所有隔夜持仓明细保存到历史持仓明细表 用于下一个交易日生成对应的持仓状态
        /// 这里包含了持仓明细的盯市盈亏计算，该计算需要当日结算价
        /// </summary>
        public void SavePositionDetails()
        {
            debug(datastoreheader + "Save PositionDetails....", QSEnumDebugLevel.MUST);
            int i=0;
            //遍历所有交易帐户
            foreach (IAccount account in _clearcentre.Accounts)
            {
                //遍历交易帐户下所有未平仓持仓对象
                foreach (Position pos in account.GetPositionsHold())
                {
                    //遍历该未平仓持仓对象下的所有持仓明细
                    foreach (PositionDetail pd in pos.PositionDetailTotal.Where(pd => !pd.IsClosed()))
                    {
                        //表明该持仓明细是今日新开仓持仓明细 交易日设定为当前交易日
                        if (pd.Tradingday == 0)
                        {
                            pd.Tradingday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
                        }

                        //结算日
                        pd.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;

                        //计算留仓保证金和盯市浮动盈亏
                        pd.Margin = pd.CalMargin();
                        pd.PositionProfitByDate = pd.CalUnRealizedProfitByDate();

                        //保存持仓明细到数据库
                        ORM.MSettlement.InsertPositionDetail(pd);
                        i++;
                    }   
                }
            }
            debug(string.Format("Saved {0} PositionDetails Successfull",i),QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 2.将日内交易数据传储到历史交易记录表
        /// </summary>
        public void Dump2Log()
        {
            debug(datastoreheader + "Dump TradingInfo(Order,Trade,OrderAction)",QSEnumDebugLevel.INFO);
            int onum, tnum, cnum, prnum;

            ORM.MTradingInfo.DumpIntradayOrders(out onum);
            ORM.MTradingInfo.DumpIntradayTrades(out tnum);
            ORM.MTradingInfo.DumpIntradayOrderActions(out cnum);
            ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

            debug("Order       Saved:" + onum.ToString(),QSEnumDebugLevel.INFO);
            debug("Trade       Saved:" + tnum.ToString(), QSEnumDebugLevel.INFO);
            debug("OrderAction Saved:" + cnum.ToString(), QSEnumDebugLevel.INFO);
            debug("PosTrans    Saved:" + prnum.ToString(), QSEnumDebugLevel.INFO);
        }

        #endregion
    }
}
