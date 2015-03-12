﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;


namespace TradingLib.Core
{
    public partial class DataRepository
    {

        #region 数据保存

        string datacleanheader = "#####DataClean:";

        /// <summary>
        /// 结算后清空日内临时记录表
        /// </summary>
        public void CleanTempTable()
        {
            debug(datacleanheader + "Clean Tmp_XXX Tables", QSEnumDebugLevel.INFO);
            ORM.MTradingInfo.ClearIntradayOrders(TLCtxHelper.ModuleSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayTrades(TLCtxHelper.ModuleSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayOrderActions(TLCtxHelper.ModuleSettleCentre.NextTradingday);
            ORM.MTradingInfo.ClearIntradayPosTransactions(TLCtxHelper.ModuleSettleCentre.NextTradingday);
            debug("Cleaned Success", QSEnumDebugLevel.INFO);
        }

        //public void SaveHoldInfo()
        //{
        //    debug(datastoreheader + "Save PositionRound Open Into DataBase", QSEnumDebugLevel.INFO);
        //    foreach (PositionRound pr in this.PositionRoundTracker.RoundOpened)
        //    {
        //        ORM.MSettlement.InsertHoldPositionRound(pr, TLCtxHelper.CmdSettleCentre.NextTradingday);
        //    }
        //    debug(datastoreheader + "Save Positionround Open Successfull", QSEnumDebugLevel.INFO);
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
            
            //检查所有系统持仓按照一定的逻辑获得 结算价 目前如果结算价不存在则取持仓最新价来替代(持仓最新价 当没有tick时是以持仓成本作价)
            foreach (Position pos in TLCtxHelper.ModuleClearCentre.TotalPositions)
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
            foreach (IAccount account in TLCtxHelper.ModuleAccountManager.Accounts)
            {
                //遍历交易帐户下所有未平仓持仓对象
                foreach (Position pos in account.GetPositionsHold())
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
            debug(string.Format("Saved {0} Account PositionDetails Successfull", i), QSEnumDebugLevel.INFO);

            i = 0;
            //遍历所有成交接口
            foreach (IBroker broker in TLCtxHelper.ServiceRouterManager.Brokers)
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
    }
}