﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using TradingLib.API;
using TradingLib.Common;
using TradingLib.BrokerXAPI;


namespace TradingLib.Core
{
    public partial class BrokerRouter
    {
        #region Broker向本地回报操作
        /// <summary>
        /// 交易接口查询某个symbol的当前最新Tick快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        Tick Broker_GetSymbolTickEvent(string symbol)
        {
            try
            {
                return DataFeedRouter.GetTickSnapshot(symbol);
            }
            catch (Exception ex)
            {
                debug(PROGRAM + ":get symbol tick snapshot error:" + ex.ToString(), QSEnumDebugLevel.ERROR);
                return null;
            }
        }

        
       

        /// <summary>
        /// 发送内部产生的委托错误
        /// </summary>
        /// <param name="o"></param>
        /// <param name="errortitle"></param>
        void GotOrderErrorNotify(Order o, string errortitle)
        {
            RspInfo info = RspInfoImpl.Fill(errortitle);
            o.Comment = info.ErrorMessage;
            Broker_GotOrderError(o, info);
        }


        /// <summary>
        /// 当交易通道有Order错误信息时,进行处理
        /// </summary>
        void Broker_GotOrderError(Order order, RspInfo error)
        {
            if (order.Breed == QSEnumOrderBreedType.ROUTER)
            {
                debug("Reply ErrorOrder To Spliter:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage, QSEnumDebugLevel.INFO);
                _splittracker.GotSonOrderError(order, error);
            }
            debug("Reply ErrorOrder To MessageExch:" + order.GetOrderInfo() + " ErrorTitle:" + error.ErrorMessage, QSEnumDebugLevel.INFO);
            _errorordernotifycache.Write(new OrderErrorPack(order, error));
        }


        /// <summary>
        /// 当有成交时候回报msgexch
        /// </summary>
        void Broker_GotFill(Trade fill)
        {
            if (fill != null && fill.isValid)
            {
                if (fill.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    debug("Reply Fill To Spliter:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
                    _splittracker.GotSonFill(fill);
                    return;
                }
                debug("Reply Fill To MessageExch:" + fill.GetTradeInfo(), QSEnumDebugLevel.INFO);
                _fillcache.Write(new TradeImpl(fill));
            }
        }

        /// <summary>
        /// 委托正确回报时回报msgexch
        /// 这里回报需要判断是否需同通过委托拆分器处理,如果是子委托则通过委托拆分器处理
        /// </summary>

        void Broker_GotOrder(Order o)
        {
            if (o != null && o.isValid)
            {
                //这里需要判断,该委托回报是拆分过的子委托还是分帐户侧的委托 如果是拆分过的委托则需要回报给拆分器
                if (o.Breed == QSEnumOrderBreedType.ROUTER)
                {
                    debug("Reply Order To Spliter:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                    _splittracker.GotSonOrder(o);
                    return;
                }
                debug("Reply Order To MessageExch:" + o.GetOrderInfo(), QSEnumDebugLevel.INFO);
                _ordercache.Write(new OrderImpl(o));
            }
        }

        /// <summary>
        /// 撤单正确回报时回报msgexch
        /// </summary>

        void Broker_GotCancel(long oid)
        {
            debug("Reply Cancel To MessageExch:" + oid.ToString());
            _cancelcache.Write(oid);
        }

        #endregion 

       
    }
}
