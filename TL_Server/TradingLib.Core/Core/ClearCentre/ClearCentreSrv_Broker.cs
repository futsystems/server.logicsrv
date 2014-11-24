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
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Order> SelectBrokerOrders(string token)
        {
            return ORM.MTradingInfo.SelectBrokerOrders().Where(o => o.Broker.Equals(token)).Select(o => { o.oSymbol = BasicTracker.SymbolTracker[o.Symbol]; return o; });
        }


        /// <summary>
        /// ���ĳ���ɽ��ӿ��������ڳɽ�����
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<Trade> SelectBrokerTrades(string token)
        {
            return ORM.MTradingInfo.SelectBrokerTrades().Where(t => t.Broker.Equals(token)).Select(t => { t.oSymbol = BasicTracker.SymbolTracker[t.Symbol]; return t; });
        }

        /// <summary>
        /// ���ĳ���ɽ��ӿ������ϸ������յ����гֲ���ϸ����
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IEnumerable<PositionDetail> SelectBrokerPositionDetails(string token)
        {
            return ORM.MSettlement.SelectBrokerPositionDetails(TLCtxHelper.Ctx.SettleCentre.LastSettleday).Where(p => p.Broker.Equals(token)).Select(pos => { pos.oSymbol = BasicTracker.SymbolTracker[pos.Symbol]; return pos; });
        }
    }
}
