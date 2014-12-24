using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;


namespace TradingLib.Common
{
    /// <summary>
    /// 用于将内部clearcentreSrv转换成IBroker需要使用的接口模式,用于部分暴露功能函数,这样IBroker便无法访问IClearCentreSrv的
    /// 所有功能
    /// </summary>
    public class ClearCentreAdapterToBroker : IBrokerClearCentre
    {
        private IClearCentreSrv _clearcentre;
        public ClearCentreAdapterToBroker(IClearCentreSrv c)
        {
            _clearcentre = c;
        }


        public IEnumerable<Order> GetOrdersViaBroker(string broker)
        {
            Util.Debug("Broker:" + broker, QSEnumDebugLevel.INFO);
            foreach (Order o in _clearcentre.TotalOrders)
            {
                if (o.Broker.Equals(broker))
                {

                    string b = o.Broker;
                }
            }
            return _clearcentre.TotalOrders.Where(o => o.Broker.Equals(broker));
        }

        public IEnumerable<Trade> GetTradesViaBroker(string broker)
        {
            return _clearcentre.TotalTrades.Where(f => f.Broker.Equals(broker));
        }
        /// <summary>
        /// 获得清算中心下所有交易账户
        /// </summary>
        //IAccount[] Accounts { get; }
        //返回某个交易账户
        //IAccount this[string accid] { get; }

        /// <summary>
        /// 返回某个账户 某个symbol的持仓情况
        /// </summary>
        /// <param name="account"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Position getPosition(string account, string symbol,bool side)
        {
            IAccount acc = _clearcentre[account];
            return acc.GetPosition(symbol, side);
        }


        public Order SentOrder(long id,QSEnumOrderBreedType type = QSEnumOrderBreedType.ACCT)
        {
            if (type == QSEnumOrderBreedType.ACCT)
            {
                return _clearcentre.SentOrder(id);
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
            return _clearcentre.SelectBrokerOrders(token);
        }


        /// <summary>
        /// 获得日内成交接口的所有成交
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return _clearcentre.SelectBrokerTrades(token);
        }

        /// <summary>
        /// 获得成交接口上个结算日所有持仓数据
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return _clearcentre.SelectBrokerPositionDetails(token);
        }

    }


}
