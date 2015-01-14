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
        #region 记录交易帐户交易日志
        internal void LogAcctOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        internal void LogAcctOrderUpdate(Order o)
        {
            _asynLoger.updateOrder(o);
        }

        internal void LogAcctOrderAction(OrderAction o)
        {
            _asynLoger.newOrderAction(o);
        }

        internal void LogAcctTrade(Trade f)
        {
            _asynLoger.newTrade(f);
        }

        internal void LogAcctPositionRound(PositionRound pr)
        {
            _asynLoger.newPositonRound(pr);
        }
        
        internal void LogAcctPositionCloseDetail(PositionCloseDetail detail)
        {
            //设定该平仓明细所在结算日
            detail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
            //异步保存平仓明细
            _asynLoger.newPositionCloseDetail(detail);
        
        }
        #endregion


        /// <summary>
        /// 记录委托数据
        /// </summary>
        /// <param name="o"></param>
        internal void LogRouterOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        /// <summary>
        /// 记录委托更新数据
        /// </summary>
        /// <param name="o"></param>
        internal void LogRouterOrderUpdate(Order o)
        {
            _asynLoger.updateOrder(o);
        }


        /// <summary>
        /// 记录委托数据
        /// </summary>
        /// <param name="o"></param>
        internal void LogBrokerOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        /// <summary>
        /// 记录委托更新数据
        /// </summary>
        /// <param name="o"></param>
        internal void LogBrokerOrderUpdate(Order o)
        {
            _asynLoger.updateOrder(o);
        }

        /// <summary>
        /// 记录成交数据
        /// </summary>
        /// <param name="f"></param>
        internal void LogBrokerTrade(Trade f)
        {
            _asynLoger.newTrade(f);
        }
        
        /// <summary>
        /// 记录成交明细
        /// </summary>
        /// <param name="detail"></param>
        internal void LogBrokerPositionCloseDetail(PositionCloseDetail detail)
        {
            //设定该平仓明细所在结算日
            detail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
            //异步保存平仓明细
            _asynLoger.newPositionCloseDetail(detail);
        
        }



    }
}
