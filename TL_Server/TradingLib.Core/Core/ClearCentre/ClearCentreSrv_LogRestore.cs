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
        /// ��¼ί������
        /// </summary>
        /// <param name="o"></param>
        internal void LogRouterOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        /// <summary>
        /// ��¼ί�и�������
        /// </summary>
        /// <param name="o"></param>
        internal void LogRouterOrderUpdate(Order o)
        {
            _asynLoger.updateOrder(o);
        }


        /// <summary>
        /// ��¼ί������
        /// </summary>
        /// <param name="o"></param>
        internal void LogBrokerOrder(Order o)
        {
            _asynLoger.newOrder(o);
        }

        /// <summary>
        /// ��¼ί�и�������
        /// </summary>
        /// <param name="o"></param>
        internal void LogBrokerOrderUpdate(Order o)
        {
            _asynLoger.updateOrder(o);
        }

        /// <summary>
        /// ��¼�ɽ�����
        /// </summary>
        /// <param name="f"></param>
        internal void LogBrokerTrade(Trade f)
        {
            _asynLoger.newTrade(f);
        }
        
        /// <summary>
        /// ��¼�ɽ���ϸ
        /// </summary>
        /// <param name="detail"></param>
        internal void LogBrokerPositionCloseDetail(PositionCloseDetail detail)
        {
            //debug("ƽ����ϸ����:" + obj.GetPositionCloseStr(), QSEnumDebugLevel.INFO);
            //�趨��ƽ����ϸ���ڽ�����
            detail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;

            //�첽����ƽ����ϸ
            _asynLoger.newPositionCloseDetail(detail);
        
        }

        /// <summary>
        /// ���ĳ���ɽ��ӿ��������ڽ�������
        /// �ӿڲ��ί�зֽ⣬�п������ʻ���Ҳ�п�����·�ɲ�ֽ����ί�У������Ҫͨ�����÷��Ϸ����ҵ���Ӧ��ί��
        /// �ʻ�ί�к�·�ɲ�ί�����ȼ���,���ڽӿ�����ʱ��Ȼ���ҵ���Ӧ��ί�� �Ӷ���ö�Ӧ������
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => {o.oSymbol = GetSymbolViaToken(o.Account,o.Symbol); return o; });
        }


        /// <summary>
        /// ���ĳ���ɽ��ӿ��������ڳɽ�����
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return ORM.MTradingInfo.SelectBrokerTrades().Where(t => t.Broker.Equals(token)).Select(t => { t.oSymbol = GetSymbolViaToken(t.Account, t.Symbol); return t; });
        }

        /// <summary>
        /// ���ĳ���ɽ��ӿ������ϸ������յ����гֲ���ϸ����
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return ORM.MSettlement.SelectBrokerPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Where(p => p.Broker.Equals(token)).Select(pos => { pos.oSymbol = GetSymbolViaToken(pos.Account, pos.Symbol); return pos; });
        }

        /// <summary>
        /// ���·�ɲ����зֽ�ί��
        /// ·�ɲ�ֽ��ί��Դ��ʱ���ʻ����ί�� ���ֱ�ӵ���ClearCentre.SentOrder�Ϳ�����ȷ��ø�ί��
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectRouterOrders()
        {
            return ORM.MTradingInfo.SelectRouterOrders().Select(ro => { Order fo = this.SentOrder(ro.FatherID); ro.oSymbol = fo != null ? fo.oSymbol : null; return ro; });
        }

        //ͨ��Token�ҵ���Ӧ��IBroker�Ӷ����Ի�ø���,��Ϳ��Ի�ö�Ӧ�ĺ�Լ
        Symbol GetSymbolViaToken(string token,string symbol)
        {
            Domain domain = BasicTracker.ConnectorConfigTracker.GetBrokerDomain(token);
            Symbol sym = null;
            if (domain != null)
                sym = domain.GetSymbol(symbol);
            else
                sym = null;// BasicTracker.DomainTracker.SuperDomain.GetSymbol(symbol);
            return sym;
        }

        public Order BrokerSentOrder(long id, QSEnumOrderBreedType? type = QSEnumOrderBreedType.ACCT)
        {
            if (type == QSEnumOrderBreedType.ROUTER)
            {
                return TLCtxHelper.Ctx.MessageExchange.SentRouterOrder(id);
            }
            else if (type == QSEnumOrderBreedType.ACCT)
            {
                return this.SentOrder(id);
            }
            else if(type == null)
            {
                return this.SentOrder(id);
            }
            return null;
        }
    }
}
