using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// ClearCentreSrv适配器 用于整理ClearCentreSrv接口并规范成IBrokerClearCentre接口
    /// 暴露需要暴露的功能操作
    /// </summary>
    public class ClearCentreAdapterToBroker : IBrokerClearCentre
    {
        private IClearCentreSrv _clearcentre;
        public ClearCentreAdapterToBroker()
        {
        }


        public IEnumerable<Order> GetOrdersViaBroker(string broker)
        {
            return TLCtxHelper.CmdTotalInfo.TotalOrders.Where(o => o.Broker.Equals(broker));
        }

        public IEnumerable<Trade> GetTradesViaBroker(string broker)
        {
            return TLCtxHelper.CmdTotalInfo.TotalTrades.Where(f => f.Broker.Equals(broker));
        }

        /// <summary>
        /// 成交接口 加载成交侧委托,需要恢复日内委托关系链
        /// 需要找到成交侧委托的父委托,父委托有可能是路由委托也有可能是直接帐户委托
        /// 这里需要判断然后从清算中心或路由中心获得对应的委托对象
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Order SentOrder(long id,QSEnumOrderBreedType type = QSEnumOrderBreedType.ACCT)
        {
            if (type == QSEnumOrderBreedType.ACCT)
            {
                return TLCtxHelper.CmdTotalInfo.SentOrder(id);
            }
            if (type == QSEnumOrderBreedType.ROUTER)
            {
                return TLCtxHelper.Ctx.MessageExchange.SentRouterOrder(id);
            }
            return null;
        }


        /// <summary>
        /// 获得日内成交接口的所有委托
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return TLCtxHelper.DataRepository.SelectBrokerOrders(token);
        }


        /// <summary>
        /// 获得日内成交接口的所有成交
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return TLCtxHelper.DataRepository.SelectBrokerTrades(token);
        }

        /// <summary>
        /// 获得成交接口上个结算日所有持仓数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return TLCtxHelper.DataRepository.SelectBrokerPositionDetails(token);
        }

    }


}
