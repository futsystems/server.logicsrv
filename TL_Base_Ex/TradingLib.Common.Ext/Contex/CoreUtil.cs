﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Common
{
    public class CoreUtil : IUtil
    {
        /// <summary>
        /// 返回某个合约的可用价格
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public decimal GetAvabilePrice(string symbol)
        {
            return TLCtxHelper.DataRouter.GetAvabilePrice(symbol);
        }

        /// <summary>
        /// 获得合约市场快照
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public Tick GetTickSnapshot(string symbol)
        {
            return TLCtxHelper.DataRouter.GetTickSnapshot(symbol);
        }


        public void AssignOrderID(ref Order o)
        {
            TLCtxHelper.Ctx.MessageExchange.AssignOrderID(ref o);
        }
        /// <summary>
        /// 发送委托
        /// </summary>
        /// <param name="o"></param>
        public void SendOrder(Order o)
        {

            TLCtxHelper.Ctx.MessageExchange.SendOrder(o);
        }

        /// <summary>
        /// 取消委托
        /// </summary>
        /// <param name="oid"></param>
        public void CancelOrder(long oid)
        {
            TLCtxHelper.Ctx.MessageExchange.CancelOrder(oid);
        }

        public void SendOrderInternal(Order o)
        {
            TLCtxHelper.Ctx.MessageExchange.SendOrderInternal(o);
        }

        public Order SentOrder(long id)
        {
            return TLCtxHelper.Ctx.ClearCentre.SentOrder(id);
        }


        public void ManualInsertOrder(Order o)
        {
            TLCtxHelper.Ctx.MessageExchange.ManualInsertOrder(o);
        }

        public void ManualInsertTrade(Trade t)
        {
            TLCtxHelper.Ctx.MessageExchange.ManualInsertTrade(t);
        }
        public void RegisterSymbol(Symbol sym)
        {
            TLCtxHelper.Ctx.MessageExchange.RegisterSymbol(sym);
        }
    }
}
