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
        /// 获得某个成交接口所有日内交易数据
        /// 接口侧的委托分解，有可能是帐户侧也有可能是路由侧分解的子委托，因此需要通过调用符合方法找到对应的委托
        /// 帐户委托和路由侧委托优先加载,后期接口启动时必然能找到对应的委托 从而获得对应的数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => { o.oSymbol = GetSymbolViaToken(o.Account, o.Symbol); return o; });
        }


        /// <summary>
        /// 获得某个成交接口所有日内成交数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return ORM.MTradingInfo.SelectBrokerTrades().Where(t => t.Broker.Equals(token)).Select(t => { t.oSymbol = GetSymbolViaToken(t.Account, t.Symbol); return t; });
        }

        /// <summary>
        /// 获得某个成交接口所有上个结算日的所有持仓明细数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return ORM.MSettlement.SelectBrokerPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Where(p => p.Broker.Equals(token)).Select(pos => { pos.oSymbol = GetSymbolViaToken(pos.Account, pos.Symbol); return pos; });
        }

        /// <summary>
        /// 获得路由侧所有分解委托
        /// 路由侧分解的委托源均时分帐户侧的委托 因此直接调用ClearCentre.SentOrder就可以正确获得该委托
        /// 路由侧委托中的合约对象与对应的父委托合约对象一致
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectRouterOrders()
        {
            return ORM.MTradingInfo.SelectRouterOrders().Select(ro => { Order fo = this.SentOrder(ro.FatherID); ro.oSymbol = fo != null ? fo.oSymbol : null; return ro; });
        }

        /// <summary>
        /// 接口侧交易信息 Account字段为对应的BrokerToken信息
        /// 通过Token找到对应的IBroker从而可以获得该域,则就可以获得对应的合约
        /// </summary>
        /// <param name="token"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Symbol GetSymbolViaToken(string token, string symbol)
        {
            Domain domain = BasicTracker.ConnectorConfigTracker.GetBrokerDomain(token);
            Symbol sym = null;
            if (domain != null)
                sym = domain.GetSymbol(symbol);
            else
                sym = null;// BasicTracker.DomainTracker.SuperDomain.GetSymbol(symbol); 如果没有对应的合约这里需要进行容错处理
            return sym;
        }
    }
}
