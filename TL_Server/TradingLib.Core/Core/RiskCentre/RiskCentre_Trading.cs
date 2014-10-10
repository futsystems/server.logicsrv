using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;
using TradingLib.Common;

namespace TradingLib.Core
{
    public partial class RiskCentre
    {
        public event AssignOrderIDDel AssignOrderIDEvent;
        public event OrderDelegate newSendOrderRequest;
        public event LongDelegate newOrderCancelRequest;

        #region 获得某个合约的当前价格信息
        public event GetSymbolTickDel newSymbolTickRequest;
        protected Tick getSymbolTick(string symbol)
        {
            if (newSymbolTickRequest != null)
                return newSymbolTickRequest(symbol);
            else
                return null;
        }
        /// <summary>
        /// 获得某个合约的有效价格
        /// 如果返回-1则价格无效
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        protected decimal GetAvabilePrice(string symbol)
        {
            Tick k = getSymbolTick(symbol);//获得当前合约的最新数据
            if (k == null) return -1;

            decimal price = somePrice(k);

            //如果价格有效则返回价格 否则返回-1无效价格
            return price > 0 ? price : -1;
        }

        /// <summary>
        /// 从Tick数据采获当前可用的价格
        /// 优先序列 最新价/ ask / bid 如果均不可用则返回价格0
        /// </summary>
        /// <param name="k"></param>
        /// <returns></returns>
        private decimal somePrice(Tick k)
        {
            if (k.isTrade)
                return k.trade;
            if (k.hasAsk)
                return k.ask;
            if (k.hasBid)
                return k.bid;
            else
                return -1;
        }
        #endregion


        /// <summary>
        /// 用于提前分配委托ID 便于跟踪委托
        /// </summary>
        /// <param name="o"></param>
        void AssignOrderID(ref Order o)
        {
            if (AssignOrderIDEvent != null)
                AssignOrderIDEvent(ref o);
        }

        public void SendOrder(Order o)
        {
            try
            {
                o.date = Util.ToTLDate(DateTime.Now);
                o.time = Util.ToTLTime(DateTime.Now);

                if (newSendOrderRequest != null)
                    newSendOrderRequest(o);
            }
            catch (Exception ex)
            {
                debug("发送委托异常:" + o.ToString() + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }


        public void CancelOrder(long number)
        {
            try
            {
                if (newOrderCancelRequest != null)
                    newOrderCancelRequest(number);
            }
            catch (Exception ex)
            {
                debug("取消委托异常:" + number.ToString() + ex.ToString(), QSEnumDebugLevel.ERROR);
            }
        }

    }
}
