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
        /// ���ĳ���ɽ��ӿ��������ڽ�������
        /// �ӿڲ��ί�зֽ⣬�п������ʻ���Ҳ�п�����·�ɲ�ֽ����ί�У������Ҫͨ�����÷��Ϸ����ҵ���Ӧ��ί��
        /// �ʻ�ί�к�·�ɲ�ί�����ȼ���,���ڽӿ�����ʱ��Ȼ���ҵ���Ӧ��ί�� �Ӷ���ö�Ӧ������
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => { o.oSymbol = GetSymbolViaToken(o.Account, o.Symbol); return o; });
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
        /// ·�ɲ�ί���еĺ�Լ�������Ӧ�ĸ�ί�к�Լ����һ��
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Order> SelectRouterOrders()
        {
            return ORM.MTradingInfo.SelectRouterOrders().Select(ro => { Order fo = this.SentOrder(ro.FatherID); ro.oSymbol = fo != null ? fo.oSymbol : null; return ro; });
        }

        /// <summary>
        /// �ӿڲཻ����Ϣ Account�ֶ�Ϊ��Ӧ��BrokerToken��Ϣ
        /// ͨ��Token�ҵ���Ӧ��IBroker�Ӷ����Ի�ø���,��Ϳ��Ի�ö�Ӧ�ĺ�Լ
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
                sym = null;// BasicTracker.DomainTracker.SuperDomain.GetSymbol(symbol); ���û�ж�Ӧ�ĺ�Լ������Ҫ�����ݴ���
            return sym;
        }
    }
}
