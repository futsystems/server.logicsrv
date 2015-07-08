﻿using System;
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


        /// <summary>
        /// 保存结算价格
        /// 通过行情路由获得当前市场快照然后保存快照中所有合约的结算价格
        /// </summary>
        void SaveSettlementPrice()
        {
            //清空结算价信息
            _settlementPriceTracker.Clear();
            //保存结算价信息 只保存持仓对应的合约 1.9版本 通过获得所有合约市场快照来保存所有合约的结算价信息
            foreach (Position pos in _clearcentre.TotalPositions.Where(pos => !pos.isFlat))
            {
                Tick k = TLCtxHelper.CmdUtils.GetTickSnapshot(pos.Symbol);
                if (k != null && k.Settlement != 0 && (double)k.Settlement < double.MaxValue)
                {
                    MarketData data = new MarketData();
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
                    data.SettleDay = TLCtxHelper.CmdSettleCentre.NextTradingday;
                    data.Settlement = k.Settlement;
                    data.Symbol = k.Symbol;
                    data.UpperLimit = k.UpperLimit;
                    data.Vol = k.Vol;

                    _settlementPriceTracker.UpdateSettlementPrice(data);
                }
                else
                {
                    MarketData data = new MarketData();
                    data.AskPrice = -1;
                    data.AskSize = -1;
                    data.BidPrice = -1;
                    data.BidSize = -1;
                    data.Close = -1;
                    data.High = -1;
                    data.Low = -1;
                    data.LowerLimit = -1;
                    data.OI = -1;
                    data.Open = -1;
                    data.PreOI = -1;
                    data.PreSettlement = -1;
                    data.SettleDay = TLCtxHelper.CmdSettleCentre.NextTradingday;
                    data.Settlement = -1;
                    data.Symbol = pos.Symbol;
                    data.UpperLimit = -1;
                    data.Vol = -1;
                    _settlementPriceTracker.UpdateSettlementPrice(data);
                }
            }
        }

        /// <summary>
        /// 将结算价格绑定到持仓对象
        /// </summary>
        void BindSettlementPrice()
        {
            MarketData target = null;
            //遍历所有分帐户侧持仓
            foreach (Position pos in _clearcentre.TotalPositions.Where(pos=>!pos.isFlat))
            {
                //如果系统设置成以最新价进行结算 则结算价为最新价格
                if (_settleWithLatestPrice)//如果以最新价进行结算
                {
                    pos.SettlementPrice = pos.LastPrice;//将最新价设定到持仓的结算价
                }
                else
                {
                    //如果持仓合约有对应的结算价信息 设定结算价
                    target = _settlementPriceTracker[pos.Symbol];
                    if (target != null && target.Settlement > 0)
                    {
                        pos.SettlementPrice = target.Settlement;
                    }
                }

                //如果没有设定结算价 则将持仓的最新价格设置成为结算价
                if (pos.SettlementPrice == null)
                {
                    pos.SettlementPrice = pos.LastPrice;
                }

            }

            //遍历所有接口侧持仓
            foreach (IBroker broker in TLCtxHelper.Ctx.RouterManager.Brokers)
            {
                //接口没有启动 则没有交易数据
                if (!broker.IsLive)
                    continue;

                //遍历成交接口有持仓的 持仓，将该持仓的持仓明细保存到数据库
                foreach (Position pos in broker.Positions.Where(p => !p.isFlat))
                {
                    //如果持仓合约有对应的结算价信息 设定结算价
                    target = _settlementPriceTracker[pos.Symbol];
                    if (target != null && target.Settlement > 0)
                    {
                        pos.SettlementPrice = target.Settlement;
                    }
                    //如果没有设定结算价 则将持仓的最新价格设置成为结算价
                    if (pos.SettlementPrice == null)
                    {
                        pos.SettlementPrice = pos.LastPrice;
                    }
                }
            }

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
            //foreach (Position pos in _clearcentre.TotalPositions.Where(pos => !pos.isFlat))
            //{
            //    //如果系统设定按最新价来执行结算 则将结算价格设为持仓的最新价
            //    if (_settleWithLatestPrice)//如果以最新价进行结算
            //    {
            //        pos.SettlementPrice = pos.LastPrice;//将最新价设定到持仓的结算价
            //    }
            //    else
            //    {
            //        //默认情况下 系统按结算价进行结算，如果结算价缺失则按最新价进行结算
            //        if (pos.SettlementPrice == null)
            //            pos.SettlementPrice = pos.LastPrice;
            //    }
            //}


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
            debug(string.Format("Saved {0} Account PositionDetails Successfull",i),QSEnumDebugLevel.INFO);

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