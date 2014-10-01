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
            ORM.MTradingInfo.ClearIntradayOrders(CurrentTradingday);
            ORM.MTradingInfo.ClearIntradayTrades(CurrentTradingday);
            ORM.MTradingInfo.ClearIntradayOrderActions(CurrentTradingday);
            ORM.MTradingInfo.ClearIntradayPosTransactions(CurrentTradingday);
            debug(datacleanheader + "清空日内交易记录临时表完毕", QSEnumDebugLevel.INFO);
        }



        void BindPositionSettlePrice()
        {
            //从清算中心获得所有持仓 如果持仓未关闭则记录到结算持仓表
            foreach (Position pos in _clearcentre.TotalPositions)//总统计中的postion与account中的分帐户统计是不同的postion数据 需要进行同步
            {
                debug(pos.ToString() + " set settleprice to:" + pos.LastPrice,QSEnumDebugLevel.INFO);
                //1.设定总统计持仓结算价
                pos.SettlePrice = pos.LastPrice;
                //2.设定分帐户持仓结算价
                IAccount account = _clearcentre[pos.Account];
                account.GetPosition(pos.Symbol,pos.isLong).SettlePrice = pos.SettlePrice;
            }
        }

        string datastoreheader = "#####DataStore:";
        public void SaveHoldInfo()
        {
            debug(datastoreheader + "保存结算持仓和结算回合记录到相关记录表", QSEnumDebugLevel.INFO);
            foreach (PositionRound pr in _clearcentre.PositionRoundTracker.RoundOpened)
            {
                ORM.MSettlement.InsertHoldPositionRound(pr,CurrentTradingday);
            }

            //从清算中心获得所有持仓 如果持仓未关闭则记录到结算持仓表
            foreach (Position pos in _clearcentre.TotalPositions)
            {

                ORM.MSettlement.InsertHoldPosition(pos, CurrentTradingday);
            }
            debug(datastoreheader + "保存PR数据完毕", QSEnumDebugLevel.INFO);
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

            debug("委托转储:" + onum.ToString());
            debug("成交转储:" + tnum.ToString());
            debug("成交转储:" + cnum.ToString());
            debug("交易回合转储:" + prnum.ToString());
            debug(datastoreheader + "转储交易信息 委托 取消 成交 完毕", QSEnumDebugLevel.INFO);
        }

        #endregion
    }
}
