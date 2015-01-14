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
        #region ��¼�����ʻ�������־
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
            //�趨��ƽ����ϸ���ڽ�����
            detail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
            //�첽����ƽ����ϸ
            _asynLoger.newPositionCloseDetail(detail);
        
        }
        #endregion


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
            //�趨��ƽ����ϸ���ڽ�����
            detail.Settleday = TLCtxHelper.Ctx.SettleCentre.NextTradingday;
            //�첽����ƽ����ϸ
            _asynLoger.newPositionCloseDetail(detail);
        
        }



    }
}
