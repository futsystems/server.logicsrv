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
            //debug("平仓明细生成:" + obj.GetPositionCloseStr(), QSEnumDebugLevel.INFO);
            //设定该平仓明细所在结算日
            detail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;

            //异步保存平仓明细
            _asynLoger.newPositionCloseDetail(detail);
        
        }

        /// <summary>
        /// 获得某个成交接口所有日内交易数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => { o.oSymbol = BasicTracker.SymbolTracker[o.Symbol]; return o; });
        }


        /// <summary>
        /// 获得某个成交接口所有日内成交数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return ORM.MTradingInfo.SelectBrokerTrades().Where(t => t.Broker.Equals(token)).Select(t => { t.oSymbol = BasicTracker.SymbolTracker[t.Symbol]; return t; });
        }

        /// <summary>
        /// 获得某个成交接口所有上个结算日的所有持仓明细数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return ORM.MSettlement.SelectBrokerPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Where(p => p.Broker.Equals(token)).Select(pos => { pos.oSymbol = BasicTracker.SymbolTracker[pos.Symbol]; return pos; });
        }
    }
}
