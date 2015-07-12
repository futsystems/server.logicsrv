using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TradingLib.API;

namespace TradingLib.Common
{
    public partial class AccountBase
    {
    
         /// <summary>
        /// 交易账户产生委托回报事件
        /// </summary>
        public event OrderDelegate GotOrderEvent;

        /// <summary>
        /// 交易账户产生成交回报事件
        /// </summary>
        public event FillDelegate GotFillEvent;


        /// <summary>
        /// 新的平仓明细生成事件
        /// </summary>
        public event Action<Trade, PositionCloseDetail> GotPositionCloseDetailEvent;

        /// <summary>
        /// 新的持仓明细生成事件
        /// </summary>
        public event Action<Trade, PositionDetail> GotPositionDetailEvent;



        public void FireOrderEvent(Order o)
        {
            if (GotOrderEvent != null)
            {
                GotOrderEvent(o);
            }
        }

        public void FireFillEvent(Trade f)
        {
            if (GotFillEvent != null)
            {
                GotFillEvent(f);
            }
        }

        /// <summary>
        /// 触发持仓明细生成事件
        /// </summary>
        /// <param name="f"></param>
        /// <param name="detail"></param>
        public void FirePositoinDetailEvent(Trade f, PositionDetail detail)
        {
            if (GotPositionDetailEvent != null)
            {
                GotPositionDetailEvent(f, detail);
            }
        }

        /// <summary>
        /// 触发平仓明细事件
        /// </summary>
        /// <param name="f"></param>
        /// <param name="close"></param>
        public void FirePositionCloseDetailEvent(Trade f, PositionCloseDetail close)
        {
            if (GotPositionCloseDetailEvent != null)
            {
                GotPositionCloseDetailEvent(f, close);
            }
        }

    }
}
