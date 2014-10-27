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
            debug(datacleanheader + "清空日内交易记录临时表", QSEnumDebugLevel.INFO);
            ORM.MTradingInfo.ClearIntradayOrders(NextTradingday);
            ORM.MTradingInfo.ClearIntradayTrades(NextTradingday);
            ORM.MTradingInfo.ClearIntradayOrderActions(NextTradingday);
            ORM.MTradingInfo.ClearIntradayPosTransactions(NextTradingday);
            debug(datacleanheader + "清空日内交易记录临时表完毕", QSEnumDebugLevel.INFO);
        }


        /// <summary>
        /// 设定隔夜持仓的结算价格
        /// 如果是历史结算则历史持仓的价格需要获得对应交易日的结算价格 目前这里没有保存历史行情数据
        /// 这里我们取原来的成本价格 将结算浮动盈亏调整到最后一个交易日
        /// </summary>
        void BindPositionSettlePrice()
        {
            //从清算中心获得所有持仓 如果持仓未关闭则记录到结算持仓表
            foreach (Position pos in _clearcentre.TotalPositions)//总统计中的postion与account中的分帐户统计是不同的postion数据 需要进行同步
            {
                debug(pos.ToString() + " set settleprice to:" + pos.LastPrice,QSEnumDebugLevel.INFO);
                if (this.IsNormal)
                {
                    //1.设定总统计持仓结算价
                    pos.SettlementPrice = pos.LastPrice;
                    //2.设定分帐户持仓结算价
                    IAccount account = _clearcentre[pos.Account];
                    account.GetPosition(pos.Symbol, pos.isLong).SettlementPrice = pos.SettlementPrice;
                }
                else
                {
                    //1.设定总统计持仓结算价
                    pos.SettlementPrice = pos.AvgPrice;
                    //2.设定分帐户持仓结算价
                    IAccount account = _clearcentre[pos.Account];
                    account.GetPosition(pos.Symbol, pos.isLong).SettlementPrice = pos.SettlementPrice;
                }
            }
        }

        string datastoreheader = "#####DataStore:";
        public void SaveHoldInfo()
        {
            debug(datastoreheader + "保存结算持仓和结算回合记录到相关记录表", QSEnumDebugLevel.INFO);
            foreach (PositionRound pr in _clearcentre.PositionRoundTracker.RoundOpened)
            {
                ORM.MSettlement.InsertHoldPositionRound(pr, NextTradingday);
            }

            //从清算中心获得所有持仓 如果持仓未关闭则记录到结算持仓表
            foreach (Position pos in _clearcentre.TotalPositions.Where(p=>!p.isFlat))
            {
                ORM.MSettlement.InsertHoldPosition(pos.ToSettlePosition(), NextTradingday);
            }
            //debug(datastoreheader + "保存PR数据完毕", QSEnumDebugLevel.INFO);
        }

        /// <summary>
        /// 2.将日内交易数据传储到历史交易记录表
        /// </summary>
        public void Dump2Log()
        {
            debug(datastoreheader + "转储交易信息 委托 取消 成交",QSEnumDebugLevel.INFO);
            int onum, tnum, cnum, prnum;

            ORM.MTradingInfo.DumpIntradayOrders(out onum);
            ORM.MTradingInfo.DumpIntradayTrades(out tnum);
            ORM.MTradingInfo.DumpIntradayOrderActions(out cnum);
            ORM.MTradingInfo.DumpIntradayPosTransactions(out prnum);

            debug("委托转储:" + onum.ToString(),QSEnumDebugLevel.INFO);
            debug("成交转储:" + tnum.ToString(), QSEnumDebugLevel.INFO);
            debug("委托操作转储:" + cnum.ToString(), QSEnumDebugLevel.INFO);
            debug("交易回合转储:" + prnum.ToString(), QSEnumDebugLevel.INFO);
        }

        #endregion
    }
}
