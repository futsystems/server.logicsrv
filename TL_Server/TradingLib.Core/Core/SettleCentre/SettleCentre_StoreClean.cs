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



        public void SaveHoldInfo()
        {
            debug(datastoreheader + "Save PositionRound Open Into DataBase", QSEnumDebugLevel.INFO);
            foreach (PositionRound pr in _clearcentre.PositionRoundTracker.RoundOpened)
            {
                ORM.MSettlement.InsertHoldPositionRound(pr, NextTradingday);
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
            foreach (Position pos in _clearcentre.TotalPositions)
            {
                if (pos.SettlementPrice == null)
                    pos.SettlementPrice = pos.LastPrice;
            }


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

                        //保存结算持仓明细时要将结算日更新为当前
                        pd.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
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
